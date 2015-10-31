using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine.ImportTerrainBitmap
{
	/// <summary>
	/// Driver class for the ImportTerrainBitmap plug-in.
	/// </summary>
	public class Driver : Voyage.Terraingine.PlugIn
	{
		#region Data Members
		private OpenFileDialog	_dlgOpen;
		#endregion

		#region Methods
		/// <summary>
		/// Creates the Driver class.
		/// </summary>
		public Driver()
		{
			base.InitializeBase();
			_name = "Import Terrain Bitmap";
			this.CenterToParent();

			_dlgOpen = new OpenFileDialog();
			_dlgOpen.Filter = "Bitmap Files (*.bmp)|*.bmp|All files (*.*)|*.*" ;
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
					LoadBitmap();
					_success = true;
				}
			}
		}

		/// <summary>
		/// Writes the bitmap data to the chosen file in the SaveFileDialog.
		/// </summary>
		private void LoadBitmap()
		{
			if ( _dlgOpen.FileName != null )
			{
				Bitmap bmp = new Bitmap( _dlgOpen.FileName );
				int rows = bmp.Size.Height;
				int columns = bmp.Size.Width;
				Color color;
				Vector3 position;

				_page.TerrainPatch.CreatePatch( rows, columns );

				for ( int i = 0; i < rows; i++ )
				{
					for ( int j = 0; j < columns; j++ )
					{
						color = bmp.GetPixel( i, j );
						position = _page.TerrainPatch.Vertices[i * rows + j].Position;
						position.Y = ( int ) color.R / 255.0f * _page.MaximumVertexHeight;
						_page.TerrainPatch.Vertices[i * rows + j].Position = position;
					}
				}
			}
		}
		#endregion
	}
}
