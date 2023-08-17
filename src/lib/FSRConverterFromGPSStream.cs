#define	ForceLatLonAlt		// Always output Lat, Lon and Alt on every data frame
					// (ForceLatLonAlt keeps plane from wandering when not moving)
//#define	ForceEverything		// Always output all parameter values
//#define	NoMagHeadingGlobal	// Do not calculate/send "Magnetic Heading"; Sim will do for us

//#define	NoSendBankPitch		// Do not calculate/send bank or pitch

//#define	NoSendAirspeed		// does not appear to make a difference 


/*

	FSRConverterFromGPSStream.cs - GPS Stream => MSFS2K Video Conversion
    Copyright (C) 2003-2004, Marty Ross


	Conversion from GPS data stream to Microsoft Flight Simulator 2002 Video.

	This module provides direct and simple translation of "track" values obtained
	from an external source (such as time, latitude, longitude, altitude, speed and
	course) into MSFS2K "Video" format files (as defined in Microsoft's "Netpipes
	SDK" which is currently available on their website, at:
	http://www.microsoft.com/games/flightsimulator/fs2002_downloads_sdk.asp

	By [... the translation is] "direct and simple", it is meant that only
	simulator parameters that can easily be calculated or imputed on the fly
	(e.g., without buffering) during a single pass of the external data stream
	are considered.  Parameters such as "pitch", "bank" and "throttle setting"
	(changes in whose values produce a lagged effect in an airplane) should
	not be imputed, as any meaningful value derived for these parameters from
	external data would require looking ahead (e.g. to observe the lagged effect)
	which could be accomplished by buffering or muliple passes on the external
	data stream.


	NOTES:

	(1)	Notwithstanding the comment above about not attempting to impute
		airplane's control inputs from their (lagged) effects, it was felt
		that watching an airplane ascend/descent and turn without any
		pitch or bank changes was just TOO strange.  Therefore, an attempt
		to impute pitch and bank was added to the code, and is inherently
		incorrect due to disregarding the lag effect.  However, it improves
		the visual appearance of the flight enough that I left it in.

	(2)	There is no "ground awareness" currently in the translation.  This
		means that the simulated airplane will be invariably interacting
		with the ground (e.g. parts of the plane will actually be shown
		underneath the surface of the ground) due to inaccuracies and
		variability of the vertical component of the GPS data.  Therefore,
		it is imperative to disable the "realism" setting in the simulator
		that will result in a resetting of the flight upon "crashing" into
		the ground, or else it will not be possible to play back sections
		of your flight(s) that are near to the ground level.  There is
		actually a data frame parameter in the video file format that may
		be useful to impute for this when the ground level (MSL) is known
		for a given location: "SIM_ON_GROUND" (boolean).  Other undesired
		effects that could be prevented with this information include
		bank and pitch changes due to turns (e.g. taxiing turns).

	(3)	A generalized mechanism for delivering "external" vs. "internal"
		(e.g. "imputed") parameters to the FSR stream is envisioned where
		the set of "external" parameters delivered in a particular input
		stream format (e.g. "NMEA" vs. "GPX") is captured in a derived
		class handling the particular input stream.  This module (e.g.
		the "base" class) will then be able to "impute" a standard set
		of "internal" parameters not delivered by the derived class in a
		particular time frame.  This would support, for instance, future
		input streams in which parameters that are currently imputed
		(such as pitch and bank) could be delivered as "external" data,
		without modification to the base class code.  It would also
		greatly improve the overall readibility and organization of the
		code in the "FlushPropertyValues" method.

*/

using System ;
using System.IO ;
using System.Collections ;

namespace fsrlib
{

	public abstract class FSRConverterFromGPSStream : FSRConverterFrom, IFSRPropertyValueStream
	{


		//
		//	Public stuff
		//

		public class AuxiliaryData
		{

			public AuxiliaryData()
			{
			}

			public AuxiliaryData(
				string name,
				string description,
				double tickspersec,
				double startsec,
				double numsec,
				DateTime newdatetime,
				double minimumAltitude,
				bool jitterReduction
			) {
				m_name= name ;
				m_description= description ;
				m_tickspersec= tickspersec ;
				m_startsec= startsec ;
				m_numsec= numsec ;
				m_newdatetime= newdatetime ;
				m_minimumAltitude= minimumAltitude ;
				m_jitterReduction= jitterReduction ;
			}

			public string Name { get { return(m_name) ; } }
			public string Description { get { return(m_description) ; } }
			public double TicksPerSecond { get { return(m_tickspersec) ; } }
			public double StartSecond { get { return(m_startsec) ; } }
			public double TotalSeconds { get { return(m_numsec) ; } }
			public DateTime ForcedDateTime { get { return(m_newdatetime) ; } }
			public double MinimumAltitude { get { return(m_minimumAltitude) ; } }
			public bool JitterReduction { get { return(m_jitterReduction) ; } }

			private string m_name= "" ;
			private string m_description= "" ;
			private double m_tickspersec= 18.0 ;
			private double m_startsec= 0 ;
			private double m_numsec= double.MaxValue ;
			private DateTime m_newdatetime= DateTime.MaxValue ;
			private double m_minimumAltitude= double.MinValue ;
			private bool m_jitterReduction= true ;

		}


		//
		//	Constructors
		//

		protected FSRConverterFromGPSStream()
		{
			// derived object must call "Construct"!
		}

		public FSRConverterFromGPSStream(AuxiliaryData auxdata, StreamParser GPSStreamParser)
		{
			Construct(auxdata, GPSStreamParser) ;
		}

		public void Convert(FSRReaderStreamParser rdr, FSRWriter wrr)
		{
			m_input= rdr ;
			m_output= wrr ;
			Convert() ;
		}


		//
		//	"IFSRConverterFrom" implementation
		//

		public override void Convert(FSRReader rdr, FSRWriter wrr)	// IFSRConverterFrom
		{
			Convert((FSRReaderStreamParser) rdr, wrr) ;
		}


		//
		//	"IFSRPropertyValueStream" implementation
		//

		/// <summary>
		/// See comment in IFSRPropertyValueStream.
		/// </summary>
		public void WritePropertyValue(int PropId, Object PropObj)
		{

			if (
				(m_atimeuser0 != 0)
			     &&	(PropId == (int) FSRPropertyId.ABSOLUTE_TIME)
			) {
				double atime= (double) PropObj ;
				if (m_atimegps0 == 0) {
					m_atimegps0= atime ;
				}
				atime-= m_atimegps0 ;
				atime+= m_atimeuser0 ;
				PropObj= (Object) atime ;
			}

			m_propvals[PropId]= PropObj ;
		}

		/*

			NOTES REGARDING IMPUTING FLIGHT MODEL PARAMETERS:
	

			A.) Bank angle for coordinated turn (BA):
	
				general:
					BA	is proportional to (delta heading / time)
					BA	is inversely proportional to airspeed
		
				at standard rate, (delta heading / time) is (3 degrees / second),
				the rule of thunb for f(airspeed) is:
					BA	= (airspeed / 100) * 1.5
				or	BA	= (airspeed / 10) + 7
		
				without a "rule of thumb" for how this varies with (delta
				heading / time) (e.g., non-standard turns), this is assumed
				to be strictly proportional (e.g., coefficient 1.0).
		
					BA	= ((delta heading / time) / 3) * f(airspeed)

				Here are Stata regressions for "vaio1.fsr" showing both of
				these relationships:

			. reg bank_abs speed if onground == "False":bool

			      Source |       SS       df       MS              Number of obs =    3317
			-------------+------------------------------           F(  1,  3315) =  494.98
			       Model |  25373.7469     1  25373.7469           Prob > F      =  0.0000
			    Residual |  169933.101  3315  51.2618707           R-squared     =  0.1299
			-------------+------------------------------           Adj R-squared =  0.1297
			       Total |  195306.848  3316  58.8983257           Root MSE      =  7.1597

			------------------------------------------------------------------------------
			    bank_abs |      Coef.   Std. Err.      t    P>|t|     [95% Conf. Interval]
			-------------+----------------------------------------------------------------
			       speed |   .2908199   .0130716    22.25   0.000     .2651906    .3164491
			       _cons |  -19.03061   1.152438   -16.51   0.000    -21.29017   -16.77105
			------------------------------------------------------------------------------

			. reg bank_abs dhdgpt_abs if onground == "False":bool

			      Source |       SS       df       MS              Number of obs =    2372
			-------------+------------------------------           F(  1,  2370) =  275.54
			       Model |  14519.6457     1  14519.6457           Prob > F      =  0.0000
			    Residual |   124886.72  2370  52.6948186           R-squared     =  0.1042
			-------------+------------------------------           Adj R-squared =  0.1038
			       Total |  139406.366  2371  58.7964427           Root MSE      =  7.2591

			------------------------------------------------------------------------------
			    bank_abs |      Coef.   Std. Err.      t    P>|t|     [95% Conf. Interval]
			-------------+----------------------------------------------------------------
			  dhdgpt_abs |   .2816536   .0169676    16.60   0.000     .2483807    .3149266
			       _cons |   5.801045   .1544242    37.57   0.000     5.498224    6.103865
			------------------------------------------------------------------------------

			
	
			B.)	pitch for climbs/descents:
	
				the relationship seems to be:
		
					PA	is proportional to climb rate
					PA	is inversely proportional to airspeed *
		
				* this relationship to airspeed is much less clear than
				in the bank angle case.  Indeed, it seems that PA is
				in fact better described as independent of airspeed.
		
				but we don't want to ever go into a "dive", so we should
				constrain the results to "reasonable" values, such as
				between -7 degrees and +15 degrees (just a guess).
		
				examples:
					+2000 fpm	=> we want +15 degrees
					0 fpm		=> we want 0 degrees
					-2000 fpm	=> we want -7 degrees
					(all of these would also depend on airspeed)
		
				For now, we'll just ignore airspeed.
				
				Jan 22, 2004
				The best regression obtained from a sample video recorded
				on Vaio (SMO pattern) indicates that pitch (degrees) is a
				function solely of change in altitude per time (feet per
				second):
			
			. reg pitch daltpt if onground == "False":bool & abs(daltpt)< 30

			      Source |       SS       df       MS              Number of obs =    2230
			-------------+------------------------------           F(  1,  2228) = 9605.17
			       Model |  27854.5326     1  27854.5326           Prob > F      =  0.0000
			    Residual |  6461.09364  2228  2.89995226           R-squared     =  0.8117
			-------------+------------------------------           Adj R-squared =  0.8116
			       Total |  34315.6263  2229  15.3950768           Root MSE      =  1.7029

			------------------------------------------------------------------------------
			       pitch |      Coef.   Std. Err.      t    P>|t|     [95% Conf. Interval]
			-------------+----------------------------------------------------------------
			      daltpt |  -.4825446   .0049236   -98.01   0.000       -.4922   -.4728893
			       _cons |   -3.35362   .0361894   -92.67   0.000    -3.424589   -3.282652
			------------------------------------------------------------------------------

			Other notes:

			(1)	Looking at the matrix graphs of VELOCITY_WORLD_{X,Y,Z} against change
				in latitude, longitude and altitude, it appears that X corresponds to
				longitude, Y corresponds to altitude and Z corresponds to latitude.

			Here are the regressions:

			a.)	wvx is VELOCITY_WORLD_X, dlonpt is change in longitude per second:

			. reg wvx dlonpt if onground== "False":bool & abs(dlonpt)< 0.001

			      Source |       SS       df       MS              Number of obs =    2178
			-------------+------------------------------           F(  1,  2176) =       .
			       Model |  2387422.16     1  2387422.16           Prob > F      =  0.0000
			    Residual |  40337.4572  2176  18.5374344           R-squared     =  0.9834
			-------------+------------------------------           Adj R-squared =  0.9834
			       Total |  2427759.62  2177  1115.18586           Root MSE      =  4.3055

			------------------------------------------------------------------------------
			         wvx |      Coef.   Std. Err.      t    P>|t|     [95% Conf. Interval]
			-------------+----------------------------------------------------------------
			      dlonpt |   124355.3    346.517   358.87   0.000     123675.8    125034.9
			       _cons |  -.1502819   .0922771    -1.63   0.104    -.3312423    .0306785
			------------------------------------------------------------------------------


			b.)	wvz is VELOCITY_WORLD_Z, dlatpt is change in latitude per second:

			. reg wvz dlatpt if onground== "False":bool & abs(dlatpt)< 0.001

			      Source |       SS       df       MS              Number of obs =    2175
			-------------+------------------------------           F(  1,  2173) =       .
			       Model |   2524389.5     1   2524389.5           Prob > F      =  0.0000
			    Residual |  33247.9199  2173  15.3004694           R-squared     =  0.9870
			-------------+------------------------------           Adj R-squared =  0.9870
			       Total |  2557637.42  2174  1176.46615           Root MSE      =  3.9116

			------------------------------------------------------------------------------
			         wvz |      Coef.   Std. Err.      t    P>|t|     [95% Conf. Interval]
			-------------+----------------------------------------------------------------
			      dlatpt |   150766.7   371.1758   406.19   0.000     150038.8    151494.6
			       _cons |  -.1402564   .0841604    -1.67   0.096    -.3052997    .0247868
			------------------------------------------------------------------------------



			c.)	wvy is VELOCITY_WORLD_Y, daltpt is change in altitude (feet) per second:

			. reg wvy daltpt if onground== "False":bool & abs(daltpt)< 20

			      Source |       SS       df       MS              Number of obs =    2207
			-------------+------------------------------           F(  1,  2205) =37862.65
			       Model |   16637.862     1   16637.862           Prob > F      =  0.0000
			    Residual |  968.935952  2205  .439426736           R-squared     =  0.9450
			-------------+------------------------------           Adj R-squared =  0.9449
			       Total |  17606.7979  2206  7.98132272           Root MSE      =  .66289

			------------------------------------------------------------------------------
			         wvy |      Coef.   Std. Err.      t    P>|t|     [95% Conf. Interval]
			-------------+----------------------------------------------------------------
			      daltpt |   .3971783   .0020412   194.58   0.000     .3931755    .4011812
			       _cons |   -.005871   .0141516    -0.41   0.678    -.0336229    .0218809
			------------------------------------------------------------------------------

		*/

		/// <summary>
		///	See explanation of this method in IFSRPropertyValueStream.
		///	Effects linear transition between property values so as to
		///	achieve desired number of "frames per second" (as defined
		///	by DTicksDenominator).  See comments atop this module for
		///	more information about this method.
		/// </summary>
		public void FlushPropertyValues()
		{

			//
			// update client with progress
			//
			DoProgressUpdate(m_input) ;

			//
			//	Don't flush any values until we have minimum value set.
			//
			if (
				(((double) m_propvals[(int) FSRPropertyId.ABSOLUTE_TIME]) == 0)
			     || (((double) m_propvals[(int) FSRPropertyId.AIRSPEED_INDICATED]) == -1.0)
			     || (((double) m_propvals[(int) FSRPropertyId.PLANE_HEADING_DEGREES_TRUE]) == -1.0)
			) {
				return ;
			}

			//
			//	Get previous and current values.
			//	Calculate difference (range).
			//
			double atimeTo= ((double) m_propvals[(int) FSRPropertyId.ABSOLUTE_TIME]) ;
			double atimeFrom= ((double) m_propvalsSent[(int) FSRPropertyId.ABSOLUTE_TIME]) ;
			if (atimeFrom == 0) {
				//
				//	This is a little trick to force the output
				//	of a single data frame the first time through
				//	(e.g., when there has been no previous data
				//	frame "Sent").
				//
				atimeFrom= atimeTo - (1.0 / DTicksDenominator) ;
				if (m_atime0 == 0) m_atime0= atimeTo ;	// m_atime0 = start of GPS sequence
			}
			double datime= atimeTo - atimeFrom ;

			double altTo= ((double) m_propvals[(int) FSRPropertyId.PLANE_ALTITUDE]) ;
			double altFrom= ((double) m_propvalsSent[(int) FSRPropertyId.PLANE_ALTITUDE]) ;
			if (altFrom == 0) altFrom= altTo ;
			double dalt= altTo - altFrom ;

#if !NoMagHeadingGlobal
			double mhdgTo= ((double) m_propvals[(int) FSRPropertyId.PLANE_HEADING_DEGREES_MAGNETIC]) ;
			double mhdgFrom= ((double) m_propvalsSent[(int) FSRPropertyId.PLANE_HEADING_DEGREES_MAGNETIC]) ;
			if (mhdgFrom == 0) mhdgFrom= mhdgTo ;
			double dmhdg= HdgDifference(mhdgTo - mhdgFrom) ;
#endif
	
			double thdgTo= ((double) m_propvals[(int) FSRPropertyId.PLANE_HEADING_DEGREES_TRUE]) ;
			double thdgFrom= ((double) m_propvalsSent[(int) FSRPropertyId.PLANE_HEADING_DEGREES_TRUE]) ;
			if (thdgFrom == 0) thdgFrom= thdgTo ;
			double dthdg= HdgDifference(thdgTo - thdgFrom) ;

			double alatTo= ((double) m_propvals[(int) FSRPropertyId.PLANE_LATITUDE]) ;
			double alatFrom= ((double) m_propvalsSent[(int) FSRPropertyId.PLANE_LATITUDE]) ;
			if (alatFrom == 0) alatFrom= alatTo ;
			double dalat= alatTo - alatFrom ;

			double alonTo= ((double) m_propvals[(int) FSRPropertyId.PLANE_LONGITUDE]) ;
			double alonFrom= ((double) m_propvalsSent[(int) FSRPropertyId.PLANE_LONGITUDE]) ;
			if (alonFrom == 0) alonFrom= alonTo ;
			double dalon= alonTo - alonFrom ;
	
			double asTo= ((double) m_propvals[(int) FSRPropertyId.AIRSPEED_INDICATED]) ;
			double asFrom= ((double) m_propvalsSent[(int) FSRPropertyId.AIRSPEED_INDICATED]) ;
			if (asFrom == 0) asFrom= asTo ;
			double das= asTo - asFrom ;

#if	!NoSendBankPitch
			double pdFrom= ((double) m_propvalsSent[(int) FSRPropertyId.PLANE_PITCH_DEGREES]) ;
			double pdTo= 0 ;
			if (dalt != 0) {
				if (dalt> 0) {
					pdTo= -(dalt / datime) / 2.2 ;	// aim for 2000 fpm => ~15 degrees
					if (pdTo< -15.0) pdTo= -15.0 ;
				}
				else {
					pdTo= -(dalt / datime) / 4.7 ;	// aim for -2000 fpm => ~-7 degrees
					if (pdTo> 7.0) pdTo= 7.0 ;
				}
			}
			double dpd= pdTo - pdFrom ;

			double baFrom= ((double) m_propvalsSent[(int) FSRPropertyId.PLANE_BANK_DEGREES]) ;
			double baTo= 0 ;
			if (dthdg != 0) {
				baTo= -((dthdg / datime) / 3) * ((asTo / 10.0) + 7.0) ;
			}
			double dba= baTo - baFrom ;
#endif

			//
			//	Walk linear progression for each parameter varying from "from"
			//	(previous) value to "to" (current) value to help smooth transitions.
			//
			//	Loop from "atimeFrom" to "atimeTo", outputting all
			//	simulator values linearly progressing from "from"
			//	values to "to" values.
			//

			uint ticksFrom= m_ticks ;
			double dticks= DTicksDenominator * datime ;

			bool bJitterReduction= m_auxiliaryData.JitterReduction ;
			double altMin= m_auxiliaryData.MinimumAltitude ;
			double atimeMin= m_auxiliaryData.StartSecond ;
			double atimeMax= m_auxiliaryData.StartSecond + m_auxiliaryData.TotalSeconds ;
			int loopCount= (int) (dticks + 0.5) ;

			bool bWroteData= false ;

			for (int i= 0; i< loopCount; i++) {

				double dfrac= ((double) (i + 1)) / ((double) loopCount) ;

				double atime= atimeFrom + (dfrac * datime) ;

				double dtime0= atime - m_atime0 ;	// secs since start of GPS data
				if (dtime0< atimeMin) continue ;	// don't write - yet
				if (dtime0>= atimeMax) break ;		// past end - done

				double alat= alatFrom	+ (dfrac * dalat) ;
				double alon= alonFrom	+ (dfrac * dalon) ;
				double alt= altFrom	+ (dfrac * dalt) ;
#if !NoMagHeadingGlobal
				double mhdg= mhdgFrom	+ (dfrac * dmhdg) ;
#endif
				double thdg= thdgFrom	+ (dfrac * dthdg) ;
				double asp= asFrom	+ (dfrac * das) ;
#if	!NoSendBankPitch
				double ba= baFrom	+ (dfrac * dba) ;
				double pd= pdFrom	+ (dfrac * dpd) ;
#endif
				m_ticks= ticksFrom	+ (uint) ((dfrac * dticks) + 0.5) ;

				//
				//	Write data frame
				//

				m_propvals[(int) FSRPropertyId.ABSOLUTE_TIME]= atime ;
				m_propvals[(int) FSRPropertyId.PLANE_LATITUDE]= alat ;
				m_propvals[(int) FSRPropertyId.PLANE_LONGITUDE]= alon ;

				bool bSimOnGround= (alt<= altMin) ;

				if (bSimOnGround) {		// if plane "on ground",
					alt= altMin ;		// set to "ground altitude"
#if	!NoSendBankPitch
					ba= pd= 0 ;		// set bank and pitch to zero
#endif
				}

				if (!m_bSentSimOnGround) {	// make sure initial "SIM_ON_GROUND" goes out
					m_bSentSimOnGround= true ;
					m_propvalsSent[(int) FSRPropertyId.SIM_ON_GROUND]= (bool) !bSimOnGround ;
				}

				m_propvals[(int) FSRPropertyId.SIM_ON_GROUND]= (bool) bSimOnGround ;

				m_propvals[(int) FSRPropertyId.PLANE_ALTITUDE]= alt ;
				m_propvals[(int) FSRPropertyId.INDICATED_ALTITUDE]= alt ;

#if	!NoSendBankPitch
				m_propvals[(int) FSRPropertyId.PLANE_BANK_DEGREES]= ba ;
				m_propvals[(int) FSRPropertyId.PLANE_PITCH_DEGREES]= pd ;
#endif
				//
				// attempt to suppress jitter (small fluctuations
				// due to GPS variability and inaccuracy).
				//
				if (!bJitterReduction || (asp> 1.0)) {

#if	!NoSendAirspeed
					m_propvals[(int) FSRPropertyId.AIRSPEED_INDICATED]= asp ;
#endif
#if !NoMagHeadingGlobal
					m_propvals[(int) FSRPropertyId.PLANE_HEADING_DEGREES_MAGNETIC]= (
						HdgRangeGuard(mhdg)
					) ;
#endif
					m_propvals[(int) FSRPropertyId.PLANE_HEADING_DEGREES_TRUE]= (
						HdgRangeGuard(thdg)
					) ;
				}

				if (!bWroteData) {
					//
					//	Ensure that this data frame gets its own "BFIB" section,
					//	so that it's pumped to simulator immediately if being
					//	sent in real-time.
					//
					bWroteData= true ;
					m_output.FsibBfibEnd(m_bfibChunk) ;
					m_bfibChunk= m_output.FsibBfibStart() ;
				}

				WriteDataFrame() ;
			}

		}



		//
		//	protected stuff
		//

		protected void Construct(AuxiliaryData auxdata, StreamParser GPSStreamParser)
		{
			m_auxiliaryData= auxdata ;
			m_gpsstreamparser= GPSStreamParser ;
			InitPropData() ;
		}


		//
		//	private stuff
		//

		private void InitPropData()
		{

			// calculate time values
			m_atimeuser0= 0 ;
			if (m_auxiliaryData.ForcedDateTime != DateTime.MaxValue) {
				m_atimeuser0= (
					(double) m_auxiliaryData.ForcedDateTime.Ticks
				      / (double) 10000000.0
				) ;
			}

			// Allocate and load a standard FS2002 property dictionary
			m_propdict= FSRPropertyDictionaryFS2002.Create() ;


			//
			//	Create a primary and a backup set of values
			//	for all of the dictionary properties.
			//
			m_propvals= FSRPropertyValues.Create(m_propdict) ;
			m_propvalsSent= FSRPropertyValues.Create(m_propdict) ;


			//
			//	Setup initial "missing" values so we know when we have actual data...
			//
			m_propvals[(int) FSRPropertyId.ABSOLUTE_TIME]= (double) 0.0 ;
#if	!NoSendAirspeed
			m_propvals[(int) FSRPropertyId.AIRSPEED_INDICATED]= (double) -1.0 ;
#endif
			m_propvals[(int) FSRPropertyId.PLANE_HEADING_DEGREES_TRUE]= (double) -1.0 ;

			//
			//	Setup initial parameter values that will not be changed..
			//
			m_propvals[(int) FSRPropertyId.G_FORCE]= (double) 1.0 ;
			m_propvalsSent[(int) FSRPropertyId.G_FORCE]= (double) 0.0 ;

		}

		private void Convert()
		{

			//
			//	Open the initial chunks of the file
			//

			FSRWriter.Chunk startChunk= m_output.Start() ;
			FSRWriter.Chunk fsibChunk= m_output.FsibStart() ;

			//	Open an initial Data Record
			m_bfibChunk= m_output.FsibBfibStart() ;


			//
			//	Write the Object Definitions data section
			//

			FSRWriter.Chunk odibChunk= m_output.FsibBfibOdibStart() ;

			FSRObjectMapEntry[] omap= FSRObjectMapFS2002.Create() ;
			for (int mapindex= 0; mapindex< omap.Length; mapindex++) {
				FSRObjectMapEntry omapEntry= omap[mapindex] ;
				m_output.FsibBfibOdibObItem(
					(ushort) omapEntry.ObjectID,
					(string) omapEntry.ObjectName
				) ;
			}

			m_output.FsibBfibOdibEnd(odibChunk) ;


			//
			//	Write the Property Definitions section
			//

			FSRWriter.Chunk pdibChunk= m_output.FsibBfibPdibStart() ;

			for (int PropID= 0; PropID< m_propdict.Length; PropID++) {
				FSRPropertyDetail detail= m_propdict[PropID] ;
				if (!detail.IsSet) continue ;
				m_output.FsibBfibPdibPrItem(
					(ushort) detail.ObjectID,
					(ushort) PropID,
					(ushort) detail.DataTypeID,
					detail.UnitName,
					detail.PropertyName
				) ;
			}

			m_output.FsibBfibPdibEnd(pdibChunk) ;


			//
			//	Convert the input to one or more Frame Data sections,
			//	possibly closing and opening new Data Records as appropriate...
			//	(it appears that the simulator buffers Data Records, so it is
			//	necessary to close a Data Record befor the simulator will
			//	respond to it - e.g., when streaming over anonymous pipe).
			//

			m_input.Parse(m_gpsstreamparser) ;

			//	Close out the final Data Record
			m_output.FsibBfibEnd(m_bfibChunk) ;


			//
			//	Open and write the File Trailer
			//

			FSRWriter.Chunk trailerChunk= m_output.FsibTrailerStart() ;
			m_output.FsibTrailerDaItem(
				(ushort) FSRTrailerDataItemId.Name,
				m_auxiliaryData.Name
			) ;
			m_output.FsibTrailerDaItem(
				(ushort) FSRTrailerDataItemId.Description,
				m_auxiliaryData.Description
			) ;
			m_output.FsibTrailerEnd(trailerChunk) ;


			//
			//	Close the outermost file chunk and finish the file
			//

			m_output.FsibEnd(fsibChunk) ;
			m_output.End(startChunk) ;
		}


		/// <summary>
		/// Write out a single "Frame Data Section", containing all of the
		/// property values that have changed since the last time (as
		/// recorded in the backup "property value" set "m_propvalsSent").
		/// </summary>
		private void WriteDataFrame()
		{

			//
			//	Output all changed values in a single "FRIB" Chunk...
			//

			FSRWriter.Chunk fribChunk= m_output.FsibBfibFribStart() ;
		
			double atime= (double) m_propvals[(int) FSRPropertyId.ABSOLUTE_TIME] ;

			m_output.FsibBfibFribTsItem(
				m_frameno++,
				m_ticks,
				(long) (atime * 256.0)
			) ;

			for (int PropID= 0; PropID< m_propdict.Length; PropID++) {

				FSRPropertyDetail detail= m_propdict[PropID] ;
				if (!detail.IsSet) continue ;

				Object o= m_propvals[PropID] ;
				if (o == null) continue ;

#if	!ForceEverything
				if (
					o.Equals(m_propvalsSent[PropID])
#if ForceLatLonAlt
				     &&	(
						(PropID != (int) FSRPropertyId.PLANE_ALTITUDE)
					     && (PropID != (int) FSRPropertyId.PLANE_LATITUDE)
					     && (PropID != (int) FSRPropertyId.PLANE_LONGITUDE)
					)
#endif
				) continue ;
#endif

				m_output.FsibBfibFribFrItem(
					(ushort) detail.ObjectID,
					(ushort) PropID,
					(ushort) detail.DataTypeID,
					o
				) ;

				// store that value that we have now "sent"
				m_propvalsSent[PropID]= o ;
			}

			m_output.FsibBfibFribEnd(fribChunk) ;
		}

		private double DTicksDenominator {
			//
			//	Note that flights recorded with my MSFS2002 seem
			//	to all use a ratio of 26.3 ticks per second, while
			//	the "theoretical" value would be 18.0 (based upon
			//	the standard "clock", as implied in the Netpipes
			//	SDK documentation).  My guess is that this ratio
			//	represents the actual frame rate achieved by the
			//	hardware setup used for the recording.
			//
			get {
				return(m_auxiliaryData.TicksPerSecond) ;
			}
		}

		private static double HdgDifference(double hdiff)
		{
			while(hdiff< -180) hdiff+= 360.0 ;
			while(hdiff>= 180) hdiff-= 360.0 ;
			return(hdiff) ;
		}

		private static double HdgRangeGuard(double hdg)
		{
			while(hdg< 0) hdg+= 360.0 ;
			while(hdg> 360.0) hdg-= 360.0 ;
			return(hdg) ;
		}



		/*
			private data
		*/

		private FSRReaderStreamParser m_input ;
		private FSRWriter m_output ;
		private FSRWriter.Chunk m_bfibChunk ;
		private FSRPropertyDetail[] m_propdict ;
		private Object[] m_propvals ;
		private Object[] m_propvalsSent ;
		private AuxiliaryData m_auxiliaryData ;
		private StreamParser m_gpsstreamparser ;

		private uint m_frameno= 0 ;
		private double m_atime0= 0 ;
		private double m_atimegps0= 0 ;
		private double m_atimeuser0= 0 ;
		private uint m_ticks= 0 ;
		private bool m_bSentSimOnGround= false ;

	}

}

