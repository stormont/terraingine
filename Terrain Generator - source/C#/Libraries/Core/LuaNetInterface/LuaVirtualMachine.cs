using System;
using System.Collections;
using System.Reflection;
using LuaInterface;

namespace Voyage.LuaNetInterface
{
	/// <summary>
	/// A class for providing an interface to a Lua virtual machine.
	/// </summary>
	public class LuaVirtualMachine
	{
		#region Data Members
		private Lua _lua = null;				// Lua virtual machine
		private Hashtable _functions = null;	// Registered functions in the Lua virtual machine
		private Hashtable _packages = null;		// Registered packages in the Lua virtual machine
		#endregion

		#region Properties
		/// <summary>
		/// Gets the Lua virtual machine.
		/// </summary>
		public Lua Lua
		{
			get { return _lua; }
		}

		/// <summary>
		/// Gets the table of Lua-registered functions.
		/// </summary>
		public Hashtable Functions
		{
			get { return _functions; }
		}

		/// <summary>
		/// Gets the table of Lua-registered packages.
		/// </summary>
		public Hashtable Packages
		{
			get { return _packages; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates an object for interfacing with the Lua virtual machine.
		/// </summary>
		/// <param name="register">Whether to register the built-in functions.</param>
		public LuaVirtualMachine( bool register )
		{
			_lua = new Lua();
			_functions = new Hashtable();
			_packages = new Hashtable();

			// Register the built-in Lua functions
			if ( register )
				RegisterLuaFunctions( this );
		}

		/// <summary>
		/// Registers the Lua-attributed functions in the target object.
		/// </summary>
		/// <param name="target">The target to load Lua-attributed functions from.</param>
		public void RegisterLuaFunctions( object target )
		{
			RegisterLuaFunctions( target, null, null );
		}

		/// <summary>
		/// Registers the Lua-attributed functions in the target object.
		/// </summary>
		/// <param name="target">The target to load Lua-attributed functions from.</param>
		/// <param name="package">The package to register the functions under.</param>
		/// <param name="packageDocumentation">The documentation for the package.</param>
		public void RegisterLuaFunctions( object target, string package, string packageDocumentation )
		{
			// Sanity checks
			if ( target == null || _lua == null || _functions == null || _packages == null )
				return;

			try
			{
				LuaPackageDescriptor pPackage = null;

				Console.WriteLine("Loading Lua library: " + target.ToString());

				if ( package != null )
				{
					// Check if the package already exists
					if ( !_packages.ContainsKey( "package" ) )
					{
						// Create a new package
						_lua.DoString( package + " = {}" );
						pPackage = new LuaPackageDescriptor( package, packageDocumentation );
					}
					else	// Access the old package
						pPackage = (LuaPackageDescriptor) _packages["package"];
				}

				// Get the target type
				Type targetType = target.GetType();

				// ... and simply iterate through all its methods
				foreach ( MethodInfo info in targetType.GetMethods() )
				{
					// ... then through all this method's attributes
					foreach ( Attribute attr in Attribute.GetCustomAttributes( info ) )
					{
						// ... and if they happen to be one of our LuaFunctionAttribute attributes
						if ( attr.GetType() == typeof( LuaFunctionAttribute ) )
						{
							LuaFunctionAttribute luaAttr = (LuaFunctionAttribute) attr;
							ArrayList paramList = new ArrayList();
							ArrayList paramDocs = new ArrayList();

							// Get the desired function name and doc string, along with parameter info
							string fName = luaAttr.FunctionName;
							string fDoc = luaAttr.FunctionDocumentation;
							string[] pDocs = luaAttr.FunctionParameters;

							// Now get the expected parameters from the MethodInfo object
							ParameterInfo[] pInfo = info.GetParameters();

							// If they don't match, someone forgot to add some documentation to the
							// attribute, complain and go to the next method
							if ( pDocs != null && ( pInfo.Length != pDocs.Length ) )
							{
								Console.WriteLine( "Function " + info.Name + " (exported as " +
									fName + ") argument number mismatch. Declared " +
									pDocs.Length + " but requires " + pInfo.Length + "." );

								break;
							}

							// Build a parameter <-> parameter doc hashtable
							for ( int i = 0; i < pInfo.Length; i++ )
							{
								paramList.Add( pInfo[i].Name );
								paramDocs.Add( pDocs[i] );
							}

							// Get a new function descriptor from this information
							LuaFunctionDescriptor func = new LuaFunctionDescriptor( fName, fDoc, paramList,
								paramDocs );

							if ( pPackage != null )
							{
								// Check if the package already contains the function
								if ( !pPackage.Functions.ContainsKey( fName ) )
								{
									// Add the new package function
									pPackage.AddFunction( func );
									_lua.RegisterFunction( package + fName, target, info );
									_lua.DoString( package + "." + fName + " = " + package + fName );
									_lua.DoString( package + fName + " = nil" );
								}
							}
							else
							{
								// Check if the function has already been loaded
								if ( !_functions.ContainsKey( fName ) )
								{
									// Add it to the global hashtable
									_functions.Add( fName, func );

									// And tell the VM to register it
									_lua.RegisterFunction( fName, target, info );
								}
							}
						}
					}
				}

				if ( pPackage != null && !_packages.ContainsKey( package ) )
					_packages.Add( package, pPackage );
			}
			catch ( Exception e )
			{
			}
			finally
			{
			}
		}
		#endregion

		#region LuaMethods
		/// <summary>
		/// Displays available commands to the console.
		/// </summary>
		[LuaFunctionAttribute( "help", "List available commands." )]
		public void Help()
		{
			string[] results;
			int counter;

			// Display stand-alone commands
			if ( _functions.Count > 0 )
			{
				results = new string[_functions.Count];
				counter = 0;

				Console.WriteLine( "Available commands:" );
				Console.WriteLine();

				IDictionaryEnumerator funcs = _functions.GetEnumerator();

				while ( funcs.MoveNext() )
				{
					results[counter] = ( (LuaFunctionDescriptor) funcs.Value ).FunctionHeader;
					counter++;
				}

				Array.Sort( results );

				for ( int i = 0; i < counter; i++ )
					Console.WriteLine( results[i] );
			}

			// Display packages
			if ( _packages.Count > 0 )
			{
				results = new string[_packages.Count];
				counter = 0;

				Console.WriteLine();
				Console.WriteLine( "Available packages:" );

				IDictionaryEnumerator pkgs = _packages.GetEnumerator();

				while ( pkgs.MoveNext() )
				{
					results[counter] += pkgs.Key.ToString();
					counter++;
				}

				Array.Sort( results );

				for ( int i = 0; i < counter; i++ )
					Console.WriteLine( results[i] );
			}
		}

		/// <summary>
		/// Displays information about the specified command to the console.
		/// </summary>
		/// <param name="command">The command to display information for.</param>
		[LuaFunctionAttribute( "helpcmd", "Show help for a given command.",
			 "Command to get help of (put in quotes)." )]
		public void Help( string command )
		{
			if ( _functions.Contains( command ) )
			{
				LuaFunctionDescriptor func = (LuaFunctionDescriptor) _functions[command];

				Console.WriteLine( func.FunctionFullDocumentation );

				return;
			}

			if ( command.IndexOf( "." ) == -1 )
			{
				if ( _packages.ContainsKey( command ) )
				{
					LuaPackageDescriptor pkg = (LuaPackageDescriptor) _packages[command];

					Console.WriteLine( pkg.WriteHelp( command ) );

					return;
				}
				else
				{
					Console.WriteLine( "No such function or package: " + command );

					return;
				}
			}

			string[] parts = command.Split( '.' );

			if ( !_packages.ContainsKey( parts[0] ) )
			{
				Console.WriteLine( "No such function or package: " + command );

				return;
			}
			
			LuaPackageDescriptor desc = (LuaPackageDescriptor) _packages[ parts[0] ];

			if ( !desc.HasFunction( parts[1] ) )
			{
				Console.WriteLine( "Package " + parts[0] + " doesn't have a " + parts[1] + " function." );

				return;
			}

			Console.WriteLine( desc.WriteHelp( parts[1] ) );
		}
		#endregion
	}
}
