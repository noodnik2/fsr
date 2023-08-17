using System ;
using System.Text ;
using System.IO ;
using fsrlib ;

namespace fsrwintool
{

	public class LogBuffer
	{

		internal class BufferedTextWriter : TextWriter
		{
			internal BufferedTextWriter(StringWriter sw)
			{
				m_sw= sw ;
			}

			public override void Write(char c)
			{
				m_sw.Write(c) ;
			}

			public override Encoding Encoding {
				get {
					return(Encoding.ASCII) ;
				}
			}

			private StringWriter m_sw ;
		}

		public LogBuffer()
		{
			m_sb= new StringBuilder() ;
			m_tw= new BufferedTextWriter(new StringWriter(m_sb)) ;
		}

		public bool IsEmpty()
		{
			return(m_sb.Length == 0) ;
		}

		public void Clear()
		{
			m_sb.Length= 0 ;
		}

		public override string ToString()
		{
			return(m_sb.ToString()) ;
		}

		public TextWriter Out {
			get {
				return(m_tw) ;
			}
		}


		//
		//
		//

		private StringBuilder m_sb ;
		private TextWriter m_tw ;

	}
}
