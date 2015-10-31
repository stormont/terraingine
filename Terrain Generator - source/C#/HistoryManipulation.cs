using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataInterfacing;
using Voyage.Terraingine.DXViewport;

namespace Voyage.Terraingine
{
	/// <summary>
	/// A user control for manipulating terrain history.
	/// </summary>
	public class HistoryManipulation : System.Windows.Forms.UserControl
	{
		#region Data Members
		private DataInterfacing.ViewportInterface	_viewport;
		private DataInterfacing.DataManipulation	_terrainData;
		private DXViewport.Viewport	_dx;
		private TerrainViewport		_owner;
		private int					_rollbackIndex;

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListBox lstHistory;
		private System.Windows.Forms.Button btnRollback;
		private System.Windows.Forms.Button btnRestore;
		private System.Windows.Forms.Button btnRestoreAll;
		private System.Windows.Forms.Label lblActive;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Basic Form Methods
		/// <summary>
		/// Creates a history manipulation user control.
		/// </summary>
		public HistoryManipulation()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			_rollbackIndex = 0;
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
		/// Initializes the control's data members.
		/// </summary>
		/// <param name="owner">The TerrainViewport that contains the control.</param>
		public void Initialize( TerrainViewport owner )
		{
			// Shortcut variables for the DirectX viewport and the terrain data
			_owner = owner;
			_viewport = owner.MainViewport;
			_terrainData = owner.MainViewport.TerrainData;
			_dx = owner.MainViewport.DXViewport;

			// Register tooltips
			ToolTip t = new ToolTip();

			// History group tooltips
			t.SetToolTip( btnRollback, "Rollback to the selected previous terrain state" );
			t.SetToolTip( btnRestore, "Restore to the selected terrain state" );
			t.SetToolTip( btnRestoreAll, "Restore to the most recent terrain state" );
		}
		#endregion

		#region History
		/// <summary>
		/// Selects a history action from the list.
		/// </summary>
		private void lstHistory_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// Enable Rollback button?
			if ( lstHistory.SelectedIndex > _rollbackIndex )
				btnRollback.Enabled = true;
			else
				btnRollback.Enabled = false;

			// Enable Restore button?
			if ( _rollbackIndex > lstHistory.SelectedIndex )
				btnRestore.Enabled = true;
			else
				btnRestore.Enabled = false;

			// Enable Restore All button?
			if ( _rollbackIndex > 0 )
				btnRestoreAll.Enabled = true;
			else
				btnRestoreAll.Enabled = false;

			// Display whether the action is active or not
			if ( lstHistory.SelectedIndex > -1 )
			{
				if ( lstHistory.SelectedIndex < _rollbackIndex )
					lblActive.Text = "Rolled-back Terrain Action";
				else
					lblActive.Text = "Active Terrain Action";
			}
			else
				lblActive.Text = "";
		}

		/// <summary>
		/// Performs a rollback to the selected history action.
		/// </summary>
		private void lstHistory_DoubleClick(object sender, System.EventArgs e)
		{
			if ( lstHistory.SelectedIndex > _rollbackIndex )
				btnRollback_Click( sender, e );
			else if ( lstHistory.SelectedIndex < _rollbackIndex && lstHistory.SelectedIndex > -1 )
				btnRestore_Click( sender, e );
		}

		/// <summary>
		/// Rolls back the terrain history to the selected action.
		/// </summary>
		private void btnRollback_Click(object sender, System.EventArgs e)
		{
			// Performs an "undo" action for each history action above the selected action
			for ( int i = _rollbackIndex; i < lstHistory.SelectedIndex; i++ )
				_terrainData.UndoLastPageAction();

			( (MainForm) _owner ).UndoTerrainAction();
			_rollbackIndex = lstHistory.SelectedIndex;
			btnRollback.Enabled = false;
			btnRestore.Enabled = false;
			btnRestoreAll.Enabled = true;
		}

		/// <summary>
		/// Restores the terrain history to the selected action.
		/// </summary>
		private void btnRestore_Click(object sender, System.EventArgs e)
		{
			// Performs a "redo" action for each history action above the selected action
			for ( int i = _rollbackIndex; i > lstHistory.SelectedIndex; i-- )
				_terrainData.RedoLastPageAction();

			( (MainForm) _owner ).RedoTerrainAction();
			_rollbackIndex = lstHistory.SelectedIndex;
			lblActive.Text = "Active Terrain Action";
			btnRollback.Enabled = false;
			btnRestore.Enabled = false;

			if ( _rollbackIndex > 0 )
				btnRestoreAll.Enabled = true;
			else
				btnRestoreAll.Enabled = false;
		}

		/// <summary>
		/// Restores all terrain history actions.
		/// </summary>
		private void btnRestoreAll_Click(object sender, System.EventArgs e)
		{
			// Performs a "redo" action for each history action above the selected action
			for ( int i = 0; i < _rollbackIndex; i++ )
				_terrainData.RedoLastPageAction();

			( (MainForm) _owner ).RedoTerrainAction();
			_rollbackIndex = 0;
			lblActive.Text = "Active Terrain Action";
			lstHistory.SelectedIndex = 0;
			btnRollback.Enabled = false;
			btnRestore.Enabled = false;
			btnRestoreAll.Enabled = false;
		}

		/// <summary>
		/// Clears the current list of history actions.
		/// </summary>
		public void ClearHistory()
		{
			lstHistory.Items.Clear();
			lstHistory.SelectedIndex = -1;
			btnRollback.Enabled = false;
			btnRestore.Enabled = false;
			btnRestoreAll.Enabled = false;
		}
		#endregion

		#region Additional Methods
		/// <summary>
		/// Loads the history actions into the list.
		/// </summary>
		public void LoadHistoryActions()
		{
			object[] undoHistory = _terrainData.DataHistory.PageHistoryActions.ToArray();
			object[] redoHistory = _terrainData.RedoDataHistory.PageHistoryActions.ToArray();

			_rollbackIndex = redoHistory.Length;
			lstHistory.Items.Clear();

			foreach ( string s in redoHistory )
				lstHistory.Items.Add( s );

			foreach ( string s in undoHistory )
				lstHistory.Items.Add( s );

			if ( undoHistory.Length < _terrainData.DataHistory.MaximumPageHistory )
				lstHistory.Items.Add( "New Terrain File" );
		}
		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnRestoreAll = new System.Windows.Forms.Button();
			this.btnRestore = new System.Windows.Forms.Button();
			this.btnRollback = new System.Windows.Forms.Button();
			this.lstHistory = new System.Windows.Forms.ListBox();
			this.lblActive = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lblActive);
			this.groupBox1.Controls.Add(this.btnRestoreAll);
			this.groupBox1.Controls.Add(this.btnRestore);
			this.groupBox1.Controls.Add(this.btnRollback);
			this.groupBox1.Controls.Add(this.lstHistory);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(160, 288);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "History";
			// 
			// btnRestoreAll
			// 
			this.btnRestoreAll.Enabled = false;
			this.btnRestoreAll.Location = new System.Drawing.Point(8, 256);
			this.btnRestoreAll.Name = "btnRestoreAll";
			this.btnRestoreAll.Size = new System.Drawing.Size(144, 23);
			this.btnRestoreAll.TabIndex = 3;
			this.btnRestoreAll.Text = "Restore All Actions";
			this.btnRestoreAll.Click += new System.EventHandler(this.btnRestoreAll_Click);
			// 
			// btnRestore
			// 
			this.btnRestore.Enabled = false;
			this.btnRestore.Location = new System.Drawing.Point(8, 224);
			this.btnRestore.Name = "btnRestore";
			this.btnRestore.Size = new System.Drawing.Size(144, 23);
			this.btnRestore.TabIndex = 2;
			this.btnRestore.Text = "Restore Actions";
			this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
			// 
			// btnRollback
			// 
			this.btnRollback.Enabled = false;
			this.btnRollback.Location = new System.Drawing.Point(8, 192);
			this.btnRollback.Name = "btnRollback";
			this.btnRollback.Size = new System.Drawing.Size(144, 23);
			this.btnRollback.TabIndex = 1;
			this.btnRollback.Text = "Rollback";
			this.btnRollback.Click += new System.EventHandler(this.btnRollback_Click);
			// 
			// lstHistory
			// 
			this.lstHistory.Location = new System.Drawing.Point(8, 16);
			this.lstHistory.Name = "lstHistory";
			this.lstHistory.Size = new System.Drawing.Size(144, 147);
			this.lstHistory.TabIndex = 0;
			this.lstHistory.DoubleClick += new System.EventHandler(this.lstHistory_DoubleClick);
			this.lstHistory.SelectedIndexChanged += new System.EventHandler(this.lstHistory_SelectedIndexChanged);
			// 
			// lblActive
			// 
			this.lblActive.Location = new System.Drawing.Point(8, 168);
			this.lblActive.Name = "lblActive";
			this.lblActive.Size = new System.Drawing.Size(144, 16);
			this.lblActive.TabIndex = 4;
			// 
			// HistoryManipulation
			// 
			this.Controls.Add(this.groupBox1);
			this.Name = "HistoryManipulation";
			this.Size = new System.Drawing.Size(176, 304);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
