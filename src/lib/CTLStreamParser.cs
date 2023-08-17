/*

	CTLStreamParser.cs - Cetus Track Log Parser
    Copyright (C) 2004, Marty Ross


	Provides a simple parser to extract and deliver the minimum values
	necessary for the "FSRConverterFromGPSStream" object from a Cetus
	(Palm O/S Application) Track Log (via a "DatumHandlerDelegate", in
	the format of a "StreamDatum", defined here).

	For more information about Cetus Track Logs, see:

		http://www.cetusgps.dk/download


	Information about Cetus' track log database format was gleaned from
	the "tracklog.h" source file found in the "Tracklog 1.3 source.zip"
	currently (Jan'04) available at the above URL.  The following excerpt
	from atop that source file gives credit for this information, some
	of which appears verbatim within comments of this module:
	
		Project:      Cetus GPS track export
		Description:  This header file contains defines that are shared between
			Cetus GPS and the the Cetus TrackLog program. There is
			also a description of the Cetus GPS track file format.
		File:         tracklog.h
		Author:       Kjeld Jensen <kjeld@CetusGPS.dk>
		Homepage:     http://www.CetusGPS.dk

*/

using System ;
using System.IO ;

namespace fsrlib
{

	public class CTLStreamParser : StreamParser
	{

		public CTLStreamParser()
		{
			m_info= new CetusTrackFileInfo() ;
			m_record= new CetusTrackRecord() ;
			m_block= new CetusTracklogBlock() ;
			m_datum= new StreamDatum() ;
		}

		public delegate void DatumHandlerDelegate(StreamDatum d) ;

		public void SetDatumHandler(DatumHandlerDelegate d)
		{
			m_dhandler= d ;
		}

		public void Parse(BinaryReader input)
		{
			m_recno= 0 ;
			m_prevhourutc= 0 ;
			m_dayspassedTicks= 0 ;
			PDBReader.Parse(
				input,
				"strm",
				new PDBReader.PushRecordData(PushCTLPDBRec)
			) ;
		}

		public override void Parse(Stream s)
		{
			Parse(new BinaryReader(s)) ;
		}

		void PushCTLPDBRec(BinaryReader input, int recsize)
		{

			Logger.LogDebug(
				"CTLStreamParser.PushCTLPDBRec("
			      + m_recno
			      + ") received; "
			      + recsize
			      + " bytes."
			) ;

			if (!m_block.Read(input)) {
				throw new ApplicationException(FSRMsgs.Load("StatusInvalidFile")) ;
			}

			long BlockDataPos= input.BaseStream.Position ;
			long RecPosFence= BlockDataPos + m_block.Length - CetusTrackRecord.Size - 1 ;

			for (long RecPos= BlockDataPos; RecPos< RecPosFence; RecPos+= CetusTrackRecord.Size) {

				// skip to start of current record (if not already there)
				for (
					long SkipCount= RecPos - input.BaseStream.Position ;
					SkipCount> 0 ;
					SkipCount--
				) {
					input.ReadByte() ;
				}

				m_recno++ ;

				switch(m_recno) {

					case 1:
						if (!m_info.ReadRec1(input)) {
							throw new ApplicationException(
								FSRMsgs.Load("StatusInvalidFile")
							) ;
						}
						m_prevhourutc= (int) m_info.Hour ;
						break ;

					case 2:	m_info.ReadRec2(input) ;	break ;
					case 3:	m_info.ReadRec3(input) ;	break ;

					default:
						m_record.Read(input) ;
						PushStreamDatum() ;
						break ;
				}
			}
		}

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



		//
		//
		//

		void PushStreamDatum()
		{

			m_datum.AltFeet= (((double) m_record.Altitude) / 100.0) * FSRGlobals.FeetPerMeter ;
			m_datum.CourseTrueDeg= ((double) m_record.Course) / 10.0 ;
			m_datum.Lat= (double) (m_record.Latitude) / 10000000.0 ;
			m_datum.Lon= (double) (m_record.Longitude) / 10000000.0 ;
			m_datum.SpeedKnots= ((double) m_record.Speed) / 10.0 ;
			m_datum.nsat= m_record.NumSat ;

			switch(m_record.Status >> 5)
			{
				case 2:
					m_datum.fix= FixType.Fix2D ;
					break ;

				case 3:
				case 4:	m_datum.fix= FixType.Fix3D ;
					break ;

				default:
					m_datum.fix= FixType.FixNone ;
					break ;
			}

			int hour= (int) (m_record.Status & 31) ;
			int minute= (int) m_record.Minute ;
			int second= (int) m_record.Second ;
			int msec= ((int) m_record.DecSecond) * 10 ;
			int year= m_info.Year + 2000 ; 
			int month= m_info.Month ;
			int day= m_info.Day ;

			// handle day-wrap: (NOTE: assumes at least one movement per day!!)
			if (hour< m_prevhourutc) {
				m_dayspassedTicks+= TimeSpan.TicksPerDay ;
			}
			m_prevhourutc= hour ;

			m_datum.TimeUTC= new DateTime(
				year,
				month,
				day,
				hour,
				minute,
				second,
				msec
			) ;

			m_datum.TimeUTC.AddTicks(m_dayspassedTicks) ;

			if (m_dhandler != null) {
				m_dhandler(m_datum) ;
			}

		}

		//
		//
		//

		/*

			The following information is excerpted from the Cetus source
			file: tracklog.h.  See credits at the top of this module for
			more information about the Cetus source.


			FILE DATA DESCRIPTION

			The track file starts with a header and then follows a set of 4096 bytes
			data blocks each initiated with the characters 'DBLK' and a 4 byte block
			id.

			Cetus GPS writes the track records after the block header. The 4096
			bytes data block is filled with empty bytes at the end, so each data
			block begins with a new record.

		*/
		struct CetusTracklogBlock
		{

			internal byte[] Magic ;
			internal int Length ;

			internal static byte[] MagicWanted= new byte[] {
				(byte) 'D',
				(byte) 'B',
				(byte) 'L',
				(byte) 'K'
			} ;

			internal bool Read(BinaryReader input)
			{
				Magic= input.ReadBytes(MagicWanted.Length) ;
				if (!IsMagicOk()) {
					Logger.LogError(
						"invalid magic("
					      + PDBReader.ZeroTerminatedByteArrayToString(Magic)
					      + "); expected '"
					      + PDBReader.ZeroTerminatedByteArrayToString(MagicWanted)
					      + "'"
					) ;
					return(false) ;
				}
				Length= PDBReader.ReadPDBint(input) ;
				return(true) ;
			}

			internal bool IsMagicOk()
			{
				if (Magic.Length != MagicWanted.Length) return(false) ;
				for (int i= 0; i< Magic.Length; i++) {
					if (Magic[i] != MagicWanted[i]) return(false) ;
				}
				return(true) ;
			}

		}

		/*

			The following information is excerpted from the Cetus source
			file: tracklog.h.  See credits at the top of this module for
			more information about the Cetus source.


			TRACK FILE INFORMATION RECORD DESCRIPTION

			The first two records in the track log contains information about the
			conditions under which the track was obtained.

			Record 1

			Byte    Type            Name        Description
			------- --------------- ----------- -------------------------------------
			0,1     char s[2]       ID          track ID ("CG")
			2       char            version     track format version (3)
			3       unsigned char   interval    User selected track save
							0 All positions
							1 Movements
							2 Every second
							3 Every 2 seconds
							4 Every 10 seconds
							5 Every minute
			4,5     unsigned short  gps         GPS unit information
							0 No GPS detected (status is NoGps)
							1 Unknown GPS (generic)
							2 Demo mode enabled
							3 Generic Garmin GPS
							4 Generic Rockwell chipset GPS
							5 Generic Magellan GPS
							6 DeLorme Tripmate GPS
							7 Generic Motorola GPS
			6       unsigned char   year        year of track start (UTC)
			7       unsigned char   month       month of track start (UTC)
			8       unsigned char   day         day of track start (UTC)
			9       unsigned char   hour        hour of track start (UTC)
			10      unsigned char   minute      minute of track start (UTC)
			11      unsigned char   second      second of track start (UTC)
			12      unsigned char   decSecond   1/100 seconds (UTC)
			13      char            timeOffset  user preference UTC time offset (hours)
			14-21   unused

			Record 2

			Byte    Type            Name        Description
			------- --------------- ----------- -------------------------------------
			0       char *          desc        first character in the description
							string 25 chars (continued in the
							next record)
			Record 3

			Byte    Type            Name        Description
			------- --------------- ----------- -------------------------------------
			0       char *          desc        description string (continued)
			3-21    unused                                   
                               
		*/
		struct CetusTrackFileInfo
		{

			internal byte[] Trackid ;
			internal byte Version ;
			internal byte Interval ;
			internal ushort Gps ;
			internal byte Year ;
			internal byte Month ;
			internal byte Day ;
			internal byte Hour ;
			internal byte Minute ;
			internal byte Second ;
			internal byte DecSecond ;
			internal sbyte TimeOffset ;
			internal byte[] Description ;

			internal static byte[] TrackidWanted= new byte[] {
				(byte) 'C',
				(byte) 'G'
			} ;

			internal static byte VersionWanted= 3 ;

			internal bool IsTrackidOk()
			{
				if (Trackid.Length != TrackidWanted.Length) return(false) ;
				for (int i= 0; i< Trackid.Length; i++) {
					if (Trackid[i] != TrackidWanted[i]) return(false) ;
				}
				return(true) ;
			}

			internal bool ReadRec1(BinaryReader input)
			{

				Trackid= input.ReadBytes(TrackidWanted.Length) ;

				if (!IsTrackidOk()) {
					Logger.LogError(
						"invalid trackid("
					      + PDBReader.ZeroTerminatedByteArrayToString(Trackid)
					      + "); expected '"
					      + PDBReader.ZeroTerminatedByteArrayToString(TrackidWanted)
					      + "'"
					) ;
					return(false) ;
				}

				Version= input.ReadByte() ;
				if (Version != VersionWanted) {
					Logger.LogError(
						"invalid Cetus tracklog version("
					      + Version
					      + "); expected '"
					      + VersionWanted
					      + "'"
					) ;
					return(false) ;
				}

				Interval= input.ReadByte() ;
				Gps= PDBReader.ReadPDBushort(input) ;
				Year= input.ReadByte() ;
				Month= input.ReadByte() ;
				Day= input.ReadByte() ;
				Hour= input.ReadByte() ;
				Minute= input.ReadByte() ;
				Second= input.ReadByte() ;
				DecSecond= input.ReadByte() ;
				TimeOffset= input.ReadSByte() ;

				return(true) ;
			}

			internal void ReadRec2(BinaryReader input)
			{
				Description= input.ReadBytes(22) ;
			}

			internal void ReadRec3(BinaryReader input)
			{
				int oldLength= Description.Length ;
				byte[] cont= new Byte[oldLength + 3] ;
				Description.CopyTo(cont, 0) ;
				input.ReadBytes(3).CopyTo(cont, oldLength) ;
				Description= cont ;
			}

		}


		/*

			The following information is excerpted from the Cetus source
			file: tracklog.h.  See credits at the top of this module for
			more information about the Cetus source.

			TRACK RECORD DESCRIPTION

			Description of the track record structure as stored in the track file.
			All word size values are stored as 'high byte first'

			Byte    Type            Name        Description
			------- --------------- ----------- -------------------------------------
			0            unsigned char   status  0 No GPS (bit 5-7)
							1 No fix
							2 2d fix
							3 3d fix
							4 DGPS (3d) fix

			0                            hour    hours of position fix (UTC) (bit0-4)

			1            unsigned char   minute  minutes of position fix (UTC)
			2            unsigned char   second  seconds of position fix (UTC)
			3            unsigned char   decSecond 1/100 seconds of position fix (UTC)

			4            unsigned char   sat     number of satellites being tracked
			5            unsigned char   hdop    HDOP * 10 (0.1 precision)

			6,7,8,9      long            lat     lat*10^7 (+ is north, - is south)
			10,11,12,13  long            lon     lon*10^7 (+ is east, - is west)

			14,15        unsigned short  speed   current speed in knots*10 (0.1 km/h prec.)
			16,17        unsigned short  course  course made good*10 (0.1 deg precision)
			18,19,20,21  long            alt     altitude in meter * 100 (0.01 meter precision)

		*/

		struct CetusTrackRecord
		{

			internal const int Size= 22 ;

			internal byte Status ;
			internal byte Minute ;
			internal byte Second ;
			internal byte DecSecond ;
			internal byte NumSat ;
			internal byte Hdop ;
			internal int Latitude ;
			internal int Longitude ;
			internal ushort Speed ;
			internal ushort Course ;
			internal int Altitude ;

			internal void Read(BinaryReader input)
			{
				Status= input.ReadByte() ;
				Minute= input.ReadByte() ;
				Second= input.ReadByte() ;
				DecSecond= input.ReadByte() ;
				NumSat= input.ReadByte() ;
				Hdop= input.ReadByte() ;
				Latitude= PDBReader.ReadPDBint(input) ;
				Longitude= PDBReader.ReadPDBint(input) ;
				Speed= PDBReader.ReadPDBushort(input) ;
				Course= PDBReader.ReadPDBushort(input) ;
				Altitude= PDBReader.ReadPDBint(input) ;
			}
		}

		DatumHandlerDelegate m_dhandler= null ;
		CetusTrackFileInfo m_info ;
		CetusTrackRecord m_record ;
		CetusTracklogBlock m_block ;
		StreamDatum m_datum ;
		int m_recno ;
		int m_prevhourutc ;
		long m_dayspassedTicks ;

	}

}

