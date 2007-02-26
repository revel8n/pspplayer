// ----------------------------------------------------------------------------
// PSP Player Emulation Suite
// Copyright (C) 2006 Ben Vanik (noxa)
// Licensed under the LGPL - see License.txt in the project root for details
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using Noxa.Emulation.Psp.Cpu;

namespace Noxa.Emulation.Psp.Bios
{
	#region BiosStubAttribute

	[global::System.AttributeUsage( AttributeTargets.Method, Inherited = false, AllowMultiple = false )]
	public sealed class BiosStubAttribute : Attribute
	{
		private readonly uint _nid;
		private readonly string _name;
		private readonly bool _hasReturn;
		private readonly int _parameterCount;
		private readonly bool _doubleWordReturn;

		public BiosStubAttribute( uint nid, string name, bool hasReturn, int parameterCount )
			: this( nid, name, hasReturn, parameterCount, false )
		{
		}

		public BiosStubAttribute( uint nid, string name, bool hasReturn, int parameterCount, bool doubleWordReturn )
		{
			_nid = nid;
			_name = name;
			_hasReturn = hasReturn;
			_parameterCount = parameterCount;
			_doubleWordReturn = doubleWordReturn;
		}

		public uint NID
		{
			get
			{
				return _nid;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public bool HasReturn
		{
			get
			{
				return _hasReturn;
			}
		}

		public int ParameterCount
		{
			get
			{
				return _parameterCount;
			}
		}

		public bool DoubleWordReturn
		{
			get
			{
				return _doubleWordReturn;
			}
		}
	}

	#endregion

	#region BiosStubIncompleteAttribute

	[global::System.AttributeUsage( AttributeTargets.Method, Inherited = false, AllowMultiple = false )]
	public sealed class BiosStubIncompleteAttribute : Attribute
	{
		public BiosStubIncompleteAttribute()
		{
		}
	}

	#endregion

	#region BiosStubStatelessAttribute

	[global::System.AttributeUsage( AttributeTargets.Method, Inherited = false, AllowMultiple = false )]
	public sealed class BiosStubStatelessAttribute : Attribute
	{
		public BiosStubStatelessAttribute()
		{
		}
	}

	#endregion

	public delegate int BiosStubDelegate( IMemory memory, int a0, int a1, int a2, int a3, int sp );

	public class BiosFunction
	{
		public IModule Module;
		public bool IsImplemented;
		public bool IsStateless;
		public uint NID;
		public string Name;
		public BiosStubDelegate Target;
		public bool HasReturn;
		public int ParameterCount;
		public bool DoubleWordReturn;

		// Could have other stuff here

		public BiosFunction( IModule module, bool isImplemented, bool isStateless, uint nid, string name, BiosStubDelegate target, bool hasReturn, int parameterCount )
			: this( module, isImplemented, isStateless, nid, name, target, hasReturn, parameterCount, false )
		{
		}

		public BiosFunction( IModule module, bool isImplemented, bool isStateless, uint nid, string name, BiosStubDelegate target, bool hasReturn, int parameterCount, bool doubleWordReturn )
		{
			this.Module = module;
			this.IsImplemented = isImplemented;
			this.IsStateless = isStateless;
			this.NID = nid;
			this.Name = name;
			this.Target = target;
			this.HasReturn = hasReturn;
			this.ParameterCount = parameterCount;
			this.DoubleWordReturn = doubleWordReturn;
		}
	}
}
