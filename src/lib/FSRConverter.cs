/*

	FSRConverter.cs - fsrlib Converter Helper
    Copyright (C) 2003-2004, Marty Ross


	This class provides the main static helper method used for
	handling all fsrlib conversion operations.

*/

using System ;
using System.IO ;

namespace fsrlib
{

	public class FSRConverter
	{
		public static void Convert(
			FSRConverterFrom conv,
			Stream input,
			FSRWriter wrr,
			FSRProgressUpdateDelegate updateDelegate
		) {
			conv.Register(updateDelegate) ;
			conv.Convert(input, wrr) ;
		}
	}

}
