/*

	AnonymousPipe.cs - Create/manage Anonymous Pipe
    Copyright (C) 2003-2004, Marty Ross

	Using this class, it is possible to create and use an anonymous pipe to
	stream data to an application in near real-time.

*/


using System ;
using System.Runtime.InteropServices ;

namespace fsrlib
{

	public class AnonymousPipe : IDisposable
	{

		public AnonymousPipe()
		{
			m_hPipe= INVALID_HANDLE_VALUE ;
			m_hPipeForExport= INVALID_HANDLE_VALUE ;
			m_bufferSize= 4096 ;
			m_bDisposed= false ;
		}

		~AnonymousPipe() { Dispose(false) ; }

		public enum OpenMode { Read, Write } ;

		public void Open(OpenMode m, int bufferSize)
		{
			BufferSize= bufferSize ;
			Open(m) ;
		}

		/// <summary>
		/// Open the anonymous pipe using specified mode.
		/// </summary>
		/// <param name="m">mode specifying which "end" of the pipe we want locally (read or write)</param>
		public void Open(OpenMode m)
		{

			// validate mode...
			if (
				(m != OpenMode.Read)
			     && (m != OpenMode.Write)
			) {
				throw new ApplicationException("AnonymousPipe: Invalid OpenMode") ;
			}

			int hReadPipe= INVALID_HANDLE_VALUE ;
			int hWritePipe= INVALID_HANDLE_VALUE ;

			if (
				!CreatePipe(
					ref hReadPipe,
					ref hWritePipe,
					null,
					m_bufferSize
				)
			) {
				throw new ApplicationException(
					"AnonymousPipe: "
				      + "Failed to create communication pipe"
				) ;
			}

			if (m == OpenMode.Write) {
				m_hPipe= hWritePipe ;
				m_hPipeForExport= hReadPipe ;
			}
			else {
				m_hPipe= hReadPipe ;
				m_hPipeForExport= hWritePipe ;
			}

			// revive the object if it's been disposed
			if (m_bDisposed) {
				GC.ReRegisterForFinalize(this) ;
				m_bDisposed= false ;
			}

		}

		/// <summary>
		/// Closes the "AnonymousPipe" instance.
		/// </summary>
		public void Close()
		{
			Dispose() ;
		}

		/// <summary>
		/// Cleans up all resources used by the anonymous pipe instance.
		/// </summary>
		public void Dispose()
		{
			GC.SuppressFinalize(this) ;
			Dispose(true) ;
		}

		public int BufferSize
		{
			get {
				return(m_bufferSize) ;
			}
			set {
				m_bufferSize= value ;
			}
		}

		public bool IsOpen
		{
			get {
				return(
					(m_hPipeForExport != INVALID_HANDLE_VALUE)
				     && (m_hPipe != INVALID_HANDLE_VALUE)
				) ;
			}
		}

		public IntPtr PeerHandle { get { return((IntPtr) m_hPipeForExport) ; } }
		public IntPtr LocalHandle { get { return((IntPtr) m_hPipe) ; } }


		//
		//	protected methods
		//

		/// <summary>
		/// Frees all unmanaged (and optionally, managed) resources
		/// associated with this "AnonymousPipe" instance.
		/// 
		/// Closes both ends of the pipe.
		/// (perhaps we should only close the "local" end?)
		/// 
		/// No known exceptions are thrown which should be handled.
		/// </summary>
		/// <param name="bDisposing">not used</param>
		protected virtual void Dispose(bool bDisposing)
		{
			// nothing to do if already disposed
			if (m_bDisposed) return ;

			// mark as disposed
			m_bDisposed= true ;

			// cleanup "managed objects"
			// if (bDisposing) {
			//	(there are none; nothing to do)
			// }

			// cleanup "unmanaged objects"
			//
			// NOTE:
			// I simply want to to the best I can at closing any
			// open handles here.  I don't want any exceptions
			// to be thrown if "close" fails, but since it's from
			// kernel32.dll, I'm not sure what will happen when it
			// fails -- I'll guess that wrapping "close" in
			// "try/catch" will have the effect I want, but am
			// not sure about this.
			//
			if (m_hPipe != INVALID_HANDLE_VALUE) {
				// not sure about the following...
				//	FlushFileBuffers(m_hPipe) ;
				try { CloseHandle(m_hPipe) ; } catch { }		// "try/catch" works?
				m_hPipe= INVALID_HANDLE_VALUE ;
			}
			if (m_hPipeForExport != INVALID_HANDLE_VALUE) {
				try { CloseHandle(m_hPipeForExport) ; } catch { }	// "try/catch" works?
				m_hPipeForExport= INVALID_HANDLE_VALUE ;
			}

		}


		//
		//	internal data
		//

		const int INVALID_HANDLE_VALUE= -1 ;

		int m_bufferSize ;
		int m_hPipe ;
		int m_hPipeForExport ;
		bool m_bDisposed ;


		//
		//	externals
		//

		[StructLayout(LayoutKind.Sequential)]
		class SECURITY_ATTRIBUTES {
			internal int nLength= 0 ;
			internal int lpSecurityDescriptor= 0 ;
			internal int bInheritHandle= 0 ;
		}
			
		[DllImport("kernel32.dll")]
		static extern bool CreatePipe(
			ref int hReadPipe,
			ref int hWritePipe,
			SECURITY_ATTRIBUTES lpSecurityAttributes,
			int nSize
		) ;

		[DllImport("kernel32.dll")]
		static extern bool CloseHandle(int hFile) ;

	}

}
