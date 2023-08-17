#define	STAT3		// support FSRWriterStata
//#define	STAT2		// support FSRWriterSTAT2
//#define	STAT		// support FSRWriterSTAT
//#define	SupportFSRFiltering

/*

	FSRConverterHandler - Handle fsrlib conversions
    Copyright (C) 2003-2004, Marty Ross


	This module defines and manages all conversions supported by fsrlib.

	Highlights:

		IFSRConverterHandlerClient
		- user interface for supporting conversion process
		- must be implemented by client wishing to use FSRConverterHandler

		FSRConverterHandler.ConvertType
		- enumerated type
		- defines set of supported conversions

		FSRConverterHandler.ConvertType FSRConverterHandler.GetConvertType()
		- overloaded, static method
		- returns conversion type for specified input and output files or streams

		bool FSRConverterHandler.Convert()
		- method to carry out conversion given input and output files or streams
		- only throws fatal exceptions (e.g. SystemException)
		- performs interaction with user via client's IFSRConverterHandlerClient interface
		- returns true iff conversion carried out successfully

*/

using System ;
using System.IO ;

namespace fsrlib
{

	public interface IFSRConverterHandlerClient
	{
		bool ConfirmReplace(string fn) ;
		void UpdateConversionStatus(string msg) ;
		FSRConverterFromGPSStream.AuxiliaryData GetAuxiliaryGPSData() ;
	}

	public class FSRConverterHandler
	{

		//
		//	public methods
		//

		public FSRConverterHandler(
			IFSRConverterHandlerClient client,
			FSRProgressUpdateDelegate progressUpdateDelegate
		) {
			m_client= client ;
			m_progressUpdateDelegate= progressUpdateDelegate ;
		}

		/// <summary>
		/// Perform file conversion operation.
		/// Sends [at least one] status update(s) to client.
		/// Does not throw ApplicationException.
		/// </summary>
		/// <param name="inFile">input file name</param>
		/// <param name="outFile">output file name</param>
		/// <returns>true iff conversion succeeded</returns>
		public bool Convert(string inFile, string outFile)
		{

			string fqfnin= Path.GetFullPath(inFile) ;
			string fqfnout= Path.GetFullPath(outFile) ;

			FSRConverterHandler.ConvertType ctype= FSRConverterHandler.GetConvertType(
				fqfnin,
				fqfnout
			) ;

			if (ctype == FSRConverterHandler.ConvertType.Unknown) {
				m_client.UpdateConversionStatus(
					FSRMsgs.Load("StatusOperationNotSupported")
				) ;
				return(false) ;
			}

			if (!File.Exists(fqfnin)) {
				m_client.UpdateConversionStatus(
					FSRMsgs.Format("StatusFileNotFound", fqfnin)
				) ;
				return(false) ;
			}

			if (File.Exists(fqfnout)) {
				if (!m_client.ConfirmReplace(fqfnout)) {
					m_client.UpdateConversionStatus(
						FSRMsgs.Load("StatusOperationCanceled")
					) ;
					return(false) ;
				}
			}

			Stream inputStream ;
			try {
				inputStream= File.OpenRead(fqfnin) ;
			}
			catch(Exception exc) {
				m_client.UpdateConversionStatus(exc.Message) ;
				return(false) ;
			}

			Stream outputStream ;
			try {
				outputStream= File.Create(fqfnout) ;
			}
			catch(Exception exc) {
				inputStream.Close() ;
				m_client.UpdateConversionStatus(exc.Message) ;
				return(false) ;
			}

			try {
				ConvertFile(ctype, inputStream, outputStream) ;
			}
			catch(Exception exc) {
				m_client.UpdateConversionStatus(exc.Message) ;
				if (!(exc is ApplicationException)) {
					Logger.LogError(exc.StackTrace) ;
				}
				return(false) ;
			}
			finally {
				inputStream.Close() ;
				outputStream.Close() ;
			}

			m_client.UpdateConversionStatus(
				FSRMsgs.Load("StatusOperationCompleted")
			) ;
			return(true) ;
		}


		//
		//	internal methods
		//

		/// <summary>
		/// Handles specified conversion (according to conversion type),
		/// using "input" as input stream and "output" as output stream.
		/// </summary>
		/// <param name="ctype">conversion type</param>
		/// <param name="input">input stream</param>
		/// <param name="output">output stream</param>
		void ConvertFile(ConvertType ctype, Stream input, Stream output)
		{

			switch(ctype) {


				// PDB to ?

				case ConvertType.PNL2NMEA:
					(new NMEAConverterFromPNL()).Convert(
						input,
						output,
						m_progressUpdateDelegate
					) ;
					break ;


				// NMEA to ?

				case ConvertType.NMEA2XML:
					FSRConverter.Convert(
						new FSRConverterFromNMEA(GetGPSStreamAuxiliaryData()),
						input,
						new FSRWriterXML(output),
						m_progressUpdateDelegate
					) ;
					break ;

				case ConvertType.NMEA2FSR:
					FSRConverter.Convert(
						new FSRConverterFromNMEA(GetGPSStreamAuxiliaryData()),
						input,
						new FSRWriterFSR(output),
						m_progressUpdateDelegate
					) ;
					break ;

#if	STAT
				case ConvertType.NMEA2STAT:
					FSRConverter.Convert(
						new FSRConverterFromNMEA(GetGPSStreamAuxiliaryData()),
						input,
						new FSRWriterSTAT(output),
						m_progressUpdateDelegate
					) ;
					break ;
#endif

#if	STAT2
				case ConvertType.NMEA2STAT:
					FSRConverter.Convert(
						new FSRConverterFromNMEA(GetGPSStreamAuxiliaryData()),
						input,
						new FSRWriterSTAT2(output),
						m_progressUpdateDelegate
					) ;
					break ;
#endif

#if	STAT3
				case ConvertType.NMEA2STAT:
					FSRConverter.Convert(
						new FSRConverterFromNMEA(GetGPSStreamAuxiliaryData()),
						input,
						new FSRWriterStata(output),
						m_progressUpdateDelegate
					) ;
					break ;
#endif


				// XML to ?

				case ConvertType.XML2FSR:
					FSRConverter.Convert(
						new FSRConverterFromXML(),
						input,
						new FSRWriterFSR(output),
						m_progressUpdateDelegate
					) ;
					break ;

				// GPX to ?

				case ConvertType.GPX2FSR:
					FSRConverter.Convert(
						new FSRConverterFromGPX(GetGPSStreamAuxiliaryData()),
						input,
						new FSRWriterFSR(output),
						m_progressUpdateDelegate
					) ;
					break ;

				case ConvertType.GPX2XML:
					FSRConverter.Convert(
						new FSRConverterFromGPX(GetGPSStreamAuxiliaryData()),
						input,
						new FSRWriterXML(output),
						m_progressUpdateDelegate
					) ;
					break ;


				// CTL to ?

				case ConvertType.CTL2FSR:
					FSRConverter.Convert(
						new FSRConverterFromCTL(GetGPSStreamAuxiliaryData()),
						input,
						new FSRWriterFSR(output),
						m_progressUpdateDelegate
					) ;
					break ;

				case ConvertType.CTL2XML:
					FSRConverter.Convert(
						new FSRConverterFromCTL(GetGPSStreamAuxiliaryData()),
						input,
						new FSRWriterXML(output),
						m_progressUpdateDelegate
					) ;
					break ;

#if	STAT
				case ConvertType.XML2STAT:
					FSRConverter.Convert(
						new FSRConverterFromXML(),
						input,
						new FSRWriterSTAT(output),
						m_progressUpdateDelegate
					) ;
					break ;
#endif

#if	STAT2
				case ConvertType.XML2STAT:
					FSRConverter.Convert(
						new FSRConverterFromXML(),
						input,
						new FSRWriterSTAT2(output),
						m_progressUpdateDelegate
					) ;
					break ;
#endif

#if	STAT3
				case ConvertType.XML2STAT:
					FSRConverter.Convert(
						new FSRConverterFromXML(),
						input,
						new FSRWriterStata(output),
						m_progressUpdateDelegate
					) ;
					break ;
#endif

				// FSR to ?

				case ConvertType.FSR2XML:
					FSRConverter.Convert(
						new FSRConverterFromFSR(),
						input,
						new FSRWriterXML(output),
						m_progressUpdateDelegate
					) ;
					break ;

#if	STAT
				case ConvertType.FSR2STAT:
					FSRConverter.Convert(
						new FSRConverterFromFSR(),
						input,
						new FSRWriterSTAT(output),
						m_progressUpdateDelegate
					) ;
					break ;
#endif

#if	STAT2
				case ConvertType.FSR2STAT:
					FSRConverter.Convert(
						new FSRConverterFromFSR(),
						input,
						new FSRWriterSTAT2(output),
						m_progressUpdateDelegate
					) ;
					break ;
#endif

#if	STAT3
				case ConvertType.FSR2STAT:
					FSRConverter.Convert(
						new FSRConverterFromFSR(),
						input,
						new FSRWriterStata(output),
						m_progressUpdateDelegate
					) ;
					break ;
#endif


#if	SupportFSRFiltering
				case ConvertType.FSR2FSR:
					FSRConverter.Convert(
						new FSRConverterFromFSR(),
						input,
						new FSRWriterFilter(new FSRWriterFSR(output)),
						m_progressUpdateDelegate
					) ;
					break ;
#endif


				default:
					throw new ApplicationException(
						FSRMsgs.Load("StatusOperationNotSupported")
					) ;

			}

		}


		/// <summary>
		/// Collect and return auxiliary data from user for conversion process
		/// involviing GPS track log input.  If user cancels auxiliary data
		/// dialog, cancel operation.
		/// </summary>
		FSRConverterFromGPSStream.AuxiliaryData GetGPSStreamAuxiliaryData()
		{
			FSRConverterFromGPSStream.AuxiliaryData auxdata= m_client.GetAuxiliaryGPSData() ;
			if (auxdata == null) {
				throw new ApplicationException(
					FSRMsgs.Load("StatusOperationCanceled")
				) ;
			}
			return(auxdata) ;
		}


		//
		//	Calculate conversion and file types
		//

		/// <summary>
		/// Impute conversion type from input and output file names.
		/// </summary>
		/// <param name="ifn">input file name</param>
		/// <param name="ofn">output file name</param>
		/// <returns>imputed conversion type</returns>
		public static ConvertType GetConvertType(string ifn, string ofn)
		{
			return(
				GetConvertType(
					FSRFile.GetType(ifn),
					FSRFile.GetType(ofn)
				)
			) ;
		}

		/// <summary>
		/// Impute conversion type from input and output file types.
		/// </summary>
		/// <param name="ift">input file type</param>
		/// <param name="oft">output file type</param>
		/// <returns>imputed conversion type</returns>
		public static ConvertType GetConvertType(FSRFile.Type ift, FSRFile.Type oft)
		{

			// FSR to ?
			if ((ift == FSRFile.Type.FSR) && (oft == FSRFile.Type.XML)) {
				return(ConvertType.FSR2XML) ;	
			}

#if	SupportFSRFiltering
			if ((ift == FSRFile.Type.FSR) && (oft == FSRFile.Type.FSR)) {
				return(ConvertType.FSR2FSR) ;	
			}
#endif

#if	STAT || STAT2 || STAT3
			if ((ift == FSRFile.Type.FSR) && (oft == FSRFile.Type.STAT)) {
				return(ConvertType.FSR2STAT) ;	
			}
#endif

			// XML to ?

			if ((ift == FSRFile.Type.XML) && (oft == FSRFile.Type.FSR)) {
				return(ConvertType.XML2FSR) ;	
			}


			// GPX to ?

			if ((ift == FSRFile.Type.GPX) && (oft == FSRFile.Type.FSR)) {
				return(ConvertType.GPX2FSR) ;	
			}

			if ((ift == FSRFile.Type.GPX) && (oft == FSRFile.Type.XML)) {
				return(ConvertType.GPX2XML) ;	
			}


			// CTL to ?

			if ((ift == FSRFile.Type.CTL) && (oft == FSRFile.Type.FSR)) {
				return(ConvertType.CTL2FSR) ;	
			}

			if ((ift == FSRFile.Type.CTL) && (oft == FSRFile.Type.XML)) {
				return(ConvertType.CTL2XML) ;	
			}


			// NMEA to ?

			if ((ift == FSRFile.Type.NMEA) && (oft == FSRFile.Type.FSR)) {
				return(ConvertType.NMEA2FSR) ;	
			}

			if ((ift == FSRFile.Type.NMEA) && (oft == FSRFile.Type.XML)) {
				return(ConvertType.NMEA2XML) ;	
			}

#if	STAT || STAT2 || STAT3
			if ((ift == FSRFile.Type.NMEA) && (oft == FSRFile.Type.STAT)) {
				return(ConvertType.NMEA2STAT) ;	
			}
#endif

			// PDB to ?

			if ((ift == FSRFile.Type.PNL) && (oft == FSRFile.Type.NMEA)) {	// PathAway to NMEA
				return(ConvertType.PNL2NMEA) ;	
			}


			return(ConvertType.Unknown) ;
		}

		public enum ConvertType {
			Unknown,
			FSR2XML,
			FSR2FSR,
			XML2FSR,
			GPX2FSR,
			GPX2XML,
			CTL2FSR,
			CTL2XML,
			NMEA2XML,
			NMEA2FSR,
			PNL2NMEA,
#if	STAT || STAT2 || STAT3
			FSR2STAT,
			XML2STAT,
			NMEA2STAT,
#endif
		} ;


		//
		//
		//

		IFSRConverterHandlerClient m_client ;
		FSRProgressUpdateDelegate m_progressUpdateDelegate ;

	}

}
