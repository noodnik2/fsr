/*

	NmeaDataHandler.cs - Base class for parsing NMEA Sentence Data
    Copyright (C) 2003-2004, Marty Ross


	This module defines a base class for parsing a particular NMEA sentence.

	Derived classes must implement the "HandleData(string[])" method, responsible
	for parsing the "data" part of the NMEA sentence (the part after the command
	name).  The data are only delivered if the sentence is complete (e.g. checksum
	was correct).

	The "IsValid" attribute and the "SetIsValid()" method can be used by derived
	class(es) to convey the parsed state of the sentence data.  The "valid" state
	is initially set to "false".

*/


using System ;

namespace fsrlib
{

	public abstract class NmeaDataHandler
	{
		
		public NmeaDataHandler(string sCommand)
		{
			m_command= sCommand ;
			m_isvalid= false ;
		}

		public abstract string HandleData(string[] sArgs) ;

		public bool IsValid { get { return(m_isvalid) ; } } 
		public string Command { get { return(m_command) ; } }

		protected void SetIsValid(bool v) { m_isvalid= v ; }

		private bool m_isvalid ;
		private string m_command ;
	}

}
