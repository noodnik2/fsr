/*

	FSRReaderStreamParser.cs - Reader for FSR-class input stream parsing
    Copyright (C) 2003-2004, Marty Ross


	This module defines a class to extend FSRReader for input streams
	that can be parsed using a parser of type StreamParser.

*/

using System ;
using System.IO ;

namespace fsrlib
{

	/// <summary>
	/// FSRReader for streams to be parsed using
	/// pull model with supplied parser object.
	/// </summary>
	public class FSRReaderStreamParser : FSRReader
	{

		/*
			public area
		*/

		public FSRReaderStreamParser(Stream sin)
		{
			m_input= sin ;
		}

		public void Parse(StreamParser parser)
		{
			parser.Parse(m_input) ;
		}

		public override void Close()
		{
			m_input.Close() ;
		}

		public override double FractionRead
		{
			get {
				return(GetStreamFractionRead(m_input)) ;
			}
		}

		/*
			protected area
		*/

		protected Stream m_input ;

	}

}
