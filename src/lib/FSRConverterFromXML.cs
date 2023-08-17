/*

	FSRConverterFromXML.cs - FSR Conversion from FSR/XML (text) format
    Copyright (C) 2003-2004, Marty Ross


	This module handles FSR conversions from FSR/XML (text) format.

*/


using System ;
using System.IO ;
using System.Xml ;

namespace fsrlib
{

	public class FSRConverterFromXML : FSRConverterFrom
	{

		/*
			public interface
		*/

		public override void Convert(Stream input, FSRWriter wrr)
		{
			Convert(new FSRReaderXML(input), wrr) ;
		}

		public override void Convert(FSRReader rdr, FSRWriter wrr)	// IFSRConverterFrom
		{
			Convert((FSRReaderXML) rdr, wrr) ;
		}

		public void Convert(FSRReaderXML rdr, FSRWriter wrr)
		{
			m_input= rdr ;
			m_output= wrr ;
			Convert() ;
		}


		/*
			private area
		*/

		void Convert()
		{
			bool bProcessed= false ;

			FSRWriter.Chunk fchunk= m_output.Start() ;
			while(m_input.NextElement()) {
				if (m_input.XmlTextReader.Name == "fsib") {
					if (bProcessed) {
						throw new ApplicationException("two 'fsib' elements found!") ;
					}
					bProcessed= true ;
					ConvertFsib() ;
					continue ;
				}
				IgnoredWarning("root element", m_input.XmlTextReader.Name) ;
				m_input.XmlTextReader.Skip() ;
			}
			m_output.End(fchunk) ;
		}

		void ConvertFsib()
		{
			if (!m_input.XmlTextReader.Read()) return ;

			//Logger.LogDebug("Converting fsib") ;

			FSRWriter.Chunk fchunk= m_output.FsibStart() ;
			while(m_input.NextElement()) {
				// Logger.LogDebug("parseFSIB.Read: " + m_input.NodeType.ToString() + ":" + m_input.Name) ;
				if (m_input.XmlTextReader.Name == "bfib") {
					ConvertFsibBfib() ;
					continue ;
				}
				if (m_input.XmlTextReader.Name == "trailer") {
					ConvertFsibTrailer() ;
					continue ;
				}
				IgnoredWarning("fsib element", m_input.XmlTextReader.Name) ;
				m_input.XmlTextReader.Skip() ;
			}
			m_output.FsibEnd(fchunk) ;
		}

		void ConvertFsibBfib()
		{
			if (!m_input.XmlTextReader.Read()) return ;

			//Logger.LogDebug("Parsing fsib.bfib") ;
			FSRWriter.Chunk fchunk= m_output.FsibBfibStart() ;
			while(m_input.NextElement()) {
				DoProgressUpdate(m_input) ;
				if (m_input.XmlTextReader.Name == "odib") {
					ConvertFsibBfibOdib() ;
					continue ;
				}
				if (m_input.XmlTextReader.Name == "pdib") {
					ConvertFsibBfibPdib() ;
					continue ;
				}
				if (m_input.XmlTextReader.Name == "frib") {
					ConvertFsibBfibFrib() ;
					continue ;
				}
				IgnoredWarning("bfib element", m_input.XmlTextReader.Name) ;
				m_input.XmlTextReader.Skip() ;
			}
			m_output.FsibBfibEnd(fchunk) ;
		}

		void ConvertFsibBfibOdib()
		{
			//Logger.LogDebug("Parsing fsib.bfib.odib") ;
			m_input.XmlTextReader.Read() ;
			FSRWriter.Chunk fchunk= m_output.FsibBfibOdibStart() ;
			while(m_input.NextElement()) {

				if (m_input.XmlTextReader.Name != "ob") {
					IgnoredWarning("bfib.odib element", m_input.XmlTextReader.Name) ;
					m_input.XmlTextReader.Skip() ;
					continue ;
				}

				if (!m_input.XmlTextReader.MoveToAttribute("ob")) {
					throw new ApplicationException("missing 'od' attribute") ;
				}

				ushort oid= ushort.Parse(m_input.XmlTextReader.Value) ;
				while(m_input.XmlTextReader.MoveToNextAttribute()) ;
				m_input.XmlTextReader.Read() ; // skip to string - should we check "NodeType" here?
				string oname= m_input.XmlTextReader.ReadString() ;

				//Logger.LogDebug("ob=" + oid + ", name='" + oname + "'") ;

				m_output.FsibBfibOdibObItem(oid, oname) ;
				m_input.XmlTextReader.Skip() ;
			}
			m_output.FsibBfibOdibEnd(fchunk) ;
		}

		void ConvertFsibBfibPdib()
		{
			if (!m_input.XmlTextReader.Read()) return ;

			//Logger.LogDebug("Parsing fsib.bfib.pdib") ;
			FSRWriter.Chunk fchunk= m_output.FsibBfibPdibStart() ;
			while(m_input.NextElement()) {
				if (m_input.XmlTextReader.Name == "pr") {
					ConvertFsibBfibPdibPr() ;
					continue ;
				}
				IgnoredWarning("pdib attribute", m_input.XmlTextReader.Name) ;
				m_input.XmlTextReader.Skip() ;
			}
			m_output.FsibBfibPdibEnd(fchunk) ;
		}

		void ConvertFsibBfibPdibPr()
		{
			// Logger.LogDebug("Parsing fsib.bfib.pdib.pr") ;
			ushort oid= 0, pid= 0, tid= 0 ;
			string punits= "" ;
			while(m_input.XmlTextReader.MoveToNextAttribute()) {
				if (m_input.XmlTextReader.Name == "ob") {
					oid= ushort.Parse(m_input.XmlTextReader.Value) ;
					continue ;
				}
				if (m_input.XmlTextReader.Name == "pr") {
					pid= ushort.Parse(m_input.XmlTextReader.Value) ;
					continue ;
				}
				if (m_input.XmlTextReader.Name == "pt") {
					tid= ushort.Parse(m_input.XmlTextReader.Value) ;
					continue ;
				}
				if (m_input.XmlTextReader.Name == "un") {
					punits= m_input.XmlTextReader.Value ;
					continue ;
				}
				IgnoredWarning("pdib.pr attribute", m_input.XmlTextReader.Name) ;
			}
			m_input.XmlTextReader.Read() ;

			string pname= m_input.XmlTextReader.ReadString() ;

			//Logger.LogDebug("fsib.bfib.pdib.pr: pname='" + pname + "', ob=" + oid + ", pid=" + pid + ", tid=" + tid + ", un='" + punits + "'") ;
			m_output.FsibBfibPdibPrItem(oid, pid, tid, punits, pname) ;

			m_input.XmlTextReader.Skip() ;
		}

		void ConvertFsibBfibFrib()
		{
			if (!m_input.XmlTextReader.Read()) return ;

			//Logger.LogDebug("Parsing fsib.bfib.frib") ;
			FSRWriter.Chunk fchunk= m_output.FsibBfibFribStart() ;
			while(m_input.NextElement()) {
				if (m_input.XmlTextReader.Name == "ts") {
					ConvertFsibBfibFribTs() ;
					continue ;
				}
				if (m_input.XmlTextReader.Name == "fr") {
					ConvertFsibBfibFribFr() ;
					continue ;
				}
				IgnoredWarning("frib attribute", m_input.XmlTextReader.Name) ;
				m_input.XmlTextReader.Skip() ;
			}
			m_output.FsibBfibFribEnd(fchunk) ;
		}

		void ConvertFsibBfibFribTs()
		{
			// Logger.LogDebug("Parsing fsib.bfib.frib.ts") ;
			uint frno= 0, ticks= 0 ;
			while(m_input.XmlTextReader.MoveToNextAttribute()) {
				if (m_input.XmlTextReader.Name == "fr") {
					frno= uint.Parse(m_input.XmlTextReader.Value) ;
					continue ;
				}
				if (m_input.XmlTextReader.Name == "ti") {
					ticks= uint.Parse(m_input.XmlTextReader.Value) ;
					continue ;
				}
				IgnoredWarning("frib.ts attribute", m_input.XmlTextReader.Name) ;
			}
			m_input.XmlTextReader.Read() ;

			long timeval= long.Parse(m_input.XmlTextReader.ReadString()) ;
			//Logger.LogDebug("fsib.bfib.frib.ts: timeval='" + timeval + "', frno=" + frno + ", ticks=" + ticks) ;
			m_output.FsibBfibFribTsItem(frno, ticks, timeval) ;

			m_input.XmlTextReader.Skip() ;
		}

		void ConvertFsibBfibFribFr()
		{
			// Logger.LogDebug("Parsing fsib.bfib.frib.fr") ;
			ushort oid= 0, pid= 0, tid= 0 ;

			while(m_input.XmlTextReader.MoveToNextAttribute()) {
				if (m_input.XmlTextReader.Name == "ob") {
					oid= ushort.Parse(m_input.XmlTextReader.Value) ;
					continue ;
				}
				if (m_input.XmlTextReader.Name == "pr") {
					pid= ushort.Parse(m_input.XmlTextReader.Value) ;
					continue ;
				}
				if (m_input.XmlTextReader.Name == "pt") {
					tid= ushort.Parse(m_input.XmlTextReader.Value) ;
					continue ;
				}
				IgnoredWarning("frib.fr attribute", m_input.XmlTextReader.Name) ;
			}
			m_input.XmlTextReader.Read() ;

			string sval= m_input.XmlTextReader.ReadString() ;
			Object o= FSRReader.StringToObject(sval, (FSRPropertyDataType) tid) ;

			//Logger.LogDebug("fsib.bfib.frib.fr: fval='" + fval + "', ob=" + oid + ", pid=" + pid + ", tid=" + tid) ;
			m_output.FsibBfibFribFrItem(oid, pid, tid, o) ;

			m_input.XmlTextReader.Skip() ;
		}

		void ConvertFsibTrailer()
		{
			if (!m_input.XmlTextReader.Read()) return ;

			//Logger.LogDebug("Converting fsib.trailer") ;
			FSRWriter.Chunk fchunk= m_output.FsibTrailerStart() ;
			while(m_input.NextElement()) {
				if (m_input.XmlTextReader.Name == "da") {
					ConvertFsibTrailerDa() ;
					continue ;
				}
				IgnoredWarning("trailer element", m_input.XmlTextReader.Name) ;
				m_input.XmlTextReader.Skip() ;
			}
			m_output.FsibTrailerEnd(fchunk) ;
		}

		void ConvertFsibTrailerDa()
		{
			// Logger.LogDebug("Parsing fsib.trailer.da") ;
			ushort dtype= 0 ;
			while(m_input.XmlTextReader.MoveToNextAttribute()) {
				if (m_input.XmlTextReader.Name == "dt") {
					dtype= ushort.Parse(m_input.XmlTextReader.Value) ;
					continue ;
				}
				IgnoredWarning("trailer attribute", m_input.XmlTextReader.Name) ;
			}
			m_input.XmlTextReader.Read() ;

			string dval= "" ;
			if (m_input.XmlTextReader.NodeType == XmlNodeType.Text) {
				dval= m_input.XmlTextReader.ReadString() ;
			}

			//Logger.LogDebug("fsib.trailer.da: dt=" + dtype + ", dval='" + dval + "'") ;
			m_output.FsibTrailerDaItem(dtype, dval) ;

			m_input.XmlTextReader.Skip() ;
		}

		void IgnoredWarning(string msgWhatIs, string sValue)
		{
			Logger.LogWarning(
				"FSRConverterFromXML: unknown "
			      + msgWhatIs
			      + " skipped: '"
			      + sValue
			      + "'"
			) ;
		}


		/*
			private data
		*/

		FSRReaderXML m_input ;
		FSRWriter m_output ;

	}
}
