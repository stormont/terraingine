using System;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;

namespace Voyage.Terraingine.DataInterfacing
{
	/// <summary>
	/// A class for handling plug-ins.
	/// </summary>
	public class PlugIns
	{
		#region Enumerations
		/// <summary>
		/// Enumeration for different plug-in types.
		/// </summary>
		public enum PlugInTypes
		{
			/// <summary>
			/// Plug-in type for manipulating vertices.
			/// </summary>
			Vertices,
			
			/// <summary>
			/// Plug-in type for manipulating textures.
			/// </summary>
			Textures,
			
			/// <summary>
			/// Plug-in type for importing data.
			/// </summary>
			Importing,
			
			/// <summary>
			/// Plug-in type for exporting data.
			/// </summary>
			Exporting
		}
		#endregion

		#region Data Members
		private ArrayList		_vertexPlugins;
		private ArrayList		_texturePlugins;
		private ArrayList		_importPlugins;
		private ArrayList		_exportPlugins;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the collection of vertex-manipulating plug-ins.
		/// </summary>
		public ArrayList VertexPlugIns
		{
			get { return _vertexPlugins; }
		}

		/// <summary>
		/// Gets the collection of texture-manipulating plug-ins.
		/// </summary>
		public ArrayList TexturePlugIns
		{
			get { return _texturePlugins; }
		}

		/// <summary>
		/// Gets the collection of file-importing plug-ins.
		/// </summary>
		public ArrayList FileImportPlugIns
		{
			get { return _importPlugins; }
		}

		/// <summary>
		/// Gets the collection of file-exporting plug-ins.
		/// </summary>
		public ArrayList FileExportPlugIns
		{
			get { return _exportPlugins; }
		}
		#endregion

		#region Basic Methods
		/// <summary>
		/// Creates an object for handling plug-ins.
		/// </summary>
		public PlugIns()
		{
			_vertexPlugins = new ArrayList();
			_texturePlugins = new ArrayList();
			_importPlugins = new ArrayList();
			_exportPlugins = new ArrayList();
		}
		#endregion

		#region Loading Plug-Ins
		/// <summary>
		/// Loads the plug-ins associated with the application.
		/// </summary>
		public void LoadPlugIns()
		{
			try
			{
				string plugDirName = Path.GetDirectoryName( Application.ExecutablePath ) + "\\Plug-Ins";

				try
				{
					// Load vertex-manipulating plug-ins
					LoadPlugInDirectory( plugDirName + "\\Vertices", PlugInTypes.Vertices );
				}
				catch ( Exception e )
				{
					Debug.Assert( true, e.Source + ": " + e.Message, e.StackTrace );
				}

				try
				{
					// Load texture-manipulating plug-ins
					LoadPlugInDirectory( plugDirName + "\\Textures", PlugInTypes.Textures );
				}
				catch ( Exception e )
				{
					Debug.Assert( true, e.Source + ": " + e.Message, e.StackTrace );
				}

				try
				{
					// Load file-importing plug-ins
					LoadPlugInDirectory( plugDirName + "\\Importing", PlugInTypes.Importing );
				}
				catch ( Exception e )
				{
					Debug.Assert( true, e.Source + ": " + e.Message, e.StackTrace );
				}

				try
				{
					// Load file-exporting plug-ins
					LoadPlugInDirectory( plugDirName + "\\Exporting", PlugInTypes.Exporting );
				}
				catch ( Exception e )
				{
					Debug.Assert( true, e.Source + ": " + e.Message, e.StackTrace );
				}
			}
			catch ( Exception e )
			{
				Debug.Assert( true, e.Source + ": " + e.Message, e.StackTrace );
			}
			finally
			{
			}
		}

		/// <summary>
		/// Loads the plug-ins from the specified directory, including sub-directories.
		/// </summary>
		/// <param name="dir">Directory to load plug-ins from.</param>
		/// <param name="type">Type of plug-in to load.</param>
		private void LoadPlugInDirectory( string dir, PlugInTypes type )
		{
			try
			{
				// Load the plug-ins withing the current directory
				LoadPlugInsWithinDirectory( dir, type );

				// Load the plug-ins nested inside sub-directories
				string[] directories = Directory.GetDirectories( dir );

				foreach ( string d in directories )
				{
					LoadPlugInDirectory( d, type );
				}
			}
			catch ( Exception e )
			{
				Debug.Assert( true, e.Source + ": " + e.Message, e.StackTrace );
			}
		}

		/// <summary>
		/// Load the plug-ins from within the specified directory.
		/// </summary>
		/// <param name="dir">Directory to load plug-ins from.</param>
		/// <param name="type">Type of plug-in to load.</param>
		private void LoadPlugInsWithinDirectory( string dir, PlugInTypes type )
		{
			string[] files = Directory.GetFiles( dir, "*.dll" );
			Assembly asm;
			ArrayList plugs;

			if ( files != null )
			{
				// Load each file found into the appropriate plug-in type collection
				switch ( type )
				{
					case PlugInTypes.Vertices:
						try
						{
							foreach ( string f in files )
							{
								asm = Assembly.LoadFile( f );
								plugs = LoadPlugIns( asm );

								foreach ( PlugIn p in plugs )
								{
									_vertexPlugins.Add( p );
								}
							}
						}
						catch ( Exception e )
						{
							Debug.Assert( true, e.Source + ": " + e.Message, e.StackTrace );
						}
						break;

					case PlugInTypes.Textures:
						try
						{
							foreach ( string f in files )
							{
								asm = Assembly.LoadFile( f );
								plugs = LoadPlugIns( asm );

								foreach ( PlugIn p in plugs )
								{
									_texturePlugins.Add( p );
								}
							}
						}
						catch ( Exception e )
						{
							Debug.Assert( true, e.Source + ": " + e.Message, e.StackTrace );
						}
						break;

					case PlugInTypes.Importing:
						try
						{
							foreach ( string f in files )
							{
								asm = Assembly.LoadFile( f );
								plugs = LoadPlugIns( asm );

								foreach ( PlugIn p in plugs )
								{
									_importPlugins.Add( p );
								}
							}
						}
						catch ( Exception e )
						{
							Debug.Assert( true, e.Source + ": " + e.Message, e.StackTrace );
						}
						break;

					case PlugInTypes.Exporting:
						try
						{
							foreach ( string f in files )
							{
								asm = Assembly.LoadFile( f );
								plugs = LoadPlugIns( asm );

								foreach ( PlugIn p in plugs )
								{
									_exportPlugins.Add( p );
								}
							}
						}
						catch ( Exception e )
						{
							Debug.Assert( true, e.Source + ": " + e.Message, e.StackTrace );
						}
						break;

					default:
						// Do nothing
						break;
				}
			}
		}

		/// <summary>
		/// Loads a list of valid plug-ins from the specified assembly.
		/// </summary>
		/// <param name="asm">Assembly to load plug-ins from.</param>
		/// <returns>List of valid plug-ins.</returns>
		private ArrayList LoadPlugIns( Assembly asm )
		{
			ArrayList plugs = new ArrayList();
			object o;
			bool valid;

			try
			{
				foreach ( Type t in asm.GetTypes() )
				{
					// Make sure the type is a class and public
					if ( t.IsClass && !t.IsNotPublic )
					{
						// Make sure the class inherits from PlugIn
						valid = t.BaseType == typeof( PlugIn );

						if ( valid )
						{
							// Load an instance of the class
							o = Activator.CreateInstance( t );
							plugs.Add( o );
						}
					}
				}
			}
			catch ( Exception e )
			{
				Debug.Assert( true, e.Source + ": " + e.Message, e.StackTrace );
			}

			return plugs;
		}

		/// <summary>
		/// Loads a plug-in of the specified type.
		/// </summary>
		/// <param name="type">The type of plug-in to load.</param>
		public void LoadPlugIn( PlugInTypes type )
		{
			try
			{
				OpenFileDialog dlgOpen = new OpenFileDialog();
				DialogResult result = DialogResult.OK;
				bool valid = false;
				string filename = null;
				string destFile = null;
				string plugType = null;
			
				dlgOpen.Filter = "DLL Files (*.dll)|*.dll|All files (*.*)|*.*" ;
				dlgOpen.InitialDirectory = Path.GetDirectoryName( Application.ExecutablePath );

				switch ( type )
				{
					case PlugInTypes.Vertices:
						plugType = "Vertices\\";
						break;

					case PlugInTypes.Textures:
						plugType = "Textures\\";
						break;

					case PlugInTypes.Importing:
						plugType = "Importing\\";
						break;

					case PlugInTypes.Exporting:
						plugType = "Exporting\\";
						break;

					default:
						break;
				}

				while ( !valid && plugType != null )
				{
					result = dlgOpen.ShowDialog();

					if ( result == DialogResult.OK && dlgOpen.FileName != null )
					{
						filename = Path.GetFileName( dlgOpen.FileName );
						destFile = Path.GetDirectoryName( Application.ExecutablePath ) +
							"\\Plug-Ins\\" + plugType + filename;

						if ( !File.Exists( destFile ) )
							valid = true;
						else
							MessageBox.Show( "Plug-in already exists!", "Cannot Load Plug-In",
								MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
					}
					else
						valid = true;
				}

				if ( result == DialogResult.OK && dlgOpen.FileName != null && plugType != null )
				{
					File.Copy( dlgOpen.FileName, destFile, false );
					RefreshPlugInList( type );
				}
			}
			catch
			{
				MessageBox.Show( "The plug-in could not be loaded.", "Cannot Load Plug-In",
					MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			finally
			{
			}
		}

		/// <summary>
		/// Refreshes the list of plug-ins of the specified type.
		/// </summary>
		/// <param name="type">The type of plug-in list to refresh.</param>
		protected void RefreshPlugInList( PlugInTypes type )
		{
			string dir = null;

			switch ( type )
			{
				case PlugInTypes.Vertices:
					dir = Path.GetDirectoryName( Application.ExecutablePath ) + "\\Plug-Ins\\Vertices\\";
					_vertexPlugins.Clear();
					break;

				case PlugInTypes.Textures:
					dir = Path.GetDirectoryName( Application.ExecutablePath ) + "\\Plug-Ins\\Textures\\";
					_texturePlugins.Clear();
					break;

				case PlugInTypes.Importing:
					dir = Path.GetDirectoryName( Application.ExecutablePath ) + "\\Plug-Ins\\Importing\\";
					_importPlugins.Clear();
					break;

				case PlugInTypes.Exporting:
					dir = Path.GetDirectoryName( Application.ExecutablePath ) + "\\Plug-Ins\\Exporting\\";
					_exportPlugins.Clear();
					break;

				default:
					break;
			}

			if ( dir != null )
				LoadPlugInsWithinDirectory( dir, type );
		}
		#endregion
	}
}
