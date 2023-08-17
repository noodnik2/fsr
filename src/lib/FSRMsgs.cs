/*

	FSRMsgs - External fsrlib Messages/Strings
    Copyright (C) 2003-2004, Marty Ross


	Stub for internationalization support of fsrlib library.

	Uses "<namespace>.MyResources.resources" in <namespace> assembly for default resources.

	Will use "[culture]/<namespace>.MyResources.<culture>.resources" as satellite assembly
	for culture-specific overrides for resources.

*/


using System ;
using System.Resources ;
using System.Reflection ;

namespace fsrlib
{

	public class FSRMsgs
	{

		static FSRMsgs()
		{
			string assemblyName= (
				Assembly.GetExecutingAssembly().GetName().Name
			) ;
			string baseName= (
				string.Format( "{0}.MyResources", assemblyName )
			) ;      
			c_rm= new ResourceManager(
				baseName,
				Assembly.GetExecutingAssembly()
			) ;
		}

		public static string Load(string keyword)
		{
			return(c_rm.GetString(keyword)) ;
		}

		public static string Format(string keyword, params object[] args)
		{
			string sFormat= Load(keyword) ;
			if (sFormat == null) return(null) ;
			return(string.Format(sFormat, args)) ;
		}

		private static ResourceManager c_rm ;

	}
}
