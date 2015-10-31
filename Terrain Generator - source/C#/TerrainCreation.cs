using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DXViewport;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine
{
	/// <summary>
	/// Summary description for TerrainCreation.
	/// </summary>
	public class TerrainCreation : TerrainViewport
	{
		#region Data Members
		private bool			_accepted;
		private DataInterfacing.DataManipulation	_terrainData;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numRows;
		private System.Windows.Forms.NumericUpDown numColumns;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.CheckBox chkGridDimensions;
		private System.Windows.Forms.GroupBox grpGridDimensions;
		private System.Windows.Forms.CheckBox chkGridSize;
		private System.Windows.Forms.NumericUpDown numColumnDistance;
		private System.Windows.Forms.NumericUpDown numRowDistance;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox grpGridSize;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		#region Properties
		/// <summary>
		/// Gets if the OK button has been pressed.
		/// </summary>
		public bool Accepted
		{
			get { return _accepted; }
		}
		#endregion

		#region Basic Form Functions
		/// <summary>
		/// Creates the TerrainCreation form.
		/// </summary>
		public TerrainCreation()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.CenterToParent();
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
		#endregion

		#region Initialization
		/// <summary>
		/// Creates the TerrainCreation form.
		/// </summary>
		private void TerrainCreation_Load(object sender, System.EventArgs e)
		{
			// DXViewport initialization
			_viewport.InitializeViewport();
			_viewport.InitializeDXDefaults();

			_viewport.DXViewport.ClearColor = Color.Gray;
			_viewport.DXViewport.Camera.FirstPerson = false;
			_viewport.FillMode = FillMode.WireFrame;
			_viewport.CullMode = Cull.CounterClockwise;
			_viewport.DXViewport.Camera.FollowDistance = 2.0f;
			
			_viewport.InitializeCamera();

			// Terrain initialization
			_viewport.TerrainData.EnableLighting = false;

			// Additional uninherited initialization
			_accepted = false;
			_terrainData = new DataInterfacing.DataManipulation( _viewport.DXViewport );
			UpdateTerrain();
		}
		#endregion

		#region Terrain Functions
		/// <summary>
		/// Creates the TerrainPatch of the form.
		/// </summary>
		/// <param name="rows">The number of rows in the TerrainPatch.</param>
		/// <param name="columns">The number of columns in the TerrainPatch.</param>
		/// <param name="height">The total height of the TerrainPatch.</param>
		/// <param name="width">The total width of the TerrainPatch.</param>
		public void CreateTerrain( int rows, int columns, float height, float width )
		{
			if ( _viewport.TerrainData.TerrainPage != null )
				_viewport.TerrainData.TerrainPage.Dispose();

			_viewport.TerrainData.TerrainPage = _terrainData.CreateTerrain( rows, columns, height, width );
			_viewport.TerrainData.RefreshAllBuffers();

			if ( height > width )
				_viewport.DXViewport.Camera.FollowDistance = height * 2.0f;
			else
				_viewport.DXViewport.Camera.FollowDistance = width * 2.0f;

			_viewport.InitializeCamera();
		}

		/// <summary>
		/// Updates the TerrainPatch in the form.
		/// </summary>
		private void UpdateTerrain()
		{
			CreateTerrain( Convert.ToInt32( numRows.Value ), Convert.ToInt32( numColumns.Value ),
				( float ) numRowDistance.Value, ( float ) numColumnDistance.Value );
		}

		/// <summary>
		/// Cancels the TerrainPatch creation.
		/// </summary>
		public void btnCancel_Click()
		{
			this.Close();
		}

		/// <summary>
		/// Updates and accepts the TerrainPatch creation.
		/// </summary>
		public void btnOK_Click()
		{
			UpdateTerrain();
			_accepted = true;
			this.Close();
		}
		#endregion

		#region Button Clicks
		/// <summary>
		/// Cancels the TerrainPatch creation.
		/// </summary>
		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			btnCancel_Click();
		}

		/// <summary>
		/// Updates and accepts the TerrainPatch creation.
		/// </summary>
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			btnOK_Click();
		}
		#endregion

		#region Other Form Callbacks
		/// <summary>
		/// Updates the TerrainPage when the control has a changed value.
		/// </summary>
		private void numRows_ValueChanged(object sender, System.EventArgs e)
		{
			if ( chkGridDimensions.Checked )
				numColumns.Value = numRows.Value;

			UpdateTerrain();
		}

		/// <summary>
		/// Updates the TerrainPage when the control has a changed value.
		/// </summary>
		private void numColumns_ValueChanged(object sender, System.EventArgs e)
		{
			if ( chkGridDimensions.Checked )
				numRows.Value = numColumns.Value;

			UpdateTerrain();
		}

		/// <summary>
		/// Updates the TerrainPage when the control loses focus.
		/// </summary>
		private void numRows_Leave(object sender, System.EventArgs e)
		{
			if ( chkGridDimensions.Checked )
				numColumns.Value = numRows.Value;

			UpdateTerrain();
		}

		/// <summary>
		/// Updates the TerrainPage when the control loses focus.
		/// </summary>
		private void numColumns_Leave(object sender, System.EventArgs e)
		{
			if ( chkGridDimensions.Checked )
				numRows.Value = numColumns.Value;

			UpdateTerrain();
		}

		/// <summary>
		/// Keeps the rows and columns equal in the TerrainPatch.
		/// </summary>
		private void chkGridDimensions_CheckedChanged(object sender, System.EventArgs e)
		{
			if ( chkGridDimensions.Checked )
			{
				numColumns.Value = numRows.Value;
				UpdateTerrain();
			}
		}

		/// <summary>
		/// Updates the distance between rows in the TerrainPatch.
		/// </summary>
		private void numRowDistance_ValueChanged(object sender, System.EventArgs e)
		{
			if ( chkGridSize.Checked )
				numColumnDistance.Value = numRowDistance.Value;

			UpdateTerrain();
		}

		/// <summary>
		/// Updates the distance between rows in the TerrainPatch.
		/// </summary>
		private void numRowDistance_Leave(object sender, System.EventArgs e)
		{
			if ( chkGridSize.Checked )
				numColumnDistance.Value = numRowDistance.Value;

			UpdateTerrain();
		}

		/// <summary>
		/// Updates the distance between columns in the TerrainPatch.
		/// </summary>
		private void numColumnDistance_ValueChanged(object sender, System.EventArgs e)
		{
			if ( chkGridSize.Checked )
				numRowDistance.Value = numColumnDistance.Value;

			UpdateTerrain();
		}

		/// <summary>
		/// Updates the distance between columns in the TerrainPatch.
		/// </summary>
		private void numColumnDistance_Leave(object sender, System.EventArgs e)
		{
			if ( chkGridSize.Checked )
				numRowDistance.Value = numColumnDistance.Value;

			UpdateTerrain();
		}

		/// <summary>
		/// Keeps the distances between rows and columns equal in the TerrainPatch.
		/// </summary>
		private void chkGridSize_CheckedChanged(object sender, System.EventArgs e)
		{
			if ( chkGridSize.Checked )
			{
				numColumnDistance.Value = numRowDistance.Value;
				UpdateTerrain();
			}
		}

		/// <summary>
		/// Safely disposes of the DirectX viewport.
		/// </summary>
		private void TerrainCreation_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_viewport.DXViewport.Dispose();
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
			this.numRows = new System.Windows.Forms.NumericUpDown();
			this.numColumns = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.chkGridDimensions = new System.Windows.Forms.CheckBox();
			this.grpGridDimensions = new System.Windows.Forms.GroupBox();
			this.chkGridSize = new System.Windows.Forms.CheckBox();
			this.numColumnDistance = new System.Windows.Forms.NumericUpDown();
			this.numRowDistance = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.grpGridSize = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.numRows)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numColumns)).BeginInit();
			this.grpGridDimensions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numColumnDistance)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numRowDistance)).BeginInit();
			this.grpGridSize.SuspendLayout();
			this.SuspendLayout();
			// 
			// _viewport
			// 
			this._viewport.Location = new System.Drawing.Point(0, 24);
			this._viewport.Name = "_viewport";
			this._viewport.Size = new System.Drawing.Size(200, 192);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.TabIndex = 1;
			this.label1.Text = "Preview:";
			// 
			// numRows
			// 
			this.numRows.Location = new System.Drawing.Point(48, 16);
			this.numRows.Maximum = new System.Decimal(new int[] {
																	1000,
																	0,
																	0,
																	0});
			this.numRows.Minimum = new System.Decimal(new int[] {
																	2,
																	0,
																	0,
																	0});
			this.numRows.Name = "numRows";
			this.numRows.Size = new System.Drawing.Size(56, 20);
			this.numRows.TabIndex = 2;
			this.numRows.Value = new System.Decimal(new int[] {
																  25,
																  0,
																  0,
																  0});
			this.numRows.ValueChanged += new System.EventHandler(this.numRows_ValueChanged);
			this.numRows.Leave += new System.EventHandler(this.numRows_Leave);
			// 
			// numColumns
			// 
			this.numColumns.Location = new System.Drawing.Point(168, 16);
			this.numColumns.Maximum = new System.Decimal(new int[] {
																	   1000,
																	   0,
																	   0,
																	   0});
			this.numColumns.Minimum = new System.Decimal(new int[] {
																	   2,
																	   0,
																	   0,
																	   0});
			this.numColumns.Name = "numColumns";
			this.numColumns.Size = new System.Drawing.Size(56, 20);
			this.numColumns.TabIndex = 3;
			this.numColumns.Value = new System.Decimal(new int[] {
																	 25,
																	 0,
																	 0,
																	 0});
			this.numColumns.ValueChanged += new System.EventHandler(this.numColumns_ValueChanged);
			this.numColumns.Leave += new System.EventHandler(this.numColumns_Leave);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 23);
			this.label2.TabIndex = 4;
			this.label2.Text = "Rows:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(112, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 23);
			this.label3.TabIndex = 5;
			this.label3.Text = "Columns:";
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(232, 176);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(72, 23);
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(344, 176);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(72, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// chkGridDimensions
			// 
			this.chkGridDimensions.Checked = true;
			this.chkGridDimensions.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkGridDimensions.Location = new System.Drawing.Point(40, 40);
			this.chkGridDimensions.Name = "chkGridDimensions";
			this.chkGridDimensions.Size = new System.Drawing.Size(184, 24);
			this.chkGridDimensions.TabIndex = 9;
			this.chkGridDimensions.Text = "Lock Grid Dimensions";
			this.chkGridDimensions.CheckedChanged += new System.EventHandler(this.chkGridDimensions_CheckedChanged);
			// 
			// grpGridDimensions
			// 
			this.grpGridDimensions.Controls.Add(this.numColumns);
			this.grpGridDimensions.Controls.Add(this.label2);
			this.grpGridDimensions.Controls.Add(this.label3);
			this.grpGridDimensions.Controls.Add(this.chkGridDimensions);
			this.grpGridDimensions.Controls.Add(this.numRows);
			this.grpGridDimensions.Location = new System.Drawing.Point(200, 8);
			this.grpGridDimensions.Name = "grpGridDimensions";
			this.grpGridDimensions.Size = new System.Drawing.Size(232, 72);
			this.grpGridDimensions.TabIndex = 11;
			this.grpGridDimensions.TabStop = false;
			this.grpGridDimensions.Text = "Grid Dimensions";
			// 
			// chkGridSize
			// 
			this.chkGridSize.Checked = true;
			this.chkGridSize.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkGridSize.Location = new System.Drawing.Point(32, 40);
			this.chkGridSize.Name = "chkGridSize";
			this.chkGridSize.Size = new System.Drawing.Size(192, 24);
			this.chkGridSize.TabIndex = 14;
			this.chkGridSize.Text = "Lock Terrain Size";
			this.chkGridSize.CheckedChanged += new System.EventHandler(this.chkGridSize_CheckedChanged);
			// 
			// numColumnDistance
			// 
			this.numColumnDistance.DecimalPlaces = 2;
			this.numColumnDistance.Increment = new System.Decimal(new int[] {
																				1,
																				0,
																				0,
																				131072});
			this.numColumnDistance.Location = new System.Drawing.Point(168, 16);
			this.numColumnDistance.Maximum = new System.Decimal(new int[] {
																			  1000,
																			  0,
																			  0,
																			  0});
			this.numColumnDistance.Minimum = new System.Decimal(new int[] {
																			  1,
																			  0,
																			  0,
																			  131072});
			this.numColumnDistance.Name = "numColumnDistance";
			this.numColumnDistance.Size = new System.Drawing.Size(56, 20);
			this.numColumnDistance.TabIndex = 13;
			this.numColumnDistance.Value = new System.Decimal(new int[] {
																			100,
																			0,
																			0,
																			131072});
			this.numColumnDistance.ValueChanged += new System.EventHandler(this.numColumnDistance_ValueChanged);
			this.numColumnDistance.Leave += new System.EventHandler(this.numColumnDistance_Leave);
			// 
			// numRowDistance
			// 
			this.numRowDistance.DecimalPlaces = 2;
			this.numRowDistance.Increment = new System.Decimal(new int[] {
																			 1,
																			 0,
																			 0,
																			 131072});
			this.numRowDistance.Location = new System.Drawing.Point(48, 16);
			this.numRowDistance.Maximum = new System.Decimal(new int[] {
																		   1000,
																		   0,
																		   0,
																		   0});
			this.numRowDistance.Minimum = new System.Decimal(new int[] {
																		   1,
																		   0,
																		   0,
																		   131072});
			this.numRowDistance.Name = "numRowDistance";
			this.numRowDistance.Size = new System.Drawing.Size(56, 20);
			this.numRowDistance.TabIndex = 12;
			this.numRowDistance.Value = new System.Decimal(new int[] {
																		 10,
																		 0,
																		 0,
																		 65536});
			this.numRowDistance.ValueChanged += new System.EventHandler(this.numRowDistance_ValueChanged);
			this.numRowDistance.Leave += new System.EventHandler(this.numRowDistance_Leave);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(120, 16);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(48, 23);
			this.label5.TabIndex = 11;
			this.label5.Text = "Height:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(40, 23);
			this.label4.TabIndex = 10;
			this.label4.Text = "Width:";
			// 
			// grpGridSize
			// 
			this.grpGridSize.Controls.Add(this.numColumnDistance);
			this.grpGridSize.Controls.Add(this.numRowDistance);
			this.grpGridSize.Controls.Add(this.label5);
			this.grpGridSize.Controls.Add(this.label4);
			this.grpGridSize.Controls.Add(this.chkGridSize);
			this.grpGridSize.Location = new System.Drawing.Point(200, 88);
			this.grpGridSize.Name = "grpGridSize";
			this.grpGridSize.Size = new System.Drawing.Size(232, 72);
			this.grpGridSize.TabIndex = 12;
			this.grpGridSize.TabStop = false;
			this.grpGridSize.Text = "Total Terrain Size";
			// 
			// TerrainCreation
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(442, 216);
			this.Controls.Add(this.grpGridSize);
			this.Controls.Add(this.grpGridDimensions);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Location = new System.Drawing.Point(0, 0);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TerrainCreation";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Create a Terrain Patch";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.TerrainCreation_Closing);
			this.Load += new System.EventHandler(this.TerrainCreation_Load);
			this.Controls.SetChildIndex(this._viewport, 0);
			this.Controls.SetChildIndex(this.label1, 0);
			this.Controls.SetChildIndex(this.btnOK, 0);
			this.Controls.SetChildIndex(this.btnCancel, 0);
			this.Controls.SetChildIndex(this.grpGridDimensions, 0);
			this.Controls.SetChildIndex(this.grpGridSize, 0);
			((System.ComponentModel.ISupportInitialize)(this.numRows)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numColumns)).EndInit();
			this.grpGridDimensions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numColumnDistance)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numRowDistance)).EndInit();
			this.grpGridSize.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
