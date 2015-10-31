using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine.Snowfall
{
	/// <summary>
	/// Class for applying a snowfall generator to a TerrainPage.
	/// </summary>
	public class Driver : Voyage.Terraingine.PlugIn
	{
		#region Data Members
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnRun;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.NumericUpDown numLevel;
		private System.Windows.Forms.NumericUpDown numBlend;

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
		/// Creates a snowfall generator form.
		/// </summary>
		public Driver()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.CenterToParent();
			base.InitializeBase();
			_name = "Snowfall";
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
				numLevel.Maximum = Convert.ToDecimal( _page.MaximumVertexHeight );
				numBlend.Maximum = Convert.ToDecimal( _page.MaximumVertexHeight );
				numLevel.Value = Convert.ToDecimal( _page.MaximumVertexHeight / 2f );

				if ( _page.TerrainPatch.GetTexture() != null )
					ShowDialog( _owner );
				else
					MessageBox.Show( "No texture is currently selected", "Cannot Perform Operation",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
			}
		}

		/// <summary>
		/// Applies the snowfall generator to the TerrainPage.
		/// </summary>
		/// <param name="level">The snow level (elevation).</param>
		/// <param name="blend">The snow blending distance.</param>
		private void CreateSnow( float level, float blend )
		{
			DataCore.Texture tex = _page.TerrainPatch.GetTexture();
			DataCore.Texture newTex = new Voyage.Terraingine.DataCore.Texture();
			Bitmap image = new Bitmap( 128, 128, System.Drawing.Imaging.PixelFormat.Format32bppArgb );
			float xScale = _page.TerrainPatch.Width / image.Width;
			float yScale = _page.TerrainPatch.Height / image.Height;
			string filename;
			Vector2 origin;
			Vector3 point;
			int v1, v2, v3;
			float alpha;
			int fileCount = -1;

			for ( int j = 0; j < image.Height; j++ )
			{
				for ( int i = 0; i < image.Width; i++ )
				{
					origin = new Vector2( (float) i * xScale, (float) j * yScale );
					_page.GetPlane( origin, out v1, out v2, out v3, out point );

					if ( point.Y >= level )
					{
						if ( point.Y >= level + blend )
							image.SetPixel( i, j, Color.FromArgb( 0, Color.White ) );
						else
						{
							alpha = 255f - ( point.Y - level ) / blend * 255f;
							image.SetPixel( i, j, Color.FromArgb( (int) alpha, Color.White ) );
						}
					}
					else
						image.SetPixel( i, j, Color.FromArgb( 255, Color.White ) );
				}
			}

			do
			{
				fileCount++;
				filename = Path.GetDirectoryName( tex.FileName ) + "\\" +
					Path.GetFileNameWithoutExtension( tex.FileName ) + "_snowfall" + fileCount + ".bmp";
			} while ( File.Exists( filename ) );

			image.Save( filename, System.Drawing.Imaging.ImageFormat.Bmp );
			newTex.FileName = filename;
			newTex.Name = "Snowfall";
			newTex.Operation = TextureOperation.BlendTextureAlpha;
			newTex.OperationText = "Use Texture Alpha";
			_page.TerrainPatch.AddTexture( newTex );

			_textures = _page.TerrainPatch.Textures;
		}

		/// <summary>
		/// Runs the snowfall generator.
		/// </summary>
		private void btnRun_Click(object sender, System.EventArgs e)
		{
			float level = ( float ) numLevel.Value;
			float blend = ( float ) numBlend.Value;

			CreateSnow( level, blend );
			_success = true;
			_modifiedTextures = true;
			this.Close();
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.numLevel = new System.Windows.Forms.NumericUpDown();
			this.numBlend = new System.Windows.Forms.NumericUpDown();
			this.btnRun = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numLevel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numBlend)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Snow Level:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Blend Distance:";
			// 
			// numLevel
			// 
			this.numLevel.DecimalPlaces = 3;
			this.numLevel.Increment = new System.Decimal(new int[] {
																	   1,
																	   0,
																	   0,
																	   196608});
			this.numLevel.Location = new System.Drawing.Point(112, 8);
			this.numLevel.Maximum = new System.Decimal(new int[] {
																	 1,
																	 0,
																	 0,
																	 0});
			this.numLevel.Name = "numLevel";
			this.numLevel.Size = new System.Drawing.Size(64, 20);
			this.numLevel.TabIndex = 2;
			// 
			// numBlend
			// 
			this.numBlend.DecimalPlaces = 3;
			this.numBlend.Increment = new System.Decimal(new int[] {
																	   1,
																	   0,
																	   0,
																	   196608});
			this.numBlend.Location = new System.Drawing.Point(112, 32);
			this.numBlend.Maximum = new System.Decimal(new int[] {
																	 1,
																	 0,
																	 0,
																	 0});
			this.numBlend.Name = "numBlend";
			this.numBlend.Size = new System.Drawing.Size(64, 20);
			this.numBlend.TabIndex = 3;
			this.numBlend.Value = new System.Decimal(new int[] {
																   1,
																   0,
																   0,
																   65536});
			// 
			// btnRun
			// 
			this.btnRun.Location = new System.Drawing.Point(8, 64);
			this.btnRun.Name = "btnRun";
			this.btnRun.TabIndex = 6;
			this.btnRun.Text = "Run";
			this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(112, 64);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			// 
			// Driver
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(200, 96);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnRun);
			this.Controls.Add(this.numBlend);
			this.Controls.Add(this.numLevel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Location = new System.Drawing.Point(0, 0);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Driver";
			this.ShowInTaskbar = false;
			this.Text = "Snowfall Emulator";
			((System.ComponentModel.ISupportInitialize)(this.numLevel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numBlend)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
