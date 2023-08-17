/*

	FSRPlayer.cs - fsrlib Player Helper
    Copyright (C) 2003-2004, Marty Ross


	This class provides a set of static helper methods used for handling
	all fsrlib play operations.

*/


using System ;
using System.Diagnostics ;
using System.Runtime.InteropServices ;

namespace fsrlib
{

	/// <summary>
	/// Suggested user interface for FSR player clients.
	/// An object implementing this interface should be supplied
	/// to the FSR player when performing a playback operation.
	/// </summary>
	public interface IFSRPlayerClient
	{
		void UpdatePlayStatus(string msg) ;
		void WaitForUserAfterPlaying() ;
	}


	public class FSRPlayer
	{

		/// <summary>
		/// Return true iff can identify currently running
		/// Simulator instance on which other methods can
		/// perform operations.
		/// </summary>
		/// <returns>true iff Simulator found</returns>
		public static bool CanFindSimulator()
		{
			IntPtr hWnd= GetSimulatorHandle() ;
			return(hWnd != IntPtr.Zero) ;
		}

		/// <summary>
		/// Activates Simulator application (brings it into
		/// foreground and gives it input focus) on which other
		/// methods can operate.
		/// 
		/// Throws ApplicationException() if can't find Simulator.
		/// </summary>
		public static void ActivateSimulator()
		{
			const int SW_RESTORE= 9 ;

			IntPtr hWnd= SimulatorHandle ;
			if (IsIconic(hWnd)) {
				ShowWindowAsync(hWnd, SW_RESTORE) ;
			}
			SetForegroundWindow(hWnd) ;
		}

		/// <summary>
		/// Commands currently active Simulator to start playing
		/// video using FSR file identified by fqfnfsr.
		/// 
		/// Throws ApplicationException() if can't find Simulator.
		/// </summary>
		/// <param name="fqfnfsr">fully-qualified name of input FSR file</param>
		public static void StartPlayback(string fqfnfsr)
		{
			Tell(Action.StartPlaying, fqfnfsr) ;
		}

		public static void StopPlayback()
		{
			Tell(Action.StopPlaying, null) ;
		}

		/// <summary>
		/// Commands currently active Simulator to start recording
		/// video into FSR file identified by fqfnfsr.
		/// 
		/// Throws ApplicationException() if can't find Simulator.
		/// </summary>
		/// <param name="fqfnfsr">fully-qualified name of output FSR file</param>
		public static void StartRecording(string fsrfn)
		{
			Tell(Action.StartRecording, fsrfn) ;
		}

		public static void StopRecording()
		{
			Tell(Action.StopRecording, null) ;
		}

		/// <summary>
		/// Builds a "filename" that can be used in "StartPlayback"
		/// (and perhaps "StartRecording()", though this has not been
		/// verified) for communicating data to the Simulator through
		/// an operating system pipe.
		/// </summary>
		/// <param name="pipeHandle">handle of the O/S pipe</param>
		/// <returns>resulting "filename" corresponding to "pipeHandle"</returns>
		public static string BuildFilenameFromHandle(IntPtr pipeHandle)
		{
			return(
				"<HA>"
			      + ((uint) pipeHandle).ToString("X8")
			      + Process.GetCurrentProcess().Id.ToString("X8")
			) ;
		}


		//
		//
		//

		enum Action {
			StartPlaying,
			StopPlaying,
			StartRecording,
			StopRecording
		} ;

		/// <summary>
		/// Command Flight Simulator to take specified action.
		/// 
		/// See Microsoft's "Netpipes SDK 2002" documentation for
		/// more information about what's going on in this function.
		/// </summary>
		/// <param name="action">specified action to take</param>
		/// <param name="fn">file name (if action requires file)</param>
		static void Tell(Action action, string fn)
		{

			const int LET_START_VIDEO_RECORDING=	0x0001B001 ;
			const int LET_STOP_VIDEO_RECORDING=	0x0001B002 ;
			const int LET_START_VIDEO_PLAYBACK=	0x0001B003 ;
			const int LET_STOP_VIDEO_PLAYBACK=	0x0001B004 ;

			const int WM_COPYDATA=			0x004A ;

			IntPtr hWnd= SimulatorHandle ;

			COPYDATASTRUCT cpdata= new COPYDATASTRUCT() ;

			switch(action) {
	
				case Action.StartPlaying:
					cpdata.dwData= LET_START_VIDEO_PLAYBACK ;
					cpdata.lpData= Marshal.StringToHGlobalAnsi(fn) ;
					cpdata.cbData= fn.Length + 1 ;
					break ;
	
				case Action.StartRecording:
					cpdata.dwData= LET_START_VIDEO_RECORDING ;
					cpdata.lpData= Marshal.StringToHGlobalAnsi(fn) ;
					cpdata.cbData= fn.Length + 1 ;
					break ;
	
				case Action.StopPlaying:
					cpdata.dwData= LET_STOP_VIDEO_PLAYBACK ;
					cpdata.lpData= IntPtr.Zero ;
					break ;
	
				case Action.StopRecording:
					cpdata.dwData= LET_STOP_VIDEO_RECORDING ;
					cpdata.lpData= IntPtr.Zero ;
					break ;

				default:
					throw new ApplicationException("FSRPlayer.Tell() invalid action") ;
	
			}

			IntPtr dcpstruc= Marshal.AllocHGlobal(Marshal.SizeOf(cpdata)) ;
			Marshal.StructureToPtr(cpdata, dcpstruc, false) ;

			int rc= SendMessage(hWnd, WM_COPYDATA, (int) hWnd, (int) dcpstruc) ;

			Marshal.FreeHGlobal(dcpstruc) ;
			if (cpdata.lpData != IntPtr.Zero) {
				Marshal.FreeHGlobal(cpdata.lpData) ;
			}

			if (rc != 0) {
				throw new ApplicationException("FSRPlayer.Tell(); R(" + rc + ") from SendMessage()") ;
			}

		}

		static IntPtr SimulatorHandle
		{
			get {
				IntPtr hWnd= GetSimulatorHandle() ;
				if (hWnd == IntPtr.Zero) {
					throw new ApplicationException("Cannot find running simulator.") ;
				}
				return(hWnd) ;
			}
		}

		static IntPtr GetSimulatorHandle()
		{

			//
			// look for a Microsoft Flight Simulator
			// (or compatible) process, by name
			//
			foreach (string s in FSRGlobals.SimulatorProcessNames) {
				Process[] fsimprocs= Process.GetProcessesByName(s) ;
				if (fsimprocs.Length == 1) return(fsimprocs[0].MainWindowHandle) ;
			}

			// none found
			return(IntPtr.Zero) ;
		}


		//
		//	externals
		//

		[StructLayout(LayoutKind.Sequential)]
		struct COPYDATASTRUCT {
			public int dwData ;
			public int cbData ;
			public IntPtr lpData ;
		}

		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd) ;

		[DllImport("user32.dll")]
		private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow) ;

		[DllImport("user32.dll")]
		private static extern bool IsIconic(IntPtr hWnd) ;

		[DllImport("user32.dll", EntryPoint="SendMessage")]
		private static extern int SendMessage(IntPtr hWnd, int msgid, int wParam, int lParam) ;

	}

}
