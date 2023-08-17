/*

	FSRUserInterface.cs - Client User Interface Support
    Copyright (C) 2003-2004, Marty Ross


	This class provides the fsrlib library with run-time support for
	interaction with client user interfaces.

*/

using System ;

namespace fsrlib
{


	/// <summary>
	/// Delegate method for delivering "percent complete" progress/status
	/// to user via supplied user interface.
	/// </summary>
	public delegate bool FSRProgressUpdateDelegate(double percentComplete) ;

}
