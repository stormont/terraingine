using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine.NoiseGenerator
{
	/// <summary>
	/// Class for applying a noise modifier to a TerrainPage.
	/// </summary>
	public class Driver : Voyage.Terraingine.PlugIn
	{
		#region Data Members
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numMinimum;
		private System.Windows.Forms.NumericUpDown numMaximum;
		private System.Windows.Forms.CheckBox chkOverwrite;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnRun;
		private System.Windows.Forms.Button btnCancel;

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
		/// Creates a noise modifier form.
		/// </summary>
		public Driver()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.CenterToParent();
			base.InitializeBase();
			_name = "Noise Generator";
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
				ShowDialog( _owner );
		}

		/// <summary>
		/// Applies the noise modifier to the TerrainPage.
		/// </summary>
		/// <param name="maxScale">The maximum noise modifier.</param>
		/// <param name="minScale">The minimum noise modifier.</param>
		/// <param name="overwrite">Whether to overwrite or modify the previous positions.</param>
		private void CreateNoise( float maxScale, float minScale, bool overwrite )
		{
			float scale = maxScale - minScale;
			Random noiseLevel = new Random();
			int numVertices = _page.TerrainPatch.NumVertices;
			Vector3 position = new Vector3();

			// Randomize Y-position of each vertex
			for ( int i = 0; i < numVertices; i++ )
			{
				position = _page.TerrainPatch.Vertices[i].Position;

				if ( overwrite )	// Over-writes each position
					position.Y = ( float ) noiseLevel.NextDouble() * scale + minScale;
				else				// Modifies each position
					position.Y += ( float ) noiseLevel.NextDouble() * scale + minScale;

				_page.TerrainPatch.Vertices[i].Position = position;
			}
		}

		/// <summary>
		/// Runs the noise modifier.
		/// </summary>
		private void btnRun_Click(object sender, System.EventArgs e)
		{
			float max = ( float ) numMaximum.Value;
			float min = ( float ) numMinimum.Value;

			CreateNoise( max, min, chkOverwrite.Checked );
			_success = true;
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
			this.numMinimum = new System.Windows.Forms.NumericUpDown();
			this.numMaximum = new System.Windows.Forms.NumericUpDown();
			this.chkOverwrite = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btnRun = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numMinimum)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numMaximum)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Minimum Modifier:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Maximum Modifier:";
			// 
			// numMinimum
			// 
			this.numMinimum.DecimalPlaces = 2;
			this.numMinimum.Increment = new System.Decimal(new int[] {
																		 1,
																		 0,
																		 0,
																		 131072});
			this.numMinimum.Location = new System.Drawing.Point(112, 8);
			this.numMinimum.Maximum = new System.Decimal(new int[] {
																	   255,
																	   0,
																	   0,
																	   0});
			this.numMinimum.Name = "numMinimum";
			this.numMinimum.Size = new System.Drawing.Size(64, 20);
			this.numMinimum.TabIndex = 2;
			// 
			// numMaximum
			// 
			this.numMaximum.DecimalPlaces = 2;
			this.numMaximum.Increment = new System.Decimal(new int[] {
																		 1,
																		 0,
																		 0,
																		 131072});
			this.numMaximum.Location = new System.Drawing.Point(112, 32);
			this.numMaximum.Maximum = new System.Decimal(new int[] {
																	   255,
																	   0,
																	   0,
																	   0});
			this.numMaximum.Minimum = new System.Decimal(new int[] {
																	   1,
																	   0,
																	   0,
																	   131072});
			this.numMaximum.Name = "numMaximum";
			this.numMaximum.Size = new System.Drawing.Size(64, 20);
			this.numMaximum.TabIndex = 3;
			this.numMaximum.Value = new System.Decimal(new int[] {
																	 1,
																	 0,
																	 0,
																	 131072});
			// 
			// chkOverwrite
			// 
			this.chkOverwrite.Location = new System.Drawing.Point(8, 56);
			this.chkOverwrite.Name = "chkOverwrite";
			this.chkOverwrite.Size = new System.Drawing.Size(176, 24);
			this.chkOverwrite.TabIndex = 4;
			this.chkOverwrite.Text = "Overwrite Previous Positions";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 80);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(176, 40);
			this.label3.TabIndex = 5;
			this.label3.Text = "(Leaving the box unchecked will only modify the current positions)";
			// 
			// btnRun
			// 
			this.btnRun.Location = new System.Drawing.Point(8, 120);
			this.btnRun.Name = "btnRun";
			this.btnRun.TabIndex = 6;
			this.btnRun.Text = "Run";
			this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(112, 120);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			// 
			// Noise
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(200, 150);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnRun);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.chkOverwrite);
			this.Controls.Add(this.numMaximum);
			this.Controls.Add(this.numMinimum);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Noise";
			this.ShowInTaskbar = false;
			this.Text = "Noise Generator";
			((System.ComponentModel.ISupportInitialize)(this.numMinimum)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numMaximum)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
