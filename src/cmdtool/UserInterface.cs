/*

	UserInterface.cs - User Interface class for fsrcmdtool
    Copyright (C) 2003-2004, Marty Ross

*/

using System ;
using System.IO ;

namespace fsrcmdtool
{

	public class UserInterface
	{

		public UserInterface()
		{
			m_output= Console.Out ;
			m_input= Console.In ;
			m_error= Console.Error ;
		}

		public void SayToUser(string msg)
		{
			m_output.WriteLine(msg) ;
		}

		public void ReportError(string msg)
		{
			m_error.WriteLine(msg) ;
		}

		public void SayToUserNoNewline(string msg)
		{
			m_output.Write(msg + "\r") ;
		}

		public void WaitForUserToPressEnter()
		{
			m_input.ReadLine() ;
		}

		public string GetUserData(string prompt)
		{
			m_output.Write(prompt) ;
			return(m_input.ReadLine()) ;
		}

		public char GetSingleCharacterAnswer(string prompt, string validset)
		{
			while(true) {
				string answer= GetUserData(prompt) ;
				if (answer.Length == 1) {
					if (validset.IndexOf(answer)>= 0) {
						return(answer[0]) ;
					}
				}
				m_output.WriteLine(
					"(Please input single character reply from set: '"
				      + validset
				      + "'"
				) ;
			}
		}

		public bool GetYesNoAnswerIsYes(string prompt)
		{
			return(GetSingleCharacterAnswer(prompt, "yn") == 'y') ;
		}


		//
		//
		//

		TextReader m_input ;
		TextWriter m_output ;
		TextWriter m_error ;

	}

}
