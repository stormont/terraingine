using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Xml;
using Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine.ImportTerrainProject
{
	/// <summary>
	/// Driver class for the ImportTerrainProject plug-in.
	/// </summary>
	public class Driver : PlugIn
	{
		#region Data Members
		private OpenFileDialog	_dlgOpen;
		private XmlDocument		_xmlDoc;
		private string			_projectName;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of the project imported.
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
			_name = "Import Terrain Project";
			this.CenterToParent();

			_xmlDoc = null;
			_projectName = null;
			_dlgOpen = new OpenFileDialog();
			_dlgOpen.Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*" ;
			_dlgOpen.InitialDirectory = Path.GetDirectoryName( Application.ExecutablePath ) + "\\Projects";
		}

		/// <summary>
		/// Runs the plug-in.
		/// </summary>
		public override void Run()
		{
			DialogResult result = _dlgOpen.ShowDialog( _owner );

			if ( result == DialogResult.OK && _dlgOpen.FileName != null )
			{
				_xmlDoc = new XmlDocument();
				LoadXML();
				_success = true;
			}
		}

		/// <summary>
		/// Runs the plug-in as an automatic function.
		/// </summary>
		/// <param name="objects">Additional data sent to the plug-in.</param>
		public override void AutoRun( ArrayList objects )
		{
			_dlgOpen.FileName = objects[0].ToString();
			_xmlDoc = new XmlDocument();
			LoadXML();
			_success = true;
		}
		#endregion

		#region XML Handling
		/// <summary>
		/// Loads XML data from the chosen file in the OpenFileDialog.
		/// </summary>
		private void LoadXML()
		{
			if ( _dlgOpen.FileName != null )
			{
				try
				{
					XmlNodeList xmlNodes;
					StreamReader reader = new StreamReader( _dlgOpen.FileName );

					_xmlDoc.Load( reader.BaseStream );
					xmlNodes = _xmlDoc.GetElementsByTagName( "TerrainPage" );

					// Load the TerrainPage(s)
					_page = new TerrainPage();
					LoadTerrainPage( xmlNodes.Item( 0 ) );
					reader.Close();
				}
				catch ( Exception e )
				{
					string message = "The XML file is not properly formed!\n\n";

					message += "Source: " + e.Source + "\n";
					message += "Error: " + e.Message;

					MessageBox.Show( null, message, "Error Running Application", MessageBoxButtons.OK,
						MessageBoxIcon.Error );
				}
			}
		}

		/// <summary>
		/// Loads data about the TerrainPage from the XML document.
		/// </summary>
		/// <param name="xmlNode">The XML node to load data from.</param>
		private void LoadTerrainPage( XmlNode xmlNode )
		{
			XmlNode xmlData = xmlNode.FirstChild;

			// Load the TerrainPage name
			_page.Name = LoadTextNode( xmlData );

			// Load the TerrainPage position
			xmlData = xmlData.NextSibling;
			_page.Position = LoadVector3Node( xmlData );

			// Load the TerrainPage rotation
			xmlData = xmlData.NextSibling;
			_page.Rotation = LoadQuaternionNode( xmlData );

			// Load the TerrainPage scale
			xmlData = xmlData.NextSibling;
			_page.Scale = LoadVector3Node( xmlData );

			// Load the maximum vertex height for the TerrainPage
			xmlData = xmlData.NextSibling;
			_page.MaximumVertexHeight = Convert.ToSingle( LoadTextNode( xmlData ) );

			// Load the TerrainPatch data from the XML file
			xmlData = xmlData.NextSibling;
			LoadTerrainPatch( xmlData );
		}

		/// <summary>
		/// Loads data about the TerrainPatch from the XML document.
		/// </summary>
		/// <param name="xmlNode">The XML node to load data from.</param>
		private void LoadTerrainPatch( XmlNode xmlNode )
		{
			XmlNode xmlData = xmlNode.FirstChild;
			int rows, columns;
			float height, width;
			XmlNodeList xmlList;

			// Load the number of TerrainPatch rows
			rows = Convert.ToInt32( LoadTextNode( xmlData ) );

			// Load the number of TerrainPatch columns
			xmlData = xmlData.NextSibling;
			columns = Convert.ToInt32( LoadTextNode( xmlData ) );

			// Load the TerrainPatch height
			xmlData = xmlData.NextSibling;
			height = Convert.ToSingle( LoadTextNode( xmlData ) );

			// Load the TerrainPatch width
			xmlData = xmlData.NextSibling;
			width = Convert.ToSingle( LoadTextNode( xmlData ) );

			// Create the TerrainPatch
			_page.TerrainPatch.CreatePatch( rows, columns, height, width );

			// Load the TerrainPatch textures
			xmlList = _xmlDoc.GetElementsByTagName( "Texture" );
			_textures.Clear();

			for ( int i = 0; i < xmlList.Count; i++ )
				LoadTexture( xmlList.Item( i ) );

			// Load the TerrainPatch vertices
			xmlList = _xmlDoc.GetElementsByTagName( "Vertex" );

			for ( int i = 0; i < xmlList.Count; i++ )
				LoadVertex( xmlList.Item( i ), i );
		}

		/// <summary>
		/// Loads data about a texture from the XML document.
		/// </summary>
		/// <param name="xmlNode">The XML node to load data from.</param>
		private void LoadTexture( XmlNode xmlNode )
		{
			DataCore.Texture tex = new Texture();

			// Load the texture name
			xmlNode = xmlNode.FirstChild;
			tex.Name = xmlNode.InnerText;

			// Load the texture filename
			xmlNode = xmlNode.NextSibling;
			tex.FileName = xmlNode.InnerText;

			// Load the texture operation
			xmlNode = xmlNode.NextSibling;
			tex.OperationText = xmlNode.InnerText;

			// Load if the texture is a mask
			xmlNode = xmlNode.NextSibling;
			tex.Mask = Convert.ToBoolean( xmlNode.InnerText );

			// Load the texture scale value
			xmlNode = xmlNode.NextSibling;
			tex.Scale = LoadVector2Node( xmlNode );

			// Load the texture shift value
			xmlNode = xmlNode.NextSibling;
			tex.Shift = LoadVector2Node( xmlNode );

			_textures.Add( tex );
			_page.TerrainPatch.AddBlankTexture();
			_modifiedTextures = true;
		}

		/// <summary>
		/// Loads data about a vertex from the XML document.
		/// </summary>
		/// <param name="xmlNode">The XML node to load data from.</param>
		/// <param name="index">The index of the vertex to load.</param>
		private void LoadVertex( XmlNode xmlNode, int index )
		{
			D3D.CustomVertex.PositionNormal vert = _page.TerrainPatch.Vertices[index];
			Vector2 texCoord;
			int texCount;

			// Load the vertex position
			xmlNode = xmlNode.FirstChild;
			vert.Position = LoadVector3Node( xmlNode );
			
			// Load the vertex normal
			xmlNode = xmlNode.NextSibling;
			vert.Normal = LoadVector3Node( xmlNode );

			if ( xmlNode.NextSibling != null )
			{
				xmlNode = xmlNode.NextSibling.FirstChild;

				if ( xmlNode != null )
				{
					texCount = 0;

					do
					{
						texCoord = LoadVector2Node( xmlNode );
						( (Vector2[]) _page.TerrainPatch.TextureCoordinates[texCount] )[index] = texCoord;
						xmlNode = xmlNode.NextSibling;
						texCount++;
					}
					while ( xmlNode != null );
				}
			}

			_page.TerrainPatch.Vertices[index] = vert;
		}
		#endregion

		#region Element Retrieval
		/// <summary>
		/// Loads text data from an XmlNode.
		/// </summary>
		/// <param name="xmlElem">The XmlNode from which to load data.</param>
		/// <returns>The text data contained in the XmlNode.</returns>
		private string LoadTextNode( XmlNode xmlElem )
		{
			// Load the XML data
			return xmlElem.InnerText;
		}

		/// <summary>
		/// Loads Vector2 data from an XmlNode.
		/// </summary>
		/// <param name="xmlElem">The XmlNode from which to load data.</param>
		/// <returns>The Vector2 data contained in the XmlNode.</returns>
		private Vector2 LoadVector2Node( XmlNode xmlElem )
		{
			// Load the XML data
			Vector2 result = new Vector2();

			// X-element
			result.X = Convert.ToSingle( xmlElem.FirstChild.InnerText );

			// Y-element
			result.Y = Convert.ToSingle( xmlElem.FirstChild.NextSibling.InnerText );

			return result;
		}

		/// <summary>
		/// Loads Vector3 data from an XmlNode.
		/// </summary>
		/// <param name="xmlElem">The XmlNode from which to load data.</param>
		/// <returns>The Vector3 data contained in the XmlNode.</returns>
		private Vector3 LoadVector3Node( XmlNode xmlElem )
		{
			// Load the XML data
			Vector3 result = new Vector3();

			// X-element
			result.X = Convert.ToSingle( xmlElem.FirstChild.InnerText );

			// Y-element
			result.Y = Convert.ToSingle( xmlElem.FirstChild.NextSibling.InnerText );

			// Z-element
			result.Z = Convert.ToSingle( xmlElem.FirstChild.NextSibling.NextSibling.InnerText );

			return result;
		}

		/// <summary>
		/// Loads Quaternion data from an XmlNode.
		/// </summary>
		/// <param name="xmlElem">The XmlNode from which to load data.</param>
		/// <returns>The Quaternion data contained in the XmlNode.</returns>
		private Quaternion LoadQuaternionNode( XmlNode xmlElem )
		{
			// Load the XML data
			Quaternion result = new Quaternion();;

			// X-element
			result.X = Convert.ToSingle( xmlElem.FirstChild.InnerText );

			// Y-element
			result.Y = Convert.ToSingle( xmlElem.FirstChild.NextSibling.InnerText );

			// Z-element
			result.Z = Convert.ToSingle( xmlElem.FirstChild.NextSibling.NextSibling.InnerText );

			// W-element
			result.W = Convert.ToSingle( xmlElem.FirstChild.NextSibling.NextSibling.NextSibling.InnerText );

			return result;
		}
		#endregion
	}
}
