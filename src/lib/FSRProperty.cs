//#define		NoDefineExtraneousParams

/*

	FSRProperty.cs - MS Flight Simulator Video Properties
    Copyright (C) 2003-2004, Marty Ross

	Provides dictionary and value set for storing/retrieving MS Flight Simulator Video
	aircraft state properties, and related interfaces and values.

	Please see the Microsoft Flight Simulator 2002 Software Development Kit document
	entitled "Netpipes: Data Input/Output" dated June 2002 for supplementary information
	regarding the code contained in this module.

*/

using System ;
using System.Collections ;

namespace fsrlib
{


	/*
		IFSRPropertyValueStream
	*/

	internal interface IFSRPropertyValueStream
	{

		/// <summary>
		/// Updates specified property value.
		/// </summary>
		/// <param name="PropID">Property to update</param>
		/// <param name="val">New value for property</param>
		void WritePropertyValue(int PropID, Object val) ;

		/// <summary>
		///
		///	Writes updated track properties to FSR file.  These properties are updated
		///	between calls to this method in the "WritePropertyValue" callback (above).
		///
		///	Should be called for each "significant" movement event in GPS input stream
		///	(which is normally about once per second, if the GPS writes one track datum
		///	each second).
		///	
		/// </summary>
		void FlushPropertyValues() ;

	}


	/// <summary>
	/// Property data types, as defined in "Netpipes SDK".
	/// </summary>
	internal enum FSRPropertyDataType
	{
		Bool=		1,		// FlightRecordBooleanProperty
		Double=		2,		// FlightRecordNumberProperty
		Int=		3,		// FlightRecordOrdinalProperty
		String=		4		// FlightRecordStringProperty
	}


	/// <summary>
	/// FSRPropertyDetail contains the information about a property
	/// read from the Property Definitions section of the FSR file.
	/// </summary>
	internal struct FSRPropertyDetail
	{

		internal FSRPropertyDetail(
			int ObjectID,
			FSRPropertyDataType DataTypeID,
			string UnitName,
			string PropertyName
		) {
			this.ObjectID= ObjectID ;
			this.DataTypeID= DataTypeID ;
			this.UnitName= UnitName ;
			this.PropertyName= PropertyName ;
			this.IsSet= true ;
		}

		internal int ObjectID ;
		internal FSRPropertyDataType DataTypeID ;
		internal string UnitName ;
		internal string PropertyName ;
		internal bool IsSet ;		// defaults to "false"

	}

	/// <summary>
	/// Helper class for managing property values.
	/// </summary>
	internal class FSRPropertyValues
	{

		/// <summary>
		/// Create an array of initial property values corresponding
		/// to the passed set of property descriptions (in 'dict').
		/// 
		/// The "Property ID" value is used as the direct index into
		/// the array returned for accessing each property value
		/// (please see the comment about this in the method for
		/// creating the Property Dictionary in this module).
		/// </summary>
		/// <param name="dict">Property Dictionary</param>
		/// <returns>
		/// Array of empty values for properties in 'dict',
		/// to be directly indexed by "Property ID"
		/// </returns>
		internal static Object[] Create(FSRPropertyDetail[] dict)
		{
			Object[] valarray= new Object[dict.Length] ;
			for (int PropID= 0; PropID< dict.Length; PropID++) {
				switch(dict[PropID].DataTypeID) {

					case FSRPropertyDataType.Bool:
						valarray[PropID]= (bool) false ;
						break ;

					case FSRPropertyDataType.Int:
						valarray[PropID]= (int) 0 ;
						break ;

					case FSRPropertyDataType.Double:
						valarray[PropID]= (double) 0.0 ;
						break ;

					case FSRPropertyDataType.String:
						valarray[PropID]= String.Empty ;
						break ;
				}
			}
			return(valarray) ;
		}

	}


	/*

		MS Flight Simulator 2002 specific

	*/

	/// <summary>
	/// These are the "Object ID" names and values that were returned
	/// in a video recorded with Flight Simulator 2002.
	/// </summary>
	internal enum FSRObjectId
	{
		UserAircraft	= 4096,
		Environment	= 8192
	}

	/// <summary>
	/// These are the "Property ID" names and values that were returned 
	/// in a video recorded with Flight Simulator 2002.  Only the set
	/// of propertys used by fsrlib are left in the enumeration.
	/// </summary>
	internal enum FSRPropertyId
	{
		PLANE_LATITUDE				= 1,
		PLANE_LONGITUDE				= 2,
		PLANE_ALTITUDE				= 3,
		PLANE_PITCH_DEGREES			= 4,
		PLANE_BANK_DEGREES			= 5,
		PLANE_HEADING_DEGREES_TRUE		= 6,
		SIM_ON_GROUND				= 7,
		VELOCITY_WORLD_X			= 8,
		VELOCITY_WORLD_Y			= 9,
		VELOCITY_WORLD_Z			= 10,
		INDICATED_ALTITUDE			= 11,
		AIRSPEED_INDICATED			= 12,
		PLANE_HEADING_DEGREES_MAGNETIC		= 13,
		G_FORCE					= 14,
		GENERAL_ENG1_THROTTLE_LEVER_POSITION	= 15,
		GENERAL_ENG1_MIXTURE_LEVER_POSITION	= 16,
		GENERAL_ENG1_PROPELLER_LEVER_POSITION	= 17,
		GENERAL_ENG2_THROTTLE_LEVER_POSITION	= 18,
		GENERAL_ENG2_MIXTURE_LEVER_POSITION	= 19,
		GENERAL_ENG2_PROPELLER_LEVER_POSITION	= 20,
		GENERAL_ENG3_THROTTLE_LEVER_POSITION	= 21,
		GENERAL_ENG3_MIXTURE_LEVER_POSITION	= 22,
		GENERAL_ENG3_PROPELLER_LEVER_POSITION	= 23,
		GENERAL_ENG4_THROTTLE_LEVER_POSITION	= 24,
		GENERAL_ENG4_MIXTURE_LEVER_POSITION	= 25,
		GENERAL_ENG4_PROPELLER_LEVER_POSITION	= 26,
		AUTOPILOT_MASTER			= 27,
		YOKE_Y_POSITION				= 28,
		YOKE_X_POSITION				= 29,
		RUDDER_PEDAL_POSITION			= 30,
		ELEVATOR_POSITION			= 31,
		AILERON_POSITION			= 32,
		RUDDER_POSITION				= 33,
		FLAPS_HANDLE_INDEX			= 34,
		SPOILERS_HANDLE_POSITION		= 35,
		GEAR_HANDLE_POSITION			= 36,
		WATER_RUDDER_HANDLE_POSITION		= 37,
		CONCORDE_VISOR_NOSE_HANDLE		= 38,
		LIGHT_STATES				= 39,
		ABSOLUTE_TIME				= 40,
		TIME_ZONE_OFFSET			= 41,
	}


	/// <summary>
	/// FSRTrailerDataItemId values are observed values for the data items found 
	/// in the "Trailer" section of the FSR file.
	/// </summary>
	internal enum FSRTrailerDataItemId {
		Name					= 1,	// FlightRecordTrailerTitleItem
		Description				= 2,	// FlightRecorderTrailerDescription
	}

	/// <summary>
	/// Helper class for building standard MSFS 2002 Property Dictionary.
	/// </summary>
	internal class FSRPropertyDictionaryFS2002
	{


		/// <summary>
		/// 
		/// Returns a standard FS2002 Property Dictionary as an array
		/// of FSRPropertyDetail objects.  The "Property ID" value is
		/// the direct index into this array.
		/// 
		/// NOTES:
		/// (1)	Using "Property ID" as the direct index into the
		///	Property Dictionary is done for speed.  Previously
		///	an indirect index (via linear search then via hash-
		///	table) was employed, but the performance penalty
		///	was unacceptable.
		///	
		/// (2)	Because of (1), the following restrictions are placed
		///	upon "Property ID" values here: (a) they must be non-
		///	negative, (b) the lowest values should be "close" to
		///	zero and (c) they should "cluster" together (i.e. there
		///	should not be many, or large gaps in the range of
		///	"Property ID" values).
		///
		/// </summary>
		/// <returns>
		/// Standard FS2002 Property Dictionary as array of
		/// FSRPropertyDetail objects, to be directly indexed
		/// by "Property ID"
		/// </returns>
		internal static FSRPropertyDetail[] Create()
		{

			//
			//	Get the largest "FSRPropertyId" value in order to
			//	construct an array to hold all possible values.
			//	This is awful; looking for a better way!
			//
			FSRPropertyId pidvar= FSRPropertyId.ABSOLUTE_TIME ;
			Array a= Enum.GetValues(pidvar.GetType()) ;
			int PropIDMax= (int) a.GetValue(a.Length - 1) ;

			FSRPropertyDetail[] m_dict= new FSRPropertyDetail[PropIDMax + 1] ;


			//
			// Add the Flight Simulator 2002 properties to the dictionary.
			//

			m_dict[(int) FSRPropertyId.PLANE_LATITUDE]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"degrees",
				"PLANE LATITUDE"
			) ;

			m_dict[(int) FSRPropertyId.PLANE_LONGITUDE]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"degrees",
				"PLANE LONGITUDE"
			) ;

			m_dict[(int) FSRPropertyId.PLANE_ALTITUDE]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"ft",
				"PLANE ALTITUDE"
			) ;

			m_dict[(int) FSRPropertyId.PLANE_PITCH_DEGREES]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"degrees",
				"PLANE PITCH DEGREES"
			) ;

			m_dict[(int) FSRPropertyId.PLANE_BANK_DEGREES]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"degrees",
				"PLANE BANK DEGREES"
			) ;

			m_dict[(int) FSRPropertyId.PLANE_HEADING_DEGREES_TRUE]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"degrees",
				"PLANE HEADING DEGREES TRUE"
			) ;

			m_dict[(int) FSRPropertyId.SIM_ON_GROUND]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Bool,
				"Enum",
				"SIM ON GROUND"
			) ;

#if !NoDefineExtraneousParams
			m_dict[(int) FSRPropertyId.VELOCITY_WORLD_X]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"meters/second",
				"VELOCITY WORLD X"
			) ;

			m_dict[(int) FSRPropertyId.VELOCITY_WORLD_Y]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"meters/second",
				"VELOCITY WORLD Y"
			) ;

			m_dict[(int) FSRPropertyId.VELOCITY_WORLD_Z]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"meters/second",
				"VELOCITY WORLD Z"
			) ;
#endif
			m_dict[(int) FSRPropertyId.INDICATED_ALTITUDE]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"ft",
				"INDICATED ALTITUDE"
			) ;

			m_dict[(int) FSRPropertyId.AIRSPEED_INDICATED]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"knot",
				"AIRSPEED INDICATED"
			) ;

#if	!NoDefineExtraneousParams || !NoMagHeadingGlobal
			m_dict[(int) FSRPropertyId.PLANE_HEADING_DEGREES_MAGNETIC]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"degrees",
				"PLANE HEADING DEGREES MAGNETIC"
			) ;
#endif

			m_dict[(int) FSRPropertyId.G_FORCE]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"GForce",
				"G FORCE"
			) ;

#if !NoDefineExtraneousParams
			m_dict[(int) FSRPropertyId.GENERAL_ENG1_THROTTLE_LEVER_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"GENERAL ENG1 THROTTLE LEVER POSITION"
			) ;

			m_dict[(int) FSRPropertyId.GENERAL_ENG1_MIXTURE_LEVER_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"GENERAL ENG1 MIXTURE LEVER POSITION"
			) ;

			m_dict[(int) FSRPropertyId.GENERAL_ENG1_PROPELLER_LEVER_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"GENERAL ENG1 PROPELLER LEVER POSITION"
			) ;

			m_dict[(int) FSRPropertyId.GENERAL_ENG2_THROTTLE_LEVER_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"GENERAL ENG2 THROTTLE LEVER POSITION"
			) ;

			m_dict[(int) FSRPropertyId.GENERAL_ENG2_MIXTURE_LEVER_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"GENERAL ENG2 MIXTURE LEVER POSITION"
			) ;

			m_dict[(int) FSRPropertyId.GENERAL_ENG2_PROPELLER_LEVER_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"GENERAL ENG2 PROPELLER LEVER POSITION"
			) ;

			m_dict[(int) FSRPropertyId.GENERAL_ENG3_THROTTLE_LEVER_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"GENERAL ENG3 THROTTLE LEVER POSITION"
			) ;

			m_dict[(int) FSRPropertyId.GENERAL_ENG3_MIXTURE_LEVER_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"GENERAL ENG3 MIXTURE LEVER POSITION"
			) ;

			m_dict[(int) FSRPropertyId.GENERAL_ENG3_PROPELLER_LEVER_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"GENERAL ENG3 PROPELLER LEVER POSITION"
			) ;

			m_dict[(int) FSRPropertyId.GENERAL_ENG4_THROTTLE_LEVER_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"GENERAL ENG4 THROTTLE LEVER POSITION"
			) ;

			m_dict[(int) FSRPropertyId.GENERAL_ENG4_MIXTURE_LEVER_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"GENERAL ENG4 MIXTURE LEVER POSITION"
			) ;

			m_dict[(int) FSRPropertyId.GENERAL_ENG4_PROPELLER_LEVER_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"GENERAL ENG4 PROPELLER LEVER POSITION"
			) ;

			m_dict[(int) FSRPropertyId.AUTOPILOT_MASTER]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Bool,
				"Enum",
				"AUTOPILOT MASTER"
			) ;

			m_dict[(int) FSRPropertyId.YOKE_Y_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"YOKE Y POSITION"
			) ;

			m_dict[(int) FSRPropertyId.YOKE_X_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"YOKE X POSITION"
			) ;

			m_dict[(int) FSRPropertyId.RUDDER_PEDAL_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"RUDDER PEDAL POSITION"
			) ;

			m_dict[(int) FSRPropertyId.ELEVATOR_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"ELEVATOR POSITION"
			) ;

			m_dict[(int) FSRPropertyId.AILERON_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"AILERON POSITION"
			) ;

			m_dict[(int) FSRPropertyId.RUDDER_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"percent",
				"RUDDER POSITION"
			) ;

			m_dict[(int) FSRPropertyId.FLAPS_HANDLE_INDEX]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Int,
				"numbers",
				"FLAPS HANDLE INDEX"
			) ;

			m_dict[(int) FSRPropertyId.SPOILERS_HANDLE_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"position",
				"SPOILERS HANDLE POSITION"
			) ;

			m_dict[(int) FSRPropertyId.GEAR_HANDLE_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"position",
				"GEAR HANDLE POSITION"
			) ;

			m_dict[(int) FSRPropertyId.WATER_RUDDER_HANDLE_POSITION]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Double,
				"position",
				"WATER RUDDER HANDLE POSITION"
			) ;

			m_dict[(int) FSRPropertyId.CONCORDE_VISOR_NOSE_HANDLE]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Int,
				"position",
				"CONCORDE VISOR NOSE HANDLE"
			) ;

			m_dict[(int) FSRPropertyId.LIGHT_STATES]= new FSRPropertyDetail(
				(int) FSRObjectId.UserAircraft,
				FSRPropertyDataType.Int,
				"mask",
				"LIGHT STATES"
			) ;
#endif
			m_dict[(int) FSRPropertyId.ABSOLUTE_TIME]= new FSRPropertyDetail(
				(int) FSRObjectId.Environment,
				FSRPropertyDataType.Double,
				"second",
				"ABSOLUTE TIME"
			) ;

#if !NoDefineExtraneousParams
			m_dict[(int) FSRPropertyId.TIME_ZONE_OFFSET]= new FSRPropertyDetail(
				(int) FSRObjectId.Environment,
				FSRPropertyDataType.Double,
				"second",
				"TIME ZONE OFFSET"
			) ;
#endif

			return(m_dict) ;
		}
	}

	/// <summary>
	/// Map between "Object ID" and "Object Name".
	/// This mapping is significant in that it appears the Simulator
	/// uses the string representation of the simulated "Object"
	/// (rather than its numerical "Object ID" value) to identify
	/// the object.
	/// </summary>
	internal struct FSRObjectMapEntry
	{

		internal FSRObjectMapEntry(int ObjectID, string ObjectName)
		{
			this.ObjectID= ObjectID ;
			this.ObjectName= ObjectName ;
		}

		internal int ObjectID ;
		internal string ObjectName ;
	}

	/// <summary>
	/// Helper class to create the set of standard FS2002 "Simulated Objects".
	/// </summary>
	internal class FSRObjectMapFS2002
	{

		/// <summary>
		/// Creates and returns the Simulated Object Map which provides the
		/// mapping between the numeric and string representation of the standard
		/// Simulator Objects.
		/// </summary>
		/// <returns>
		/// Mapping array containing set of "FSRObjectMapEntry" objects.  There
		/// is no significance to the relative or absolute order of the elements
		/// within the array; the mapping between "Object ID" and "Object Name"
		/// is given entirely by the values within the "FSRObjectMapEntry"
		/// structures.
		/// </returns>
		static internal FSRObjectMapEntry[] Create()
		{

			ArrayList alist= new ArrayList() ;

			alist.Add(
				new FSRObjectMapEntry(
					(int) FSRObjectId.UserAircraft,
					"UserAircraft"
				)
			) ;

			/*
				NOTE regarding misspelling of "Environment" (below).
				Several dozen hours of debugging were spent trying to
				find a problem involving the inability of the simulator
				to synchronize itself with the time frame progression
				of the video, until this misspelling was found.
			
				Apparently, MSFS2002 uses the string version of this
				to identify the "object" (e.g., the numeric value is
				not used, but the string mapping to the numeric value
				indicated here is used instead).  If you spell the
				word "Environment" correctly, the simulator seems to
				never "see" the environment parameters (such as the
				ABSOLUTE_TIME parameter), and therefore does not
				synchronize with the video - it just runs full speed.
			
				The correct spelling for FS2002 is therefore "Enviroment".
			*/

			alist.Add(
				new FSRObjectMapEntry(
					(int) FSRObjectId.Environment,
					"Enviroment"	// sic - see above
				)
			) ;

			FSRObjectMapEntry[] aarray= new FSRObjectMapEntry[alist.Count] ;
			alist.CopyTo(aarray) ;
			
			return(aarray) ;
		}

	}

}
