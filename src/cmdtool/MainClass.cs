/*

	fsrcmdtool - Command Line FSRtool Utility
    Copyright (C) 2003-2004, Marty Ross

*/

using System ;
using System.Reflection ;

namespace fsrcmdtool
{

	class MainClass
	{

		[STAThread]
		static int Main(string[] args)
		{
			MainClass mc= new MainClass() ;
			return((int) mc.Run(args)) ;
		}

		MainClass()
		{
			m_ui= new UserInterface() ;
		}

		CmdToolReturnCode Run(string[] args)
		{
			if (args.Length == 0) {
				Syntax() ;
				return(CmdToolReturnCode.Success) ;
			}

			Processor proc= null ;
			switch(args[0]) {

				case "play":
					proc= new PlayProcessor(m_ui) ;
					break ;

				case "convert":
					proc= new ConvertProcessor(m_ui) ;
					break ;

			}

			if (proc == null) {
				m_ui.ReportError("Invalid command: '" + args[0] + "'") ;
				return(CmdToolReturnCode.SyntaxError) ;
			}

			return(proc.Run(args, 1)) ;
		}

		void Syntax()
		{
			AssemblyName aname= null ;
			AssemblyCopyrightAttribute acr= null ;
			AssemblyDescriptionAttribute ada= null ;
			string cmdname= "<cmd>" ;

			Assembly a= Assembly.GetExecutingAssembly() ;

			if (a != null) {
				Object[] ocra= a.GetCustomAttributes(
					Type.GetType("System.Reflection.AssemblyCopyrightAttribute"),
					false
				) ;
				if ((ocra != null) && (ocra.Length> 0)) {
					acr= (AssemblyCopyrightAttribute) ocra[0] ;
				}
				Object[] odaa= a.GetCustomAttributes(
					Type.GetType("System.Reflection.AssemblyDescriptionAttribute"),
					false
				) ;
				if ((odaa != null) & (odaa.Length> 0)) {
					ada= (AssemblyDescriptionAttribute) odaa[0] ;
				}

				aname= a.GetName() ;
			}

			if (aname != null) {	
				cmdname= aname.Name ;
				m_ui.SayToUser(cmdname + " - " + aname.Version) ;
				if (ada != null) m_ui.SayToUser(ada.Description) ;
				if (acr != null) m_ui.SayToUser(acr.Copyright) ;
				m_ui.SayToUser("") ;
			}

			m_ui.SayToUser("Syntax:") ;
			m_ui.SayToUser("\t" + cmdname + " convert <fn1> <fn2>") ;
			m_ui.SayToUser("\t" + cmdname + " play <fsrfn>") ;

		}


		//
		//
		//

		UserInterface m_ui ;

	}

}
