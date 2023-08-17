/*

	FSRWriterFSR.cs - Writer for creating FSR (binary) files
    Copyright (C) 2003-2004, Marty Ross


	This module provides a FSRWriter implemented to create actual FSR
	(binary) files.  See Microsoft's "Flight Simulator 2002 SDK" document
	named "Netpipes.doc" for complete information about this binary format.

*/


using System ;
using System.IO ;

namespace fsrlib
{

	public class FSRWriterFSR : FSRWriter
	{

		public FSRWriterFSR(Stream os)
		{
			Construct(os, 4096) ;
		}

		public FSRWriterFSR(Stream os, int blocksize)
		{
			Construct(os, blocksize) ;
		}

		public override FSRWriter.Chunk Start()
		{
			// nothing to do
			return(null) ;
		}

		public override void End(FSRWriter.Chunk startChunk)
		{
			m_bout.Flush() ;
			m_bout.Close() ;
			if (m_os != null) {
				m_os.Flush() ;
				m_os.Close() ;
			}
		}

		public override FSRWriter.Chunk FsibStart()
		{
			/*
				Excerpt from "Netpipes.doc":

				The file header is composed of three fields:
				�	A four-character string. For Flight Simulator 2002,
					this string is always �FSIB�
				�	A two-character string for the format version. For
					Flight Simulator 2002, this string is always �80�.
				�	A single word for the Flight Simulator version. For
					Flight Simulator 2002, this value must be 0x800.
			*/
			SectionFSR.WriteName("FSIB", m_bout) ;
			WriteUInt16(0x8) ;
			WriteUInt16(0x800) ;
			return(null) ;
		}

		public override void FsibEnd(FSRWriter.Chunk startChunk)
		{
			FlushChunkBuffer() ;		// write the rest of the document
		}

		public override FSRWriter.Chunk FsibBfibStart()
		{
			/*
				Excerpt from "Netpipes.doc":

				Between the file-header and the file-trailer are data
				records of �chunks� of data. A �chunk� is composed of:
				�	A header of the form:
					�	A four-character string. In FS2002 it is
						always: �BFIB�
					�	A DWORD for the total size in bytes,including
						the header and footer, of this data chunk
				�	Some number of data-sections which have a header and a
					data block.
				�	A footer
				�	A DWORD crc. For Flight Simulator 2002, this must be 0
				�	A DWORD for the total size in bytes of this data chunk,
					including the header and footer. The size in the footer
					should match the size in the header.
			*/

			// create chunk, write "header"
			return new ChunkFSR("BFIB", m_bout) ;
		}

		public override void FsibBfibEnd(FSRWriter.Chunk bfibChunk)
		{
			ChunkFSR bfibChunkFSR= ((ChunkFSR) bfibChunk) ;

			uint csize= bfibChunkFSR.Size ;			// remember current size
			WriteUInt32(0) ;				// add on CRC
			WriteUInt32(csize + ChunkFSR.HeaderSize) ;	// add on total size
			bfibChunkFSR.ChunkEnd() ;			// finish the chunk
			FlushChunkBuffer() ;				// write the BFIB chunk
		}

		public override FSRWriter.Chunk FsibBfibOdibStart()
		{
			// create chunk, write "header"
			return new ChunkFSR("ODIB", m_bout) ;
		}

		public override void FsibBfibOdibObItem(ushort oid, string oname)
		{
			WriteUInt16(oid) ;
			WriteUInt16((ushort) oname.Length) ;
			WriteChars(oname) ;
		}

		public override void FsibBfibOdibEnd(FSRWriter.Chunk bfibodibChunk)
		{
			((ChunkFSR) bfibodibChunk).ChunkEnd() ;
		}

		public override FSRWriter.Chunk FsibBfibPdibStart()
		{
			// create chunk, write "header"
			return new ChunkFSR("PDIB", m_bout) ;
		}

		public override void FsibBfibPdibPrItem(
			ushort oid,
			ushort pid,
			ushort tid,
			string punits,
			string pname
		) {
			WriteUInt16(oid) ;
			WriteUInt16(pid) ;
			WriteUInt16(tid) ;
			WriteUInt16((ushort) pname.Length) ;
			WriteUInt16((ushort) punits.Length) ;
			WriteChars(pname) ;
			WriteChars(punits) ;
		}

		public override void FsibBfibPdibEnd(FSRWriter.Chunk bfibpdibChunk)
		{
			((ChunkFSR) bfibpdibChunk).ChunkEnd() ;
		}

		public override FSRWriter.Chunk FsibBfibFribStart()
		{
			// create chunk, write "header"
			return new ChunkFSR("FRIB", m_bout) ;
		}

		public override void FsibBfibFribTsItem(uint frno, uint ticks, long timeval)
		{
			WriteInt64(timeval) ;
			WriteUInt32(frno) ;
			WriteUInt32(ticks) ;
		}

		public override void FsibBfibFribFrItem(ushort oid, ushort pid, ushort tid, Object o)
		{

			WriteUInt16(oid) ;
			WriteUInt16(pid) ;
			WriteUInt16(tid) ;

			switch((FSRPropertyDataType) tid) {

				case FSRPropertyDataType.Bool:
					WriteUInt16((ushort) (((bool) o)? 1: 0)) ;
					break ;

				case FSRPropertyDataType.Double:
					WriteDouble((double) o) ;
					break ;

				case FSRPropertyDataType.Int:
					WriteInt32((int) o) ;
					break ;

				case FSRPropertyDataType.String:
					string sval= (string) o ;
					WriteUInt32((ushort) sval.Length) ;
					WriteChars(sval) ;
					break ;

				default:
					throw new ApplicationException("unknown property type: " + tid) ;

			}
		}

		public override void FsibBfibFribEnd(FSRWriter.Chunk bfibfribChunk)
		{

			//
			//	Apparently, FRIB sections must occupy a multiple of 4 bytes.
			//
			int mod= (int) ((((ChunkFSR) bfibfribChunk).Size) & 3) ;
			if (mod != 0) {
				WriteFiller(4 - mod) ;
			}

			((ChunkFSR) bfibfribChunk).ChunkEnd() ;
		}

		public override FSRWriter.Chunk FsibTrailerStart()
		{
			// create chunk, write "header"
			return new ChunkFSR("TAIB", m_bout) ;
		}

		public override void FsibTrailerDaItem(ushort dtype, string dval)
		{
			WriteUInt16(dtype) ;
			WriteUInt16((ushort) dval.Length) ;
			WriteChars(dval) ;
			WriteFiller((4 - (dval.Length & 3)) & 3) ;
		}

		public override void FsibTrailerEnd(FSRWriter.Chunk trailerChunk)
		{
			uint size= ((ChunkFSR) trailerChunk).Size ;

			WriteChars("TAIB") ;		// write TAIB footer
			WriteUInt32(size + 8) ;		// "+8" accommodates for this footer

			((ChunkFSR) trailerChunk).ChunkEnd() ;
		}


		//
		//	public "FSRWriter.Chunk" subclass,
		//	specific to "FSRWriterFSR"
		//

		public class ChunkFSR : FSRWriter.Chunk
		{

			public ChunkFSR()
			{
			}

			public ChunkFSR(string sname, BinaryWriter bout)
			{
				ChunkStart(sname, bout) ;
			}

			public void ChunkStart(string sname, BinaryWriter bout)
			{
				m_bout= bout ;
				m_startOffset= (ulong) m_bout.BaseStream.Position ;
				m_sname= sname ;
				SectionFSR.WriteName(m_sname, m_bout) ;
				m_bout.Write((uint) 0) ;
			}

			public void ChunkEnd()
			{
				ulong cpos= (ulong) m_bout.BaseStream.Position ;
				m_bout.Seek((int) (m_startOffset + 4), SeekOrigin.Begin) ;
				m_bout.Write((uint) ((ulong) cpos - m_startOffset)) ;
				m_bout.Seek((int) cpos, SeekOrigin.Begin) ;
				m_sname= null ;
				m_bout= null ;
			}

			//
			//	HeaderSize:	# of bytes of taken by "Chunk Header" at front of "Chunk"
			//
			public const int HeaderSize= 8 ;

			//
			//	Size:		# of bytes of "Chunk" currently in file, including header
			//
			public uint Size {
				get {
					return(
						(uint) (
							(ulong) m_bout.BaseStream.Position
						      - m_startOffset
						)
					) ;
				}
			}

			private ulong m_startOffset ;
			private string m_sname ;
			private BinaryWriter m_bout ;
		}


		//
		//	private "SectionFSR" class containing static items
		//	for accessing "section" data in FSR files.
		//

		private class SectionFSR
		{
			public const uint NameSize= 4 ;
			public static void WriteName(string sname, BinaryWriter bout)
			{
				if (sname.Length != NameSize) {
					throw new ApplicationException("invalid section name: '" + sname + "'") ;
				}
				bout.Write(sname.ToCharArray()) ;
			}
		}


		//
		//	private formatted output helper methods
		//

		private void WriteUInt16(ushort u16)
		{
			m_bout.Write(u16) ;
		}

		private void WriteInt32(int i32)
		{
			m_bout.Write(i32) ;
		}

		private void WriteUInt32(uint u32)
		{
			m_bout.Write(u32) ;
		}

		private void WriteInt64(long i64)
		{
			m_bout.Write(i64) ;
		}

		private void WriteByte(byte b)
		{
			m_bout.Write(b) ;
		}

		private void WriteChars(string s)
		{
			m_bout.Write(s.ToCharArray()) ;
		}

		private void WriteDouble(double d)
		{
			m_bout.Write(d) ;
		}

		private void WriteFiller(int len)
		{
			for (int i= 0; i< len; i++) {
				m_bout.Write((byte) 0) ;
			}
		}


		//
		//	private helper method for construction of instance
		//

		private void Construct(Stream os, int blocksize)
		{
			m_bUsingBuffer= !os.CanSeek ;
			m_blocksize= blocksize ;

			if (m_bUsingBuffer) {
				// use a memory stream for binary writer
				m_bout= new BinaryWriter(
					new MemoryStream(m_blocksize)
				) ;
				// keep the underlying output stream separate
				m_os= os ;
			}
			else {
				// use the underlying output stream for binary writer
				m_bout= new BinaryWriter(os) ;
				// underlying output stream is owned by binary writer
				m_os= null ;
			}
		}

		/// <summary>
		/// If we're using a memory buffer, then write the contents of the memory
		/// stream underneath "m_bout" to the "m_os" output stream and reset the
		/// memory stream.
		/// </summary>
		public void FlushChunkBuffer()
		{
			if (m_bUsingBuffer) {

				m_bout.Flush() ;
				MemoryStream ms= (MemoryStream) m_bout.BaseStream ;

				byte[] msBuffer= ms.GetBuffer() ;
				long leftToWriteCount= ms.Length ;
				int writeFromOffset= 0 ;

				while(leftToWriteCount> 0) {
					int writeCount= (
						(leftToWriteCount> (long) m_blocksize)
					      ? m_blocksize
					      : (int) leftToWriteCount
					) ;
					m_os.Write(msBuffer, writeFromOffset, writeCount) ;
					leftToWriteCount-= (long) writeCount ;
					writeFromOffset+= writeCount ;
				}

				m_bout.Close() ;
				m_bout= new BinaryWriter(
					new MemoryStream(m_blocksize)
				) ;
			}
		}


		//
		//	internal data
		//

		private BinaryWriter m_bout ;	// binary writer for formatting output
		private Stream m_os ;		// underlying memory stream, if "m_bUsingBuffer"

		//
		// Since the FSR file format contains length-prefixed "chunks", we must be able
		// to seek backwards in the file in order to write the length prefixes after
		// completing a chunk (because its not until after we've finished the chunk that
		// we know its length).  If the underlying output stream doesn't support seeking,
		// then we must substitute a memory stream buffer to which we can seek.  We must
		// flush the memory stream to the output stream often so that it doesn't grow too
		// large.  The "chunked" architecture of the FSR file encourages us to create
		// "BFIB" (outer) chunks containing up to several seconds of simulator parameter
		// values, flushing our buffer between chunks.
		//
		// We've written the code so that it uses the memory buffer only if necessary
		// (based upon whether or not the underlying stream allows seeking) in order
		// to reduce the memory consumption where possible.  The "m_bUsingBuffer" flag
		// identifies the "mode" we're in.
		//
		private bool m_bUsingBuffer ;	// are we using a memory stream for chunks?

		//
		//	m_blocksize is set to the maximum size that we should write
		//	to the underlying stream in a single write operation.
		//
		//	it is assumed that this is relevant only in the case where
		//	we are using a memory stream to cache "chunks", since the
		//	other writes (e.g., non chunked) are assumed to be small
		//	and therefore less than this maximum size.
		//
		private int m_blocksize ;	// max. size for single write to stream

	}

}
