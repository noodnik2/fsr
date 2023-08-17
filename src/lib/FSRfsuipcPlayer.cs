/*

	FSRfsuipcPlayer.cs - Handle fsrlib "Play in FSUIPC" operations
    Copyright (C) 2003-2004, Marty Ross


	This module provides an interface and a base class to be used for fsrlib
	operations involving simulator playback using FSUIPC.

*/

using System ;
using System.IO ;
using FsuipcSdk ;

namespace fsrlib
{

	/// <summary>
	/// Base class for "Playback in FSUIPC" operations.
	/// </summary>
	class FSRfsuipcPlayer
	{

		/// <summary>
		/// Sets up the player for FSUIPC.
		/// </summary>
		/// <param name="conv">converter to use for generating FSR data</param>
		/// <param name="input">input stream to feed converter (ownership is taken)</param>
		public FSRfsuipcPlayer(
			FSRConverterFrom conv,
			Stream input
		) {
			m_conv= conv ;
			m_input= input ;
		}

		public void Close()
		{
			m_input.Close() ;
		}


		/// <summary>
		/// Play the video associated with this player.
		/// 
		/// NOTE:
		/// It is intended that this method handle all exceptions
		/// that could be thrown internally.  Any exceptions that
		/// are yet thrown may leave behind resources...  
		/// </summary>
		/// <param name="client">client interface instance</param>
		public void Play(IFSRPlayerClient client)
		{
			int dwResult= -1 ;

			Fsuipc fsuipc= new Fsuipc() ;
			// Open FSUIPC and "register" it using "fsrwintool" application key
			if (!fsuipc.FSUIPC_Open(0, ref dwResult, "5OGTU9QS2CS7")) {
				throw new ApplicationException(
					FSRMsgs.Load("StatusCantOpenFSUIPC")
				) ;
			}

			try {
				if (FSRGlobals.ActivateSimulatorOnPlay) {
					FSRPlayer.ActivateSimulator() ;
				}
			}
			catch {
				fsuipc.FSUIPC_Close() ;
				throw ;
			}

			client.UpdatePlayStatus(FSRMsgs.Load("StatusPlayingVideo")) ;

			try {
				m_conv.Convert(m_input, new FSRWriterFSUIPC(fsuipc)) ;
			}
			catch {
				throw ;
			}
			finally {
				// Cleanup() ;
				fsuipc.FSUIPC_Close() ;
			}

			client.UpdatePlayStatus(FSRMsgs.Load("StatusVideoDelivered")) ;
			// client.WaitForUserAfterPlaying() ;

		}


		//
		//
		//

		private FSRConverterFrom m_conv ;
		private Stream m_input ;

	}

}
