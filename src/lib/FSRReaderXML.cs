/*

	FSRReaderXML.cs - Helper for reading MSFS FSR XML Files
    Copyright (C) 2003-2004, Marty Ross

*/

using System ;
using System.IO ;
using System.Xml ;

namespace fsrlib
{

	public class FSRReaderXML : FSRReader
	{

		/*
			public area
		*/

		public FSRReaderXML(Stream sin)
		{
			m_sin= sin ;
			m_input= new XmlTextReader(m_sin) ;
		}

		public override void Close()
		{
			m_input.Close() ;
		}

		public override double FractionRead {
			get {
				return(GetStreamFractionRead(m_sin)) ;
			}
		}

		public bool NextElement()
		{
			m_input.MoveToContent() ;
			if (m_input.NodeType == XmlNodeType.EndElement) {
				if (!m_input.Read()) return(false) ;
			}
			return(m_input.NodeType == XmlNodeType.Element) ;
		}

		//
		//	XmlTextReader property:
		//		Allow client access to underlying XmlTextReader stream.
		//
		public XmlTextReader XmlTextReader { get { return(m_input) ; } }


		/*
			private area
		*/

		private Stream m_sin ;
		private XmlTextReader m_input ;

	}
}
