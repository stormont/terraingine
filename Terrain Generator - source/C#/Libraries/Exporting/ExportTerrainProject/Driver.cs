using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Xml;
using Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine.ExportTerrainProject
{
	/// <summary>
	/// Driver class for the ExportTerrainProject plug-in.
	/// </summary>
	public class Driver : PlugIn
	{
		#region Data Members
		private SaveFileDialog	_dlgSave;
		private XmlDocument		_xmlDoc;
		private string			_projectName;
		private bool			_fileExists;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of the project exported.
		/// </summary>
		public string ProjectName
		{
			get { return _projectName; }
		}
		#endregion

		#region Basic Plug-In Methods
		/// <summary>
		/// Creates the Driver class.
		/// </summary>
		public Driver()
		{
			base.InitializeBase();
			_name = "Export Terrain Project";
			this.CenterToParent();

			_xmlDoc = null;
			_projectName = null;
			_fileExists = false;
			_dlgSave = new SaveFileDialog();
			_dlgSave.Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*" ;
			_dlgSave.InitialDirectory = Path.GetDirectoryName( Application.ExecutablePath ) + "\\Projects";
		}

		/// <summary>
		/// Runs the plug-in.
		/// </summary>
		public override void Run()
		{
			if ( _page != null )
			{
				DialogResult result = _dlgSave.ShowDialog( _owner );

				if ( result == DialogResult.OK && _dlgSave.FileName != null )
				{
					FileInfo file = new FileInfo( _dlgSave.FileName );

					if ( File.Exists( file.FullName ) )
						_fileExists = true;

					_xmlDoc = new XmlDocument();
					CreateXML();
					FileStorage();
					_success = true;
				}
			}
		}

		/// <summary>
		/// Runs the plug-in with a pre-set filename.
		/// </summary>
		public void Run( string filename )
		{
			if ( _page != null )
			{
				FileInfo file = new FileInfo( _dlgSave.FileName );

				if ( File.Exists( file.FullName ) )
					_fileExists = true;

				_dlgSave.FileName = filename;
				_xmlDoc = new XmlDocument();
				CreateXML();
				FileStorage();
				_success = true;
			}
		}
		#endregion

		#region XML Handling
		/// <summary>
		/// Creates and saves XML data to the chosen file in the SaveFileDialog.
		/// </summary>
		private void CreateXML()
		{
			if ( _dlgSave.FileName != null )
			{
				XmlNode xmlNode;
				XmlElement xmlParent;

				// Create the XML declaration
				xmlNode = _xmlDoc.CreateNode( XmlNodeType.XmlDeclaration, "", "" );
				_xmlDoc.AppendChild( xmlNode );

				// Create the root node
				xmlParent = _xmlDoc.CreateElement( "", "TerrainProject", "Voyage" );
				_xmlDoc.AppendChild( xmlParent );

				// Add version data to the root node
				xmlParent.AppendChild( CreateTextNode( "Version", "Version 1.0" ) );

				// Create the TerrainPage(s)
				WriteTerrainPage( xmlParent );
			}
		}

		/// <summary>
		/// Stores data about the TerrainPage into the XML document.
		/// </summary>
		/// <param name="xmlParent">The XML node to insert data at.</param>
		private void WriteTerrainPage( XmlElement xmlParent )
		{
			XmlElement xmlTPPar;

			// Create the TerrainPage element
			xmlTPPar = _xmlDoc.CreateElement( "", "TerrainPage", "" );

			// Store the TerrainPage name
			xmlTPPar.AppendChild( CreateTextNode( "Name", _page.Name ) );

			// Store the TerrainPage position
			xmlTPPar.AppendChild( CreateVector3Node( "Position", _page.Position ) );

			// Store the TerrainPage rotation
			xmlTPPar.AppendChild( CreateQuaternionNode( "Rotation", _page.Rotation ) );

			// Store the TerrainPage scale
			xmlTPPar.AppendChild( CreateVector3Node( "Scale", _page.Scale ) );

			// Store the maximum vertex height of the TerrainPage
			xmlTPPar.AppendChild( CreateTextNode( "MaximumVertexHeight",
				_page.MaximumVertexHeight.ToString() ) );

			// Write the TerrainPatch data to the XML file
			WriteTerrainPatch( xmlTPPar );

			// Store the TerrainPage data
			xmlParent.AppendChild( xmlTPPar );
		}

		/// <summary>
		/// Stores data about the TerrainPatch into the XML document.
		/// </summary>
		/// <param name="xmlParent">The XML node to insert data at.</param>
		private void WriteTerrainPatch( XmlElement xmlParent )
		{
			XmlElement xmlTPPar, xmlElem;

			// Create the TerrainPatch element
			xmlTPPar = _xmlDoc.CreateElement( "", "TerrainPatch", "" );

			// Store the number of rows in the TerrainPatch
			xmlTPPar.AppendChild( CreateTextNode( "Rows", _page.TerrainPatch.Rows.ToString() ) );

			// Store the number of columns in the TerrainPatch
			xmlTPPar.AppendChild( CreateTextNode( "Columns",
				_page.TerrainPatch.Columns.ToString() ) );

			// Store the height of the TerrainPatch
			xmlTPPar.AppendChild( CreateTextNode( "Height",
				_page.TerrainPatch.Height.ToString() ) );

			// Store the width of the TerrainPatch
			xmlTPPar.AppendChild( CreateTextNode( "Width",
				_page.TerrainPatch.Width.ToString() ) );

			if ( _page.TerrainPatch.NumTextures > 0 )
			{
				xmlElem = _xmlDoc.CreateElement( "", "Textures", "" );

				for ( int i = 0; i < _page.TerrainPatch.NumTextures; i++ )
					WriteTexture( xmlElem, (DataCore.Texture) _page.TerrainPatch.Textures[i] );

				xmlTPPar.AppendChild( xmlElem );
			}

			// Store the vertices of the TerrainPatch
			xmlElem = _xmlDoc.CreateElement( "", "Vertices", "" );

			for ( int i = 0; i < _page.TerrainPatch.NumVertices; i++ )
				WriteVertex( xmlElem, i );

			xmlTPPar.AppendChild( xmlElem );

			// Store the TerrainPatch data
			xmlParent.AppendChild( xmlTPPar );
		}

		/// <summary>
		/// Stores data about a texture into the XML document.
		/// </summary>
		/// <param name="xmlParent">The XML node to insert data at.</param>
		/// <param name="tex">The DataCore.Texture to get data from.</param>
		private void WriteTexture( XmlElement xmlParent, DataCore.Texture tex )
		{
			XmlElement xmlTPPar;
			string filepath, filename = null;

			// Create the texture element
			xmlTPPar = _xmlDoc.CreateElement( "", "Texture", "" );

			// Store the texture name
			xmlTPPar.AppendChild( CreateTextNode( "Name", Path.GetFileName( tex.Name ) ) );

			// Store the texture filename
			if ( _fileExists )
			{
				filepath = Path.GetDirectoryName( _dlgSave.FileName );
				filename = filepath + "\\Textures\\";
				filename += Path.GetFileNameWithoutExtension( tex.Name ) + Path.GetExtension( tex.FileName );
			}
			else
			{
				filepath = Path.GetDirectoryName( _dlgSave.FileName );
				filepath += "\\" + Path.GetFileNameWithoutExtension( _dlgSave.FileName );
				filename = filepath + "\\Textures\\";
				filename += Path.GetFileNameWithoutExtension( tex.Name ) + Path.GetExtension( tex.FileName );
			}

			xmlTPPar.AppendChild( CreateTextNode( "FileName", filename ) );

			// Store the texture texture operation
			xmlTPPar.AppendChild( CreateTextNode( "TextureOperation", tex.OperationText ) );

			// Store if the texture is a mask
			xmlTPPar.AppendChild( CreateTextNode( "IsMask", tex.Mask.ToString() ) );

			// Store the texture scale values
			xmlTPPar.AppendChild( CreateVector2Node( "Scale", tex.Scale ) );

			// Store the texture shift values
			xmlTPPar.AppendChild( CreateVector2Node( "Shift", tex.Shift ) );

			// Store the texture element
			xmlParent.AppendChild( xmlTPPar );
		}

		/// <summary>
		/// Stores data about a vertex into the XML document.
		/// </summary>
		/// <param name="xmlParent">The XML node to insert data at.</param>
		/// <param name="index">Index of the vertex to get data from.</param>
		private void WriteVertex( XmlElement xmlParent, int index )
		{
			XmlElement xmlElem, xmlTPPar;

			// Create the vertex element
			xmlTPPar = _xmlDoc.CreateElement( "", "Vertex", "" );

			// Store the vertex position
			xmlTPPar.AppendChild( CreateVector3Node( "Position", _page.TerrainPatch.Vertices[index].Position ) );

			// Store the vertex normal
			xmlTPPar.AppendChild( CreateVector3Node( "Normal", _page.TerrainPatch.Vertices[index].Normal ) );

			// Store the texture coordinates for each texture
			if ( _page.TerrainPatch.NumTextures > 0 )
			{
				xmlElem = _xmlDoc.CreateElement( "", "TextureCoordinates", "" );

				for ( int i = 0; i < _page.TerrainPatch.NumTextures; i++ )
				{
					xmlElem.AppendChild( CreateVector2Node( "TextureCoordinate" + (i + 1),
						( (Vector2[]) _page.TerrainPatch.TextureCoordinates[i] )[index] ) );
				}
				
				xmlTPPar.AppendChild( xmlElem );
			}

			// Store the vertex element
			xmlParent.AppendChild( xmlTPPar );
		}
		#endregion

		#region Element Creation
		/// <summary>
		/// Creates an XmlElement for text data.
		/// </summary>
		/// <param name="label">The label for the XmlElement.</param>
		/// <param name="data">The text data to insert into the XmlElement.</param>
		/// <returns>The XmlElement with packaged data.</returns>
		private XmlElement CreateTextNode( string label, string data )
		{
			// Create the XML node
			XmlElement xmlElem = _xmlDoc.CreateElement( "", label, "" );
			XmlText xmlText = _xmlDoc.CreateTextNode( data );
			xmlElem.AppendChild( xmlText );

			return xmlElem;
		}

		/// <summary>
		/// Creates an XmlElement for Vector2 data.
		/// </summary>
		/// <param name="label">The label for the XmlElement.</param>
		/// <param name="data">The Vector2 data to insert into the XmlElement.</param>
		/// <returns>The XmlElement with packaged data.</returns>
		private XmlElement CreateVector2Node( string label, Vector2 data )
		{
			// Create the XML node
			XmlElement xmlElem = _xmlDoc.CreateElement( "", label, "" );
			XmlElement xmlElem2;
			XmlText xmlText;

			// X-element
			xmlElem2 = _xmlDoc.CreateElement( "", "X", "" );
			xmlText = _xmlDoc.CreateTextNode( data.X.ToString() );
			xmlElem2.AppendChild( xmlText );
			xmlElem.AppendChild( xmlElem2 );

			// Y-element
			xmlElem2 = _xmlDoc.CreateElement( "", "Y", "" );
			xmlText = _xmlDoc.CreateTextNode( data.Y.ToString() );
			xmlElem2.AppendChild( xmlText );
			xmlElem.AppendChild( xmlElem2 );

			return xmlElem;
		}

		/// <summary>
		/// Creates an XmlElement for Vector3 data.
		/// </summary>
		/// <param name="label">The label for the XmlElement.</param>
		/// <param name="data">The Vector3 data to insert into the XmlElement.</param>
		/// <returns>The XmlElement with packaged data.</returns>
		private XmlElement CreateVector3Node( string label, Vector3 data )
		{
			// Create the XML node
			XmlElement xmlElem = _xmlDoc.CreateElement( "", label, "" );
			XmlElement xmlElem2;
			XmlText xmlText;

			// X-element
			xmlElem2 = _xmlDoc.CreateElement( "", "X", "" );
			xmlText = _xmlDoc.CreateTextNode( data.X.ToString() );
			xmlElem2.AppendChild( xmlText );
			xmlElem.AppendChild( xmlElem2 );

			// Y-element
			xmlElem2 = _xmlDoc.CreateElement( "", "Y", "" );
			xmlText = _xmlDoc.CreateTextNode( data.Y.ToString() );
			xmlElem2.AppendChild( xmlText );
			xmlElem.AppendChild( xmlElem2 );

			// Z-element
			xmlElem2 = _xmlDoc.CreateElement( "", "Z", "" );
			xmlText = _xmlDoc.CreateTextNode( data.Z.ToString() );
			xmlElem2.AppendChild( xmlText );
			xmlElem.AppendChild( xmlElem2 );

			return xmlElem;
		}

		/// <summary>
		/// Creates an XmlElement for Quaternion data.
		/// </summary>
		/// <param name="label">The label for the XmlElement.</param>
		/// <param name="data">The Quaternion data to insert into the XmlElement.</param>
		/// <returns>The XmlElement with packaged data.</returns>
		private XmlElement CreateQuaternionNode( string label, Quaternion data )
		{
			// Create the XML node
			XmlElement xmlElem = _xmlDoc.CreateElement( "", label, "" );
			XmlElement xmlElem2;
			XmlText xmlText;

			// X-element
			xmlElem2 = _xmlDoc.CreateElement( "", "X", "" );
			xmlText = _xmlDoc.CreateTextNode( data.X.ToString() );
			xmlElem2.AppendChild( xmlText );
			xmlElem.AppendChild( xmlElem2 );

			// Y-element
			xmlElem2 = _xmlDoc.CreateElement( "", "Y", "" );
			xmlText = _xmlDoc.CreateTextNode( data.Y.ToString() );
			xmlElem2.AppendChild( xmlText );
			xmlElem.AppendChild( xmlElem2 );

			// Z-element
			xmlElem2 = _xmlDoc.CreateElement( "", "Z", "" );
			xmlText = _xmlDoc.CreateTextNode( data.Z.ToString() );
			xmlElem2.AppendChild( xmlText );
			xmlElem.AppendChild( xmlElem2 );

			// W-element
			xmlElem2 = _xmlDoc.CreateElement( "", "W", "" );
			xmlText = _xmlDoc.CreateTextNode( data.W.ToString() );
			xmlElem2.AppendChild( xmlText );
			xmlElem.AppendChild( xmlElem2 );

			return xmlElem;
		}
		#endregion

		#region File Storage
		/// <summary>
		/// Stores the files used into a project sub-directory.
		/// </summary>
		private void FileStorage()
		{
			string filename;
			DataCore.Texture tex;
			FileInfo file = new FileInfo( _dlgSave.FileName );
			FileInfo texFile;
			DirectoryInfo dir = Directory.GetParent( file.FullName );
			StreamWriter writer;

			if ( File.Exists( file.FullName ) )
			{
				// Overwrite existing files
				file.Delete();
				_xmlDoc.Save( file.FullName );
				_projectName = file.FullName;
			}
			else
			{
				filename = Path.GetFileNameWithoutExtension( file.FullName );

				// Create a new project directory
				dir = Directory.CreateDirectory( dir.FullName + "\\" + filename );

				// Save the XML file
				writer = new StreamWriter( dir.FullName + "\\" + filename + Path.GetExtension( file.FullName ),
					false );
				_xmlDoc.Save( writer );
				writer.Close();
				_projectName = dir.FullName + "\\" + filename + Path.GetExtension( file.FullName );
			}

			// Create the textures sub-directory if it doesn't already exist
			if ( !Directory.Exists( dir.FullName + "\\Textures" ) )
				dir = Directory.CreateDirectory( dir.FullName + "\\Textures" );
			else
				dir = new DirectoryInfo( dir.FullName + "\\Textures" );
			
			// Copy texture files to "Textures" folder
			for ( int i = 0; i < _page.TerrainPatch.NumTextures; i++ )
			{
				// Get the new path and name for the texture
				tex = (DataCore.Texture) _page.TerrainPatch.Textures[i];
				texFile = new FileInfo( tex.FileName );

				// Rename the file, if the texture name has been changed
				if ( tex.Name.Length > 0 && tex.Name != tex.FileName )
					filename = dir.FullName + "\\" + tex.Name + Path.GetExtension( texFile.FullName );
				else
					filename = dir.FullName + "\\" + Path.GetFileName( texFile.FullName );

				// Copy the file to the new folder
				texFile.CopyTo( filename, true );
			}
		}
		#endregion
	}
}
