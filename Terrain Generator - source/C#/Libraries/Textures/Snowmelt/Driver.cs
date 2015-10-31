using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine.Snowmelt
{
	/// <summary>
	/// Class for applying a snowmelt generator to a TerrainPage.
	/// </summary>
	public class Driver : Voyage.Terraingine.PlugIn
	{
		#region Data Members


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Properties
		/// <summary>
		/// Gets the TerrainPage used.
		/// </summary>
		public TerrainPage TerrainPage
		{
			get { return _page; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates a snowmelt generator form.
		/// </summary>
		public Driver()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.CenterToParent();
			base.InitializeBase();
			_name = "Snowmelt";
			_desiredData.Add( PlugIn.DesiredData.Lighting );
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Runs the plug-in.
		/// </summary>
		public override void Run()
		{
			if ( _page != null )
			{
				if ( _page.TerrainPatch.GetTexture() != null )
				{
					if ( _receivedData.Count > 0 )
					{
						MeltSnow();
						_success = true;
						_modifiedTextures = true;
					}
					else
						MessageBox.Show( "No lighting data was received", "Cannot Perform Operation",
							MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
				}
				else
					MessageBox.Show( "No texture is currently selected", "Cannot Perform Operation",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
			}
		}

		/// <summary>
		/// Applies the snowmelt generator to the TerrainPage.
		/// </summary>
		private void MeltSnow()
		{
			DataCore.Texture oldTex = _page.TerrainPatch.GetTexture();
			DataCore.Texture tex = new Voyage.Terraingine.DataCore.Texture();
			Bitmap oldImage = oldTex.GetImage();
			Bitmap image = new Bitmap( oldImage.Width, oldImage.Height,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb );
			float xScale = _page.TerrainPatch.Width / image.Width;
			float yScale = _page.TerrainPatch.Height / image.Height;
			string filename;
			Vector2 origin;
			Vector3 point, normal;
			Vector3 n1, n2, n3;
			int v1, v2, v3;
			Vector3 light = (Vector3) _receivedData[0];
			float alpha, dot;
			Color color;
			int fileCount = -1;
			float dist1, dist2, dist3;

			for ( int j = 0; j < image.Height; j++ )
			{
				for ( int i = 0; i < image.Width; i++ )
				{
					// Get original pixel color
					color = oldImage.GetPixel( i, j );

					// Find point on terrain
					origin = new Vector2( (float) i * xScale, (float) j * yScale );
					_page.GetPlane( origin, out v1, out v2, out v3, out point );

					// Determine weighted distance from each vertex
					dist1 = ( (Vector3) _page.TerrainPatch.Vertices[v1].Position - point ).Length();
					dist2 = ( (Vector3) _page.TerrainPatch.Vertices[v2].Position - point ).Length();
					dist3 = ( (Vector3) _page.TerrainPatch.Vertices[v3].Position - point ).Length();

					// Determine weighted normal
					n1 = ( dist1 / ( dist1 + dist2 + dist3 ) ) * _page.TerrainPatch.Vertices[v1].Normal;
					n2 = ( dist2 / ( dist1 + dist2 + dist3 ) ) * _page.TerrainPatch.Vertices[v2].Normal;
					n3 = ( dist3 / ( dist1 + dist2 + dist3 ) ) * _page.TerrainPatch.Vertices[v3].Normal;
					normal = n1 + n2 + n3;
					normal.Normalize();

					// Determine angle of light against pixel position
					dot = Vector3.Dot( normal, -light );

					// Determine alpha of new pixel
					if ( dot >= 0f )
						alpha = oldImage.GetPixel( i, j ).A - 255f * dot;
					else
						alpha = oldImage.GetPixel( i, j ).A;

					if ( alpha < 0f )
						alpha = 0f;
					else if ( alpha > 255f )
						alpha = 255f;

					color = Color.FromArgb( color.ToArgb() );
					image.SetPixel( i, j, color );
				}
			}

			do
			{
				fileCount++;
				filename = Path.GetDirectoryName( oldTex.FileName ) + "\\" +
					Path.GetFileNameWithoutExtension( oldTex.FileName ) + "_snowmelt" + fileCount + ".bmp";
			} while ( File.Exists( filename ) );

			image.Save( filename, System.Drawing.Imaging.ImageFormat.Bmp );
			tex.FileName = filename;
			tex.Name = "Snowmelt";
			tex.Operation = TextureOperation.BlendTextureAlpha;
			tex.OperationText = "Use Texture Alpha";
			_page.TerrainPatch.AddTexture( tex );

			_textures = _page.TerrainPatch.Textures;
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// Driver
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(200, 96);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Location = new System.Drawing.Point(0, 0);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Driver";
			this.ShowInTaskbar = false;
			this.Text = "Snowfall Emulator";

		}
		#endregion
	}
}
