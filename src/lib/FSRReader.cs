/*

	FSRReader.cs - Basic FSR Reader Services
    Copyright (C) 2003-2004, Marty Ross


	This module provides methods helpful for reading FSR-class files
	from within in the fsrlib library.

*/


using System ;
using System.IO ;

namespace fsrlib
{

	public class FSRReader
	{

		public virtual void Close()
		{
			// doesn't do anything in base class implementation
		}

		public virtual double FractionRead { get { return(0.0) ; } }

		internal static Object StringToObject(string sval, FSRPropertyDataType ptype)
		{
			switch(ptype) {

				case FSRPropertyDataType.Bool:
					return(bool.Parse(sval)) ;

				case FSRPropertyDataType.Int:
					return(int.Parse(sval)) ;

				case FSRPropertyDataType.String:
					return(sval) ;

				case FSRPropertyDataType.Double:
					return(StringToDouble(sval)) ;

			}

			throw new ApplicationException("unknown property type: " + (int) ptype) ;
		}


		//
		//
		//

		protected static double GetStreamFractionRead(Stream sin)
		{

			if (sin == null) return(0) ;
			if (sin.Length == 0) return(0) ;

			return(
				(double) sin.Position
			      / (double) sin.Length
			) ;

		}


		//
		//
		//

		private static double StringToDouble(string sval)
		{
#if	true	// make sure to keep counterpart in "FSRWriter" in sync!
			return(double.Parse(sval)) ;
#else
			/*
				Normally, we would use the following:

					double d= double.Parse(sval) ;

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
					BinaryWriter.Write(BitConverter.Int64BitsToDouble(long.Parse(sval))) ;

				And, this seems to solve the problem.  So, we use
				the following:

					double d= BitConverter.Int64BitsToDouble(long.Parse(sval)) ;

			*/

			return(BitConverter.Int64BitsToDouble(long.Parse(sval))) ;
#endif
		}

	}

}
