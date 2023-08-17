using System ;
using System.IO ;

namespace fsrlib
{

	public abstract class FSRConverterFrom
	{

		public abstract void Convert(FSRReader rdr, FSRWriter wrr) ;
		public abstract void Convert(Stream input, FSRWriter wrr) ;

		public void Register(FSRProgressUpdateDelegate updateMethod)
		{
			m_updateMethod= updateMethod ;
			m_secsBetweenUpdates= FSRGlobals.SecondsBetweenProgressUpdates_Large ;
		}


		//
		//
		//

		protected void DoProgressUpdate(FSRReader rdr)
		{
			if (m_updateMethod == null) return ;

			//
			//	We'd like to "update" our client every 3 seconds.
			//

			DateTime timeNow= DateTime.Now ;
			if (timeNow.Subtract(m_lastReportAt).TotalSeconds< m_secsBetweenUpdates) {
				return ;
			}
			
			if (m_updateMethod(rdr.FractionRead)) {
				throw new ApplicationException(
					FSRMsgs.Load("StatusOperationAborted")
				) ;
			}

			m_lastReportAt= timeNow ;
		}


		//
		//
		//

		private double m_secsBetweenUpdates ;
		private FSRProgressUpdateDelegate m_updateMethod ;
		private DateTime m_lastReportAt ;

	}
}
