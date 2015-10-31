using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Collections;
using Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine.ExportTerrainText
{
	/// <summary>
	/// Driver class for the ExportTerrainText plug-in.
	/// </summary>
	public class Driver : PlugIn
	{
		#region Data Members
		private SaveFileDialog	_dlgSave;
		#endregion

		#region Methods
		/// <summary>
		/// Creates the Driver class.
		/// </summary>
		public Driver()
		{
			base.InitializeBase();
			_name = "Export Terrain Text";
			this.CenterToParent();

			_dlgSave = new SaveFileDialog();
			_dlgSave.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*" ;
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
					WriteText();
					_success = true;
				}
			}
		}

		/// <summary>
		/// Writes the text data to the chosen file in the SaveFileDialog.
		/// </summary>
		private void WriteText()
		{
			if ( _dlgSave.FileName != null )
			{
				StreamWriter writer = new StreamWriter( _dlgSave.FileName );

				// Write TerrainPage data
				WriteTerrainPageData( ref writer );

				// Write TerrainPatch header data
				WriteTerrainPatchHeaderData( ref writer );

				// Write TerrainPatch texture data
				WriteTerrainPatchTextureData( ref writer );

				// Write TerrainPatch vertex data
				WriteTerrainPatchVertexData( ref writer );

				writer.Close();
			}
		}

		/// <summary>
		/// Writes the TerrainPage data to the output stream.
		/// </summary>
		/// <param name="writer">The output stream to write data to.</param>
		protected void WriteTerrainPageData( ref StreamWriter writer )
		{
			Vector3 position;
			Quaternion quat;

			// Write text file header
			writer.WriteLine( "TerrainPage Data:" );
			writer.WriteLine( "=================" );
			writer.WriteLine( "" );

			// Write TerrainPage name
			writer.WriteLine( "Name: " + _page.Name );
			writer.WriteLine( "" );

			// Write TerrainPage position
			position = _page.Position;
			writer.WriteLine( "Position:" );
			writer.WriteLine( "  X: " + position.X );
			writer.WriteLine( "  Y: " + position.Y );
			writer.WriteLine( "  Z: " + position.Z );
			writer.WriteLine( "" );

			// Write TerrainPage scale factors
			position = _page.Scale;
			writer.WriteLine( "Scale Factors:" );
			writer.WriteLine( "  X: " + position.X );
			writer.WriteLine( "  Y: " + position.Y );
			writer.WriteLine( "  Z: " + position.Z );
			writer.WriteLine( "" );

			// Write TerrainPage rotation quaternion
			quat = _page.Rotation;
			writer.WriteLine( "Rotation:" );
			writer.WriteLine( "  X: " + quat.X );
			writer.WriteLine( "  Y: " + quat.Y );
			writer.WriteLine( "  Z: " + quat.Z );
			writer.WriteLine( "  W: " + quat.W );
			writer.WriteLine( "" );

			// Write the maximum vertex height within the TerrainPage
			writer.WriteLine( "Maximum Vertex Height: " + _page.MaximumVertexHeight );
			writer.WriteLine( "" );
		}

		/// <summary>
		/// Writes the TerrainPatch header data to the output stream.
		/// </summary>
		/// <param name="writer">The output stream to write data to.</param>
		protected void WriteTerrainPatchHeaderData( ref StreamWriter writer )
		{
			// Write the TerrainPatch header
			writer.WriteLine( "" );
			writer.WriteLine( "TerrainPatch:" );
			writer.WriteLine( "=============" );
			writer.WriteLine( "" );

			// Write the number of rows in the TerrainPatch
			writer.WriteLine( "Rows: " + _page.TerrainPatch.Rows );

			// Write the number of columns in the TerrainPatch
			writer.WriteLine( "Columns: " + _page.TerrainPatch.Columns );

			// Write the height of the TerrainPatch
			writer.WriteLine( "Height: " + _page.TerrainPatch.Height );

			// Write the width of the TerrainPatch
			writer.WriteLine( "Width: " + _page.TerrainPatch.Width );
			writer.WriteLine( "" );
		}

		/// <summary>
		/// Writes the TerrainPatch texture data to the output stream.
		/// </summary>
		/// <param name="writer">The output stream to write data to.</param>
		protected void WriteTerrainPatchTextureData( ref StreamWriter writer )
		{
			Texture tex;

			// Write the texture coordinates for each texture
			for ( int i = 0; i < _page.TerrainPatch.NumTextures; i++ )
			{
				// Write the texture number
				tex = (Texture) _page.TerrainPatch.Textures[i];
				writer.WriteLine( "Texture " + (i + 1) + ":" );

				// Write the texture filename
				writer.WriteLine( "  Filename: " + tex.Name );

				// Write the texture operation
				writer.WriteLine( "  Texture Operation: " + tex.OperationText );

				// Write whether the texture is a mask
				writer.WriteLine( "  Is Mask: " + tex.Mask );

				// Write the texture scale value
				writer.WriteLine( "  Scale:" );
				writer.WriteLine( "    U: " + tex.Scale.X );
				writer.WriteLine( "    V: " + tex.Scale.Y );

				// Write the texture shift value
				writer.WriteLine( "  Shift:" );
				writer.WriteLine( "    U: " + tex.Shift.X );
				writer.WriteLine( "    V: " + tex.Shift.Y );

				// Write delimiting line
				writer.WriteLine( "" );
			}
		}

		/// <summary>
		/// Writes the TerrainPatch vertex data to the output stream.
		/// </summary>
		/// <param name="writer">The output stream to write data to.</param>
		protected void WriteTerrainPatchVertexData( ref StreamWriter writer )
		{
			Vector3 position;
			Vector2 texCoords;

			// Write the vertex data of the TerrainPatch
			for ( int i = 0; i < _page.TerrainPatch.Rows; i++ )
			{
				for ( int j = 0; j < _page.TerrainPatch.Columns; j++ )
				{
					// Write the vertex number
					writer.WriteLine( "Vertex " + ( i * _page.TerrainPatch.Rows + j ) + ":" );

					// Write the vertex position
					position = _page.TerrainPatch.Vertices[i * _page.TerrainPatch.Rows + j].Position;
					writer.WriteLine( "  Position:" );
					writer.WriteLine( "    X: " + position.X );
					writer.WriteLine( "    Y: " + position.Y );
					writer.WriteLine( "    Z: " + position.Z );

					// Write the vertex normal
					position = _page.TerrainPatch.Vertices[i * _page.TerrainPatch.Rows + j].Normal;
					writer.WriteLine( "  Normal:" );
					writer.WriteLine( "    X: " + position.X );
					writer.WriteLine( "    Y: " + position.Y );
					writer.WriteLine( "    Z: " + position.Z );

					// Write the texture coordinates for each texture
					for ( int k = 0; k < _page.TerrainPatch.NumTextures; k++ )
					{
						texCoords = (Vector2) ( (Vector2[]) _page.TerrainPatch.TextureCoordinates[k] )[i * _page.TerrainPatch.Rows + j];
						writer.WriteLine( "  Texture Coordinate " + (k + 1) + ":" );
						writer.WriteLine( "    U: " + texCoords.X );
						writer.WriteLine( "    V: " + texCoords.Y );
					}

					// Write delimiting line
					writer.WriteLine( "" );
				}
			}
		}
		#endregion
	}
}
