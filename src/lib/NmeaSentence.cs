/*

	NmeaSentence.cs - Base class for storing NMEA Sentences
    Copyright (C) 2003-2004, Marty Ross


	This module defines a class for storing an NMEA sentence.

	The sentence is divided into "command" and "data" portions.  The "command"
	string serves to identify the NMEA sentence, and tha "data" string provides
	the detailed parameter values, specific to the NMEA "command".

	The "checksum" attribute is intended to reflect the validity of the
	sentence: true if the sentence was validated (e.g. matches the checksum
	value delivered by the GPS) or false if the sentence has not been validated.

*/


using System ;

namespace fsrlib
{

	public class NmeaSentence
	{

		public string Command {
			get { return(m_command) ; }
		}

		public string Data {
			get { return(m_data) ; }
		}

		public bool IsChecksummed {
			get { return(m_checksummed) ; }
		}

		internal string m_command ;
		internal string m_data ;
		internal bool m_checksummed ;
	}


}
