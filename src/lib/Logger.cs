/*

	Logger.cs - fsrlib Logging facility
    Copyright (C) 2003-2004, Marty Ross


	This module defines a helper class providing logging services for
	use with fsrlib.

	Logger is designed to be used as a singleton; all of the public methods
	are static, and no instance of this class need be created or maintained
	by the user.  However, no thread-safety support is provided for this
	singleton, therefore it is not extendable for use by multiple threads.

	Currently, fsrlib is designed to be used in a single-threaded environment
	therefore the above restriction is not an issue.




*/


using System ;
using System.IO ;
using System.Diagnostics ;

namespace fsrlib
{
	/// <summary>
	/// fsrlib logging facility.
	/// default interface is for "System.Console.Out"
	/// </summary>
	/// 
	public class Logger
	{

		/// <summary>
		/// Gets, sets the output destination for the log.
		/// </summary>
		public static TextWriter Out {
			get {
				return(m_out) ;
			}
			set {
				m_out= value ;
			}
		}

		/// <summary>
		/// Gets, sets the global variable enabling/disabling logging.
		/// </summary>
		public static bool Enabled {
			get {
				return(m_logEnabled) ;
			}
			set {
				m_logEnabled= value ;
			}
		}

		/// <summary>
		/// Gets, sets the "mask" of enabled logging streams.
		/// </summary>
		public static int Mask {
			get {
				return(m_logMask) ;
			}
			set {
				m_logMask= value ;
			}
		}

		/// <summary>
		/// Debugging log output.  Removed in non-debug versions of product.
		/// </summary>
		[Conditional("DEBUG")]
		public static void LogDebug(string msg)
		{
			WriteLogMessageIfMask(msg, (int) LogMasks.Debug) ;
		}

		/// <summary>
		/// Run-time assertion, only valid in debug versions of product.
		/// </summary>
		[Conditional("DEBUG")]
		public static void DebugModeAssert(bool bAssertion, string msg)
		{
			if (!bAssertion) {
				LogError(msg) ;
				Debug.Assert(false, msg) ;
			}
		}

		/// <summary>
		/// Write "note" to log.  Ignored if "note" mask not enabled or logging not enabled.
		/// </summary>
		/// <param name="msg">contents of note</param>
		public static void LogNote(string msg)
		{
			WriteLogMessageIfMask(msg, (int) LogMasks.Note) ;
		}

		/// <summary>
		/// Write "warning" to log.  Ignored if "warning" mask not enabled or logging not enabled.
		/// </summary>
		/// <param name="msg">contents of warning</param>
		public static void LogWarning(string msg)
		{
			WriteLogMessageIfMask(msg, (int) LogMasks.Warning) ;
		}

		/// <summary>
		/// Write "error" to log.  Ignored if "error" mask not enabled or logging not enabled.
		/// </summary>
		/// <param name="msg">contents of error</param>
		public static void LogError(string msg)
		{
			WriteLogMessageIfMask(msg, (int) LogMasks.Error) ;
		}

		//
		//
		//

		/// <summary>
		/// Various "log channel" masks which must be set in "mask" to enable channel output.
		/// </summary>
		public enum LogMasks
		{
			Error		= 1,
			Warning		= 2,
			Note		= 4,
			Debug		= 8,
			All		= 0xFFFF
		}

			
		//
		//
		//
		
		private static void WriteLogMessageIfMask(string msg, int userMask)
		{
			if (!m_logEnabled || ((m_logMask & userMask) == 0)) return ;
			Out.WriteLine(msg) ;
		}


		//
		//
		//

		private static bool m_logEnabled= false ;
		private static int m_logMask= 0 ;
		private static TextWriter m_out= Console.Out ;

	}
}
