using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Collections.Specialized;
using Microsoft.DirectX;
using Voyage.Terraingine.DataInterfacing;
using Voyage.Terraingine.DXViewport;
using Voyage.LuaNetInterface;

namespace Voyage.Terraingine
{
	/// <summary>
	/// A user control for manipulating terrain vertices.
	/// </summary>
	public class VertexManipulation : System.Windows.Forms.UserControl
	{
		#region Data Members
		private DataInterfacing.ViewportInterface	_viewport;
		private DataInterfacing.DataManipulation	_terrainData;
		private NameValueCollection	_verticesAlgorithms;
		private DXViewport.Viewport	_dx;
		private bool					_updateData;
		private TerrainViewport		_owner;

		private System.Windows.Forms.GroupBox grpVertexName;
		private System.Windows.Forms.Button btnVertices_Name;
		private System.Windows.Forms.TextBox txtVertices_Name;
		private System.Windows.Forms.GroupBox grpVertexDimensions;
		private System.Windows.Forms.CheckBox chkVertices_GridSize;
		private System.Windows.Forms.NumericUpDown numVertices_RowDistance;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numVertices_ColumnDistance;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox chkVertices_GridDimensions;
		private System.Windows.Forms.NumericUpDown numVertices_Columns;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numVertices_Rows;
		private System.Windows.Forms.GroupBox grpVertexCreation;
		private System.Windows.Forms.Button btnVertices_Create;
		private System.Windows.Forms.GroupBox grpVertexSelection;
		private System.Windows.Forms.CheckBox chkFalloff;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numSoft;
		private System.Windows.Forms.RadioButton radSoft;
		private System.Windows.Forms.Button btnVertexSelection;
		private System.Windows.Forms.RadioButton radSingle;
		private System.Windows.Forms.GroupBox grpVertexAlgorithms;
		private System.Windows.Forms.Button btnVertices_LoadAlgorithm;
		private System.Windows.Forms.Button btnVertices_RunAlgorithm;
		private System.Windows.Forms.ListBox lstVertices_Algorithms;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown numMaxHeight;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numVertexHeight;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets whether the control allows data updates.
		/// </summary>
		public bool EnableDataUpdates
		{
			get { return _updateData; }
			set { _updateData = value; }
		}

		/// <summary>
		/// Gets the vertex movement button.
		/// </summary>
		public Button EnableVertexMovement
		{
			get { return btnVertexSelection; }
		}

		/// <summary>
		/// Sets the soft selection value shown in the display.
		/// </summary>
		public float SoftSelectionDistance
		{
			set { numSoft.Value = Convert.ToDecimal( value ); }
		}
		#endregion

		#region Basic Data Methods
		/// <summary>
		/// Creates a vertex manipulation user control.
		/// </summary>
		public VertexManipulation()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
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

			// Initialize the control-specific data
			_verticesAlgorithms = new NameValueCollection();
			_updateData = true;

			// Register tooltips
			ToolTip t = new ToolTip();

			// Terrain Creation group tooltips
			t.SetToolTip( btnVertices_Create, "Create a piece of terrain (Ctrl+N)" );

			// Terrain Name group tooltips
			t.SetToolTip( btnVertices_Name, "Change the name of the terrain" );

			// Vertex Selection group tooltips
			t.SetToolTip( btnVertexSelection, "Enable or disable vertex movement" );
			t.SetToolTip( radSingle, "Move vertices without blending" );
			t.SetToolTip( radSoft, "Move vertices with blending" );
			t.SetToolTip( chkFalloff, "Blend nearby vertices with falloff weighting" );
			
			// Algorithms group tooltips
			t.SetToolTip( btnVertices_RunAlgorithm, "Run the selected vertex manipulation algorithm" );
			t.SetToolTip( btnVertices_LoadAlgorithm, "Load a new vertex manipulation algorithm" );

			// Modify Terrain Dimensions group tooltips
			t.SetToolTip( chkVertices_GridDimensions, "Keep rows and columns the same value" );
			t.SetToolTip( chkVertices_GridSize, "Keep height and width the same value" );
		}

		/// <summary>
		/// Sets all control displays to their default values.
		/// </summary>
		public void RestoreDefaults()
		{
			// Begin restoring defaults
			_updateData = false;

			// Restore defaults to Terrain Creation group
			grpVertexCreation.Enabled = true;

			// Restore defaults to Vertex Selection group
			grpVertexSelection.Enabled = false;
			radSingle.Checked = true;
			numSoft.Value = Convert.ToDecimal( 0.2f );
			chkFalloff.Checked = true;

			if ( btnVertexSelection.BackColor == Color.FromKnownColor( KnownColor.ControlLight ) )
				btnVertexSelection_Click( this, new System.EventArgs() );

			// Restore defaults to Vertex Algorithms group
			grpVertexAlgorithms.Enabled = false;
			lstVertices_Algorithms.SelectedIndex = -1;
			btnVertices_RunAlgorithm.Enabled = false;

			// Restore defaults to Vertex Dimensions group
			grpVertexDimensions.Enabled = false;
			numVertices_Rows.Value = 5;
			numVertices_Columns.Value = 5;
			chkVertices_GridDimensions.Checked = true;
			numVertices_ColumnDistance.Value = 1;
			numVertices_RowDistance.Value = 1;
			chkVertices_GridSize.Checked = true;
			numMaxHeight.Value = 1;

			// Restore defaults to Vertex Name group
			grpVertexName.Enabled = false;
			txtVertices_Name.Text = "";

			// Finished restoring defaults
			_updateData = true;
		}
		#endregion

		#region Event Methods
		/// <summary>
		/// Creates a piece of terrain.
		/// </summary>
		private void btnVertices_Create_Click(object sender, System.EventArgs e)
		{
			CreateTerrainDialog();
		}

		/// <summary>
		/// Updates the name of the current TerrainPage.
		/// </summary>
		private void txtVertices_Name_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if ( e.KeyCode == Keys.Enter )
				UpdateTerrainName();
		}

		/// <summary>
		/// Updates the name of the current TerrainPage.
		/// </summary>
		private void btnVertices_Name_Click(object sender, System.EventArgs e)
		{
			UpdateTerrainName();
		}
		
		/// <summary>
		/// Enables or disables vertex selection.
		/// </summary>
		private void btnVertexSelection_Click(object sender, System.EventArgs e)
		{
			if ( btnVertexSelection.BackColor == Color.FromKnownColor( KnownColor.Control ) )
				btnVertexSelection_Click( true );
			else
				btnVertexSelection_Click( false );
		}

		/// <summary>
		/// Disables soft selection.
		/// </summary>
		private void radSingle_CheckedChanged(object sender, System.EventArgs e)
		{
			if ( radSingle.Checked )
				EnableSoftSelection( false );
		}

		/// <summary>
		/// Enables soft selection.
		/// </summary>
		private void radSoft_CheckedChanged(object sender, System.EventArgs e)
		{
			if ( radSoft.Checked )
				EnableSoftSelection( true );
		}

		/// <summary>
		/// Updates the soft selection distance.
		/// </summary>
		private void numSoft_ValueChanged(object sender, System.EventArgs e)
		{
			numSoft_ValueChanged();
		}

		/// <summary>
		/// Updates whether soft selection uses falloff.
		/// </summary>
		private void chkFalloff_CheckedChanged(object sender, System.EventArgs e)
		{
			chkFalloff_CheckedChanged();
		}

		/// <summary>
		/// Changes the height of the selected vertices.
		/// </summary>
		private void numVertexHeight_ValueChanged(object sender, System.EventArgs e)
		{
			numVertexHeight_ValueChanged();
		}

		/// <summary>
		/// Changes the height of the selected vertices.
		/// </summary>
		private void numVertexHeight_Leave(object sender, System.EventArgs e)
		{
			numVertexHeight_ValueChanged();
		}
		
		/// <summary>
		/// Enables the vertex manipulation "Run Algorithm" button if an item is selected.
		/// </summary>
		private void lstVertices_Algorithms_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			lstVertices_Algorithms_SelectedIndexChanged();
		}

		/// <summary>
		/// Runs the vertex manipulation "Run Algorithm" button if an item is being clicked.
		/// </summary>
		private void lstVertices_Algorithms_DoubleClick(object sender, System.EventArgs e)
		{
			lstVertices_Algorithms_DoubleClick();
		}

		/// <summary>
		/// Runs the selected vertex manipulation algorithm.
		/// </summary>
		private void btnVertices_RunAlgorithm_Click(object sender, System.EventArgs e)
		{
			btnVertices_RunAlgorithm_Click();
		}

		/// <summary>
		/// Loads a new vertex manipulation plug-in.
		/// </summary>
		private void btnVertices_LoadAlgorithm_Click(object sender, System.EventArgs e)
		{
			LoadAlgorithm();
		}
		
		/// <summary>
		/// Changes the number of rows in the TerrainPatch.
		/// </summary>
		private void numVertices_Rows_Leave(object sender, System.EventArgs e)
		{
			if ( _updateData )
				ChangeGridSize();
		}

		/// <summary>
		/// Changes the number of rows in the TerrainPatch.
		/// </summary>
		private void numVertices_Rows_ValueChanged(object sender, System.EventArgs e)
		{
			numVertices_Rows_ValueChanged();
		}

		/// <summary>
		/// Changes the number of columns in the TerrainPatch.
		/// </summary>
		private void numVertices_Columns_Leave(object sender, System.EventArgs e)
		{
			if ( _updateData )
				ChangeGridSize();
		}

		/// <summary>
		/// Changes the number of columns in the TerrainPatch.
		/// </summary>
		private void numVertices_Columns_ValueChanged(object sender, System.EventArgs e)
		{
			numVertices_Columns_ValueChanged();
		}

		/// <summary>
		/// Changes whether the number of rows and the number of columns in the TerrainPatch must be equal.
		/// </summary>
		private void chkVertices_GridDimensions_CheckedChanged(object sender, System.EventArgs e)
		{
			chkVertices_GridDimensions_CheckedChanged();
		}

		/// <summary>
		/// Changes the distance between columns in the TerrainPatch.
		/// </summary>
		private void numVertices_ColumnDistance_ValueChanged(object sender, System.EventArgs e)
		{
			if ( _updateData )
				ChangeGridDimensions();
		}

		/// <summary>
		/// Changes the distance between columns in the TerrainPatch.
		/// </summary>
		private void numVertices_ColumnDistance_Leave(object sender, System.EventArgs e)
		{
			numVertices_ColumnDistance_Leave();
		}

		/// <summary>
		/// Changes the distance between columns in the TerrainPatch.
		/// </summary>
		private void numVertices_RowDistance_ValueChanged(object sender, System.EventArgs e)
		{
			if ( _updateData )
				ChangeGridDimensions();
		}

		/// <summary>
		/// Changes the distance between columns in the TerrainPatch.
		/// </summary>
		private void numVertices_RowDistance_Leave(object sender, System.EventArgs e)
		{
			numVertices_RowDistance_Leave();
		}

		/// <summary>
		/// Changes whether the distance between rows and the distance between columns
		/// in the TerrainPatch must be equal.
		/// </summary>
		private void chkVertices_GridSize_CheckedChanged(object sender, System.EventArgs e)
		{
			chkVertices_GridSize_CheckedChanged();
		}

		/// <summary>
		/// Changes the maximum vertex height of the TerrainPatch.
		/// </summary>
		private void numMaxHeight_ValueChanged(object sender, System.EventArgs e)
		{
			numMaxHeight_ValueChanged();
		}

		/// <summary>
		/// Changes the maximum vertex height of the TerrainPatch.
		/// </summary>
		private void numMaxHeight_Leave(object sender, System.EventArgs e)
		{
			numMaxHeight_ValueChanged();
		}
		#endregion

		#region Terrain Creation
		/// <summary>
		/// Creates a piece of terrain using the TerrainCreation dialog.
		/// </summary>
		public void CreateTerrainDialog()
		{
			TerrainCreation create = new TerrainCreation();
			Point parLoc = _owner.GetFormCenter();
			Point p = new Point( parLoc.X - create.Width / 2, parLoc.Y - create.Height / 2 );

			create.Location = p;
			grpVertexCreation.Enabled = false;
			create.Show();

			while ( create.Created )
			{
				_viewport.PreRender();

				if ( _viewport.BeginRender() )
				{
					_viewport.RenderSceneElements();
					_viewport.EndRender();
				}

				create.OnApplicationIdle( this, new EventArgs() );
				Application.DoEvents();
			}

			if ( create.Accepted )
				( (MainForm) _owner ).LoadTerrain( create.MainViewport.TerrainData.TerrainPage );
			else
				grpVertexCreation.Enabled = true;
		}
		
		/// <summary>
		/// Loads a TerrainPage into the program.
		/// </summary>
		/// <param name="name">The name of the TerrainPage.</param>
		public void LoadTerrain( string name )
		{
			RestoreDefaults();
			txtVertices_Name.Text = name;
			EnableTerrainEditing( true );
		}
		#endregion

		#region Terrain Page Name
		/// <summary>
		/// Updates the name of the current TerrainPage.
		/// </summary>
		public void UpdateTerrainName()
		{
			if ( _updateData )
				_terrainData.TerrainPage.Name = txtVertices_Name.Text;
		}
		#endregion

		#region Vertex Selection
		/// <summary>
		/// Enables or disables vertex selection.
		/// </summary>
		/// <param name="enable">Whether to enable vertex selection.</param>
		public void btnVertexSelection_Click( bool enable )
		{
			if ( enable )
			{
				btnVertexSelection.BackColor = Color.FromKnownColor( KnownColor.ControlLight );
				_terrainData.EnableVertexMovement = true;
				_terrainData.BufferObjects.ShowSelectedVertices = true;
				_dx.Camera.CurrentMovement = QuaternionCamera.MovementType.None;

				radSingle.Enabled = true;
				radSoft.Enabled = true;
				numSoft.Enabled = true;
				chkFalloff.Enabled = true;
			}
			else
			{
				btnVertexSelection.BackColor = Color.FromKnownColor( KnownColor.Control );
				_terrainData.EnableVertexMovement = false;
				_terrainData.BufferObjects.ShowSelectedVertices = false;

				radSingle.Enabled = false;
				radSoft.Enabled = false;
				numSoft.Enabled = false;
				chkFalloff.Enabled = false;
			}
		}

		/// <summary>
		/// Updates the soft selection distance.
		/// </summary>
		public void numSoft_ValueChanged()
		{
			if ( _updateData )
			{
				float result = ( float ) numSoft.Value;

				_terrainData.SoftSelectionDistanceSquared = result * result;
				_terrainData.TerrainPage.TerrainPatch.RefreshVertices = true;
			}
		}

		/// <summary>
		/// Updates whether soft selection uses falloff.
		/// </summary>
		public void chkFalloff_CheckedChanged()
		{
			if ( _updateData )
			{
				_terrainData.Falloff = chkFalloff.Checked;
				_terrainData.TerrainPage.TerrainPatch.RefreshVertices = true;
			}
		}

		/// <summary>
		/// Changes the height of the selected vertices.
		/// </summary>
		public void numVertexHeight_ValueChanged()
		{
			if ( _updateData )
			{
				_terrainData.SetVertexHeight( (float) numVertexHeight.Value, false );
			}
		}

		/// <summary>
		/// Enables or disables soft selection.
		/// </summary>
		/// <param name="enable">Whether to enable soft selection.</param>
		[LuaFunctionAttribute( "EnableSoftSelection", "Enables or disables soft selection.",
			 "Whether to enable soft selection." )]
		public void EnableSoftSelection( bool enable )
		{
			if ( _updateData )
			{
				_terrainData.SoftSelection = enable;
				_terrainData.TerrainPage.TerrainPatch.RefreshVertices = true;
			}
		}

		/// <summary>
		/// Enables or disables vertex selection.
		/// </summary>
		/// <param name="enable">Whether to enable vertex selection.</param>
		[LuaFunctionAttribute( "EnableVertexSelection", "Enables or disables vertex selection.",
			"Whether to enable vertex selection." )]
		public void EnableVertexSelection( bool enable )
		{
			if ( enable && btnVertexSelection.BackColor == Color.FromKnownColor( KnownColor.Control ) )
				btnVertexSelection_Click( this, new System.EventArgs() );
			else if ( !enable && btnVertexSelection.BackColor == Color.FromKnownColor( KnownColor.ControlLight ) )
				btnVertexSelection_Click( this, new System.EventArgs() );
		}
		#endregion

		#region Algorithms
		/// <summary>
		/// Enables the vertex manipulation "Run Algorithm" button if an item is selected.
		/// </summary>
		public void lstVertices_Algorithms_SelectedIndexChanged()
		{
			if ( lstVertices_Algorithms.SelectedIndex > -1 )
				btnVertices_RunAlgorithm.Enabled = true;
			else
				btnVertices_RunAlgorithm.Enabled = false;
		}

		/// <summary>
		/// Runs the vertex manipulation "Run Algorithm" button if an item is being clicked.
		/// </summary>
		public void lstVertices_Algorithms_DoubleClick()
		{
			if ( lstVertices_Algorithms.SelectedIndex > -1 )
			{
				btnVertices_RunAlgorithm.Enabled = true;
				btnVertices_RunAlgorithm_Click();
			}
			else
				btnVertices_RunAlgorithm.Enabled = false;
		}

		/// <summary>
		/// Runs the selected vertex manipulation algorithm.
		/// </summary>
		public void btnVertices_RunAlgorithm_Click()
		{
			if ( lstVertices_Algorithms.SelectedIndex > -1 )
				_terrainData.RunPlugIn( lstVertices_Algorithms.SelectedIndex,
					DataInterfacing.PlugIns.PlugInTypes.Vertices, _owner );
		}

		/// <summary>
		/// Loads the list of vertex manipulation algorithms.
		/// </summary>
		public void LoadAlgorithms()
		{
			_verticesAlgorithms.Clear();
			lstVertices_Algorithms.Items.Clear();

			for ( int i = 0; i < _terrainData.PlugIns.VertexPlugIns.Count; i++ )
				_verticesAlgorithms.Add( i.ToString(),
					( (PlugIn) _terrainData.PlugIns.VertexPlugIns[i] ).GetName() );

			foreach ( string key in _verticesAlgorithms.Keys )
				lstVertices_Algorithms.Items.Add( _verticesAlgorithms.GetValues( key )[0] );

			lstVertices_Algorithms.SelectedIndex = -1;
		}

		/// <summary>
		/// Runs the vertex manipulation plug-in specified.
		/// </summary>
		/// <param name="name">The name of the plug-in to run.</param>
		[LuaFunctionAttribute( "RunPlugIn", "Runs the vertex manipulation plug-in specified.",
			"The name of the plug-in to run." )]
		public void RunAlgorithm( string name )
		{
			bool found = false;
			int count = 0;

			while ( !found && lstVertices_Algorithms.Items[count].ToString() != name )
				count++;

			if ( found )
				_terrainData.RunPlugIn( count, DataInterfacing.PlugIns.PlugInTypes.Vertices, _owner );
			else
				MessageBox.Show( "Vertex manipulation plug-in " + name + " could not be found! " );
		}

		/// <summary>
		/// Runs the vertex manipulation plug-in specified in automatic mode.
		/// </summary>
		/// <param name="name">The name of the plug-in to run.</param>
		/// <param name="filename">The name of the file to load into the plug-in.</param>
		[LuaFunctionAttribute( "RunAutoPlugIn",
			"Runs the vertex manipulation plug-in specified in automatic mode.",
			"The name of the plug-in to run.", "The name of the file to load into the plug-in." )]
		public void RunAutoAlgorithm( string name, string filename )
		{
			bool found = false;
			int count = 0;

			while ( !found && lstVertices_Algorithms.Items[count].ToString() != name )
				count++;

			if ( found )
				_terrainData.RunPlugInAuto( count, DataInterfacing.PlugIns.PlugInTypes.Vertices,
					_owner, filename );
			else
				MessageBox.Show( "Vertex manipulation plug-in " + name + " could not be found! " );
		}

		/// <summary>
		/// Loads a new vertex manipulation plug-in.
		/// </summary>
		[LuaFunctionAttribute( "LoadPlugIn", "Loads a new vertex manipulation plug-in." )]
		public void LoadAlgorithm()
		{
			_terrainData.LoadPlugIn( DataInterfacing.PlugIns.PlugInTypes.Vertices );
			LoadAlgorithms();
		}
		#endregion

		#region Modify Terrain Dimensions
		/// <summary>
		/// Changes the number of rows in the TerrainPatch.
		/// </summary>
		public void numVertices_Rows_ValueChanged()
		{
			if ( _updateData )
			{
				if ( chkVertices_GridDimensions.Checked )
				{
					_updateData = false;
					numVertices_Columns.Value = numVertices_Rows.Value;
					_updateData = true;
				}

				ChangeGridSize();
			}
		}

		/// <summary>
		/// Changes the number of columns in the TerrainPatch.
		/// </summary>
		public void numVertices_Columns_ValueChanged()
		{
			if ( _updateData )
			{
				if ( chkVertices_GridDimensions.Checked )
				{
					_updateData = false;
					numVertices_Rows.Value = numVertices_Columns.Value;
					_updateData = true;
				}

				ChangeGridSize();
			}
		}

		/// <summary>
		/// Changes whether the number of rows and the number of columns in the TerrainPatch must be equal.
		/// </summary>
		public void chkVertices_GridDimensions_CheckedChanged()
		{
			if ( _updateData && chkVertices_GridDimensions.Checked )
			{
				_updateData = false;
				numVertices_Columns.Value = numVertices_Rows.Value;
				_updateData = true;
				ChangeGridSize();
			}
		}

		/// <summary>
		/// Changes the distance between columns in the TerrainPatch.
		/// </summary>
		public void numVertices_ColumnDistance_Leave()
		{
			if ( _updateData )
			{
				if ( chkVertices_GridSize.Checked )
				{
					_updateData = false;
					numVertices_RowDistance.Value = numVertices_ColumnDistance.Value;
					_updateData = true;
				}

				ChangeGridDimensions();
			}
		}

		/// <summary>
		/// Changes the distance between columns in the TerrainPatch.
		/// </summary>
		public void numVertices_RowDistance_Leave()
		{
			if ( _updateData )
			{
				if ( chkVertices_GridSize.Checked )
				{
					_updateData = false;
					numVertices_ColumnDistance.Value = numVertices_RowDistance.Value;
					_updateData = true;
				}

				ChangeGridDimensions();
			}
		}

		/// <summary>
		/// Changes whether the distance between rows and the distance between columns
		/// in the TerrainPatch must be equal.
		/// </summary>
		public void chkVertices_GridSize_CheckedChanged()
		{
			if ( _updateData && chkVertices_GridSize.Checked )
			{
				_updateData = false;
				numVertices_ColumnDistance.Value = numVertices_RowDistance.Value;
				_updateData = true;
				ChangeGridDimensions();
			}
		}

		/// <summary>
		/// Changes the maximum vertex height of the TerrainPatch.
		/// </summary>
		public void numMaxHeight_ValueChanged()
		{
			if ( _updateData )
			{
				_terrainData.UpdateMaximumTerrainHeight( (float) numMaxHeight.Value );
				numVertexHeight.Maximum = numMaxHeight.Value;
			}
		}

		/// <summary>
		/// Activates the Maximum Vertex Height numeric.
		/// </summary>
		public void UpdateMaximumVertexHeight()
		{
			Vector3[] positions = _terrainData.TerrainPage.TerrainPatch.GetSelectedVertexPositions();

			if ( positions.Length > 0 )
			{
				numVertexHeight.Enabled = true;
				numVertexHeight.Value = Convert.ToDecimal( positions[0].Y );
			}
			else
			{
				_updateData = false;
				numVertexHeight.Value = 0;
				numVertexHeight.Enabled = false;
				_updateData = true;
			}
		}

		/// <summary>
		/// Updates the maximum allowed vertex height in a TerrainPage.
		/// </summary>
		/// <param name="height">The maximum allowed vertex height.</param>
		[LuaFunctionAttribute( "SetMaximumVertexHeight",
			 "Updates the maximum allowed vertex height in a TerrainPage.",
			 "The maximum allowed vertex height." )]
		public void UpdateMaximumVertexHeight( float height )
		{
			_terrainData.UpdateMaximumTerrainHeight( height );
			UpdateMaximumVertexHeight();
		}

		/// <summary>
		/// Changes the size of the TerrainPatch grid.
		/// </summary>
		public void ChangeGridSize()
		{
			if ( _terrainData.TerrainPage != null )
			{
				if ( Convert.ToInt32( numVertices_Rows.Value ) != _terrainData.TerrainPage.TerrainPatch.Rows ||
					Convert.ToInt32( numVertices_Columns.Value ) != _terrainData.TerrainPage.TerrainPatch.Columns )
				{
					string message = "By changing the number of vertex rows or columns, " +
						"all vertices will be reset to their default positions.\n\n" +
						"This means that all modifications of vertex positions will be overwritten.";
					DialogResult result = MessageBox.Show( message,
						"Data Will Be Lost!",
						MessageBoxButtons.OKCancel, MessageBoxIcon.Warning );

					if ( result == DialogResult.OK )
						_terrainData.UpdateTerrainSize( Convert.ToInt32( numVertices_Rows.Value ),
							Convert.ToInt32( numVertices_Columns.Value ) );
					else if ( result == DialogResult.Cancel )
					{
						_updateData = false;
						numVertices_Rows.Value = _terrainData.TerrainPage.TerrainPatch.Rows;
						numVertices_Columns.Value = _terrainData.TerrainPage.TerrainPatch.Columns;
						_updateData = true;
					}
				}
			}
		}

		/// <summary>
		/// Changes the distance dimensions of the TerrainPatch.
		/// </summary>
		public void ChangeGridDimensions()
		{
			if ( _terrainData.TerrainPage != null )
			{
				if ( _terrainData.TerrainPage.TerrainPatch.Height != (float) numVertices_RowDistance.Value ||
					_terrainData.TerrainPage.TerrainPatch.Width != (float) numVertices_ColumnDistance.Value )
				{
					_terrainData.UpdateTerrainDimensions( ( float ) numVertices_RowDistance.Value,
						( float ) numVertices_ColumnDistance.Value );

					if ( numVertices_RowDistance.Value > numVertices_ColumnDistance.Value )
						_dx.Camera.FollowDistance = (float) numVertices_RowDistance.Value * 2f;
					else
						_dx.Camera.FollowDistance = (float) numVertices_ColumnDistance.Value * 2f;

					_viewport.InitializeCamera();
				}
			}
		}
		#endregion

		#region Other Terrain Functions
		/// <summary>
		/// Enables or disables the vertex editing tab group controls.
		/// </summary>
		/// <param name="enable">Whether to enable the tab group controls.</param>
		[LuaFunctionAttribute( "EnableTerrainEditing",
			"Enables or disables the vertex editing tab group controls.",
			"Whether to enable the tab group controls." )]
		public void EnableTerrainEditing( bool enable )
		{
			if ( !enable && btnVertexSelection.BackColor == Color.FromKnownColor( KnownColor.ControlLight ) )
				btnVertexSelection_Click( this, new System.EventArgs() );

			grpVertexCreation.Enabled = !enable;
			grpVertexSelection.Enabled = enable;
			grpVertexAlgorithms.Enabled = enable;
			grpVertexDimensions.Enabled = enable;
			grpVertexName.Enabled = enable;

			if ( _terrainData.TerrainPage != null )
			{
				_updateData = false;
				numVertices_Rows.Value = Convert.ToDecimal( _terrainData.TerrainPage.TerrainPatch.Rows );
				numVertices_Columns.Value = Convert.ToDecimal( _terrainData.TerrainPage.TerrainPatch.Columns );
				numVertices_RowDistance.Value =
					Convert.ToDecimal( _terrainData.TerrainPage.TerrainPatch.Height );
				numVertices_ColumnDistance.Value =
					Convert.ToDecimal( _terrainData.TerrainPage.TerrainPatch.Width );
				numMaxHeight.Value = Convert.ToDecimal( _terrainData.TerrainPage.MaximumVertexHeight );
				_updateData = true;

				if ( numVertices_Rows.Value == numVertices_Columns.Value )
					chkVertices_GridDimensions.Checked = true;
				else
					chkVertices_GridDimensions.Checked = false;

				if ( numVertices_RowDistance.Value == numVertices_ColumnDistance.Value )
					chkVertices_GridSize.Checked = true;
				else
					chkVertices_GridSize.Checked = false;

				if ( _terrainData.TerrainPage.TerrainPatch.Height >
					_terrainData.TerrainPage.TerrainPatch.Width )
					_dx.Camera.FollowDistance = _terrainData.TerrainPage.TerrainPatch.Height * 2.0f;
				else
					_dx.Camera.FollowDistance = _terrainData.TerrainPage.TerrainPatch.Width * 2.0f;
			}
		}

		/// <summary>
		/// Updates the control states.
		/// </summary>
		public void UpdateStates()
		{
			float val;

			_updateData = false;

			if ( _terrainData.TerrainPage != null )
			{
				// Update terrain editing
				EnableTerrainEditing( true );

				// Update terrain name
				txtVertices_Name.Text = _terrainData.TerrainPage.Name;

				// Enable soft selection
				if ( _terrainData.SoftSelection )
				{
					radSingle.Checked = false;
					radSoft.Checked = true;
				}
				else
				{
					radSingle.Checked = true;
					radSoft.Checked = false;
				}

				// Update soft selection distance
				val = (float) Math.Sqrt( (double) _terrainData.SoftSelectionDistanceSquared );
				numSoft.Value = Convert.ToDecimal( val );

				// Enable falloff
				if ( _terrainData.Falloff )
					chkFalloff.Checked = true;
				else
					chkFalloff.Checked = false;

				// Update grid size
				numVertices_Rows.Value = _terrainData.TerrainPage.TerrainPatch.Rows;
				numVertices_Columns.Value = _terrainData.TerrainPage.TerrainPatch.Columns;

				// Update grid dimensions
				numVertices_RowDistance.Value =
					Convert.ToDecimal( _terrainData.TerrainPage.TerrainPatch.Height );
				numVertices_ColumnDistance.Value =
					Convert.ToDecimal( _terrainData.TerrainPage.TerrainPatch.Width );

				// Update maximum vertex height
				numMaxHeight.Value = Convert.ToDecimal( _terrainData.TerrainPage.MaximumVertexHeight );
			}
			else
			{
				// Update terrain editing
				EnableTerrainEditing( false );

				// Update terrain name
				txtVertices_Name.Text = "";
			}

			_updateData = true;
		}
		#endregion

		#region Lua-Specific Functions
		/// <summary>
		/// Copies the currently selected vertex data (copies first selected vertex only).
		/// </summary>
		[LuaFunctionAttribute( "CopyVertex",
			 "Copies the currently selected vertex data (copies first selected vertex only)." )]
		public void CopyVertex()
		{
			_terrainData.CopySelectedVertexPosition();
		}

		/// <summary>
		/// Pastes copied vertex data onto the currently selected vertex (pastes one vertex only).
		/// </summary>
		[LuaFunctionAttribute( "PasteVertex",
			 "Pastes copied vertex data onto the currently selected vertex (pastes one vertex only)." )]
		public void PasteVertex()
		{
			_terrainData.SetVertexHeight( _terrainData.VertexPositionCopy, true );
		}

		/// <summary>
		/// Selects all vertices in the TerrainPage.
		/// </summary>
		[LuaFunctionAttribute( "SelectAllVertices", "Selects all vertices in the TerrainPage." )]
		public void SelectAllVertices()
		{
			bool[] selected = new bool[_terrainData.TerrainPage.TerrainPatch.NumVertices];

			for ( int i = 0; i < selected.Length; i++ )
				selected[i] = true;

			_terrainData.TerrainPage.TerrainPatch.SelectedVertices = selected;
			_terrainData.TerrainPage.TerrainPatch.AreVerticesSelected = true;
		}

		/// <summary>
		/// Selects the non-selected vertices and un-selects currently selected vertices.
		/// </summary>
		[LuaFunctionAttribute( "SelectInverseVertices",
			 "Selects the non-selected vertices and un-selects currently selected vertices." )]
		public void SelectInverseVertices()
		{
			bool[] selected = _terrainData.TerrainPage.TerrainPatch.SelectedVertices;

			for ( int i = 0; i < selected.Length; i++ )
				selected[i] = !selected[i];

			_terrainData.TerrainPage.TerrainPatch.SelectedVertices = selected;
			_terrainData.TerrainPage.TerrainPatch.AreVerticesSelected = true;
		}

		/// <summary>
		/// Selects the specified vertex.
		/// </summary>
		/// <param name="vertex">Index of the vertex.</param>
		/// <param name="endSelection">Whether to enable clearing selected vertices if one is not selected.</param>
		[LuaFunctionAttribute( "SelectVertexByIndex", "Selects the specified vertex.",
			 "Index of the vertex.", "Whether to enable clearing selected vertices if one is not selected." )]
		public void SelectVertex( int vertex, bool endSelection )
		{
			_terrainData.SelectVertex( vertex, endSelection );
		}

		/// <summary>
		/// Selects the specified vertex.
		/// </summary>
		/// <param name="row">Row of the vertex.</param>
		/// <param name="column">Column of the vertex.</param>
		/// <param name="endSelection">Whether to enable clearing selected vertices if one is not selected.</param>
		[LuaFunctionAttribute( "SelectVertex", "Selects the specified vertex.",
			 "Row of the vertex.", "Column of the vertex.",
			 "Whether to enable clearing selected vertices if one is not selected." )]
		public void SelectVertex( int row, int column, bool endSelection )
		{
			SelectVertex( _terrainData.TerrainPage.TerrainPatch.Rows * row + column, endSelection );
		}

		/// <summary>
		/// Moves the selected vertices.
		/// </summary>
		/// <param name="distChange">The change in distance to move selected vertices.</param>
		[LuaFunctionAttribute( "MoveVertices", "Moves the selected vertices.",
			 "The change in distance to move selected vertices." )]
		public void MoveVertices( float distChange )
		{
			_terrainData.MoveVertices( false, distChange );
			_terrainData.MoveVertices( true, 0.0f );
		}

		/// <summary>
		/// Sets the soft selection distance for terrain movement.
		/// </summary>
		/// <param name="distance">The soft selection distance.</param>
		[LuaFunctionAttribute( "SetSoftSelectionDistance", "Sets the soft selection distance.",
			 "The soft selection distance." )]
		public void SetSoftSelectionDistance( float distance )
		{
			_updateData = false;
			numSoft.Value = Convert.ToDecimal( distance );
			_updateData = true;

			_terrainData.SoftSelectionDistanceSquared = distance * distance;
			_terrainData.TerrainPage.TerrainPatch.RefreshVertices = true;
		}

		/// <summary>
		/// Enables or disables falloff.
		/// </summary>
		/// <param name="enable">Whether to enable or disable falloff.</param>
		[LuaFunctionAttribute( "EnableFalloff", "Enables or disables falloff.",
			 "Whether to enable or disable falloff." )]
		public void EnableFalloff( bool enable )
		{
			_updateData = false;
			chkFalloff.Checked = enable;
			_updateData = true;
			
			_terrainData.Falloff = chkFalloff.Checked;
			_terrainData.TerrainPage.TerrainPatch.RefreshVertices = true;
		}
		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.grpVertexName = new System.Windows.Forms.GroupBox();
			this.btnVertices_Name = new System.Windows.Forms.Button();
			this.txtVertices_Name = new System.Windows.Forms.TextBox();
			this.grpVertexDimensions = new System.Windows.Forms.GroupBox();
			this.numMaxHeight = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.chkVertices_GridSize = new System.Windows.Forms.CheckBox();
			this.numVertices_RowDistance = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.numVertices_ColumnDistance = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.chkVertices_GridDimensions = new System.Windows.Forms.CheckBox();
			this.numVertices_Columns = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.numVertices_Rows = new System.Windows.Forms.NumericUpDown();
			this.grpVertexCreation = new System.Windows.Forms.GroupBox();
			this.btnVertices_Create = new System.Windows.Forms.Button();
			this.grpVertexSelection = new System.Windows.Forms.GroupBox();
			this.chkFalloff = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.numSoft = new System.Windows.Forms.NumericUpDown();
			this.radSoft = new System.Windows.Forms.RadioButton();
			this.btnVertexSelection = new System.Windows.Forms.Button();
			this.radSingle = new System.Windows.Forms.RadioButton();
			this.grpVertexAlgorithms = new System.Windows.Forms.GroupBox();
			this.btnVertices_LoadAlgorithm = new System.Windows.Forms.Button();
			this.btnVertices_RunAlgorithm = new System.Windows.Forms.Button();
			this.lstVertices_Algorithms = new System.Windows.Forms.ListBox();
			this.label7 = new System.Windows.Forms.Label();
			this.numVertexHeight = new System.Windows.Forms.NumericUpDown();
			this.grpVertexName.SuspendLayout();
			this.grpVertexDimensions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numMaxHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numVertices_RowDistance)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numVertices_ColumnDistance)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numVertices_Columns)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numVertices_Rows)).BeginInit();
			this.grpVertexCreation.SuspendLayout();
			this.grpVertexSelection.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numSoft)).BeginInit();
			this.grpVertexAlgorithms.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numVertexHeight)).BeginInit();
			this.SuspendLayout();
			// 
			// grpVertexName
			// 
			this.grpVertexName.Controls.Add(this.btnVertices_Name);
			this.grpVertexName.Controls.Add(this.txtVertices_Name);
			this.grpVertexName.Enabled = false;
			this.grpVertexName.Location = new System.Drawing.Point(8, 64);
			this.grpVertexName.Name = "grpVertexName";
			this.grpVertexName.Size = new System.Drawing.Size(160, 72);
			this.grpVertexName.TabIndex = 10;
			this.grpVertexName.TabStop = false;
			this.grpVertexName.Text = "Terrain Page Name:";
			// 
			// btnVertices_Name
			// 
			this.btnVertices_Name.Location = new System.Drawing.Point(8, 40);
			this.btnVertices_Name.Name = "btnVertices_Name";
			this.btnVertices_Name.Size = new System.Drawing.Size(144, 23);
			this.btnVertices_Name.TabIndex = 1;
			this.btnVertices_Name.Text = "Update Terrain Name";
			this.btnVertices_Name.Click += new System.EventHandler(this.btnVertices_Name_Click);
			// 
			// txtVertices_Name
			// 
			this.txtVertices_Name.Location = new System.Drawing.Point(8, 16);
			this.txtVertices_Name.Name = "txtVertices_Name";
			this.txtVertices_Name.Size = new System.Drawing.Size(144, 20);
			this.txtVertices_Name.TabIndex = 0;
			this.txtVertices_Name.Text = "";
			this.txtVertices_Name.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtVertices_Name_KeyUp);
			// 
			// grpVertexDimensions
			// 
			this.grpVertexDimensions.Controls.Add(this.numMaxHeight);
			this.grpVertexDimensions.Controls.Add(this.label6);
			this.grpVertexDimensions.Controls.Add(this.chkVertices_GridSize);
			this.grpVertexDimensions.Controls.Add(this.numVertices_RowDistance);
			this.grpVertexDimensions.Controls.Add(this.label4);
			this.grpVertexDimensions.Controls.Add(this.numVertices_ColumnDistance);
			this.grpVertexDimensions.Controls.Add(this.label5);
			this.grpVertexDimensions.Controls.Add(this.chkVertices_GridDimensions);
			this.grpVertexDimensions.Controls.Add(this.numVertices_Columns);
			this.grpVertexDimensions.Controls.Add(this.label3);
			this.grpVertexDimensions.Controls.Add(this.label2);
			this.grpVertexDimensions.Controls.Add(this.numVertices_Rows);
			this.grpVertexDimensions.Enabled = false;
			this.grpVertexDimensions.Location = new System.Drawing.Point(8, 512);
			this.grpVertexDimensions.Name = "grpVertexDimensions";
			this.grpVertexDimensions.Size = new System.Drawing.Size(160, 216);
			this.grpVertexDimensions.TabIndex = 9;
			this.grpVertexDimensions.TabStop = false;
			this.grpVertexDimensions.Text = "Modify Terrain Dimensions";
			// 
			// numMaxHeight
			// 
			this.numMaxHeight.DecimalPlaces = 3;
			this.numMaxHeight.Increment = new System.Decimal(new int[] {
																		   1,
																		   0,
																		   0,
																		   196608});
			this.numMaxHeight.Location = new System.Drawing.Point(40, 184);
			this.numMaxHeight.Maximum = new System.Decimal(new int[] {
																		 100000,
																		 0,
																		 0,
																		 0});
			this.numMaxHeight.Name = "numMaxHeight";
			this.numMaxHeight.Size = new System.Drawing.Size(104, 20);
			this.numMaxHeight.TabIndex = 20;
			this.numMaxHeight.Value = new System.Decimal(new int[] {
																	   1,
																	   0,
																	   0,
																	   0});
			this.numMaxHeight.ValueChanged += new System.EventHandler(this.numMaxHeight_ValueChanged);
			this.numMaxHeight.Leave += new System.EventHandler(this.numMaxHeight_Leave);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 168);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(136, 16);
			this.label6.TabIndex = 19;
			this.label6.Text = "Maximum Vertex Height:";
			// 
			// chkVertices_GridSize
			// 
			this.chkVertices_GridSize.Checked = true;
			this.chkVertices_GridSize.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkVertices_GridSize.Location = new System.Drawing.Point(16, 136);
			this.chkVertices_GridSize.Name = "chkVertices_GridSize";
			this.chkVertices_GridSize.Size = new System.Drawing.Size(112, 16);
			this.chkVertices_GridSize.TabIndex = 18;
			this.chkVertices_GridSize.Text = "Lock Terrain Size";
			this.chkVertices_GridSize.CheckedChanged += new System.EventHandler(this.chkVertices_GridSize_CheckedChanged);
			// 
			// numVertices_RowDistance
			// 
			this.numVertices_RowDistance.DecimalPlaces = 2;
			this.numVertices_RowDistance.Increment = new System.Decimal(new int[] {
																					  1,
																					  0,
																					  0,
																					  131072});
			this.numVertices_RowDistance.Location = new System.Drawing.Point(72, 112);
			this.numVertices_RowDistance.Maximum = new System.Decimal(new int[] {
																					1000,
																					0,
																					0,
																					0});
			this.numVertices_RowDistance.Minimum = new System.Decimal(new int[] {
																					1,
																					0,
																					0,
																					131072});
			this.numVertices_RowDistance.Name = "numVertices_RowDistance";
			this.numVertices_RowDistance.Size = new System.Drawing.Size(56, 20);
			this.numVertices_RowDistance.TabIndex = 17;
			this.numVertices_RowDistance.Value = new System.Decimal(new int[] {
																				  10,
																				  0,
																				  0,
																				  65536});
			this.numVertices_RowDistance.ValueChanged += new System.EventHandler(this.numVertices_RowDistance_ValueChanged);
			this.numVertices_RowDistance.Leave += new System.EventHandler(this.numVertices_RowDistance_Leave);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 112);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(40, 23);
			this.label4.TabIndex = 16;
			this.label4.Text = "Width:";
			// 
			// numVertices_ColumnDistance
			// 
			this.numVertices_ColumnDistance.DecimalPlaces = 2;
			this.numVertices_ColumnDistance.Increment = new System.Decimal(new int[] {
																						 1,
																						 0,
																						 0,
																						 131072});
			this.numVertices_ColumnDistance.Location = new System.Drawing.Point(72, 88);
			this.numVertices_ColumnDistance.Maximum = new System.Decimal(new int[] {
																					   1000,
																					   0,
																					   0,
																					   0});
			this.numVertices_ColumnDistance.Minimum = new System.Decimal(new int[] {
																					   1,
																					   0,
																					   0,
																					   131072});
			this.numVertices_ColumnDistance.Name = "numVertices_ColumnDistance";
			this.numVertices_ColumnDistance.Size = new System.Drawing.Size(56, 20);
			this.numVertices_ColumnDistance.TabIndex = 15;
			this.numVertices_ColumnDistance.Value = new System.Decimal(new int[] {
																					 100,
																					 0,
																					 0,
																					 131072});
			this.numVertices_ColumnDistance.ValueChanged += new System.EventHandler(this.numVertices_ColumnDistance_ValueChanged);
			this.numVertices_ColumnDistance.Leave += new System.EventHandler(this.numVertices_ColumnDistance_Leave);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(16, 88);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(48, 23);
			this.label5.TabIndex = 14;
			this.label5.Text = "Height:";
			// 
			// chkVertices_GridDimensions
			// 
			this.chkVertices_GridDimensions.Checked = true;
			this.chkVertices_GridDimensions.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkVertices_GridDimensions.Location = new System.Drawing.Point(16, 64);
			this.chkVertices_GridDimensions.Name = "chkVertices_GridDimensions";
			this.chkVertices_GridDimensions.Size = new System.Drawing.Size(136, 16);
			this.chkVertices_GridDimensions.TabIndex = 10;
			this.chkVertices_GridDimensions.Text = "Lock Grid Dimensions";
			this.chkVertices_GridDimensions.CheckedChanged += new System.EventHandler(this.chkVertices_GridDimensions_CheckedChanged);
			// 
			// numVertices_Columns
			// 
			this.numVertices_Columns.Location = new System.Drawing.Point(72, 40);
			this.numVertices_Columns.Maximum = new System.Decimal(new int[] {
																				1000,
																				0,
																				0,
																				0});
			this.numVertices_Columns.Minimum = new System.Decimal(new int[] {
																				2,
																				0,
																				0,
																				0});
			this.numVertices_Columns.Name = "numVertices_Columns";
			this.numVertices_Columns.Size = new System.Drawing.Size(56, 20);
			this.numVertices_Columns.TabIndex = 7;
			this.numVertices_Columns.Value = new System.Decimal(new int[] {
																			  5,
																			  0,
																			  0,
																			  0});
			this.numVertices_Columns.ValueChanged += new System.EventHandler(this.numVertices_Columns_ValueChanged);
			this.numVertices_Columns.Leave += new System.EventHandler(this.numVertices_Columns_Leave);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 23);
			this.label3.TabIndex = 8;
			this.label3.Text = "Columns:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 23);
			this.label2.TabIndex = 6;
			this.label2.Text = "Rows:";
			// 
			// numVertices_Rows
			// 
			this.numVertices_Rows.Location = new System.Drawing.Point(72, 16);
			this.numVertices_Rows.Maximum = new System.Decimal(new int[] {
																			 1000,
																			 0,
																			 0,
																			 0});
			this.numVertices_Rows.Minimum = new System.Decimal(new int[] {
																			 2,
																			 0,
																			 0,
																			 0});
			this.numVertices_Rows.Name = "numVertices_Rows";
			this.numVertices_Rows.Size = new System.Drawing.Size(56, 20);
			this.numVertices_Rows.TabIndex = 5;
			this.numVertices_Rows.Value = new System.Decimal(new int[] {
																		   5,
																		   0,
																		   0,
																		   0});
			this.numVertices_Rows.ValueChanged += new System.EventHandler(this.numVertices_Rows_ValueChanged);
			this.numVertices_Rows.Leave += new System.EventHandler(this.numVertices_Rows_Leave);
			// 
			// grpVertexCreation
			// 
			this.grpVertexCreation.Controls.Add(this.btnVertices_Create);
			this.grpVertexCreation.Location = new System.Drawing.Point(8, 8);
			this.grpVertexCreation.Name = "grpVertexCreation";
			this.grpVertexCreation.Size = new System.Drawing.Size(160, 48);
			this.grpVertexCreation.TabIndex = 8;
			this.grpVertexCreation.TabStop = false;
			this.grpVertexCreation.Text = "Terrain Creation";
			// 
			// btnVertices_Create
			// 
			this.btnVertices_Create.Location = new System.Drawing.Point(8, 16);
			this.btnVertices_Create.Name = "btnVertices_Create";
			this.btnVertices_Create.Size = new System.Drawing.Size(144, 23);
			this.btnVertices_Create.TabIndex = 0;
			this.btnVertices_Create.Text = "Create Terrain";
			this.btnVertices_Create.Click += new System.EventHandler(this.btnVertices_Create_Click);
			// 
			// grpVertexSelection
			// 
			this.grpVertexSelection.Controls.Add(this.numVertexHeight);
			this.grpVertexSelection.Controls.Add(this.label7);
			this.grpVertexSelection.Controls.Add(this.chkFalloff);
			this.grpVertexSelection.Controls.Add(this.label1);
			this.grpVertexSelection.Controls.Add(this.numSoft);
			this.grpVertexSelection.Controls.Add(this.radSoft);
			this.grpVertexSelection.Controls.Add(this.btnVertexSelection);
			this.grpVertexSelection.Controls.Add(this.radSingle);
			this.grpVertexSelection.Enabled = false;
			this.grpVertexSelection.Location = new System.Drawing.Point(8, 144);
			this.grpVertexSelection.Name = "grpVertexSelection";
			this.grpVertexSelection.Size = new System.Drawing.Size(160, 192);
			this.grpVertexSelection.TabIndex = 6;
			this.grpVertexSelection.TabStop = false;
			this.grpVertexSelection.Text = "Vertex Selection";
			// 
			// chkFalloff
			// 
			this.chkFalloff.Checked = true;
			this.chkFalloff.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkFalloff.Enabled = false;
			this.chkFalloff.Location = new System.Drawing.Point(16, 120);
			this.chkFalloff.Name = "chkFalloff";
			this.chkFalloff.TabIndex = 5;
			this.chkFalloff.Text = "Use Falloff";
			this.chkFalloff.CheckedChanged += new System.EventHandler(this.chkFalloff_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 23);
			this.label1.TabIndex = 4;
			this.label1.Text = "Distance:";
			// 
			// numSoft
			// 
			this.numSoft.DecimalPlaces = 3;
			this.numSoft.Enabled = false;
			this.numSoft.Increment = new System.Decimal(new int[] {
																	  1,
																	  0,
																	  0,
																	  196608});
			this.numSoft.Location = new System.Drawing.Point(80, 96);
			this.numSoft.Name = "numSoft";
			this.numSoft.Size = new System.Drawing.Size(72, 20);
			this.numSoft.TabIndex = 3;
			this.numSoft.ValueChanged += new System.EventHandler(this.numSoft_ValueChanged);
			// 
			// radSoft
			// 
			this.radSoft.Enabled = false;
			this.radSoft.Location = new System.Drawing.Point(8, 64);
			this.radSoft.Name = "radSoft";
			this.radSoft.Size = new System.Drawing.Size(136, 24);
			this.radSoft.TabIndex = 2;
			this.radSoft.Text = "Blend Nearby Vertices";
			this.radSoft.Click += new System.EventHandler(this.radSoft_CheckedChanged);
			// 
			// btnVertexSelection
			// 
			this.btnVertexSelection.BackColor = System.Drawing.SystemColors.Control;
			this.btnVertexSelection.Location = new System.Drawing.Point(8, 16);
			this.btnVertexSelection.Name = "btnVertexSelection";
			this.btnVertexSelection.Size = new System.Drawing.Size(144, 24);
			this.btnVertexSelection.TabIndex = 0;
			this.btnVertexSelection.Text = "Enable Vertex Selection";
			this.btnVertexSelection.Click += new System.EventHandler(this.btnVertexSelection_Click);
			// 
			// radSingle
			// 
			this.radSingle.Checked = true;
			this.radSingle.Enabled = false;
			this.radSingle.Location = new System.Drawing.Point(8, 40);
			this.radSingle.Name = "radSingle";
			this.radSingle.TabIndex = 1;
			this.radSingle.TabStop = true;
			this.radSingle.Text = "Single Vertex";
			this.radSingle.Click += new System.EventHandler(this.radSingle_CheckedChanged);
			// 
			// grpVertexAlgorithms
			// 
			this.grpVertexAlgorithms.Controls.Add(this.btnVertices_LoadAlgorithm);
			this.grpVertexAlgorithms.Controls.Add(this.btnVertices_RunAlgorithm);
			this.grpVertexAlgorithms.Controls.Add(this.lstVertices_Algorithms);
			this.grpVertexAlgorithms.Enabled = false;
			this.grpVertexAlgorithms.Location = new System.Drawing.Point(8, 344);
			this.grpVertexAlgorithms.Name = "grpVertexAlgorithms";
			this.grpVertexAlgorithms.Size = new System.Drawing.Size(160, 160);
			this.grpVertexAlgorithms.TabIndex = 7;
			this.grpVertexAlgorithms.TabStop = false;
			this.grpVertexAlgorithms.Text = "Algorithms";
			// 
			// btnVertices_LoadAlgorithm
			// 
			this.btnVertices_LoadAlgorithm.Location = new System.Drawing.Point(8, 128);
			this.btnVertices_LoadAlgorithm.Name = "btnVertices_LoadAlgorithm";
			this.btnVertices_LoadAlgorithm.Size = new System.Drawing.Size(144, 23);
			this.btnVertices_LoadAlgorithm.TabIndex = 2;
			this.btnVertices_LoadAlgorithm.Text = "Load New Algorithm";
			this.btnVertices_LoadAlgorithm.Click += new System.EventHandler(this.btnVertices_LoadAlgorithm_Click);
			// 
			// btnVertices_RunAlgorithm
			// 
			this.btnVertices_RunAlgorithm.Enabled = false;
			this.btnVertices_RunAlgorithm.Location = new System.Drawing.Point(8, 96);
			this.btnVertices_RunAlgorithm.Name = "btnVertices_RunAlgorithm";
			this.btnVertices_RunAlgorithm.Size = new System.Drawing.Size(144, 23);
			this.btnVertices_RunAlgorithm.TabIndex = 1;
			this.btnVertices_RunAlgorithm.Text = "Run Algorithm";
			this.btnVertices_RunAlgorithm.Click += new System.EventHandler(this.btnVertices_RunAlgorithm_Click);
			// 
			// lstVertices_Algorithms
			// 
			this.lstVertices_Algorithms.Location = new System.Drawing.Point(8, 16);
			this.lstVertices_Algorithms.Name = "lstVertices_Algorithms";
			this.lstVertices_Algorithms.Size = new System.Drawing.Size(144, 69);
			this.lstVertices_Algorithms.TabIndex = 0;
			this.lstVertices_Algorithms.DoubleClick += new System.EventHandler(this.lstVertices_Algorithms_DoubleClick);
			this.lstVertices_Algorithms.SelectedIndexChanged += new System.EventHandler(this.lstVertices_Algorithms_SelectedIndexChanged);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 152);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(112, 32);
			this.label7.TabIndex = 6;
			this.label7.Text = "Current Vertex Height:";
			// 
			// numVertexHeight
			// 
			this.numVertexHeight.DecimalPlaces = 3;
			this.numVertexHeight.Enabled = false;
			this.numVertexHeight.Increment = new System.Decimal(new int[] {
																			  1,
																			  0,
																			  0,
																			  196608});
			this.numVertexHeight.Location = new System.Drawing.Point(88, 160);
			this.numVertexHeight.Name = "numVertexHeight";
			this.numVertexHeight.Size = new System.Drawing.Size(64, 20);
			this.numVertexHeight.TabIndex = 7;
			this.numVertexHeight.ValueChanged += new System.EventHandler(this.numVertexHeight_ValueChanged);
			this.numVertexHeight.Leave += new System.EventHandler(this.numVertexHeight_Leave);
			// 
			// VertexManipulation
			// 
			this.Controls.Add(this.grpVertexName);
			this.Controls.Add(this.grpVertexDimensions);
			this.Controls.Add(this.grpVertexCreation);
			this.Controls.Add(this.grpVertexSelection);
			this.Controls.Add(this.grpVertexAlgorithms);
			this.Name = "VertexManipulation";
			this.Size = new System.Drawing.Size(176, 736);
			this.grpVertexName.ResumeLayout(false);
			this.grpVertexDimensions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numMaxHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numVertices_RowDistance)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numVertices_ColumnDistance)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numVertices_Columns)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numVertices_Rows)).EndInit();
			this.grpVertexCreation.ResumeLayout(false);
			this.grpVertexSelection.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numSoft)).EndInit();
			this.grpVertexAlgorithms.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numVertexHeight)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
