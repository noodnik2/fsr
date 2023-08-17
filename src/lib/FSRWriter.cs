/*

	FSRWriter.cs - Base class for creating FSR-class files
    Copyright (C) 2003-2004, Marty Ross


	This module defines a base class used for creating FSR files.

	It supports the "Chunked" nature of FSR files, and exposes
	methods for starting, writing and closing the various sections
	of FSR files.

	Also included are helper methods useful for FSR-class writers.

*/

using System ;

namespace fsrlib
{

	public abstract class FSRWriter
	{

		public class Chunk
		{
		}

		public abstract Chunk Start() ;
		public abstract void End(Chunk startChunk) ;

		public abstract Chunk FsibStart() ;
		public abstract void FsibEnd(Chunk fsibChunk) ;

		public abstract Chunk FsibBfibStart() ;
		public abstract void FsibBfibEnd(Chunk bfibChunk) ;

		public abstract Chunk FsibBfibOdibStart() ;
		public abstract void FsibBfibOdibObItem(ushort oid, string oname) ;
		public abstract void FsibBfibOdibEnd(Chunk bfibodibChunk) ;

		public abstract Chunk FsibBfibPdibStart() ;
		public abstract void FsibBfibPdibPrItem(ushort oid, ushort pid, ushort tid, string punits, string pname) ;
		public abstract void FsibBfibPdibEnd(Chunk bfibpdibChunk) ;

		public abstract Chunk FsibBfibFribStart() ;
		public abstract void FsibBfibFribTsItem(uint frno, uint ticks, long timeval) ;
		public abstract void FsibBfibFribFrItem(ushort oid, ushort pid, ushort tid, Object o) ;
		public abstract void FsibBfibFribEnd(Chunk bfibfribChunk) ;

		public abstract Chunk FsibTrailerStart() ;
		public abstract void FsibTrailerDaItem(ushort dtype, string dval) ;
		public abstract void FsibTrailerEnd(Chunk trailerChunk) ;


		//
		//
		//

		internal static string ObjectToString(Object o, FSRPropertyDataType ptype)
		{
			switch(ptype) {

				case FSRPropertyDataType.Bool:
					return(o.ToString()) ;

				case FSRPropertyDataType.Int:
					return(o.ToString()) ;

				case FSRPropertyDataType.String:
					return(o.ToString()) ;

				case FSRPropertyDataType.Double:
					return(DoubleToString((double) o)) ;

			}

			throw new ApplicationException("unknown property type: " + (int) ptype) ;
		}


		//
		//
		//

		private static string DoubleToString(double d)
		{
#if	true	// make sure to keep counterpart in "FSRReader" in sync!
			return(d.ToString("r")) ;
#else
			/*
				Normally, we would use the following:

					string sval= d.ToString("r") ;

				However, on account of the fact that the sequence:

					double d= BinaryReader.ReadDouble() ;
					string sval= d.ToString("r") ;
					BinaryWriter.Write(double.Parse(sval)) ;

				produces a difference between the input and output
				binary streams (it seems that a zero value is read
				with "0x80" in the first byte position, yet written
				with "0x00" in the first byte position).  Therefore,
				we're converting the double value to an integral
				8-byte format, and using the following conversion
				into and out of string format to avoid this error:

					double d= BinaryReader.ReadDouble() ;
					string sval= BitConverter.DoubleToInt64(d).ToString() ;
					BinaryWriter.Write(BitConverter.Int64ToDouble(long.Parse(sval))) ;

				And, this seems to solve the problem.  So, we use
				the following:

					string sval= BitConverter.DoubleToInt64(d).ToString() ;

			*/

			return(BitConverter.DoubleToInt64Bits(d).ToString()) ;
#endif
		}

	}

}
