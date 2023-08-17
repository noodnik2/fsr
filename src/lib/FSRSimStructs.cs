/*

	FSRSimStructs.cs - Structures used to represent FSR Data
    Copyright (C) 2004, Marty Ross


*/


using System ;

namespace fsrlib
{

	/// <summary>
	/// SimObject represents a simulator "Object" as defined in
	/// the FSR file "Object Definition Section Data" (ODIB)
	/// sections.
	/// </summary>
	internal struct SimObject
	{
		/// <summary>
		/// Records the mapping between the objects numeric
		/// and string representations.
		/// </summary>
		/// <param name="sid">object numeric id</param>
		/// <param name="sval">object string name</param>
		internal SimObject(ushort sid, string sval)
		{
			this.sid= sid ;
			this.sval= sval ;
		}
		internal ushort sid ;
		internal string sval ;
	}


	/// <summary>
	/// SimParam represents the definition of a simulator parameter.
	/// </summary>
	internal struct SimParam
	{
		/// <summary>
		/// Records the definition of a simulator parameter.
		/// </summary>
		/// <param name="oid">id of object to which this parameter belongs</param>
		/// <param name="pid">numeric id of parameter</param>
		/// <param name="tid">numeric code for type of parameter</param>
		/// <param name="punits">string description of units for parameter value</param>
		/// <param name="pname">string name of parameter</param>
		internal SimParam(
			ushort oid,
			ushort pid,
			ushort tid,
			string punits,
			string pname
		) {
			this.oid= oid ;
			this.pid= pid ;
			this.tid= tid ;
			this.punits= punits ;
			this.pname= pname ;
		}
		internal ushort oid ;
		internal ushort pid ;
		internal ushort tid ;
		internal string punits ;
		internal string pname ;
	}

	/// <summary>
	/// SimValue represents simulator parameter values,
	/// </summary>
	internal struct SimValue
	{
		/// <summary>
		/// Records the value of the specified simulator parameter.
		/// </summary>
		/// <param name="pid">numeric id of simulator parameter</param>
		/// <param name="sval">value for the simulator parameter</param>
		internal SimValue(ushort pid, Object sval)
		{
			this.pid= pid ;
			this.sval= sval ;
		}
		internal ushort pid ;
		internal Object sval ;
	}

}
