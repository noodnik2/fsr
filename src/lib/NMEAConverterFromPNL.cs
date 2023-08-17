/*

	NMEAConverterFromPNL - Extract NMEA sentences from Pathaway Log
    Copyright (C) 2003, Marty Ross


	Extract the raw NMEA data stream from a "Pathaway NMEA Log" Palm O/S Database
	(i.e. using Pathaway's "Record Log Data" option).

	The name of this file is "PathawayLog.PDB" when backed up or archived
	from the Palm device.


	NOTES/TODO:

	This file implements a "heuristic" (e.g., try it until it seems to work)
	method for extracting the NMEA data stream from the records of the PDB.

	A reliable algorithm should be substitued for extracting the NMEA
	sentences from the Pathaway Log.

*/

using System ;
using System.IO ;

namespace fsrlib
{

	public class NMEAConverterFromPNL
	{

		public NMEAConverterFromPNL()
		{
			m_crlf= new byte[] { (byte) '\r', (byte) '\n' } ;
		}

		public void Convert(
			Stream sin,
			Stream sout,
			FSRProgressUpdateDelegate progressUpdateDelegate
		) {

			m_progressUpdateDelegate= progressUpdateDelegate ;
			if (m_progressUpdateDelegate != null) {
				m_spanBetweenProgressUpdates= TimeSpan.FromSeconds(
					FSRGlobals.SecondsBetweenProgressUpdates_Small
				) ;
				m_nextProgressUpdateTime= DateTime.Now.Add(
					m_spanBetweenProgressUpdates
				) ;
			}

			m_input= sin ;
			m_output= sout ;

			PDBReader.Parse(
				new BinaryReader(sin),
				"PwLg",
				new PDBReader.PushRecordData(PNLRecordCallback)
			) ;

		}


		//
		//
		//

		void PNLRecordCallback(BinaryReader input, int recsize)
		{

			byte[] pnlrec= input.ReadBytes(recsize) ;

			int i ;

			// find start
			for (i= 0; (i< pnlrec.Length) && (pnlrec[i] != '$'); i++) ;
			int startIndex= i ;

			// find end
			for (i= startIndex; (i< pnlrec.Length) && (pnlrec[i] != '*'); i++) ;
			int checksumIndex= i ;

			// nothing to do if malformed
			if ((startIndex == checksumIndex) || (checksumIndex> (pnlrec.Length - 3))) {
				Logger.LogWarning(
					"malformed sentence: '"
				      + PDBReader.ZeroTerminatedByteArrayToString(pnlrec)
				      + "'"
				) ;
				return ;
			}

			for (i= startIndex; i< checksumIndex + 3; i++) {
				if (pnlrec[i] != 0) {
					m_output.WriteByte(pnlrec[i]) ;
				}
			}

			m_output.Write(m_crlf, 0, m_crlf.Length) ;

			ConsiderUpdatingProgress() ;
		}

		void ConsiderUpdatingProgress()
		{

			if (m_progressUpdateDelegate == null) return ;

			DateTime timeNow= DateTime.Now ;
			if (timeNow< m_nextProgressUpdateTime) return ;

			double fraction= 0 ;
			double totalLength= (double) m_input.Length ;
			if (totalLength> 0) {
				fraction= (double) m_input.Position ;
				fraction/= totalLength ;
			}
				
			m_progressUpdateDelegate(fraction) ;

			m_nextProgressUpdateTime= timeNow.Add(
				m_spanBetweenProgressUpdates
			) ;
			
		}

		Stream m_input ;
		Stream m_output ;
		byte[] m_crlf ;
		FSRProgressUpdateDelegate m_progressUpdateDelegate ;
		DateTime m_nextProgressUpdateTime ;
		TimeSpan m_spanBetweenProgressUpdates ;

	}

}
