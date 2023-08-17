/*

	FSRFile.cs - fsrlib supported file types
    Copyright (C) 2003-2004, Marty Ross

	This module defines the set of file types supported by fsrlib.

	File types are distinguished solely upon the basis of the "extension"
	specified in their filename.  These "extension" values are supplied
	externally via the "FSRMsgs" class using a key of ("EXT_" + name(type)).

*/

using System ;

namespace fsrlib
{

	public class FSRFile
	{

		public static Type GetType(string fn)
		{
			string lfn= fn.ToLower() ;

			Type[] types= GetTypeValues() ;
			foreach (Type t in types) {
				if (t == Type.Unknown) continue ;
				string extkey= "EXT_" + Enum.GetName(t.GetType(), t) ;
				string tname= FSRMsgs.Load(extkey).ToLower() ;
				if (lfn.EndsWith(tname)) return(t) ;
			}
			return(Type.Unknown) ;
		}

		public static Type[] GetTypeValues()
		{
			// there's gotta be a better way to do this!
			Type t= Type.Unknown ;
			return((Type[]) Enum.GetValues(t.GetType())) ;
		}

		public enum Type
		{
			Unknown,
			FSR,		// Microsoft Flight Simulator 2002 Video
			XML,		// XML representation of an .FSR file
			NMEA,		// GPS data stream in NMEA format
			PNL,		// Pathaway NMEA Log
			STAT,		// Statistical output format for analysis
			GPX,		// GPS eXchange Format (http://www.topografix.com/gpx.asp)
			CTL,		// Cetus Track Log
		}

	}

}
