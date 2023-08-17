#define	LogDiscardedData		// Log discarded data in NMEA stream

/*

	NmeaStreamParser.cs - NMEA Format Parser
    Copyright (C) 2004, Marty Ross

	Provides a simple parser to extract and deliver the minimum values
	necessary for the "FSRConverterFromGPSStream" object from an NMEA
	data stream.

	For more information about NMEA data format, see:

		http://www.nmea.org/

	This parser tested only with NMEA data produced by Magellan Companion
	accessory for Handspring Visor; however, it should work with all NMEA
	standard data streams.

*/

using System ;
using System.IO ;
using System.Globalization ;
using System.Collections ;

namespace fsrlib
{

	public abstract class NmeaStreamParser : StreamParser
	{

		/*
			public interface
		*/

		public NmeaStreamParser()
		{
			m_state= State.Start ;
			m_sentence= new NmeaSentence() ;
			m_buffer= new char[82] ;
			m_buftail= 0 ;

			m_sentenceDataHandlers= new ArrayList(25) ; // # of GPS sentences anticipated
			m_comma= new Char[] {','} ;
		}

		public void AddDataHandler(NmeaDataHandler ndh)
		{
			m_sentenceDataHandlers.Add(ndh) ;
		}

		public virtual void HandleDiscard(NmeaSentence s)
		{
#if	LogDiscardedData
			char[] cmd= s.Command.ToCharArray() ;
			HandleDiscard(cmd, cmd.Length) ;
#endif
		}

		public override void Parse(Stream s)
		{
			int i ;
			for (
				byte[] buffer= new byte[4096] ;
				(i= s.Read(buffer, 0, 1024)) != 0 ;
			) {
				Parse(buffer, 0, i) ;	
			} 
		}

		public void Parse(byte[] data, int offset, int length)
		{
			for (int i= offset; i< length; i++) {
				Parse((char) data[i]) ;
			}
		}

		public void Parse(char cdata)
		{
			accum(cdata) ;

			switch(m_state) {

				case State.Start:
					if (cdata != '$') break ;
					if (m_buftail> 1) {
						HandleDiscard(m_buffer, m_buftail - 1) ;
					}
					m_buftail= 0 ;
					m_checksum= 0 ;
					m_sentence.m_checksummed= false ;
					m_state= State.Command ;
					break ;

				case State.Command:
					m_checksum^= (byte) cdata ;
					if ((cdata != ',') && (cdata != '*')) break ;	// TODO: this should be: "if(isalpha(cdata))"
					if (((cdata != '*') && (cdata != ',')) || (m_buftail< 2) || (m_buftail> 10)) {
						error("corrupt address") ;
						break ;
					}
					m_sentence.m_command= new string(m_buffer, 0, m_buftail - 1) ;
					m_state= (cdata == '*')? State.Checksum: State.Data ;
					m_buftail= 0 ;
					break ;

				case State.Data:
					if ((cdata != '*') && (cdata != '\r')) {
						m_checksum^= (byte) cdata ;
						break ;
					}
					m_sentence.m_data= new string(m_buffer, 0, m_buftail - 1) ;
					m_buftail= 0 ;
					m_state= (cdata == '*')? State.Checksum: State.End ;
					break ;

				case State.Checksum:
					if (m_buftail< 3) break ;
					if (cdata != '\r') {
						error(string.Format("corrupt checksum: 0x{0:X}", (int) cdata)) ;
						break ;
					}
					int checksum= byte.Parse(
						new string(m_buffer, 0, 2),
						NumberStyles.HexNumber
					) ;
					if (checksum != m_checksum) {
						error(
							String.Format(
								"invalid checksum: 0x{0:X} (published) != 0x{1:X} (calculated)",
								checksum,
								m_checksum
							)
						) ;
						break ;
					}
					m_sentence.m_checksummed= true ;
					m_state= State.End ;
					break ;

				case State.End:
					if (cdata != '\n') {
						error("missing linefeed") ;
						break ;
					}
					HandleSentence(m_sentence) ;
					m_state= State.Start ;
					m_buftail= 0 ;
					break ;

			}
		}


		/*
			private interface
		*/

		private void HandleSentence(NmeaSentence s)
		{
			// Logger.LogDebug("gps sentence: cmd='" + s.Command + "'") ;
			foreach (NmeaDataHandler h in m_sentenceDataHandlers) {
				if (h.Command == s.Command) {
					string error= h.HandleData(s.Data.Split(m_comma)) ;
					if (error != null) {
						HandleError(error + ", data='" + s.Data + "'") ;
					}
					return ;
				}
			}
			HandleDiscard(s) ;
		}

		private void error(string msg)
		{
			if (msg != null) {
				HandleError(msg + ": " + new string(m_buffer, 0, m_buftail)) ;
			}
			if (m_buftail> 0) {
				HandleDiscard(m_buffer, m_buftail) ;
			}
			m_buftail= 0 ;
			m_state= State.Start ;
		}

		private void accum(char cdata)
		{
			if (m_buftail == m_buffer.Length) {
				error("buffer overflow") ;
			}
			m_buffer[m_buftail++]= cdata ;
		}

		private State m_state ;
		private NmeaSentence m_sentence ;
	
		private enum State { Start, Command, Data, Checksum, End } ;

		private byte m_checksum ;
		private char[] m_buffer ;
		private int m_buftail ;
		private ArrayList m_sentenceDataHandlers ;	// collection of NmeaDataHandlers
		private char[] m_comma ;

	}

}
