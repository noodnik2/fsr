/*

	ConvertProcessor.cs - Handle fsrcmdtool Conversion User Interface
    Copyright (C) 2003-2004, Marty Ross

*/

using System ;
using fsrlib ;

namespace fsrcmdtool
{

	public class ConvertProcessor : Processor
	{
		public ConvertProcessor(UserInterface ui) : base(ui) { }

		public override CmdToolReturnCode Run(string[] args, int aIndex)
		{
			if ((args.Length - aIndex)< 2) {
				UI.ReportError("'convert' requires input and output filename arguments.") ;
				return(CmdToolReturnCode.SyntaxError) ;
			}

			FSRConverterHandler converterHandler= new FSRConverterHandler(
				this,
				ProgressUpdateDelegate
			) ;

			return(FinalReport(converterHandler.Convert(args[aIndex], args[aIndex+1]))) ;
		}

	}

}
