using System;
using System.Collections;

namespace Voyage.LuaNetInterface
{
	/// <summary>
	/// A class for describing a Lua package.
	/// </summary>
	public class LuaPackageDescriptor
	{
		#region Data Members
		public string _packageName;				// Name of the package
		public string _packageDocumentation;	// Name of the package documentation
		public Hashtable _packageFunctions;		// Table of package functions
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of the package.
		/// </summary>
		public string PackageName
		{
			get { return _packageName; }
		}

		/// <summary>
		/// Gets the package documentation.
		/// </summary>
		public string PackageDocumentation
		{
			get { return _packageDocumentation; }
		}

		/// <summary>
		/// Gets the package functions.
		/// </summary>
		public Hashtable Functions
		{
			get { return _packageFunctions; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates a LuaPackageDescriptor for describing a Lua package.
		/// </summary>
		/// <param name="packageName">The name of the package.</param>
		/// <param name="packageDocumentation">The documentation for the package.</param>
		public LuaPackageDescriptor( string packageName, string packageDocumentation )
		{
			_packageName = packageName;
			_packageDocumentation = packageDocumentation;
			_packageFunctions = new Hashtable();
		}

		/// <summary>
		/// Adds a function to the Lua package.
		/// </summary>
		/// <param name="function">The function to add.</param>
		public void AddFunction( LuaFunctionDescriptor function )
		{
			if ( _packageFunctions == null )
				_packageFunctions = new Hashtable();

			if ( !_packageFunctions.ContainsKey( function.FunctionName ) )
				_packageFunctions.Add( function.FunctionName, function );
		}

		/// <summary>
		/// Writes the help for the specified function.
		/// </summary>
		/// <param name="command">The name of the function to output help for.</param>
		/// <returns>The help for the specified function.</returns>
		public string WriteHelp( string command )
		{
			LuaFunctionDescriptor func = (LuaFunctionDescriptor) _packageFunctions[command];
			string output = null;

			if ( func != null )
				output = func.FunctionFullDocumentation ;

			return output;
		}

		/// <summary>
		/// Checks if the Lua package contains the specified function.
		/// </summary>
		/// <param name="command">The function to search for.</param>
		/// <returns>Whether the package contains the function.</returns>
		public bool HasFunction( string command )
		{
			return _packageFunctions.ContainsKey( command );
		}
		#endregion
	}
}
