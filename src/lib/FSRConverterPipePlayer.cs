/*

	FSRConverterPipePlayer.cs - fsrlib Pipe Player for conversions
    Copyright (C) 2003-2004, Marty Ross

	This module handles the playing of on-the-fly FSR conversions files using pipes.

*/


using System ;
using System.IO ;

namespace fsrlib
{

	class FSRConverterPipePlayer : FSRPipePlayer
	{

		public FSRConverterPipePlayer(
			FSRConverterFrom conv,
			Stream input
		) {
			m_conv= conv ;
			m_input= input ;
		}

		public override void Close()
		{
			m_input.Close() ;
		}


		//
		//
		//

		protected override void GenerateAndWriteFSRData(Stream fsrOutputfs, int blocksize)
		{
			m_conv.Convert(m_input, new FSRWriterFSR(fsrOutputfs, blocksize)) ;
		}


		//
		//
		//
		
		private FSRConverterFrom m_conv ;
		private Stream m_input ;
	}

}
