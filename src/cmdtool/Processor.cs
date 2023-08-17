/*

	Processor.cs - Handle fsrcmdtool User Interface
    Copyright (C) 2003-2004, Marty Ross

*/


using System ;
using fsrlib ;

namespace fsrcmdtool
{

	public abstract class Processor : IFSRConverterHandlerClient, IFSRPlayerHandlerClient
	{

		public Processor(UserInterface ui)
		{
			m_ui= ui ;
			m_progressUpdateDelegate= new FSRProgressUpdateDelegate(
				ProgressUpdateMethod
			) ;
		}

		public abstract CmdToolReturnCode Run(string[] args, int aIndex) ;


		//
		//	Implementation of "IFSRConverterHandlerClient" interface
		//

		public bool ConfirmReplace(string fn)
		{
			return(
				m_ui.GetYesNoAnswerIsYes(
					fn + " exists; OK to replace?"
				)
			) ;
		}

		public FSRConverterFromNMEA.AuxiliaryData GetAuxiliaryGPSData()
		{
			string videoName= m_ui.GetUserData("Enter Video Name: ") ;
			string videoDescription= m_ui.GetUserData("Enter Video Description: ") ;

			return(
				new FSRConverterFromNMEA.AuxiliaryData(
					videoName,
					videoDescription,
					18,
					0,
					double.MaxValue,
					DateTime.MaxValue,
					double.MinValue,
					true
				)
			) ;

		}

		public void UpdateConversionStatus(string msg)
		{
			m_ui.SayToUser(msg) ;
		}


		//
		//	Implementation of "IFSRPlayerHandlerClient" interface
		//

		public FSRPlayerHandler.PlayMethod GetPlayMethod()
		{
			switch(
				m_ui.GetSingleCharacterAnswer(
					"Select play method; (1)File, (2)Pipe or (3)FSUIPC: ",
					"123"
				)
			) {
				case '2':	return(FSRPlayerHandler.PlayMethod.Pipe) ;
				case '3':	return(FSRPlayerHandler.PlayMethod.FSUIPC) ;
			}
			return(FSRPlayerHandler.PlayMethod.File) ;
		}

		public void WaitForUserAfterPlaying()
		{
			m_ui.SayToUser("Press <ENTER> when ready to continue...") ;
			m_ui.WaitForUserToPressEnter() ;
		}

		public void UpdatePlayStatus(string msg)
		{
			m_ui.SayToUser(msg) ;
		}


		//
		//	Subclass helper methods
		//

		protected CmdToolReturnCode FinalReport(bool bSuccess)
		{
			m_ui.SayToUser("") ;		// advance past last progress update line
			if (!bSuccess) {
				//m_ui.ReportError(errMsg) ;		// was already delivered!
				return(CmdToolReturnCode.ReportedError) ;
			}
			return(CmdToolReturnCode.Success) ;
		}
		
		protected FSRProgressUpdateDelegate ProgressUpdateDelegate {
			get {
				return(m_progressUpdateDelegate) ;
			}
		}

		protected UserInterface UI { get { return(m_ui) ; } }


		//
		//	Internal methods
		//

		bool ProgressUpdateMethod(double fraction)
		{
			m_ui.SayToUserNoNewline(((int) ((fraction * 100.0) + 0.5)) + "% complete.") ;
			return(false) ;
		}


		//
		//	Internal data
		//

		UserInterface m_ui ;
		FSRProgressUpdateDelegate m_progressUpdateDelegate ;

	}

}
