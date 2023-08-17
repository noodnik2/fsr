/*

	PDBReader.cs - PalmOS Database Reader
    Copyright (C) 2004, Marty Ross

	Provides a simple parser to extract records from a PalmOS database.

	For more information about PalmOS databases, see:

		http://www.palmos.com/dev/support/docs/fileformats/FileFormatsTOC.html

*/

using System ;
using System.IO ;
using System.Net ;
using System.Text ;

namespace fsrlib
{

	public class PDBReader
	{

		/// <summary>
		/// Delegate method to read record from PDB database.  The record
		/// is delivered in the form of an input Stream object, currently
		/// positioned at the start of the PDB record.  The size of the
		/// record is given in the "recsize" parameter.
		/// 
		/// Constraints:
		/// (1) The delegate method must not read more data from the input
		/// stream than is specified by the "recsize" parameter (though it
		/// may read less than this amount).
		/// </summary>
		public delegate void PushRecordData(BinaryReader input, int recsize) ;

		public static void Parse(BinaryReader input, string pdbType, PushRecordData prd)
		{

			Header hdr= new Header(input) ;

			string stype= ZeroTerminatedByteArrayToString(hdr.type) ;
			string screator= ZeroTerminatedByteArrayToString(hdr.creator) ;

			Logger.LogDebug("name " + ZeroTerminatedByteArrayToString(hdr.name)) ;
			Logger.LogDebug("version " + hdr.version.ToString()) ;
			//Logger.LogDebug("attributes " + hdr.attributes.ToString()) ;
			Logger.LogDebug("type " + stype) ;
			Logger.LogDebug("creator " + screator) ;
			//Logger.LogDebug("creationDate " + hdr.creationDate) ;
			//Logger.LogDebug("backupDate " + hdr.lastBackupDate) ;
			//Logger.LogDebug("modificationNumber " + hdr.modificationNumber) ;
			Logger.LogDebug("appInfoID " + hdr.appInfoID) ;
			Logger.LogDebug("numRecords " + hdr.numRecords) ;
			Logger.LogDebug("nextRecordListID " + hdr.nextRecordListID) ;

			if (stype != pdbType) {
				Logger.LogError(
					"PalmOS Database type ("
				      + stype
				      + ") was not '"
				      + pdbType
				      + "' as expected"
				) ;
				throw new ApplicationException(
					FSRMsgs.Load("StatusInvalidFile")
				) ;
			}

			if (hdr.nextRecordListID != 0) {
				Logger.LogError(
					"PalmOS Database uses unsupported 'RecordList' feature."
				) ;
				throw new ApplicationException(
					FSRMsgs.Load("StatusInvalidFile")
				) ;
			}

			int totNumRecords= hdr.numRecords ;
			bool bHasInfo= (hdr.appInfoID != 0) ;

			if (bHasInfo) totNumRecords++ ;
			Logger.LogDebug("totNumRecords " + totNumRecords) ;

			Record[] recs= new Record[totNumRecords] ;
			int recIndex= 0 ;
			
			if (bHasInfo) {
				recs[recIndex++].localChunkID= hdr.appInfoID ;
			}

			// read record list
			for (int i= 0; i< hdr.numRecords; i++) {
				recs[recIndex++]= new Record(input) ;
			}

			// read "placeholder" bytes
			ReadPDBushort(input) ;

			long offset= (80 + (hdr.numRecords * 8)) ;

			for (int i= 0; i< totNumRecords; i++) {

				Record r= recs[i] ;

				long start= r.localChunkID ;
				long end ;

				if (i == (totNumRecords - 1)) {
					end= input.BaseStream.Length ;
				}
				else {
					end= (long) recs[i+1].localChunkID ;
				}

				if (offset != start) {
					Logger.LogWarning("PDBReader: offset " + start + " != " + offset) ;
				}

				int reclen= (int) (end - start) ;
				if (prd != null) {
					prd(input, reclen) ;	// deliver record to user
				}
				offset+= reclen ;

				// skip to end of PDB record if not already there
				for (
					long skipcount= offset - input.BaseStream.Position ;
					skipcount> 0;
					skipcount--
				) {
					input.ReadByte() ;
				}

			}

		}


		//
		//
		//

		internal struct Header
		{

			internal Header(BinaryReader input)
			{
				name= input.ReadBytes(32) ;
				attributes= (ushort) ReadPDBushort(input) ;
				version= (ushort) ReadPDBushort(input) ;
				creationDate= (uint) ReadPDBuint(input) ;
				modificationDate= (uint) ReadPDBuint(input) ;
				lastBackupDate= ReadPDBuint(input) ;
				modificationNumber= ReadPDBuint(input) ;
				appInfoID= ReadPDBuint(input) ;
				sortInfoID= ReadPDBuint(input) ;
				type= input.ReadBytes(4) ;
				creator= input.ReadBytes(4) ;
				uniqueIDSeed= ReadPDBuint(input) ;
				nextRecordListID= ReadPDBuint(input) ;
				numRecords= ReadPDBushort(input) ;
			}

			/// <summary>
			/// A 32-byte long, null-terminated string containing the
			/// name of the database on the Palm Powered handheld.  The
			/// name is restricted to 31 bytes in length, plus the 
			/// terminator byte.  This name is also used to create the
			/// file name of the PDB when it is backed up during the
			/// HotSync process.
			/// </summary>
			internal byte[] name ;

			/// <summary>
			/// The attribute flags for the database.
			/// </summary>
			internal ushort attributes ;

			/// <summary>
			/// The application-specific version of the database layout.
			/// </summary>
			internal ushort version ;

			/// <summary>
			/// The creation date of the database, specified as the
			/// number of seconds since 12:00 A.M. on January 1, 1904.
			/// </summary>
			internal uint creationDate ;

			/// <summary>
			/// The date of the most recent modification of the database,
			/// specified as the number of seconds since 12:00 A.M. on
			/// January 1, 1904.
			/// </summary>
			internal uint modificationDate ;

			/// <summary>
			/// The date of the most recent backup of the database,
			/// specified as the number of seconds since 12:00 A.M.
			/// on January 1, 1904.
			/// </summary>
			internal uint lastBackupDate ;

			/// <summary>
			/// The modification number of the database. 
			/// </summary>
			internal uint modificationNumber ;

			/// <summary>
			/// The local offset from the beginning of the database
			/// header data to the start of the optional, application-
			/// specific appInfo block.  Set to NULL for databases
			/// that do not include an appInfo block.
			/// </summary>
			internal uint appInfoID ;

			/// <summary>
			/// The local offset from the beginning of the PDB header
			/// data to the start of the optional, application-specific
			/// sortInfo block.  Set to NULL for databases that do not
			/// include an sortInfo block.
			/// </summary>
			internal uint sortInfoID ;

			/// <summary>
			/// The database type identifier. 
			/// </summary>
			internal byte[] type ;

			/// <summary>
			/// The database creator identifier. 
			/// </summary>
			internal byte[] creator ;

			/// <summary>
			/// Used internally by the Palm OS to generate unique
			/// identifiers for records on the Palm device when
			/// the database is loaded into the device.
			/// </summary>
			internal uint uniqueIDSeed ;

			/*

				Notes regading the Record List (excerpted from
				www.palmos.com support website):

					The structure of Palm databases allows for multiple
					record lists in a single database; the record lists
					are chained together by setting the nextRecordListID
					field of the first record list to the offset of the
					next list in the database.  In practice, this capability
					is very rarely used, and the nextRecordListID field in
					the database header is almost always set to 0, which
					indicates that there is only one record list in the
					database. Since a single record list can be used to
					describe the maximum number of records (64K) in a file,
					multiple record lists are never required.  PalmSource,
					Inc. recommends against building databases with chained
					headers, and that your parsing code reject databases
					that have a non-zero value in the nextRecordListID
					field, to avoid potentially truncating such a database
					if your code encounters one. 

			*/

			/// <summary>
			/// The local chunk ID of the next record list in this database.
			/// This is 0 if there is no next record list, which is almost
			/// always the case. 
			/// </summary>
			internal uint nextRecordListID ;

			/// <summary>
			/// The number of record entries in this list.
			/// </summary>
			internal ushort numRecords ;	// # of records in record list

		}

		internal struct Record
		{

			internal Record(BinaryReader input)
			{
				localChunkID= ReadPDBuint(input) ;
				attributes= input.ReadByte() ;
				uniqueID= input.ReadBytes(3) ;
			}

			/// <summary>
			///	The local offset from the top of the PDB to the
			///	start of the raw record data for this entry. 
			///	Note that you can determine the size of each
			///	chunk of raw record data by subtracting the
			///	starting offset of the chunk from the starting
			///	offset of the following chunk. If the chunk is
			///	the last chunk, it's end is determined by the
			///	end of the file.
			/// </summary>
			internal uint localChunkID ;	// 4 bytes localid

			/// <summary>
			/// Single byte set of attributes of the record.
			/// </summary>
			internal byte attributes ;

			/// <summary>
			/// A three-byte long unique ID for the record.
			/// </summary>
			internal byte[] uniqueID ;	// 3 bytes uniqueid

		}


		//
		//	TODO:	Find/use .NET platform methods instead of these
		//		hand-coded ones!!
		//
		internal static ushort ReadPDBushort(BinaryReader input)
		{
			return((ushort) IPAddress.NetworkToHostOrder(input.ReadInt16())) ;
		}

		internal static uint ReadPDBuint(BinaryReader input)
		{
			return((uint) IPAddress.NetworkToHostOrder(input.ReadInt32())) ;
		}

		internal static int ReadPDBint(BinaryReader input)
		{
			return((int) IPAddress.NetworkToHostOrder(input.ReadInt32())) ;
		}

		internal static string ZeroTerminatedByteArrayToString(byte[] ba, int len)
		{
			StringBuilder s= new StringBuilder(len) ;
			for (int i= 0; i< len; i++) {
				if (ba[i] == 0) break ;
				s.Append((char) ba[i]) ;	
			}
			return(s.ToString()) ;
		}

		internal static string ZeroTerminatedByteArrayToString(byte[] ba)
		{
			return(ZeroTerminatedByteArrayToString(ba, ba.Length)) ;
		}

	}

}
