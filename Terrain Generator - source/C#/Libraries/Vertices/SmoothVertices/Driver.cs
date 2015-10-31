using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine.SmoothVertices
{
	/// <summary>
	/// Summary description for Smooth.
	/// </summary>
	public class Driver : Voyage.Terraingine.PlugIn
	{
		#region Data Members
		private System.Windows.Forms.Button btnRun;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numVertices;

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
		/// Creates a smoothing modifier form.
		/// </summary>
		public Driver()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.CenterToParent();
			base.InitializeBase();
			_name = "Smooth Vertices";
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
		/// Smooths the vertices in the TerrainPatch based upon a box-filtering
		/// method of averaging nearby vertices.
		/// </summary>
		/// <param name="numAdjVerts">Number of adjacent vertices that affect the smoothing.</param>
		private void SmoothVertices( int numAdjVerts )
		{
			CustomVertex.PositionNormal[] origVerts = _page.TerrainPatch.Vertices;
			Vector3[] newVerts = new Vector3[_page.TerrainPatch.NumVertices];
			Vector3 position;
			int numAffecting;
			int rows = _page.TerrainPatch.Rows;
			int columns = _page.TerrainPatch.Columns;

			for ( int i = 0; i < rows; i++ )
			{
				for ( int j = 0; j < columns; j++ )
				{
					position = Vector3.Empty;
					numAffecting = 0;

					for ( int k = -numAdjVerts; k <= numAdjVerts; k++ )
					{
						for ( int l = -numAdjVerts; l <= numAdjVerts; l++ )
						{
							if ( i + k > -1 && i + k < rows && j + l > -1 && j + l < columns )
							{
								position += _page.TerrainPatch.Vertices[( i + k ) * rows + j + l].Position;
								numAffecting++;
							}
						}
					}

					newVerts[i * rows + j] = origVerts[i * rows + j].Position;
					newVerts[i * rows + j].Y = position.Y /= numAffecting;
				}
			}

			for ( int i = 0; i < newVerts.Length; i++ )
				origVerts[i].Position = newVerts[i];
		}

		/// <summary>
		/// Runs the plug-in effect.
		/// </summary>
		private void btnRun_Click(object sender, System.EventArgs e)
		{
			SmoothVertices( Convert.ToInt32( numVertices.Value ) );
			_success = true;
			this.Close();
		}

		/// <summary>
		/// Checks that the number of adjacent vertices is not more than the current
		/// size of the TerrainPatch.
		/// </summary>
		private void numVertices_ValueChanged(object sender, System.EventArgs e)
		{
			int vertices;

			if ( _page.TerrainPatch.Rows < _page.TerrainPatch.Columns )
			{
				vertices = _page.TerrainPatch.Rows;

				if ( vertices % 2 == 0 )
					vertices--;

				if ( Convert.ToInt32( numVertices.Value ) > vertices )
					numVertices.Value = Convert.ToDecimal( vertices );
			}
			else
			{
				vertices = _page.TerrainPatch.Columns;

				if ( vertices % 2 == 0 )
					vertices--;

				if ( Convert.ToInt32( numVertices.Value ) > vertices )
					numVertices.Value = Convert.ToDecimal( vertices );
			}
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnRun = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.numVertices = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numVertices)).BeginInit();
			this.SuspendLayout();
			// 
			// btnRun
			// 
			this.btnRun.Location = new System.Drawing.Point(32, 56);
			this.btnRun.Name = "btnRun";
			this.btnRun.TabIndex = 0;
			this.btnRun.Text = "Run";
			this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(152, 56);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(152, 32);
			this.label1.TabIndex = 2;
			this.label1.Text = "Number of Adjacent Vertices That Affect Smoothing:";
			// 
			// numVertices
			// 
			this.numVertices.Location = new System.Drawing.Point(168, 16);
			this.numVertices.Maximum = new System.Decimal(new int[] {
																		200,
																		0,
																		0,
																		0});
			this.numVertices.Minimum = new System.Decimal(new int[] {
																		1,
																		0,
																		0,
																		0});
			this.numVertices.Name = "numVertices";
			this.numVertices.Size = new System.Drawing.Size(64, 20);
			this.numVertices.TabIndex = 3;
			this.numVertices.Value = new System.Decimal(new int[] {
																	  1,
																	  0,
																	  0,
																	  0});
			this.numVertices.ValueChanged += new System.EventHandler(this.numVertices_ValueChanged);
			// 
			// Smooth
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(242, 88);
			this.Controls.Add(this.numVertices);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnRun);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Smooth";
			this.Text = "Smooth Terrain Vertices";
			((System.ComponentModel.ISupportInitialize)(this.numVertices)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
