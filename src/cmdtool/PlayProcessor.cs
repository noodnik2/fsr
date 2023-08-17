/*

	PlayProcessor.cs - Handle fsrcmdtool Play User Interface
    Copyright (C) 2003-2004, Marty Ross

*/

using System ;
using fsrlib ;

namespace fsrcmdtool
{

	public class PlayProcessor : Processor
	{
		public PlayProcessor(UserInterface ui) : base(ui) { }

		public override CmdToolReturnCode Run(string[] args, int aIndex)
		{
			if ((args.Length - aIndex)< 1) {
				UI.ReportError("'play' requires filename argument.") ;
				return(CmdToolReturnCode.SyntaxError) ;
			}

			FSRPlayerHandler playHandler= new FSRPlayerHandler(
				this,
				ProgressUpdateDelegate
			) ;

			return(FinalReport(playHandler.Play(args[aIndex]))) ;
		}

	}

}
