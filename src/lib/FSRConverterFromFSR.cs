/*

	FSRConverterFromFSR.cs - FSR Conversion from FSR (binary) format
    Copyright (C) 2003-2004, Marty Ross


	This module handles conversions from FSR (binary) format.

*/


using System ;
using System.IO ;

namespace fsrlib
{

	/*
		FSR File Format Notes
		(from MSFS2002 Netpipes SDK Documentation)

		Within File there is:
			1 x FileHeader
			n x DataRecord
			1 x FileTrailer

		Within DataRecord there is:
			1 x DataRecordHeader
			n x DataSection
			1 x DataRecordFooter

		Within FileTrailer there is:
			1 x DataRecordHeader
			n x FileTrailerRecords
			1 x DataRecordFooter

		Within DataRecordHeader there is:
			1 x 4-byte ID
			1 x DWORD size

		Within DataRecordFooter there is:
			1 x DWORD crc
			1 x DWORD size

		Within DataSection there is:
			1 x DataSectionHeader
			1 x DataSectionData

		DataSectionData is one of:
			ObjectDefinitionDataSectionData
			PropertyDictionaryDataSectionData
			FrameSectionDataSectionData
			
	*/

	public class FSRConverterFromFSR : FSRConverterFrom
	{

		/*
			public interface
		*/

		public override void Convert(Stream input, FSRWriter wrr)
		{
			Convert(new FSRReaderFSR(input), wrr) ;
		}

		public override void Convert(FSRReader rdr, FSRWriter wrr)
		{
			Convert((FSRReaderFSR) rdr, wrr) ;
		}

		public void Convert(FSRReaderFSR rdr, FSRWriter wrr)
		{
			m_input= rdr ;
			m_output= wrr ;
			m_skipcount= 0 ;
			Convert() ;
		}


		/*
			private area
		*/

		private void Convert()
		{
			FSRWriter.Chunk outStartChunk= m_output.Start() ;

			ParseFileHeader() ;
			
			FSRWriter.Chunk outFsibChunk= m_output.FsibStart() ;

			FSRReaderFSR.Chunk rchunk= null ;

			for (;;) {
				rchunk= m_input.ReadChunk() ;

				if (rchunk.Name == "BFIB") {
					ParseDataRecord(rchunk.Length) ;
					continue ;
				}

				if (rchunk.Name == "TAIB") {
					ParseFileTrailer(rchunk.Length) ;
					break ;
				}

				Logger.LogWarning(
					"Skipping unknown FSR record: '"
					+ rchunk.Name
					+ "'"
				) ;

				m_input.SkipInput(rchunk.Length) ;
			}
			
			m_output.FsibEnd(outFsibChunk) ;
			m_output.End(outStartChunk) ;
		}

		private void ParseFileHeader() 
		{
			FSRReaderFSR.Section section= m_input.ReadSection() ;
			if (section.Name != "FSIB") {
				throw new ApplicationException("Invalid FSR file section: " + section.Name) ;
			}

			int version= m_input.BinaryReader.ReadInt16() ;
			if (version != 0x0008) {
				throw new ApplicationException(String.Format("Invalid FSR file version: 0x{0:X}", version)) ;
			}

			int simver= m_input.BinaryReader.ReadInt16() ;
			if (simver != 0x0800) {
				throw new ApplicationException(String.Format("Invalid FSR file sim version: 0x{0:X}", simver)) ;
			}
		}

		private void ParseDataRecord(uint length)
		{
			FSRWriter.Chunk outBfibChunk= m_output.FsibBfibStart() ;

			uint count= length - DataRecordFooterSize ;
			uint minCount= FSRReaderFSR.Chunk.HeaderSize ;

			while(count>= minCount) {

				DoProgressUpdate(m_input) ;

				FSRReaderFSR.Chunk inChunk= m_input.ReadChunk() ;
				if (inChunk.Name == "ODIB") {
					ParseODIBRecordSection(inChunk.Length) ;
				}
				else if (inChunk.Name == "FRIB") {
					ParseFRIBRecordSection(inChunk.Length) ;
				}
				else if (inChunk.Name == "PDIB") {
					ParsePDIBRecordSection(inChunk.Length) ;
				}
				else {
					Logger.LogWarning(
						"Skipping unknown FSR record section: '"
					      + inChunk.Name
					      + "'"
					) ;
					m_input.SkipInput(inChunk.Length) ;
				}
				count-= inChunk.Size ;
			}

			ParseDataRecordFooter() ;
			m_output.FsibBfibEnd(outBfibChunk) ;
		}

		private void ParseODIBRecordSection(uint length)
		{
			FSRWriter.Chunk outOdibChunk= m_output.FsibBfibOdibStart() ;

			uint count= length ;
			uint minCount= 4 ;	// min. record section size

			while(count>= minCount) {

				ushort objid= m_input.BinaryReader.ReadUInt16() ;
				ushort slen= m_input.BinaryReader.ReadUInt16() ;
				string objname= new string(m_input.BinaryReader.ReadChars((int) slen)) ;

				m_output.FsibBfibOdibObItem(objid, objname) ;

				count-= (uint) (slen + 4) ;
			}
			m_output.FsibBfibOdibEnd(outOdibChunk) ;
		}

		private void ParsePDIBRecordSection(uint length)
		{
			FSRWriter.Chunk outPdibChunk= m_output.FsibBfibPdibStart() ;

			uint count= length ;
			uint minCount= 10 ;	// min. PDIB section size

			while(count>= minCount) {

				ushort objid= m_input.BinaryReader.ReadUInt16() ;
				ushort propid= m_input.BinaryReader.ReadUInt16() ;
				ushort ptype= m_input.BinaryReader.ReadUInt16() ;
				ushort namesize= m_input.BinaryReader.ReadUInt16() ;
				ushort unitsize= m_input.BinaryReader.ReadUInt16() ;
				string property= new String(m_input.BinaryReader.ReadChars((int) namesize)) ;
				string unitname= new String(m_input.BinaryReader.ReadChars((int) unitsize)) ;

				m_output.FsibBfibPdibPrItem(objid, propid, ptype, unitname, property) ;

				count-= (uint) (10 + namesize + unitsize) ;
			}

			m_output.FsibBfibPdibEnd(outPdibChunk) ;
		}

		private void ParseFRIBRecordSection(uint length)
		{
			FSRWriter.Chunk outFribChunk= m_output.FsibBfibFribStart() ;

			// write timestamp
			long timestamp= m_input.BinaryReader.ReadInt64() ;
			uint frameno= m_input.BinaryReader.ReadUInt32() ;
			uint tickcount= m_input.BinaryReader.ReadUInt32() ;

			m_output.FsibBfibFribTsItem(frameno, tickcount, timestamp) ;
	
			// start with length minus sizeof(timestamp)
			uint count= length - 16 ;
			uint minCount= 8 ;		// smallest possible frame data

			while(count>= minCount) {

				ushort objid= m_input.BinaryReader.ReadUInt16() ;
				ushort propid= m_input.BinaryReader.ReadUInt16() ;
				FSRPropertyDataType valtype= (FSRPropertyDataType) (
					m_input.BinaryReader.ReadUInt16()
				) ;

				Object o= null ;
				string sval= null ;
				bool bval ;
				double dval ;
				int ival ;

				uint vallen= 6 ;		// bytes read so far..

				switch(valtype) {

					case FSRPropertyDataType.Bool:
						bval= (m_input.BinaryReader.ReadInt16() != 0)? true: false ;
						o= bval ;
						vallen+= 2 ;
						break ;

					case FSRPropertyDataType.Double:
						dval= m_input.BinaryReader.ReadDouble() ;
						// NOTE: we use base-class method for this conversion
						//	 because of problems we had.  See notes there.
						o= dval ;
						vallen+= 8 ;
						break ;

					case FSRPropertyDataType.Int:
						ival= m_input.BinaryReader.ReadInt32() ;
						o= ival ;
						vallen+= 4 ;
						break ;

					case FSRPropertyDataType.String:
						uint slen= m_input.BinaryReader.ReadUInt32() ;
						sval= new String(m_input.BinaryReader.ReadChars((int) slen)) ;
						o= sval ;
						vallen+= (4 + slen) ;
						break ;

				}

				if (o == null) {
					throw new ApplicationException("Can't get FSR value type " + valtype) ;
				}

				m_output.FsibBfibFribFrItem(objid, propid, (ushort) valtype, o) ;

				count-= vallen ;
			}

			// there seems to be some unknown data at the end...
			if (count> 0) {
				//Logger.LogNote("frib; " + count + " bytes skipped.") ;
				m_input.SkipInput(count) ;	// skip the padding bytes...
				m_skipcount+= (int) count ;
			}

			m_output.FsibBfibFribEnd(outFribChunk) ;
		}

		private const uint DataRecordFooterSize= 8 ;	// size of file footer in stream
		private void ParseDataRecordFooter()
		{
			uint crc= m_input.BinaryReader.ReadUInt32() ;
			m_input.BinaryReader.ReadUInt32() ;	// length (ignored)
			if (crc != 0) {
				Logger.LogWarning(String.Format("FSR record crc != 0, crc=0x{0:X}", crc)) ;
			}
		}

		private void ParseFileTrailer(uint length)
		{
			FSRWriter.Chunk outTrailerChunk= m_output.FsibTrailerStart() ;

			/*
				The trailer is supposed to be a "three part" structure:

				(1) "chunk" header; already read
				(2) data header/data value pairs
				(3) duplicate "chunk" header
			*/

			uint count= length ;
			uint minCount= 4 + FSRReaderFSR.Chunk.HeaderSize ; // min. data size

			while(count>= minCount) {

				ushort wid= m_input.BinaryReader.ReadUInt16() ;
				ushort wlen= m_input.BinaryReader.ReadUInt16() ;
				string sval= new string(m_input.BinaryReader.ReadChars((int) wlen)) ;

				m_output.FsibTrailerDaItem(wid, sval) ;

				// word-align input
				count-= (uint) (4 + wlen) ;
				while((count & 3) != 0) {
					m_input.BinaryReader.ReadByte() ;
					count-- ;
				}
			}

			if (count> 0) {
				m_skipcount+= (int) count ;
				//Logger.LogWarning("FSR trailer; " + count + " bytes skipped.") ;
				// ignore the rest of the record...
				// (it's supposed to be a copy of the "TAIB" "chunk" header...
				m_input.SkipInput(count) ;
			}

			m_output.FsibTrailerEnd(outTrailerChunk) ;
		}


		/*
			Data
		*/

		private FSRReaderFSR m_input ;
		private FSRWriter m_output ;
		private int m_skipcount ;

	}
}
