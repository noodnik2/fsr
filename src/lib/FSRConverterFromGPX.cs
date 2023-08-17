
/*

	FSRConverterFromGPX.cs - FSR Conversion from GPX format
    Copyright (C) 2003-2004, Marty Ross

	This module handles FSR conversions from GPS eXchange (GPX) data streams.

	See source for GPXStreamParser for more information;

*/


using System ;
using System.IO ;

namespace fsrlib
{

	public class FSRConverterFromGPX : FSRConverterFromGPSStream
	{

		//
		//	Constructors
		//

		public FSRConverterFromGPX(AuxiliaryData auxdata)
		{
			Construct(auxdata, new GPSStreamParserFromGPX(this)) ;
		}


		//
		//	"IFSRConverterFrom" implementation
		//

		public override void Convert(Stream input, FSRWriter wrr)
		{
			Convert(new FSRReaderStreamParser(input), wrr) ;
		}

	}

	class GPSStreamParserFromGPX : GPXStreamParser
	{

		public GPSStreamParserFromGPX(IFSRPropertyValueStream pvs)
		{
			m_pvs= pvs ;
			SetDatumHandler(new DatumHandlerDelegate(DatumHandler)) ;
		}

		public void DatumHandler(GPXStreamParser.StreamDatum sd)
		{

			// Logger.LogNote(sd.Time.ToString()) ;

			// NOTE: perhaps we should discard any datum whose "fix" value is not "Fix3D"??

			m_pvs.WritePropertyValue(
				(int) FSRPropertyId.PLANE_LATITUDE,
				(double) sd.Lat
			) ;

			m_pvs.WritePropertyValue(
				(int) FSRPropertyId.PLANE_LONGITUDE,
				(double) sd.Lon
			) ;

			m_pvs.WritePropertyValue(
				(int) FSRPropertyId.PLANE_ALTITUDE,
				(double) sd.AltFeet
			) ;

			m_pvs.WritePropertyValue(
				(int) FSRPropertyId.ABSOLUTE_TIME,
				(double) (sd.TimeUTC.Ticks / 10000000.0)
			) ;

			m_pvs.WritePropertyValue(
				(int) FSRPropertyId.AIRSPEED_INDICATED,
				(double) sd.SpeedKnots
			) ;

			m_pvs.WritePropertyValue(
				(int) FSRPropertyId.PLANE_HEADING_DEGREES_TRUE,
				(double) sd.CourseTrueDeg
			) ;

#if !NoMagHeadingGlobal
			// TODO: fix bug - this doesn't output magnetic heading when magnetic variance really == 0
			if (sd.MagVarDeg != 0) {
				m_pvs.WritePropertyValue(
					(int) FSRPropertyId.PLANE_HEADING_DEGREES_MAGNETIC,
					(double) (sd.CourseTrueDeg + sd.MagVarDeg)
				) ;
			}
#endif

			m_pvs.FlushPropertyValues() ;
		}

		public IFSRPropertyValueStream ValueStream { get { return(m_pvs) ; } }				

		private IFSRPropertyValueStream m_pvs ;

	}


}
