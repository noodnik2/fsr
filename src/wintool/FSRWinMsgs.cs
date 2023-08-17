using System ;
using System.Resources ;
using System.Reflection ;

/*

	FSRWinMsgs

	Stub for internationalization support of fsrwintool application.

	Uses "<namespace>.MyResources.resources" in <namespace> assembly for default resources.

	Will use "[culture]/<namespace>.MyResources.<culture>.resources" as satellite assembly
	for culture-specific overrides for resources.

*/

namespace fsrwintool
{

	public class FSRWinMsgs
	{

		static FSRWinMsgs()
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


		//
		//
		//

		private static ResourceManager c_rm ;

        public static string StatusReady { get; internal set; }
        public static string CmdAdvancedOpen { get; internal set; }
        public static string CmdAdvancedClose { get; internal set; }
        public static string StatusEnterVideoDescription { get; internal set; }
        public static string StatusEnterStartSecond { get; internal set; }
        public static string StatusEnterTotalSeconds { get; internal set; }
    }

}
