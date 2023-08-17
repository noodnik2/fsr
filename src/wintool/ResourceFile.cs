/*

	ResourceFile.cs - Extract Embedded File from Assembly Resources
    Copyright (C) 2004, Marty Ross


	The ResourceFile class makes it possible to access a "resource file".  Such a file
	can either be placed in the same directory as the assembly from which this class is
	loaded, or embedded in a resource inside that same assembly.  If an existing disk
	file with the name specified is found in the same directory as the assembly, it is
	returned.  Otherwise, an attempt is made to load the specified resource file (from
	the resource manifest of the assembly from which this class is loaded) into the
	current Windows temporary directory using the name specified.

	The constructor looks at only the "filename" portion of the supplied resource file
	name.  The fully-qualified path/filename of the resulting file/copy is returned.

	If a copy of the resource-embedded form of this file is made, it is automatically
	deleted when the ResourceFile object is Disposed() (a call to Close() achieves this).

	The original purpose for this class was to enable me to access a help file embedded
	in the assembly manifest of the "fsrwintool" program.  In order to increase the
	flexibility of help file distribution/update, I later added the feature whereby an
	existing file (in the assembly directory) would be used instead.  Therefore, there
	are two "modes" the class now operates: "temporary mode" - where a copy of the embedded
	file is made to the Windows temporary directory and later erased, and "reference
	mode" - where an existing file (located in the same directory as the assembly) will
	be used, if present.

	I understand that this probably violates some existing standard for the location of
	help files, but since I didn't find such a standard after several searches, I created
	this mechanism.  Subject to change!!


	Usage:

		ResourceFile rf= new ResourceFile("NameOfResourceFile") ;
		rf.Open() ;		// Access a disk copy of the resource file 
		string ResourceDiskFilename= rf.Name ;

			// use ResourceDiskFilename as you wish here

		rf.Dispose() ;		// this erases the temporary disk file

		rf.Open() ;		// Access the file again; this revives the "rf" object
		string NewResourceDiskFilename= rf.Name ;

			// use NewResourceDiskFilename; not necessarily same name as ResourceDiskFilename

		rf.Close() ;		// same as "rf.Dispose()" - should call to release disk file

*/

using System ;
using System.IO ;
using System.Reflection ;

namespace fsrwintool
{

	/// <summary>
	/// Manages access to existing, or copy of embedded resource file.
	/// </summary>
	public class ResourceFile : IDisposable
	{

		/// <summary>
		/// Sets up the ResourceFile instance.  Does not do any file I/O
		/// and does not throw any exceptions.
		/// </summary>
		/// <param name="filename">name of help file, as embedded into resources or present on disk</param>
		public ResourceFile(String filename)
		{
			m_resourcefilename= Path.GetFileName(filename) ;  // ignore any existing path
			m_diskfilename= null ;
			m_bDisposed= false ;
		}

		~ResourceFile() { Dispose(false) ; }

		/// <summary>
		/// Cleans up all resources used by the ResourceFile instance.
		/// </summary>
		public void Dispose()
		{
			Dispose(true) ;
		}

		/// <summary>
		/// "Opens" this ResourceFile reference.  Does not throw
		/// any exceptions during normal operation.
		/// </summary>
		public void Open()
		{
			// nothing to do if already open
			if (m_diskfilename != null) return ;

			// look in same directory as assembly for resource file
			Type ThisType= this.GetType() ;
			Assembly ExecutingAssembly= Assembly.GetAssembly(ThisType) ;
			string AssemblyPath= Path.GetDirectoryName(ExecutingAssembly.Location) ;
			string diskfilename= Path.Combine(AssemblyPath, m_resourcefilename) ;

			// if it exists, use it
			if (File.Exists(diskfilename)) {
				m_diskfilename= diskfilename ;
				m_bFileIsTemp= false ;
				return ;
			}

			// if it doesn't exist, try to unload it from assembly resource
			// into Windows temporary directory
			string AssemblyNamespace= ThisType.Namespace ;
			string resourcename= AssemblyNamespace + "." + m_resourcefilename ;
			Stream rs= Assembly.GetExecutingAssembly().GetManifestResourceStream(
				resourcename
			) ;
			if (rs == null) return ;		// not found in assembly resource

			string tempfilename= null ;
			try {
				tempfilename= Path.Combine(
					Path.GetTempPath(),
					m_resourcefilename
				) ;
				CopyStream(rs, tempfilename, 4096) ;
			}
			catch {
				if (tempfilename != null) {
					try { File.Delete(tempfilename) ; } catch { }
				}
				return ;			// can't unload from resource into temp dir
			}

			// revive the object if it was previously disposed
			if (m_bDisposed) {
				GC.ReRegisterForFinalize(this) ;
				m_bDisposed= false ;
			}

			m_diskfilename= tempfilename ;
			m_bFileIsTemp= true ;

		}

		/// </summary>
		/// Closes the ResourceFile instance.  If the instance is currently "open"
		/// (see above), the existing temporary disk file fill be erased.
		/// </summary>
		public void Close()
		{
			Dispose() ;
		}

		/// <summary>
		/// Gets the name of the disk file copy of the embedded resource.
		/// Automatically opens the ResourceFile instance, if necessary.
		/// A null return indicates that "Open()" was not called, or that it failed.
		/// </summary>
		public string Name
		{
			get {
				return(m_diskfilename) ;
			}
		}


		//
		//
		//

		/// <summary>
		/// Frees resources associated with this ResourceFile instance.
		/// </summary>
		/// <param name="bDisposing">false iff called from destructor</param>
		protected virtual void Dispose(bool bDisposing)
		{
			// Nothing to do if already disposed
			if (m_bDisposed) return ;

			// Mark object as disposed
			m_bDisposed= true ;

			// Cleanup "managed resources"
			if (bDisposing) {
				if (m_diskfilename != null) {
					if (m_bFileIsTemp) {
						try {
							File.Delete(m_diskfilename) ;
						}
						catch {
							// Error: he Help window is still open
							// TODO: How to fix this??
							fsrlib.Logger.LogError(
								"ERROR: can't delete tempfile \""
							      + m_diskfilename
							      + "\"; fix this!"
							) ;
						}
					}
					m_diskfilename= null ;
				}
			}

			// Cleanup "unmanaged resources"
			//
			// Really, the disk file is an "unmanaged resource",
			// therefore I feel its cleanup (above) should be 
			// performed in this section.  However, the filename
			// by which it's referenced is a managed resource,
			// and it may have indeed been finalized by this
			// point, therefore we can't safely reference it
			// here.  At least, that's my understanding of this
			// "dispose" .NET pattern business... 
		}


		//
		//
		//

		private void CopyStream(Stream sIn, string fnOut, int BufferSize)
		{
			int ReadLength ;
			FileStream fsOut= File.Create(fnOut, BufferSize) ;
			byte[] BufferIn= new byte[BufferSize] ;
			do {
				ReadLength= sIn.Read(BufferIn, 0, BufferSize) ;
				fsOut.Write(BufferIn, 0, ReadLength) ;
			} while(ReadLength == BufferSize) ;
			fsOut.Close() ;
		}


		//
		//
		//

		private string m_resourcefilename ;
		private string m_diskfilename ;
		private bool m_bDisposed ;
		private bool m_bFileIsTemp ;
	}
}
