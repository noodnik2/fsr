/*

	FSRWriterXML.cs - Writer for creating FSR/XML (text) files
    Copyright (C) 2003-2004, Marty Ross


	This module provides an implementation of FSRWriter for creating a
	textual representation of an FSR file using a fixed XML structure.

*/


using System ;
using System.IO ;
using System.Xml ;

namespace fsrlib
{

	public class FSRWriterXML : FSRWriter
	{

		public FSRWriterXML(Stream os)
		{
			m_output= new XmlTextWriter(os, null) ;
			m_output.Formatting= Formatting.Indented ;
		}

		public override FSRWriter.Chunk Start()
		{
			m_output.WriteStartDocument() ;
			m_output.WriteDocType("FSRfile", null, null, null) ;
			return(null) ;
		}

		public override void End(FSRWriter.Chunk outStartChunk)
		{
			m_output.WriteEndDocument() ;
			m_output.Flush() ;
			m_output.Close() ;
		}

		public override FSRWriter.Chunk FsibStart()
		{
			m_output.WriteStartElement("fsib") ;
			return(null) ;
		}

		public override void FsibEnd(FSRWriter.Chunk outFsibChunk)
		{
			m_output.WriteEndElement() ;	// </fsib>
		}

		public override FSRWriter.Chunk FsibBfibStart()
		{
			m_output.WriteStartElement("bfib") ;
			return(null) ;
		}

		public override void FsibBfibEnd(FSRWriter.Chunk outBfibChunk)
		{
			m_output.WriteEndElement() ;	// </bfib>
		}

		public override FSRWriter.Chunk FsibBfibOdibStart()
		{
			m_output.WriteStartElement("odib") ;
			return(null) ;
		}

		public override void FsibBfibOdibObItem(ushort objid, string objname)
		{
			m_output.WriteStartElement("ob") ;
			m_output.WriteAttributeString("ob", objid.ToString()) ;
			m_output.WriteString(objname) ;
			m_output.WriteEndElement() ;
		}

		public override void FsibBfibOdibEnd(FSRWriter.Chunk outOdibChunk)
		{
			m_output.WriteEndElement() ;	// </odib>
		}

		public override FSRWriter.Chunk FsibBfibPdibStart()
		{
			m_output.WriteStartElement("pdib") ;
			return(null) ;
		}

		public override void FsibBfibPdibPrItem(
			ushort objid,
			ushort propid,
			ushort ptype,
			string unitname,
			string property
		) {
			m_output.WriteStartElement("pr") ;
			m_output.WriteAttributeString("ob", objid.ToString()) ;
			m_output.WriteAttributeString("pr", propid.ToString()) ;
			m_output.WriteAttributeString("pt", ptype.ToString()) ;
			m_output.WriteAttributeString("un", unitname) ;
			m_output.WriteString(property) ;
			m_output.WriteEndElement() ;
		}

		public override void FsibBfibPdibEnd(FSRWriter.Chunk outPdibChunk)
		{
			m_output.WriteEndElement() ;	// </pdib>
		}

		public override FSRWriter.Chunk FsibBfibFribStart()
		{
			m_output.WriteStartElement("frib") ;
			return(null) ;
		}

		public override void FsibBfibFribTsItem(uint frameno, uint tickcount, long timestamp)
		{
			m_output.WriteStartElement("ts") ;
			m_output.WriteAttributeString("fr", frameno.ToString()) ;
			m_output.WriteAttributeString("ti", tickcount.ToString()) ;
			m_output.WriteString(timestamp.ToString()) ;
			m_output.WriteEndElement() ;
		}

		public override void FsibBfibFribFrItem(ushort objid, ushort propid, ushort valtype, Object o)
		{
			m_output.WriteStartElement("fr") ;	
			m_output.WriteAttributeString("ob", objid.ToString()) ;
			m_output.WriteAttributeString("pr", propid.ToString()) ;
			m_output.WriteAttributeString("pt", valtype.ToString()) ;
			m_output.WriteString(FSRWriter.ObjectToString(o, (FSRPropertyDataType) valtype)) ;
			m_output.WriteEndElement() ;
		}

		public override void FsibBfibFribEnd(FSRWriter.Chunk outFribChunk)
		{
			m_output.WriteEndElement() ;	// </frib>
		}

		public override FSRWriter.Chunk FsibTrailerStart()
		{
			m_output.WriteStartElement("trailer") ;
			return(null) ;
		}

		public override void FsibTrailerDaItem(ushort wid, string sval)
		{
				m_output.WriteStartElement("da") ;
				m_output.WriteAttributeString("dt", wid.ToString()) ;
				m_output.WriteString(sval) ;
				m_output.WriteEndElement() ;
		}

		public override void FsibTrailerEnd(FSRWriter.Chunk outTrailerChunk)
		{
			m_output.WriteEndElement() ;	// </trailer>
		}


		/*
			private interface
		*/

		private XmlTextWriter m_output ;

	}

}
