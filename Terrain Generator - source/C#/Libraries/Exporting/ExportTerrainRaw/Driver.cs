using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine.ExportTerrainRaw
{
	/// <summary>
	/// Driver class for the ExportTerrainRaw plug-in.
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
			_name = "Export Terrain RAW Image";
			this.CenterToParent();

			_dlgSave = new SaveFileDialog();
			_dlgSave.Filter = "Raw Files (*.raw)|*.raw|All files (*.*)|*.*" ;
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
					WriteRawBinary();
					_success = true;
				}
			}
		}

		/// <summary>
		/// Writes the raw image data to the chosen file in the SaveFileDialog.
		/// </summary>
		private void WriteRawBinary()
		{
			if ( _dlgSave.FileName != null )
			{
				int rows = _page.TerrainPatch.Rows;
				int columns = _page.TerrainPatch.Columns;
				FileStream stream = new FileStream( _dlgSave.FileName, FileMode.Create, FileAccess.Write );
				BinaryWriter writer = new BinaryWriter( stream );
				Bitmap bmp = new Bitmap( columns, rows );
				byte[] height = new byte[_page.TerrainPatch.NumVertices];
				float position;

				for ( int i = 0; i < rows; i++ )
				{
					for ( int j = 0; j < columns; j++ )
					{
						position = _page.TerrainPatch.Vertices[i * rows + j].Position.Y;
						position *= 255.0f / _page.MaximumVertexHeight;

						if ( position > 255.0f )
							position = 255.0f;

						height[(rows - i - 1) * rows + j] = Convert.ToByte( (int) position );
					}
				}

				writer.Write( height );
				writer.Close();
				stream.Close();
			}
		}
		#endregion
	}
}
