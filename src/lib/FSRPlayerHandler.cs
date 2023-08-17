
/*

	FSRPlayerHandler - Handle fsrlib playback
    Copyright (C) 2003-2004, Marty Ross


	This module defines and manages all simulator playback operations
	supported by fsrlib.


	Highlights:

		IFSRPlayerHandlerClient
		- user interface for supporting playback process
		- must be implemented by client wishing to use FSRPlayerHandler

		FSRPlayerHandler.PlayType
		- enumerated type
		- defines set of supported playbacks

		FSRPlayerHandler.PlayType FSRPlayerHandler.GetPlayType()
		- overloaded, static method
		- returns playback type for specified input file or stream

		FSRPlayerHandler.PlayMethod
		- enumerated type
		- defines set of supported play methods

		FSRPlayerHandler.PlayMethod[] GetPlayMethods()
		- returns list of available play methods

		bool FSRPlayerHandler.Play()
		- method to carry out playback given input file or stream
		- only throws fatal exceptions (e.g. SystemException)
		- performs interaction with user via client's IFSRPlayerHandlerClient interface
		- returns true iff playback carried out successfully

*/


using System ;
using System.IO ;

namespace fsrlib
{

	public interface IFSRPlayerHandlerClient : IFSRPlayerClient
	{
		FSRPlayerHandler.PlayMethod GetPlayMethod() ;
		FSRConverterFromNMEA.AuxiliaryData GetAuxiliaryGPSData() ;
	}

	public class FSRPlayerHandler
	{


		public FSRPlayerHandler(
			IFSRPlayerHandlerClient client,
			FSRProgressUpdateDelegate progressUpdateDelegate
		) {
			m_client= client ;
			m_progressUpdateDelegate= progressUpdateDelegate ;
		}

		/// <summary>
		/// Play file in Simulator.
		/// Sends [at least one] status update(s) to client.
		/// Does not throw ApplicationException.
		/// </summary>
		/// <param name="inFile">name of file to play</param>
		/// <returns>true if play operation succeeded</returns>
		public bool Play(string inFile)
		{

			if (!FSRPlayer.CanFindSimulator()) {
				m_client.UpdatePlayStatus(FSRMsgs.Load("StatusCantFindSimulator")) ;
				return(false) ;
			}

			string fqfnin= Path.GetFullPath(inFile) ;
			if (!File.Exists(fqfnin)) {
				m_client.UpdatePlayStatus(
					FSRMsgs.Format("StatusFileNotFound", fqfnin)
				) ;
				return(false) ;
			}

			PlayType ptype= FSRPlayerHandler.GetPlayType(fqfnin) ;
			if (ptype == PlayType.Unknown) {
				m_client.UpdatePlayStatus(
					FSRMsgs.Load("StatusOperationNotSupported")
				) ;
				return(false) ;
			}

			try {
				switch(m_client.GetPlayMethod()) {

					case PlayMethod.File:
						PlayFile(ptype, fqfnin) ;
						break ;

					case PlayMethod.Pipe:
						PlayPipe(ptype, fqfnin) ;
						break ;

					case PlayMethod.FSUIPC:
						PlayFSUIPC(ptype, fqfnin) ;
						break ;
				}
			}
			catch(Exception exc) {
				m_client.UpdatePlayStatus(exc.Message) ;
				if (!(exc is ApplicationException)) {
					Logger.LogError(exc.StackTrace) ;
				}
				return(false) ;
			}

			m_client.UpdatePlayStatus(
				FSRMsgs.Load("StatusOperationCompleted")
			) ;
			return(true) ;
		}


		/// <summary>
		/// Commands simulator to play indicated file in "real-time", using FSUIPC.
		/// Can throw Exception.
		/// </summary>
		/// <param name="ctype">Conversion type; indicates type of input file.</param>
		/// <param name="fqfni">Fully-qualified name of input file.</param>
		void PlayFSUIPC(PlayType ptype, string fqfni)
		{

			FSRConverterFrom conv= null ;

			switch(ptype) {

				case PlayType.FSR2SIM:
					conv= new FSRConverterFromFSR() ;
					break ;

				case PlayType.XML2SIM:
					conv= new FSRConverterFromXML() ;
					break ;

				case PlayType.NMEA2SIM:
					conv= new FSRConverterFromNMEA(GetAuxiliaryGPSData()) ;
					break ;

				case PlayType.GPX2SIM:
					conv= new FSRConverterFromGPX(GetAuxiliaryGPSData()) ;
					break ;

				case PlayType.CTL2SIM:
					conv= new FSRConverterFromCTL(GetAuxiliaryGPSData()) ;
					break ;

			}

			if (conv == null) {
				throw new ApplicationException(
					FSRMsgs.Load("StatusOperationNotSupported")
				) ;
			}

			conv.Register(m_progressUpdateDelegate) ;
			FSRfsuipcPlayer fsuipcPlayer= new FSRfsuipcPlayer(conv, File.OpenRead(fqfni)) ;

			try {
				fsuipcPlayer.Play(m_client) ;
			}
			finally {
				fsuipcPlayer.Close() ;		// this will close the file also
			}

		}


		/// <summary>
		/// Commands simulator to play indicated file in "real-time", using anonymous pipe.
		/// Performs conversion to file format supported by Simulator if necessary.
		/// Can throw Exception.
		/// </summary>
		/// <param name="ctype">Conversion type; indicates type of input file.</param>
		/// <param name="fqfni">Fully-qualified name of input file.</param>
		void PlayPipe(PlayType ptype, string fqfni)
		{

			FSRPipePlayer pipePlayer= null ;
			Stream fsi= File.OpenRead(fqfni) ;

			switch(ptype) {

				case PlayType.FSR2SIM:
					pipePlayer= new FSRFilePipePlayer(fsi) ;
					((FSRFilePipePlayer) pipePlayer).Register(m_progressUpdateDelegate) ;
					break ;

				case PlayType.XML2SIM:
					FSRConverterFromXML xmlconv= new FSRConverterFromXML() ;
					xmlconv.Register(m_progressUpdateDelegate) ;
					pipePlayer= new FSRConverterPipePlayer(xmlconv, fsi) ;
					break ;

				case PlayType.NMEA2SIM:
					FSRConverterFromNMEA nmeaconv= new FSRConverterFromNMEA(
						GetAuxiliaryGPSData()
					) ;
					nmeaconv.Register(m_progressUpdateDelegate) ;
					pipePlayer= new FSRConverterPipePlayer(nmeaconv, fsi) ;
					break ;

				case PlayType.GPX2SIM:
					FSRConverterFromGPX gpxconv= new FSRConverterFromGPX(
						GetAuxiliaryGPSData()
					) ;
					gpxconv.Register(m_progressUpdateDelegate) ;
					pipePlayer= new FSRConverterPipePlayer(gpxconv, fsi) ;
					break ;

				case PlayType.CTL2SIM:
					FSRConverterFromCTL ctlconv= new FSRConverterFromCTL(
						GetAuxiliaryGPSData()
					) ;
					ctlconv.Register(m_progressUpdateDelegate) ;
					pipePlayer= new FSRConverterPipePlayer(ctlconv, fsi) ;
					break ;

				default:
					fsi.Close() ;
					throw new ApplicationException(
						FSRMsgs.Load("StatusOperationNotSupported")
					) ;

			}

			try {
				pipePlayer.Play(m_client) ;
			}
			finally {
				pipePlayer.Close() ;		// this will close "fsi" also
			}

		}

		/// <summary>
		/// Commands Simulator to play indicated file, then
		/// waits for user to acknowledge playback complete
		/// before commanding Simulator to stop playing.
		/// 
		/// Performs conversion to file format supported by
		/// Simulator using temporary file, if necessary.
		/// 
		/// Can throw ApplicationException.
		/// </summary>
		/// <param name="ctype">Conversion type; indicates type of input file.</param>
		/// <param name="fqfni">Fully-qualified input file name.</param>
		void PlayFile(PlayType ptype, string fqfni)
		{

			switch(ptype) {

				case PlayType.FSR2SIM:
					PlayFSRFile(fqfni) ;
					break ;

				case PlayType.XML2SIM:
					GenerateAndPlayTemporaryFSRFile(
						new FSRConverterFromXML(),
						File.OpenRead(fqfni)
					) ;
					break ;

				case PlayType.NMEA2SIM:
					GenerateAndPlayTemporaryFSRFile(
						new FSRConverterFromNMEA(GetAuxiliaryGPSData()),
						File.OpenRead(fqfni)
					) ;
					break ;

				case PlayType.GPX2SIM:
					GenerateAndPlayTemporaryFSRFile(
						new FSRConverterFromGPX(GetAuxiliaryGPSData()),
						File.OpenRead(fqfni)
					) ;
					break ;

				case PlayType.CTL2SIM:
					GenerateAndPlayTemporaryFSRFile(
						new FSRConverterFromCTL(GetAuxiliaryGPSData()),
						File.OpenRead(fqfni)
					) ;
					break ;

				default:
					throw new ApplicationException(
						FSRMsgs.Load("StatusOperationNotSupported")
					) ;

			}

		}

		/// <summary>
		/// Converts "input" stream to temporary video file.
		/// Commands Simulator to play temporary video file. 
		/// Deletes temporary video file.
		/// Closes the input stream.
		/// Can throw ApplicationException.
		/// </summary>
		/// <param name="conv">FSR converter to use</param>
		/// <param name="input">open stream of file to be converted</param>
		void GenerateAndPlayTemporaryFSRFile(
			FSRConverterFrom conv,
			Stream input
		) {
			string tfsrfqfn= null ;

			try {
				tfsrfqfn= ConvertToTemporaryFSRFile(conv, input) ;
			}
			finally {
				input.Close() ;
			}

			PlayFSRFile(tfsrfqfn) ;
			DeleteTemporaryFSR(tfsrfqfn) ;
		}

		/// <summary>
		/// Produce temporary FSR file by using specified converter and reader.
		/// Caller is responsible to delete this file when done with it.
		/// 
		/// Can throw ApplicationException, though will cleanup temporary file before doing so.
		/// </summary>
		/// <param name="iconv">FSR converter</param>
		/// <param name="input">input FSR stream</param>
		/// <returns>fully-qualified name (e.g., absolute path) of temporary FSR file</returns>
		string ConvertToTemporaryFSRFile(FSRConverterFrom conv, Stream input)
		{
			string tfsrfqfn= Path.GetTempFileName() ;
			Stream fsrofs= null ;

			m_client.UpdatePlayStatus(
				FSRMsgs.Load("StatusGeneratingTemporaryVideo")
			) ;

			try {
				fsrofs= File.Create(tfsrfqfn) ;
				FSRConverter.Convert(
					conv,
					input,
					new FSRWriterFSR(fsrofs),
					m_progressUpdateDelegate
				) ;
			}
			catch(ApplicationException) {
				// if there's an exception while converting,
				// make sure to delete temporary file before
				// re-throwing exception.
				fsrofs.Close() ;
				DeleteTemporaryFSR(tfsrfqfn) ;
				throw ;
			}

			fsrofs.Close() ;
			return(tfsrfqfn) ;
		}

		/// <summary>
		/// Deletes file specified.
		/// </summary>
		/// <param name="tfqfn">fully-qualified name (e.g., absolute pathname) of file to delete</param>
		void DeleteTemporaryFSR(string tfqfn)
		{
			File.Delete(tfqfn) ;
		}

		/// <summary>
		/// Activate the Simulator, then command it to play the specified file.
		/// Call the client's method to wait for the user before commanding
		/// the Simulator to stop playing.
		/// Does not throw any application exceptions.
		/// </summary>
		/// <param name="fqfnfsri">fully-qualified (e.g., absolute path) FSR filename</param>
		void PlayFSRFile(string fqfnfsri)
		{
			try {
				if (FSRGlobals.ActivateSimulatorOnPlay) {
					FSRPlayer.ActivateSimulator() ;
				}
				FSRPlayer.StartPlayback(fqfnfsri) ;	
			}
			catch(ApplicationException e) {
				// we assume that the "StartPlayback" message
				// was not successfully delivered to the Simulator,
				// therefore there's no reason to issue a
				// "StopPlayback" message here.
				m_client.UpdatePlayStatus(e.Message) ;
				return ;
			}

			m_client.UpdatePlayStatus(
				FSRMsgs.Format("StatusNowPlaying", fqfnfsri)
			) ;
			m_client.WaitForUserAfterPlaying() ;

			try {
				FSRPlayer.StopPlayback() ;
			}
			catch(ApplicationException) {
			}

		}

		FSRConverterFromNMEA.AuxiliaryData GetAuxiliaryGPSData()
		{
			FSRConverterFromNMEA.AuxiliaryData auxdata= m_client.GetAuxiliaryGPSData() ;
			if (auxdata == null) {
				throw new ApplicationException(
					FSRMsgs.Load("StatusOperationCanceled")
				) ;
			}
			return(auxdata) ;
		}

		public static PlayType GetPlayType(string ifn)
		{
			return(GetPlayType(FSRFile.GetType(ifn))) ;
		}

		public static PlayType GetPlayType(FSRFile.Type ift)
		{

			switch(ift) {

				case FSRFile.Type.FSR:
					return(PlayType.FSR2SIM) ;

				case FSRFile.Type.XML:
					return(PlayType.XML2SIM) ;

				case FSRFile.Type.NMEA:
					return(PlayType.NMEA2SIM) ;

				case FSRFile.Type.GPX:
					return(PlayType.GPX2SIM) ;

				case FSRFile.Type.CTL:
					return(PlayType.CTL2SIM) ;

			}

			return(PlayType.Unknown) ;
		}

		public static PlayMethod[] GetPlayMethods()
		{
			// there's gotta be a better way to do this!
			PlayMethod t= PlayMethod.File ;
			return((PlayMethod[]) Enum.GetValues(t.GetType())) ;
		}

		public enum PlayType {
			Unknown,
			XML2SIM,
			NMEA2SIM,
			FSR2SIM,
			GPX2SIM,
			CTL2SIM,
		} ;

		public enum PlayMethod {
			File,
			Pipe,
			FSUIPC,
		} ;


		//
		//
		//

		IFSRPlayerHandlerClient m_client ;
		FSRProgressUpdateDelegate m_progressUpdateDelegate ;

	}
}
