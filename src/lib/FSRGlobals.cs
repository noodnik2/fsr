/*

	FSRGlobals.cs - Global fsrlib Values
    Copyright (C) 2003-2004, Marty Ross

	Centralized location for values that might one day become externalized
	application parameters.

*/

using System ;

namespace fsrlib
{

	public class FSRGlobals
	{

		/*
			Some convenience constants that are never considered to change...
		*/

		public const double FeetPerMeter= 3.280839895 ;
		public const double MetersPerNauticalMile= 1852.0 ;	// International
		//public static const double MetersPerNauticalMile= 1853.18 ;	// UK, US (deprecated, I think)


		/*
			NOTE:

			These methods should be as fast as possible, as they may be
			called from worker methods that themselves have performance
			requirements.

			Whenever possible, the values returned by these methods
			should be available in a cached, or other internal source.
		*/


		//
		// Set this value to a larger number (e.g., at least one second) so that more
		// I/O can take place between updates - e.g., an update is expensive.
		public static double SecondsBetweenProgressUpdates_Large { get { return(1.0) ; } }

		//
		// This value can be set smaller for use on tasks that won't take much time
		// to complete.  The value of smoother updating is relatively more important
		// than the speed increase accompanying a less frequent progress update.
		public static double SecondsBetweenProgressUpdates_Small { get { return(0.1) ; } }

		//
		// Should the program attempt to bring the Simulator into the foreground
		// before attempting to "play" a video on it?
		public static bool ActivateSimulatorOnPlay { get { return(true) ; } }

		//
		// This is the list of the process names of known "compatible" flight simulators.
		public static string[] SimulatorProcessNames
		{
			get {
				return(
					new string[] {
						"fs2002",	// Microsoft Flight Simulator FS2002
						"fs9"		// Microsoft Flight Simulator 2004
					}
				) ;
			}
		}

	}

}
