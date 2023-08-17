/*

	StreamParser.cs - Stream Parser base class
    Copyright (C) 2003-2004, Marty Ross


	This module provides a base class for input stream parsers.  The methods
	provided by this class are default logging services for errors and warnings
	generated by the (derived) parser class.  Their use by the derived class(es)
	is recommended (though not mandatory), and their implementation may be
	overridden in the derived class(es).

*/



using System ;
using System.IO ;

namespace fsrlib
{

	/// <summary>
	/// General stream parser class with overridable handler callbacks
	/// for user feedback about errors and discarded data.
	/// </summary>
	public abstract class StreamParser
	{

		/*
			public interface
		*/

		public abstract void Parse(Stream s) ;

		public virtual void HandleError(string errMsg)
		{
			Logger.LogError(
				FSRMsgs.Format(
					"StatusStreamParserError",
					errMsg
				)
			) ;
		}

		public virtual void HandleDiscard(char[] junked, int length)
		{
#if DEBUG
			m_ndiscards++ ;

			if (m_ndiscards >= NDISCARDMAX) {
				if (m_ndiscards == NDISCARDMAX) {
					Logger.LogError(FSRMsgs.Load("StatusMoreLikeThisIgnored")) ;
				}
				return ;
			}

			Logger.LogDebug(
				FSRMsgs.Format(
					"StatusStreamParserDiscard",
					new String(junked, 0, length)
				)
			) ;
#endif
		}

#if	DEBUG
		private int m_ndiscards= 0 ;
		protected int NDISCARDMAX= 1000 ;		// don't let this grow uncontrollably...
#endif

	}

}
