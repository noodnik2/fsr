/*

	FSRWriterFSUIPC.cs - Send FSR data to Simulator via FSUIPC
    Copyright (C) 2004, Marty Ross

	Sends FSR video track data to Simulator via FSUIPC interface for real-time
	playback.

	For more information about FSUIPC, please browse:

		http://www.schiratti.com/dowson.html

*/

using System ;
using System.IO ;
using System.Text ;
using System.Collections ;
using System.Threading ;
using FsuipcSdk ;

namespace fsrlib
{

	public class FSRWriterFSUIPC : FSRWriter
	{

		public FSRWriterFSUIPC(Fsuipc fsuipc)
		{
			m_fsuipc= fsuipc ;
			m_simobjects= new ArrayList() ;
			m_simparams= new ArrayList() ;
			m_simvalues= new ArrayList() ;
			m_pctimeTicks0= DateTime.Now.Ticks ;
		}


		//
		//	Realization of "FSRWriter" abstract methods
		//

		public override Chunk Start()
		{
			return NewChunk() ;
		}

		public override void End(Chunk startChunk)
		{
		}

		public override Chunk FsibStart()
		{
			return NewChunk() ;
		}

		public override void FsibEnd(Chunk fsibChunk)
		{
		}

		public override Chunk FsibBfibStart()
		{
			return NewChunk() ;
		}

		public override void FsibBfibEnd(Chunk bfibChunk)
		{
		}

		public override Chunk FsibBfibOdibStart()
		{
			return NewChunk() ;
		}

		public override void FsibBfibOdibObItem(ushort oid, string oname)
		{
			m_simobjects.Add(new SimObject(oid, oname)) ;
		}

		public override void FsibBfibOdibEnd(Chunk bfibodibChunk)
		{
		}

		public override Chunk FsibBfibPdibStart()
		{
			return NewChunk() ;
		}

		public override void FsibBfibPdibPrItem(
			ushort oid,
			ushort pid,
			ushort tid,
			string punits,
			string pname
		) {
			m_simparams.Add(new SimParam(oid, pid, tid, punits, pname)) ;
		}

		public override void FsibBfibPdibEnd(Chunk bfibpdibChunk)
		{
		}

		public override Chunk FsibBfibFribStart()
		{
			m_simvalues.Clear() ;
			return NewChunk() ;
		}

		public override void FsibBfibFribTsItem(uint frno, uint ticks, long timeval)
		{
			m_frno= frno ;
			m_frticks= ticks ;
			m_frtimeval= timeval ;
		}

		public override void FsibBfibFribFrItem(ushort oid, ushort pid, ushort tid, Object o)
		{
			m_simvalues.Add(new SimValue(pid, o)) ;
		}

		public override void FsibBfibFribEnd(Chunk bfibfribChunk)
		{
			// Wait until frame needs to be sent
			SynchronizeFrameRate() ;

			// send all updated parameters
			foreach (SimParam p in m_simparams) {
				foreach (SimValue v in m_simvalues) {
					if (v.pid == p.pid) {
						UpdateParameter(p, v.sval) ;
						break ;
					}
				}
			}

			SendUpdates() ;
		}

		public override Chunk FsibTrailerStart()
		{
			return NewChunk() ;
		}

		public override void FsibTrailerDaItem(ushort dtype, string dval)
		{
		}

		public override void FsibTrailerEnd(Chunk trailerChunk)
		{
		}


		//
		//
		//

		Chunk NewChunk()
		{
			// no use for chunks here
			return null ;
		}


		/// <summary>
		/// Synchronize frame to "timestamp" set in "m_frtimeval".
		/// </summary>
		void SynchronizeFrameRate()
		{
			// here we must go to sleep until m_frtimeval
			// (m_frtimeval is simulated time in 1/256 second increments)
			if (m_frtimevalSent != 0) {

				// m_frsleeptime is the time we want to elapse between the
				// sending of the previous frame and the sending of the
				// current frame.
				long frsleeptime= (m_frtimeval - m_frtimevalSent) ;
				long frsleeptimeTicks= (long) ((((double) frsleeptime) * (10000000.0 / 256.0)) + 0.5) ;

				// get "pcbusytimeTicks" := ticks that have elapsed
				// (in real-time) since last frame was sent
				long pctimeTicks= DateTime.Now.Ticks ;
				long pcbusytimeTicks= (pctimeTicks - m_pctimeTicksSent) ;

				// adjust "frsleeptimeTicks" to account for time that has already
				// elapsed since sending previous frame (pcbusytimeTicks).  If
				// result is less than zero, then we are not keeping up with
				// the simulation rate.  Perhaps we should count the number of
				// cases like this and notify the user with a warning if the
				// count is greater than zero at the end of the process in
				// this situation?
				frsleeptimeTicks-= pcbusytimeTicks ;
				if (frsleeptimeTicks> 0) {
					Thread.Sleep(TimeSpan.FromTicks(frsleeptimeTicks)) ;
				}
			}

			// mark the time this (will be previous
			// next time through) frame was sent
			m_frtimevalSent= m_frtimeval ;
			m_pctimeTicksSent= DateTime.Now.Ticks ;
		}

		void SendUpdates()
		{
			int dwResult= -1 ;

			if (!m_fsuipc.FSUIPC_Process(ref dwResult)) {
				fsuipcError(dwResult, "process") ;
			}
		}

		void UpdateParameter(SimParam p, Object pval)
		{

#if	false
			Console.Out.WriteLine(
				(
					(double) (m_pctimeTicksSent - m_pctimeTicks0)
					/ 10000000.0
				)
				+ " "
				+	v.pid
				+ " "
				+ v.sval.ToString()
			) ;
#endif
	
			switch(p.pid) {

				case (ushort) FSRPropertyId.PLANE_ALTITUDE:
					SetNumericParam8(
						0x0570,
						pval,
						"altitude",
						1.0,
						FSRGlobals.FeetPerMeter
					) ;
					break ;

				case (ushort) FSRPropertyId.PLANE_LATITUDE:
					SetNumericParam8(
						0x0560,
						pval,
						"latitude",
						10001750.0,
						90.0
					) ;
					break ;

				case (ushort) FSRPropertyId.PLANE_LONGITUDE:
					SetNumericParam8(
						0x0568,
						pval,
						"longitude",
						65536.0 * 65536.0,
						360.0
					) ;
					break ;

				case (ushort) FSRPropertyId.PLANE_BANK_DEGREES:
					SetDegreeParam4(0x057C, pval, "bank") ;
					break ;

				case (ushort) FSRPropertyId.PLANE_HEADING_DEGREES_TRUE:
					SetDegreeParam4(0x0580, pval, "heading") ;
					break ;

				case (ushort) FSRPropertyId.PLANE_PITCH_DEGREES:
					SetDegreeParam4(0x0578, pval, "pitch") ;
					break ;

				case (ushort) FSRPropertyId.AIRSPEED_INDICATED:
					SetIntegerParam4(0x02BC, pval, "indicated airspeed", 128.0, 1.0) ;
					break ;

			}

		}

		void SetIntegerParam4(int dwOffset, Object pval, string parmDesc, double num, double div)
		{
			int token= 0;
			int dwResult= 0 ;
			double dval= (double) pval ;
			dval*= num ;
			dval/= div ;
			int ival= (int) dval ;
			if (!m_fsuipc.FSUIPC_Write(dwOffset, ival, ref token, ref dwResult)) {
				fsuipcError(dwResult, "setting " + parmDesc) ;
			}
		}

		void SetDegreeParam4(int dwOffset, Object pval, string parmDesc)
		{
			int token= 0;
			int dwResult= 0 ;
			double dval= (double) pval ;
			dval*= 65536.0 ;
			dval/= 360.0 ;
			dval*= 65536.0 ;
			int ival= (int) dval ;
			if (!m_fsuipc.FSUIPC_Write(dwOffset, ival, ref token, ref dwResult)) {
				fsuipcError(dwResult, "setting " + parmDesc) ;
			}
		}

		void SetNumericParam8(int dwOffset, Object pval, string parmDesc, double num, double div)
		{
			int token= 0;
			int dwResult= 0 ;
			double dval= (double) pval ;
			dval*= num ;
			dval/= div ;
			dval*= 65536.0 ;
			dval*= 65536.0 ;
			long lval= (long) dval ;
			if (!m_fsuipc.FSUIPC_Write(dwOffset, lval, ref token, ref dwResult)) {
				fsuipcError(dwResult, "setting " + parmDesc) ;
			}
		}

		void fsuipcError(int dwResult, string msg)
		{
			Logger.LogWarning("FSUIPC(" + dwResult + "); " + msg) ;
		}


		//
		//
		//

		uint m_frno ;
		uint m_frticks ;
		long m_frtimeval ;		// time for current frame
		long m_frtimevalSent ;		// time for last sent frame
		long m_pctimeTicksSent ;	// ticks on pc clock at last frame send
		long m_pctimeTicks0 ;		// Module instantiation time/ticks

		ArrayList m_simobjects ;
		ArrayList m_simparams ;
		ArrayList m_simvalues ;

		Fsuipc m_fsuipc ;		// instance of FSUIPC/CSHARP interface

	}
}
