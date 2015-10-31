using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine.Rainfall
{
	/// <summary>
	/// Class for applying a rainfall modifier to a TerrainPage.
	/// </summary>
	public class Driver : Voyage.Terraingine.PlugIn
	{
		#region Data Members
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnRun;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.NumericUpDown numRain;

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
		/// Creates a rainfall modifier form.
		/// </summary>
		public Driver()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.CenterToParent();
			base.InitializeBase();
			_name = "Rainfall";
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
				numRain.Maximum = Convert.ToDecimal( _page.MaximumVertexHeight );
				ShowDialog( _owner );
			}
		}

		/// <summary>
		/// Applies the rainfall modifier to the TerrainPage.
		/// </summary>
		/// <param name="amount">The amount of rainfall.</param>
		private void CreateRain( float amount )
		{
			float[] rain = new float[_page.TerrainPatch.NumVertices];
			bool[] processed = new bool[_page.TerrainPatch.NumVertices];
			Vector3 position;
			float height;
			int curVertex;
			int numProcessed = 0;

			// Set initial rain levels
			for ( int i = 0; i < rain.Length; i++ )
			{
				rain[i] = amount;
				processed[i] = false;
			}

			// Accumulate rainfall
			while ( numProcessed < _page.TerrainPatch.NumVertices )
			{
				height = -1f;
				curVertex = -1;

				// Determine next highest un-processed vertex
				for ( int i = 0; i < processed.Length; i++ )
				{
					if ( !processed[i] && height < _page.TerrainPatch.Vertices[i].Position.Y )
					{
						height = _page.TerrainPatch.Vertices[i].Position.Y;
						curVertex = i;
					}
				}

				// Process the chosen vertex
				if ( curVertex > -1 )
				{
					ProcessVertex( curVertex, ref rain );
					processed[curVertex] = true;
					numProcessed++;
				}
			}

			// Deform terrain according to accumulated rainfall
			for ( int i = 0; i < rain.Length; i++ )
			{
				position = _page.TerrainPatch.Vertices[i].Position;
				position.Y -= rain[i];

				if ( position.Y < 0f )
					position.Y = 0f;

				_page.TerrainPatch.Vertices[i].Position = position;
			}
		}

		/// <summary>
		/// Processes the specified vertex.
		/// </summary>
		/// <param name="vertex">The vertex to process.</param>
		/// <param name="rain">The accumulated rain.</param>
		private void ProcessVertex( int vertex, ref float[] rain )
		{
			int row = vertex / _page.TerrainPatch.Columns;
			int col = vertex - row * _page.TerrainPatch.Columns;
			int[] adjVerts = new int[6];
			float combHeight = 0f;

			// Determine the valid adjacent vertices to accumulate rain
			adjVerts[0] = vertex - _page.TerrainPatch.Columns - 1;
			adjVerts[1] = vertex - _page.TerrainPatch.Columns;
			adjVerts[2] = vertex - 1;
			adjVerts[3] = vertex + 1;
			adjVerts[4] = vertex + _page.TerrainPatch.Columns;
			adjVerts[5] = vertex + _page.TerrainPatch.Columns + 1;

			// Accumulate total height over which to disperse rain
			for ( int i = 0; i < adjVerts.Length; i++ )
			{
				// Only account valid vertices
				if ( adjVerts[i] > -1 && adjVerts[i] < _page.TerrainPatch.NumVertices )
				{
					// Only account vertices lower than the current vertex
					if ( _page.TerrainPatch.Vertices[ adjVerts[i] ].Position.Y <
						_page.TerrainPatch.Vertices[vertex].Position.Y )
					{
						combHeight += _page.TerrainPatch.Vertices[vertex].Position.Y -
							_page.TerrainPatch.Vertices[ adjVerts[i] ].Position.Y;
					}
					else
						adjVerts[i] = -1;
				}
			}

			// Disperse rain
			foreach ( int i in adjVerts )
				if ( i > -1 && i < _page.TerrainPatch.NumVertices )
					rain[i] += ( _page.TerrainPatch.Vertices[vertex].Position.Y -
						_page.TerrainPatch.Vertices[i].Position.Y ) / combHeight * rain[vertex];
		}

		/// <summary>
		/// Runs the rainfall modifier.
		/// </summary>
		private void btnRun_Click(object sender, System.EventArgs e)
		{
			CreateRain( ( float ) numRain.Value );
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
			this.numRain = new System.Windows.Forms.NumericUpDown();
			this.btnRun = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numRain)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Rain Amount:";
			// 
			// numRain
			// 
			this.numRain.DecimalPlaces = 3;
			this.numRain.Increment = new System.Decimal(new int[] {
																	  1,
																	  0,
																	  0,
																	  196608});
			this.numRain.Location = new System.Drawing.Point(112, 8);
			this.numRain.Maximum = new System.Decimal(new int[] {
																	255,
																	0,
																	0,
																	0});
			this.numRain.Minimum = new System.Decimal(new int[] {
																	1,
																	0,
																	0,
																	196608});
			this.numRain.Name = "numRain";
			this.numRain.Size = new System.Drawing.Size(64, 20);
			this.numRain.TabIndex = 2;
			this.numRain.Value = new System.Decimal(new int[] {
																  1,
																  0,
																  0,
																  196608});
			// 
			// btnRun
			// 
			this.btnRun.Location = new System.Drawing.Point(8, 40);
			this.btnRun.Name = "btnRun";
			this.btnRun.TabIndex = 6;
			this.btnRun.Text = "Run";
			this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(112, 40);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			// 
			// Driver
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(200, 72);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnRun);
			this.Controls.Add(this.numRain);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Location = new System.Drawing.Point(0, 0);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Driver";
			this.ShowInTaskbar = false;
			this.Text = "Rainfall Generator";
			((System.ComponentModel.ISupportInitialize)(this.numRain)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
