/*

	FSRPipePlayer.cs - Handle fsrlib "Play in Pipe" operations
    Copyright (C) 2003-2004, Marty Ross


	This module provides an interface and a base class to be used for fsrlib
	operations involving simulator playback using an anonymous pipe.

*/

using System ;
using System.IO ;

namespace fsrlib
{

	/// <summary>
	/// Base class for "Playback in Pipe" operations.
	/// </summary>
	abstract class FSRPipePlayer
	{

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

			AnonymousPipe pipe= new AnonymousPipe() ;
			pipe.Open(AnonymousPipe.OpenMode.Write) ;

			FileStream fsrOutputfs= new FileStream(
				pipe.LocalHandle,
				FileAccess.Write,
				false			// "Pipe" owns the handle
			) ;

			try {
				string pipefn= FSRPlayer.BuildFilenameFromHandle(pipe.PeerHandle) ;
				if (FSRGlobals.ActivateSimulatorOnPlay) {
					FSRPlayer.ActivateSimulator() ;
				}
				FSRPlayer.StartPlayback(pipefn) ;
			}
			catch {
				// Assume that the "StartPlayback" didn't go through
				// so there's no need to "StopPlayback".
				fsrOutputfs.Close() ;
				pipe.Close() ;
				throw ;
			}

			client.UpdatePlayStatus(FSRMsgs.Load("StatusPlayingVideo")) ;

			try {
				GenerateAndWriteFSRData(fsrOutputfs, pipe.BufferSize) ;
			}
			catch {
				FSRPlayer.StopPlayback() ;
				throw ;
			}
			finally {
				// Cleanup() ;
				fsrOutputfs.Close() ;
				pipe.Close() ;
			}

			client.UpdatePlayStatus(FSRMsgs.Load("StatusVideoDelivered")) ;
			client.WaitForUserAfterPlaying() ;

			try {
				FSRPlayer.StopPlayback() ;
			}
			catch(Exception e) {
				client.UpdatePlayStatus(
					FSRMsgs.Format(
						"StatusUnexpectedError",
						e.Message
					)
				) ;
			}

		}

		public virtual void Close() { }


		//
		//
		//

		protected abstract void GenerateAndWriteFSRData(Stream fsrOutputfs, int blocksize) ;

	}

}
