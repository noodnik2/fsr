/*

	FSRConverterFromCTL.cs - Cetus Track Log => MSFS2K Video Conversion
    Copyright (C) 2003, Marty Ross

	Conversion from Cetus Track Log to Microsoft Flight Simulator 2002 Video.

	See source for class: CTLStreamParser for more information.

*/


using System ;
using System.IO ;

namespace fsrlib
{

	public class FSRConverterFromCTL : FSRConverterFromGPSStream
	{

		//
		//	Constructors
		//

		public FSRConverterFromCTL(AuxiliaryData auxdata)
		{
			Construct(auxdata, new GPSStreamParserFromCTL(this)) ;
		}


		//
		//	"IFSRConverterFrom" implementation
		//

		public override void Convert(Stream input, FSRWriter wrr)
		{
			Convert(new FSRReaderStreamParser(input), wrr) ;
		}

	}

	class GPSStreamParserFromCTL : CTLStreamParser
	{

		public GPSStreamParserFromCTL(IFSRPropertyValueStream pvs)
		{
			m_pvs= pvs ;
			SetDatumHandler(new DatumHandlerDelegate(DatumHandler)) ;
		}

		public void DatumHandler(CTLStreamParser.StreamDatum sd)
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

#if !NoMagHeading
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
