/*

	FSRWriterSTAT.cs - Write basic statistics about FSR stream
    Copyright (C) 2003, Marty Ross


	This module defines a FSRWriter to calculate and write basic statistics
	about an FSR-class stream that is written through it.

*/


using System ;
using System.IO ;
using System.Text ;

namespace fsrlib
{

	public class FSRWriterSTAT : FSRWriter
	{

		public FSRWriterSTAT(Stream output)
		{
			m_output= new StatWriter(output) ;

			m_statTicks= new NumStat() ;
			m_statDTicks= new NumStat() ;

			m_statTimevals= new NumStat() ;
			m_statDTimevals= new NumStat() ;

			m_statAbstimes= new NumStat() ;
			m_statDAbstimes= new NumStat() ;

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
			m_output.WriteLine("N obitems: " + m_nObItems) ;
			m_output.WriteLine("N pritems: " + m_nPrItems) ;
			m_output.WriteLine("N bfibs: " + m_nBfibs) ;
			m_output.WriteLine() ;
			m_output.WriteLine("N fribs: " + m_nFribs) ;
			m_output.WriteLine("Fribs/Bfib: " + (double) m_nFribs / (double) m_nBfibs) ;
			m_output.WriteLine() ;
			m_output.WriteLine("Ticks (" + m_statTicks.Range / m_statAbstimes.Range + "/sec):") ;
			m_output.WriteLine("\tN: " + m_statTicks.N) ;
			m_output.WriteLine("\tMean: " + m_statTicks.Mean) ;
			m_output.WriteLine("\tMin: " + m_statTicks.Min) ;
			m_output.WriteLine("\tMax: " + m_statTicks.Max) ;
			m_output.WriteLine("\tRange: " + m_statTicks.Range) ;
			m_output.WriteLine("\tSD: " + m_statTicks.SD) ;
			m_output.WriteLine() ;
			m_output.WriteLine("DTicks:") ;
			m_output.WriteLine("\tN: " + m_statDTicks.N) ;
			m_output.WriteLine("\tMean: " + m_statDTicks.Mean) ;
			m_output.WriteLine("\tMin: " + m_statDTicks.Min) ;
			m_output.WriteLine("\tMax: " + m_statDTicks.Max) ;
			m_output.WriteLine("\tRange: " + m_statDTicks.Range) ;
			m_output.WriteLine("\tSD: " + m_statDTicks.SD) ;
			m_output.WriteLine() ;
			m_output.WriteLine("Timevals:") ;
			m_output.WriteLine("\tN: " + m_statTimevals.N) ;
			m_output.WriteLine("\tMean: " + m_statTimevals.Mean) ;
			m_output.WriteLine("\tMin: " + m_statTimevals.Min + " (" + DateString(m_statTimevals.Min / 256) + " UTC)") ;
			m_output.WriteLine("\tMax: " + m_statTimevals.Max + " (" + DateString(m_statTimevals.Max / 256) + " UTC)") ;
			m_output.WriteLine("\tRange: " + m_statTimevals.Range) ;
			m_output.WriteLine("\tSD: " + m_statTimevals.SD) ;
			m_output.WriteLine() ;
			m_output.WriteLine("DTimevals:") ;
			m_output.WriteLine("\tN: " + m_statDTimevals.N) ;
			m_output.WriteLine("\tMean: " + m_statDTimevals.Mean) ;
			m_output.WriteLine("\tMin: " + m_statDTimevals.Min) ;
			m_output.WriteLine("\tMax: " + m_statDTimevals.Max) ;
			m_output.WriteLine("\tRange: " + m_statDTimevals.Range) ;
			m_output.WriteLine("\tSD: " + m_statDTimevals.SD) ;
			m_output.WriteLine() ;
			m_output.WriteLine("Abstimes:") ;
			m_output.WriteLine("\tN: " + m_statAbstimes.N) ;
			m_output.WriteLine("\tMean: " + m_statAbstimes.Mean) ;
			m_output.WriteLine("\tMin: " + m_statAbstimes.Min + " (" + DateString(m_statAbstimes.Min) + " UTC)") ;
			m_output.WriteLine("\tMax: " + m_statAbstimes.Max + " (" + DateString(m_statAbstimes.Max) + " UTC)") ;
			m_output.WriteLine("\tRange: " + m_statAbstimes.Range) ;
			m_output.WriteLine("\tSD: " + m_statAbstimes.SD) ;
			m_output.WriteLine() ;
			m_output.WriteLine("DAbstimes:") ;
			m_output.WriteLine("\tN: " + m_statDAbstimes.N) ;
			m_output.WriteLine("\tMean: " + m_statDAbstimes.Mean) ;
			m_output.WriteLine("\tMin: " + m_statDAbstimes.Min) ;
			m_output.WriteLine("\tMax: " + m_statDAbstimes.Max) ;
			m_output.WriteLine("\tRange: " + m_statDAbstimes.Range) ;
			m_output.WriteLine("\tSD: " + m_statDAbstimes.SD) ;

		}

		public override Chunk FsibStart()
		{
			return NewChunk() ;
		}

		public override void FsibEnd(Chunk fsibChunk)
		{
			m_nFsibs++ ;
		}

		public override Chunk FsibBfibStart()
		{
			return NewChunk() ;
		}

		public override void FsibBfibEnd(Chunk bfibChunk)
		{
			m_nBfibs++ ;
		}

		public override Chunk FsibBfibOdibStart()
		{
			return NewChunk() ;
		}

		public override void FsibBfibOdibObItem(ushort oid, string oname)
		{
			m_nObItems++ ;
		}

		public override void FsibBfibOdibEnd(Chunk bfibodibChunk)
		{
			m_nOdibs++ ;
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
			m_nPrItems++ ;
		}

		public override void FsibBfibPdibEnd(Chunk bfibpdibChunk)
		{
			m_nPdibs++ ;
		}

		public override Chunk FsibBfibFribStart()
		{
			return NewChunk() ;
		}

		public override void FsibBfibFribTsItem(uint frno, uint ticks, long timeval)
		{
			m_nTsItems++ ;

			m_statTicks.Add((double) ticks) ;
			m_statTimevals.Add((double) timeval) ;

			if (m_nTsItems> 1) {
				m_statDTicks.Add((double) ticks - (double) m_prevTicks) ;
				m_statDTimevals.Add((double) timeval - (double) m_prevTimeval) ;
			}

			m_prevTicks= ticks ;
			m_prevTimeval= timeval ;
		}

		public override void FsibBfibFribFrItem(ushort oid, ushort pid, ushort tid, Object o)
		{
			m_nFrItems++ ;

			if (
				(oid == (ushort) FSRObjectId.Environment)
			     && (pid == (ushort) FSRPropertyId.ABSOLUTE_TIME)
			) {
				m_nAbstimes++ ;
				double absTime= (double) o ;
				m_statAbstimes.Add(absTime) ;
				if (m_nAbstimes> 1) {
					m_statDAbstimes.Add(absTime - m_prevAbstime) ;
				}
				m_prevAbstime= absTime ;
			}

		}

		public override void FsibBfibFribEnd(Chunk bfibfribChunk)
		{
			m_nFribs++ ;
		}

		public override Chunk FsibTrailerStart()
		{
			return NewChunk() ;
		}

		public override void FsibTrailerDaItem(ushort dtype, string dval)
		{
			m_nDaItems++ ;
		}

		public override void FsibTrailerEnd(Chunk trailerChunk)
		{
			m_nTrailers++ ;
		}


		//
		//
		//

		Chunk NewChunk()
		{
			// no use for chunks here
			return null ;
		}


		//
		//
		//

		internal class NumStat
		{

			internal void Add(double val)
			{

				// ref: http://www.megspace.com/science/sfe/i_ot_std.html

				if (m_n == 0) {
					m_mean= val ;
					m_min= m_max= val ;
					m_n++ ;
					return ;
				}

				if (val> m_max) m_max= val ;
				if (val< m_min) m_min= val ;

				double deltaMean, sqrtmp ;

				m_n++ ;
				deltaMean= (val - m_mean) / m_n ;
				sqrtmp= m_n * deltaMean ;
				m_ssd+= (sqrtmp * sqrtmp) ;
				m_mean+= deltaMean ;
				sqrtmp= val - m_mean ;
				m_ssd+= (sqrtmp * sqrtmp) ;

			}

			internal double Mean { get { return(m_mean) ; } }
			internal double SD { get { return(Math.Sqrt(m_ssd / (m_n - 1))) ; } }
			internal double N { get { return(m_n) ; } } 
			internal double Range { get { return(m_max - m_min) ; } }
			internal double Min { get { return(m_min) ; } }
			internal double Max { get { return(m_max) ; } }

			double m_ssd ;
			double m_mean ;
			double m_n ;
			double m_min ;
			double m_max ;
		}

		internal class StatWriter : TextWriter
		{
			internal StatWriter(Stream output)
			{
				m_output= output ;
			}
			public override void Write(char c)
			{
				m_output.WriteByte((byte) c) ;
			}
			public override Encoding Encoding {
				get {
					return(Encoding.ASCII) ;
				}
			}
			Stream m_output ;
		}


		//
		//
		//

		/// <summary>
		/// Translate UTC time, represented in seconds, to string using current culture.
		/// </summary>
		/// <param name="abstime">MSFS "ABSOLUTE_TIME" parameter value (seconds since
		/// 00:00 January 1, 0000, UTC)</param>
		/// <returns>String in current culture representing this UTC time value</returns>
		private string DateString(double abstime)
		{
			try {
				DateTime dt= DateTime.MinValue.AddSeconds(abstime) ;
				return(dt.ToString()) ;
			}
			catch {
				return("<invalid>") ;
			}
		}

		//
		//
		//

		StatWriter m_output ;

		protected int
			m_nFsibs,
			m_nBfibs,
			m_nObItems,
			m_nOdibs,
			m_nPrItems,
			m_nPdibs,
			m_nTsItems,
			m_nFrItems,
			m_nFribs,
			m_nDaItems,
			m_nTrailers,
			m_nAbstimes
		;

		NumStat m_statTicks ;
		NumStat m_statTimevals ;
		NumStat m_statAbstimes ;
		NumStat m_statDTicks ;
		NumStat m_statDTimevals ;
		NumStat m_statDAbstimes ;

		uint m_prevTicks ;
		long m_prevTimeval ;
		double m_prevAbstime ;

	}
}
