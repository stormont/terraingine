using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine.ExportTerrainBitmap
{
	/// <summary>
	/// Driver class for the ExportTerrainBitmap plug-in.
	/// </summary>
	public class Driver : Voyage.Terraingine.PlugIn
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
			_name = "Export Terrain Bitmap";
			this.CenterToParent();

			_dlgSave = new SaveFileDialog();
			_dlgSave.Filter = "Bitmap Files (*.bmp)|*.bmp|All files (*.*)|*.*" ;
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
					WriteBitmap();
					_success = true;
				}
			}
		}

		/// <summary>
		/// Writes the bitmap data to the chosen file in the SaveFileDialog.
		/// </summary>
		private void WriteBitmap()
		{
			if ( _dlgSave.FileName != null )
			{
				int rows = _page.TerrainPatch.Rows;
				int columns = _page.TerrainPatch.Columns;
				Bitmap bmp = new Bitmap( columns, rows );
				Color color;
				float position;

				for ( int i = 0; i < rows; i++ )
				{
					for ( int j = 0; j < columns; j++ )
					{
						position = _page.TerrainPatch.Vertices[i * rows + j].Position.Y;
						position *= 255.0f / _page.MaximumVertexHeight;

						if ( position > 255.0f )
							position = 255.0f;

						color = Color.FromArgb( ( int ) position, ( int ) position, ( int ) position );
						bmp.SetPixel( i, j, color );
					}
				}

				bmp.Save( _dlgSave.FileName );
			}
		}
		#endregion
	}
}
