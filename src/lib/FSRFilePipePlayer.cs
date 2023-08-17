/*

	FSRFilePipePlayer.cs - fsrlib Pipe Player for files
    Copyright (C) 2003-2004, Marty Ross

	This module handles the playing of FSR files using pipes.

*/


using System ;
using System.IO ;

namespace fsrlib
{

	class FSRFilePipePlayer : FSRPipePlayer
	{

		public FSRFilePipePlayer(Stream fsrInputfs)
		{
			m_secsBetweenProgressUpdates= FSRGlobals.SecondsBetweenProgressUpdates_Large ;
			m_fsrInputfs= fsrInputfs ;
			m_updateDelegate= null ;
		}

		public void Register(FSRProgressUpdateDelegate updateDelegate)
		{
			m_updateDelegate= updateDelegate ;
		}

		public override void Close()
		{
			m_fsrInputfs.Close() ;
		}


		//
		//
		//

		protected override void GenerateAndWriteFSRData(Stream fsrOutputfs, int blocksize)
		{

			TimeSpan spanDelta= TimeSpan.FromSeconds(m_secsBetweenProgressUpdates) ;
			DateTime timeNow= DateTime.Now ;
			DateTime timeNext= timeNow.Add(spanDelta) ;

			long offset= 0;
			double totalSize= (double) m_fsrInputfs.Length ;

			byte[] buffer= new byte[blocksize] ;

			while(true) {

				int readcount= m_fsrInputfs.Read(buffer, 0, blocksize) ;
				if (readcount<= 0) break ;

				fsrOutputfs.Write(buffer, 0, readcount) ;
				offset+= (long) readcount ;

				if (m_updateDelegate == null) continue ;

				timeNow= DateTime.Now ;
				if (timeNow< timeNext) continue ;

				if (m_updateDelegate(((double) offset) / totalSize)) {
					throw new ApplicationException(
						"Data transfer aborted."
					) ;
				}

				timeNext= timeNow.Add(spanDelta) ;
			}
		}


		//
		//
		//
		
		private Stream m_fsrInputfs ;
		private FSRProgressUpdateDelegate m_updateDelegate ;
		private double m_secsBetweenProgressUpdates ;

	}

}
