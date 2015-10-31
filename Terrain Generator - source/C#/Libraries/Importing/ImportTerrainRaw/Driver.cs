using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine.ImportTerrainRaw
{
	/// <summary>
	/// Driver class for the ImportTerrainRaw plug-in.
	/// </summary>
	public class Driver : Voyage.Terraingine.PlugIn
	{
		#region Data Members
		private OpenFileDialog	_dlgOpen;
		#endregion

		#region Members
		/// <summary>
		/// Creates the Driver class.
		/// </summary>
		public Driver()
		{
			base.InitializeBase();
			_name = "Import Terrain RAW Image";

			// Center the form to the center of its parent
			this.CenterToParent();

			_dlgOpen = new OpenFileDialog();
			_dlgOpen.Filter = "Raw Files (*.bmp)|*.raw|All files (*.*)|*.*" ;
			_dlgOpen.InitialDirectory = Path.GetDirectoryName( Application.ExecutablePath ) + "\\Projects";
		}

		/// <summary>
		/// Runs the plug-in.
		/// </summary>
		public override void Run()
		{
			if ( _page != null )
			{
				DialogResult result = _dlgOpen.ShowDialog( _owner );

				if ( result == DialogResult.OK && _dlgOpen.FileName != null )
				{
					LoadRaw();
					_success = true;
				}
			}
		}

		/// <summary>
		/// Creates new terrain from the raw image data specified from the SaveFileDialog.
		/// </summary>
		private void LoadRaw()
		{
			if ( _dlgOpen.FileName != null )
			{
				FileStream stream = new FileStream( _dlgOpen.FileName, FileMode.Open, FileAccess.Read );
				BinaryReader reader = new BinaryReader( stream );
				int rows = 0;
				int columns = 0;
				Vector3 position;
				Parameters param = new Parameters();
				byte[] data = reader.ReadBytes( Convert.ToInt32( stream.Length ) );

				// Check if the user is specifying an unordinary terrain size
				if ( param.ShowDialog( this ) == DialogResult.OK )
				{
					rows = param.TerrainHeight;
					columns = param.TerrainWidth;
				}

				// Set the terrain size to an ordinary square
				if ( rows == 0 || columns == 0 )
				{
					rows = Convert.ToInt32( Math.Ceiling( Math.Sqrt( data.Length ) ) );
					columns = rows;
				}

				// Create the terrain
				_page.TerrainPatch.CreatePatch( rows, columns );

				// Load the terrain heights
				for ( int i = 0; i < rows; i++ )
				{
					for ( int j = 0; j < columns; j++ )
					{
						position = _page.TerrainPatch.Vertices[i * rows + j].Position;
						position.Y = Convert.ToInt32( data[(rows - i - 1) * rows + j] ) / 255.0f *
							_page.MaximumVertexHeight;
						_page.TerrainPatch.Vertices[i * rows + j].Position = position;
					}
				}

				reader.Close();
				stream.Close();
			}
		}
		#endregion
	}
}
