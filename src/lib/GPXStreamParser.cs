/*

	GPXStreamParser.cs - GPS eXchange Format Parser
    Copyright (C) 2004, Marty Ross

	Provides a simple parser to extract and deliver the minimum values
	necessary for the "FSRConverterFromGPSStream" object from a GPX file
	(via a "DatumHandlerDelegate", in the format of a "StreamDatum",
	defined here).

	For more information about GPX file format, see:

		http://www.topografix.com/gpx.asp

	This parser tested only with GPX data produced by Cetus Tracklog
	utility, currently available at:

		http://www.cetusgps.dk/download

	NOTES:

	(1)	(January, 2004)
		While testing this code using GPX files obtained using the
		Cetus "tracklog" conversion program (converts GPS track logs
		from PalmOS .PDB format to .GPX format), incorrect "speed" and
		"altitude" output values were observed.  This problem has not
		yet been investigated; therefore, this code has currently not
		been validated to work.

*/

using System ;
using System.IO ;
using System.Xml ;

namespace fsrlib
{

	public class GPXStreamParser : StreamParser
	{

		public enum FixType
		{
			FixNone,
			Fix2D,
			Fix3D,
		} ;

		public struct StreamDatum
		{
			public DateTime TimeUTC ;
			public double Lat ;
			public double Lon ;
			public double AltFeet ;
			public double SpeedKnots ;
			public double CourseTrueDeg ;
			public double MagVarDeg ;
			public double hdop ;
			public double vdop ;
			public double nsat ;
			public FixType fix ;
		} ;

		public delegate void DatumHandlerDelegate(StreamDatum d) ;

		public void SetDatumHandler(DatumHandlerDelegate d)
		{
			m_dhandler= d ;
		}

		public override void Parse(Stream s)
		{
			XmlTextReader r= new XmlTextReader(s) ;
			r.WhitespaceHandling= WhitespaceHandling.None ;

			if (NextElement(r) != "gpx") {
				throw new ApplicationException(
					FSRMsgs.Load("StatusInvalidFile")
				) ;
			}

			for (r.Read(); NextElement(r) != null; r.Skip()) {
				if (r.Name != "trk") continue ;

				for (r.Read(); NextElement(r) != null; r.Skip()) {
					if (r.Name != "trkseg") continue ;

					for (r.Read() ;NextElement(r) != null; r.Skip()) {
						if (r.Name != "trkpt") continue ;

						ParseTrkpt(r) ;
					}
				}
			}
		}



		//
		//
		//

		void ParseTrkpt(XmlTextReader r)
		{

			StreamDatum d= new StreamDatum() ;

			while(r.MoveToNextAttribute()) {
				if (r.Name == "lat") {
					d.Lat= double.Parse(r.Value) ;
					continue ;
				}
				if (r.Name == "lon") {
					d.Lon= double.Parse(r.Value) ;
					continue ;
				}
			}

			for (r.Read(); NextElement(r) != null; r.Skip()) {
				switch(r.Name) {

					case "ele":
						// GPX units: meters
						d.AltFeet= double.Parse(r.ReadString()) ;
						d.AltFeet*= FSRGlobals.FeetPerMeter ;	// meters to feet
						break ;

					case "time":
						// GPX units: UTC; Conforms to ISO 8601
						d.TimeUTC= DateTime.Parse(r.ReadString()) ;
						break ;
				
					case "course":
						// GPX units: degrees, true
						d.CourseTrueDeg= double.Parse(r.ReadString()) ;
						break ;

					case "speed":
						// GPX units: meters per second
						d.SpeedKnots= double.Parse(r.ReadString()) ;
						d.SpeedKnots*= 3600.0 ;	// => meters per hour
						d.SpeedKnots/= FSRGlobals.MetersPerNauticalMile ;	// => knots
						break ;

					case "magvar":
						// GPX units: degrees
						// field is optional
						d.MagVarDeg= double.Parse(r.ReadString()) ;
						break ;

					case "hdop":
						d.hdop= double.Parse(r.ReadString()) ;
						break ;

					case "vdop":
						d.vdop= double.Parse(r.ReadString()) ;
						break ;

					case "sat":
						d.nsat= double.Parse(r.ReadString()) ;
						break ;

					case "fix":
						switch(r.ReadString()) {
							case "none":
								d.fix= FixType.FixNone ;
								break ;
							case "2d":
								d.fix= FixType.Fix2D ;
								break ;
							case "3d":
								d.fix= FixType.Fix3D ;
								break ;
							// should we handle "dgps", "pps", etc.??
						}
						break ;

				}
			}

			m_dhandler(d) ;
		}

		static string NextElement(XmlTextReader r)
		{
			if (r.NodeType == XmlNodeType.EndElement) {
				return(null) ;
			}
			r.MoveToContent() ;
			if (r.NodeType == XmlNodeType.EndElement) {
				if (!r.Read()) return(null) ;
			}
			if (r.NodeType != XmlNodeType.Element) {
				return(null) ;
			}
			return(r.Name) ;
		}

		private DatumHandlerDelegate m_dhandler= null ;

	}

}

