/*

	FSRConverterFromNMEA.cs - FSR Conversion from NMEA Stream
    Copyright (C) 2003-2004, Marty Ross

	This module handles FSR conversions from NMEA data streams.

*/


using System ;
using System.IO ;

namespace fsrlib
{

	public class FSRConverterFromNMEA : FSRConverterFromGPSStream
	{

		//
		//	Constructors
		//

		public FSRConverterFromNMEA(AuxiliaryData auxdata)
		{
			Construct(auxdata, new GpsStreamParserFromNMEA(this)) ;
		}


		//
		//	"IFSRConverterFrom" implementation
		//

		public override void Convert(Stream input, FSRWriter wrr)
		{
			Convert(new FSRReaderStreamParser(input), wrr) ;
		}


	}

	abstract class GpsDataHandlerFromNMEA : NmeaDataHandler
	{

		public GpsDataHandlerFromNMEA(
			GpsStreamParserFromNMEA sp,
			string sCommand
		)	: base(sCommand)
		{
			m_gpsstreamparser= sp ;
			m_gpsstreamparser.AddDataHandler(this) ;
		}


		//
		//	expose GpsStreamParser object to handlers
		//
		public GpsStreamParserFromNMEA StreamParser { get { return(m_gpsstreamparser) ; } }


		//
		//	utility parse functions for derived classes to use...
		//

		protected double MaxPrecision(double x, double p)
		{
			long xl= (long) (x * p) ;
			return(((double) xl) / p) ;
		}

		protected bool ParseIntRef(string sNum, ref int i)
		{
			i= int.Parse(sNum) ;
			return(true) ;
		}

		protected bool ParseDoubleRef(string sDouble, ref double dbl)
		{
			dbl= double.Parse(sDouble) ;
			return(true) ;
		}

		protected bool ParseVariationRef(string[] sArgs, int sPairIndex, ref double variation)
		{
			variation= double.Parse(sArgs[sPairIndex]) ;
			if (sArgs[sPairIndex+1] == "W") {
				variation= -variation ;
			}
			return(true) ;
		}

		protected bool ParseLatitudeTupleRef(string[] sArgs, int sPairIndex, ref double latitude)
		{
			latitude= double.Parse(sArgs[sPairIndex].Substring(2)) / 60.0 ;
			latitude+= double.Parse(sArgs[sPairIndex].Substring(0, 2)) ;
			if (sArgs[sPairIndex+1] == "S") {
				latitude= -latitude ;
			}
			return(true) ;
		}

		protected bool ParseLongitudeTupleRef(string[] sArgs, int sPairIndex, ref double longitude)
		{
			longitude= double.Parse(sArgs[sPairIndex].Substring(3)) / 60.0 ;
			longitude+= double.Parse(sArgs[sPairIndex].Substring(0, 3)) ;
			if (sArgs[sPairIndex+1] == "W") {
				longitude= -longitude ;
			}
			return(true) ;
		}


		//
		//	private stuff
		//

		private GpsStreamParserFromNMEA m_gpsstreamparser ;

	}

	abstract class GpsDataTimeLatLonHandlerFromNMEA : GpsDataHandlerFromNMEA
	{
		public GpsDataTimeLatLonHandlerFromNMEA(
			GpsStreamParserFromNMEA sp,
			string sCommand
		) :	base(sp, sCommand)
		{
		}

		public double Latitude { get { return(m_latitude) ; } }
		public double Longitude { get { return(m_longitude) ; } }
		public double UTCTime { get { return(m_utctime) ; } }	// HHMMSS.MS format
		public double UTCDate { get { return(m_utcdate) ; } }	// DDMMYY format

		/// <summary>
		/// Returns the number of seconds since a given point in time
		/// corresponding to the the "m_utctime" and "m_utcdate" attributes.
		/// This value corresponds to the "ABSOLUTE_TIME" FS parameter.
		/// </summary>
		/// <returns></returns>
		public double GetDateTimeSeconds()
		{

			// hour: The hours (0 through 23).
			int ihr= (int) (m_utctime / 10000.0) ;
		
			// minute: The minutes (0 through 59).
			int imin= (int) ((m_utctime / 100.0) % 100) ;	

			// second: The seconds (0 through 59).
			int isec= (int) (m_utctime % 100) ;

			// millisecond: The milliseconds.
			int imsec= (int) ((m_utctime * 1000) % 1000) ;

			// day: The day (1 through the number of days in month).
			int idy= (int) (m_utcdate / 10000.0) ;

			// month: The month (1 through 12).
			int imo= (int) ((m_utcdate / 100.0) % 100) ;

			// year: The year (1 through 9999).
			int iyr= (int) (m_utcdate % 100) ;

			// "Y2K" patch for year: any year < '90 is in century 2000
			iyr+= (iyr< 90)? 2000: 1900 ;
			
			DateTime then= new DateTime(
				iyr,
				imo,
				idy,
				ihr,
				imin,
				isec,
				imsec
			) ;

			double atime= ((double) then.Ticks) / ((double) 10000000.0)  ;

			return(atime) ;
		}

		protected double m_utctime ;
		protected double m_utcdate ;
		protected double m_latitude ;
		protected double m_longitude ;

	}

	/// <summary>
	/// We get altitude information from the GPGGA sentence.
	/// </summary>
	class GpggaHandler : GpsDataTimeLatLonHandlerFromNMEA
	{
		public GpggaHandler(GpsStreamParserFromNMEA sp) : base(sp, "GPGGA")
		{
		}

		public override string HandleData(string[] sArgs)
		{
			if (sArgs.Length < 11) {
				return("too few arguments: " + sArgs.Length + " < 11") ;
			}

			if (			
				!ParseDoubleRef(sArgs[0], ref m_utctime)
			||	!ParseLatitudeTupleRef(sArgs, 1, ref m_latitude)
			||	!ParseLongitudeTupleRef(sArgs, 3, ref m_longitude)
			||	!ParseIntRef(sArgs[5], ref m_fixquality)
			||	!ParseIntRef(sArgs[6], ref m_nsatellites)
			||	!ParseMeterAltitudeTupleRef(sArgs, 8, ref m_altitudeMeters)
			) {
				return("data parse error") ;
			}

			SetIsValid((m_fixquality> 0)? true: false) ;

			if (IsValid) {
				StreamParser.ValueStream.WritePropertyValue(
					(int) FSRPropertyId.PLANE_ALTITUDE,
					(double) (AltitudeMeters * FSRGlobals.FeetPerMeter)	// convert meters to feet
				) ;
			}

			return(null) ;
		}

		public double AltitudeMeters { get { return(m_altitudeMeters) ; } }
		public int NumberSatellites { get { return(m_nsatellites) ; } }
		public int FixQuality { get { return(m_fixquality) ; } }

		protected bool ParseMeterAltitudeTupleRef(string[] sArgs, int iPairIndex, ref double alt)
		{
			if (sArgs[iPairIndex+1] != "M") {
				return(false) ;
			}
			alt= double.Parse(sArgs[iPairIndex]) ;
			return(true) ;
		}

		private double m_altitudeMeters ;
		private int m_nsatellites ;
		private int m_fixquality ;
	}

	/// <summary>
	/// We get Lat, Lon, Heading True, Heading Magnetic, Time, and Speed
	/// from the "GPRMC" sentence.  We assume that GPS Speed (groundspeed)
	/// is the same as airspeed (which is what we return).  This is obv-
	/// iously not correct, as normally there will be a wind that will
	/// affect the groundspeed.  There should be a wind direction and
	/// speed parameter that we could use to calculate the airspeed.
	/// </summary>
	class GprmcHandler : GpsDataTimeLatLonHandlerFromNMEA
	{
		public GprmcHandler(GpsStreamParserFromNMEA sp) : base(sp, "GPRMC")
		{
		}

		public override string HandleData(string[] sArgs)
		{
			if (sArgs.Length< 10) {
				return("too few arguments: " + sArgs.Length + " < 10") ;
			}

			if (			
				!ParseDoubleRef(sArgs[0], ref m_utctime)
			||	!ParseLatitudeTupleRef(sArgs, 2, ref m_latitude)
			||	!ParseLongitudeTupleRef(sArgs, 4, ref m_longitude)
			||	!ParseDoubleRef(sArgs[6], ref m_speedKnots)
			||	!ParseDoubleRef(sArgs[7], ref m_courseTrue)
			||	!ParseDoubleRef(sArgs[8], ref m_utcdate)
			||	!ParseVariationRef(sArgs, 9, ref m_variation)
			) {
				return("data parse error") ;
			}

			m_validity= sArgs[1] ;

			SetIsValid((m_validity == "A")? true: false) ;

			if (IsValid) {

				StreamParser.ValueStream.WritePropertyValue(
					(int) FSRPropertyId.PLANE_LATITUDE,
					(double) Latitude
				) ;
				StreamParser.ValueStream.WritePropertyValue(
					(int) FSRPropertyId.PLANE_LONGITUDE,
					(double) Longitude
				) ;
				StreamParser.ValueStream.WritePropertyValue(
					(int) FSRPropertyId.ABSOLUTE_TIME,
					(double) GetDateTimeSeconds()
				) ;
				StreamParser.ValueStream.WritePropertyValue(
					(int) FSRPropertyId.AIRSPEED_INDICATED,
					(double) SpeedKnots
				) ;
				StreamParser.ValueStream.WritePropertyValue(
					(int) FSRPropertyId.PLANE_HEADING_DEGREES_TRUE,
					(double) CourseTrue
				) ;
#if !NoMagHeadingGlobal
				StreamParser.ValueStream.WritePropertyValue(
					(int) FSRPropertyId.PLANE_HEADING_DEGREES_MAGNETIC,
					(double) CourseMagnetic
				) ;
#endif
				StreamParser.ValueStream.FlushPropertyValues() ;
			}

			return(null) ;
		}

		public double SpeedKnots { get { return(m_speedKnots) ; } }
		public double CourseTrue { get { return(m_courseTrue) ; } } 

#if !NoMagHeadingGlobal
		public double CourseMagnetic {
			get {
				double cm= m_courseTrue - m_variation ;
				if (cm< 0) cm+= 360.0 ;
				return(cm) ;
			}
		} 
#endif

		public double Variation { get { return(m_variation) ; } } 
		public string Validity { get { return(m_validity) ; } }

		private double m_speedKnots ;
		private double m_courseTrue ;
		private double m_variation ;
		private string m_validity ;

	}

	class GpsStreamParserFromNMEA : NmeaStreamParser
	{

		public GpsStreamParserFromNMEA(IFSRPropertyValueStream pvs)
		{
			m_pvs= pvs ;

			//
			// create and register GPS command handlers
			//
			m_hgprmc= new GprmcHandler(this) ;
			m_hgpgga= new GpggaHandler(this) ;
		}

		public override void HandleError(string errMsg)
		{
			Logger.LogError("parser error: " + errMsg) ;
		}

		public IFSRPropertyValueStream ValueStream { get { return(m_pvs) ; } }				

		private GpggaHandler m_hgpgga ;
		private GprmcHandler m_hgprmc ;

		private IFSRPropertyValueStream m_pvs ;

	}

}
