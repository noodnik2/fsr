/*

	FSRReaderFSR.cs - Helper for reading MSFS FSR Binary Files
    Copyright (C) 2003-2004, Marty Ross


	This module provides methods helpful for reading binary FSR-class files
	from within in the fsrlib library.

*/

using System ;
using System.IO ;

namespace fsrlib
{

	/*
		FSR File Format Notes
		(from MSFS2002 Netpipes SDK Documentation)

		Within File there is:
			1 x FileHeader
			n x DataRecord
			1 x FileTrailer

		Within DataRecord there is:
			1 x DataRecordHeader
			n x DataSection
			1 x DataRecordFooter

		Within FileTrailer there is:
			1 x DataRecordHeader
			n x FileTrailerRecords
			1 x DataRecordFooter

		Within DataRecordHeader there is:
			1 x 4-byte ID
			1 x DWORD size

		Within DataRecordFooter there is:
			1 x DWORD crc
			1 x DWORD size

		Within DataSection there is:
			1 x DataSectionHeader
			1 x DataSectionData

		DataSectionData is one of:
			ObjectDefinitionDataSectionData
			PropertyDictionaryDataSectionData
			FrameSectionDataSectionData
			
	*/

	public class FSRReaderFSR : FSRReader
	{

		public override void Close()
		{
			m_input.Close() ;
		}

		public FSRReaderFSR(Stream sin)
		{
			m_input= new BinaryReader(sin) ;
		}

		public Section ReadSection()
		{
			return new Section(m_input) ;
		}

		public Chunk ReadChunk()
		{
			return(new Chunk(m_input)) ;
		}

		public void SkipInput(uint length)
		{
			for (int i= 0; i< length; i++) {
				m_input.ReadByte() ;
			}
		}

		public override double FractionRead
		{
			get {
				return(GetStreamFractionRead(m_input.BaseStream)) ;
			}
		}

		//
		//	BinaryReader property:
		//		Allow client to access base binary input stream.
		//
		public BinaryReader BinaryReader { get { return(m_input) ; } }


		/*
			internal public classes
		*/

		public class Section
		{
			public const uint NameSize= 4 ;

			public Section(BinaryReader rdr)
			{
				m_name= new string(rdr.ReadChars((int) Section.NameSize)) ;
			}

			public string Name { get { return(m_name) ; } }

			private string m_name ;
		}

		public class Chunk
		{
			public Chunk(BinaryReader rdr)
			{
				m_name= new string(rdr.ReadChars((int) Section.NameSize)) ;
				m_size= rdr.ReadUInt32() ;
			}

			public const uint HeaderSize= Section.NameSize + 4 ;

			public string Name { get { return(m_name) ; } }
			public uint Size { get { return(m_size) ; } }	// actual chunksize, including header
			public uint Length { get { return(Size - HeaderSize) ; } }

			private string m_name ;
			private uint m_size ;
		}


		/*
			private area
		*/

		private BinaryReader m_input ;

	}
}
