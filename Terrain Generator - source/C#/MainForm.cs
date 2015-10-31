using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Voyage.Terraingine.DataCore;
using Voyage.Terraingine.DXViewport;
using Voyage.Terraingine.DataInterfacing;
using Voyage.Terraingine.ExportTerrainProject;
using Voyage.Terraingine.ImportTerrainProject;
using Voyage.LuaNetInterface;
using Voyage.Terraingine;

namespace Voyage.Terraingine
{
	/// <summary>
	/// An object for controlling terrain creation and manipulation from a top-level perspective.
	/// </summary>
	public class MainForm : TerrainViewport
	{
		#region Data Members
		private DataInterfacing.DataManipulation	_terrainData;
		private LuaVirtualMachine	_lua;
		private DXViewport.Viewport	_dx;
		private string				_projectName;

		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.StatusBar statMain;
		private System.Windows.Forms.TabControl tabControls;
		private System.Windows.Forms.TabPage tpgVertices;
		private System.Windows.Forms.VScrollBar vScrollVertices;
		private System.Windows.Forms.TabPage tpgTextures;
		private System.Windows.Forms.VScrollBar vScrollTextures;
		private System.Windows.Forms.TabPage tpgHistory;
		private System.Windows.Forms.TabPage tpgLights;
		private System.Windows.Forms.VScrollBar vScrollLights;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.VScrollBar vScrollHistory;
		private Voyage.Terraingine.LightManipulation lightManip;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem10;
		private System.Windows.Forms.MenuItem menuFile;
		private System.Windows.Forms.MenuItem menuFile_New;
		private System.Windows.Forms.MenuItem menuFile_Open;
		private System.Windows.Forms.MenuItem menuFile_Close;
		private System.Windows.Forms.MenuItem menuFile_Save;
		private System.Windows.Forms.MenuItem menuFile_SaveAs;
		private System.Windows.Forms.MenuItem menuFile_Exit;
		private System.Windows.Forms.MenuItem menuEdit;
		private System.Windows.Forms.MenuItem menuEdit_Undo;
		private System.Windows.Forms.MenuItem menuEdit_Redo;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuEdit_Copy;
		private System.Windows.Forms.MenuItem menuEdit_Paste;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuFile_Import;
		private System.Windows.Forms.MenuItem menuFile_Export;
		private System.Windows.Forms.MenuItem menuView_Default;
		private System.Windows.Forms.MenuItem menuHelp_Help;
		private System.Windows.Forms.MenuItem menuHelp_About;
		private Voyage.Terraingine.CameraManipulation camManip;
		private Voyage.Terraingine.VertexManipulation vertManip;
		private System.Windows.Forms.MenuItem menuFile_Import_ImportTerrain;
		private System.Windows.Forms.MenuItem menuFile_Import_LoadMethod;
		private System.Windows.Forms.MenuItem fileMenu_Export_ExportTerrain;
		private System.Windows.Forms.MenuItem menuFile_Export_LoadMethod;
		private Voyage.Terraingine.TextureManipulation texManip;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuView_SelectAll;
		private System.Windows.Forms.MenuItem menuView_SelectInverse;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuView_DefaultColor;
		private System.Windows.Forms.MenuItem menuView_HeightColor;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem menuView_SizePlus;
		private System.Windows.Forms.MenuItem menuView_SizeMinus;
		private System.Windows.Forms.MenuItem menuCamera;
		private System.Windows.Forms.MenuItem menuCamera_Rotate;
		private System.Windows.Forms.MenuItem menuCamera_Pan;
		private System.Windows.Forms.MenuItem menuCamera_Zoom;
		private System.Windows.Forms.MenuItem menuCamera_Reset;
		private System.Windows.Forms.MenuItem menuCamera_Wireframe;
		private System.Windows.Forms.MenuItem menuCamera_Solid;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem menuView_ScreenCapture;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem menuView_Script;
		private Voyage.Terraingine.HistoryManipulation histManip;

		#endregion

		#region Properties
		/// <summary>
		/// Gets the data interface for the terrain data.
		/// </summary>
		public DataInterfacing.DataManipulation DataInterface
		{
			get { return _terrainData; }
		}
		#endregion

		#region Basic Form Methods
		/// <summary>
		/// Creates a Form of type MainForm.
		/// </summary>
		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.CenterToScreen();

			// Shortcut variables for the DirectX viewport and the terrain data
			_terrainData = _viewport.TerrainData;
			_dx = _viewport.DXViewport;
			_projectName = null;

			// Initialize manipulation controls
			camManip.Initialize( this );
			vertManip.Initialize( this );
			texManip.Initialize( this );
			histManip.Initialize( this );
			lightManip.Initialize( this );

			// Initialize _viewport MouseEvent callbacks
			_viewport.ViewportPanel.MouseDown += new MouseEventHandler(this.pnlViewport_MouseDown);
			_viewport.ViewportPanel.MouseUp += new MouseEventHandler(this.pnlViewport_MouseUp);
			_viewport.ViewportPanel.DragEnter += new DragEventHandler(ViewportPanel_DragEnter);
			_viewport.ViewportPanel.DragDrop += new DragEventHandler(ViewportPanel_DragDrop);
			_viewport.ViewportPanel.MouseWheel += new MouseEventHandler(ViewportPanel_MouseWheel);
			this.MouseWheel += new MouseEventHandler(ViewportPanel_MouseWheel);

			// Initialize Lua functionality
			InitializeLua();
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
		/// Runs further processing when the application is idle.
		/// </summary>
		public override void OnApplicationIdle(object sender, EventArgs e)
		{
			while ( AppStillIdle )
			{
				// Render a frame during idle time (no messages are waiting)
				_viewport.PreRender();

				if ( _viewport.BeginRender() )
				{
					_viewport.RenderSceneElements();
					this.RenderSceneElements();
					_viewport.EndRender();
				}
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Sets up additional data members in the form.
		/// </summary>
		private void MainForm_Load(object sender, System.EventArgs e)
		{
			float softSelectionDistance = 0.2f;

			//
			// DXViewport initialization
			//

			// Set additional OnDeviceReset handler
			_dx.Device.DeviceReset += new EventHandler( this.SetupLights );

			_dx.ClearColor = Color.Gray;
			_dx.Camera.FirstPerson = false;
			_viewport.FillMode = FillMode.Solid;
			_viewport.CullMode = Cull.CounterClockwise;
			_dx.Camera.FollowDistance = 2.0f;
			_dx.Camera.TimeToReset = DateTime.MinValue.AddMilliseconds( 300 );

			_viewport.InitializeCamera();

			//
			// Terrain initialization
			//

			_terrainData.SoftSelectionDistanceSquared = softSelectionDistance * softSelectionDistance;
			_terrainData.OriginalZoomFactor = 3.0f;

			//
			// Additional uninherited initialization
			//

			MainForm_SizeChanged( sender, e );

			// Don't update uninitialized data during display updates
			vertManip.EnableDataUpdates = false;
			vertManip.SoftSelectionDistance = softSelectionDistance;
			vertManip.EnableDataUpdates = true;
			lightManip.InitializePrimaryLight();
			vertManip.EnableVertexMovement.Click += new EventHandler(EnableVertexMovement_Click);

			// Load and display plug-ins for the system
			_terrainData.LoadPlugIns();
			vertManip.LoadAlgorithms();
			texManip.LoadAlgorithms();
		}

		/// <summary>
		/// Sets up the lights used by the main viewport.
		/// </summary>
		private void SetupLights( object sender, System.EventArgs e )
		{
			SetupLights();
		}

		/// <summary>
		/// Sets up the lights used by the main viewport.
		/// </summary>
		[LuaFunctionAttribute( "SetupLights", "Sets up the lights used by the main viewport." )]
		public void SetupLights()
		{
			Vector3 dir = new Vector3( -1.0f, 0, -1.0f );

			dir.Normalize();
			dir.Y = -1.0f;

			_dx.Device.Lights[0].Diffuse = Color.LawnGreen;
			_dx.Device.Lights[0].Type = LightType.Directional;
			_dx.Device.Lights[0].Direction = dir;
			_dx.Device.Lights[0].Update();
			_dx.Device.Lights[0].Enabled = true;

			_dx.Device.RenderState.Ambient = Color.FromArgb( 0x40, 0x40, 0x40 );
			_terrainData.EnableLighting = true;

			lightManip.InitializePrimaryLight();
		}

		/// <summary>
		/// Initializes and registers Lua functionality.
		/// </summary>
		private void InitializeLua()
		{
			_lua = new LuaVirtualMachine( false );

			// Register Lua functionality
			_lua.RegisterLuaFunctions( this );
			_lua.RegisterLuaFunctions( _terrainData, "Data",
				"Package for containing data manipulation functions." );
			_lua.RegisterLuaFunctions( vertManip, "Vertices",
				"Package for manipulating terrain vertices." );
			_lua.RegisterLuaFunctions( texManip, "Textures",
				"Package for manipulating terrain textures." );
			_lua.RegisterLuaFunctions( lightManip, "Lighting",
				"Package for manipulating terrain lighting." );
			_lua.RegisterLuaFunctions( camManip, "Camera",
				"Package for manipulating the terrain camera." );
		}
		#endregion

		#region Rendering
		/// <summary>
		/// Renders the elements in the scene.
		/// </summary>
		protected void RenderSceneElements()
		{
			if ( !_dx.LostDevice && _terrainData.TerrainPage != null )
			{
				// All remaining renderings require the same format and culling
				_dx.Device.VertexFormat = CustomVertex.PositionColored.Format;
				_dx.Device.RenderState.CullMode = Cull.None;

				// Finish rendering
				if ( _terrainData.BufferObjects.ShowSelectedVertices )
				{
					_terrainData.RenderVertices();
					DrawCenteringAxes();
				}

				// Display selected vertex data
				DisplayVertexInStatusBar();
			}
		}

		/// <summary>
		/// Displays the selected vertex position in the status bar.
		/// </summary>
		protected void DisplayVertexInStatusBar()
		{
			Vector3[] vertices = _terrainData.TerrainPage.TerrainPatch.GetSelectedVertexPositions();
			int[] indices = _terrainData.TerrainPage.TerrainPatch.GetSelectedVertexIndices();

			if ( vertices.Length > 0 )
			{
				statMain.Text = "Vertex " + indices[0] + ":   ";
				statMain.Text += "X: " + vertices[0].X + ", Y: " + vertices[0].Y + ", Z: " + vertices[0].Z;
			}
			else
				statMain.Text = "";
		}

		private void DrawCenteringAxes()		// TEMP FUNCTION
		{
			Matrix world = _dx.Camera.WorldIdentity;
			CustomVertex.PositionColored[] axes = new CustomVertex.PositionColored[6];
			Vector3 position = _terrainData.TerrainPage.Position;

			// X-axis
			axes[0].Position = new Vector3( position.X, position.Y, position.Z );
			axes[0].Color = Color.Red.ToArgb();
			axes[1].Position = new Vector3( position.X + 1f / _terrainData.OriginalZoomFactor,
				position.Y, position.Z );
			axes[1].Color = Color.Red.ToArgb();

			// Z-axis
			axes[2].Position = new Vector3( position.X, position.Y, position.Z );
			axes[2].Color = Color.Blue.ToArgb();
			axes[3].Position = new Vector3( position.X, position.Y,
				position.Z + 1f / _terrainData.OriginalZoomFactor );
			axes[3].Color = Color.Blue.ToArgb();

			// Y-axis
			axes[4].Position = new Vector3( position.X, position.Y, position.Z );
			axes[4].Color = Color.Green.ToArgb();
			axes[5].Position = new Vector3( position.X, position.Y + 1f / _terrainData.OriginalZoomFactor,
				position.Z );
			axes[5].Color = Color.Green.ToArgb();

			_dx.Device.RenderState.FillMode = FillMode.Solid;

			//world.Scale( 0.4f, 0.2f, 0.4f );
			_dx.Device.Transform.World = world;
			_dx.Device.DrawUserPrimitives( PrimitiveType.LineList, 3, axes );
		}
		#endregion

		#region State Control
		/// <summary>
		/// Updates the form display states based on undo action.
		/// </summary>
		public void UndoTerrainAction()
		{
			if ( _terrainData.TerrainPage == null )
			{
				vertManip.EnableTerrainEditing( false );
				_viewport.InitializeCamera();
			}
			else
				texManip.LoadTextures();
		}

		/// <summary>
		/// Updates the form display states based on redo action.
		/// </summary>
		public void RedoTerrainAction()
		{
			if ( _terrainData.TerrainPage == null )
				vertManip.EnableTerrainEditing( false );
			else
			{
				vertManip.EnableTerrainEditing( true );
				texManip.LoadTextures();
			}
		}

		/// <summary>
		/// Clears the terrain.
		/// </summary>
		[LuaFunctionAttribute( "ClearTerrain", "Clears the terrain." )]
		public void ClearTerrain()
		{
			_terrainData.Dispose();
			tabControls.SelectedIndex = 0;
			histManip.ClearHistory();
			vertManip.EnableTerrainEditing( false );
			_terrainData.DataHistory.ClearHistory();
		}

		/// <summary>
		/// Loads a TerrainPage into the program.
		/// </summary>
		/// <param name="page">The TerrainPage to load.</param>
		[LuaFunctionAttribute( "LoadTerrain", "Loads a TerrainPage into the program.",
			"The TerrainPage to load." )]
		public void LoadTerrain( TerrainPage page )
		{
			page.TerrainPatch.RefreshBuffers = true;
			_terrainData.LoadTerrain( page );
			vertManip.LoadTerrain( page.Name );
			_viewport.InitializeCamera();
			_viewport.DXViewport.Camera.InitializeReset();
		}

		/// <summary>
		/// Updates the display states of the program.
		/// </summary>
		public void UpdateStates()
		{
			// Update Vertex Manipulation tab
			vertManip.UpdateStates();
			vertManip.LoadAlgorithms();

			// Update Texture Manipulation tab
			texManip.LoadTextures();
			texManip.LoadAlgorithms();

			// Update camera
			if ( _terrainData.TerrainPage != null )
			{
				if ( _terrainData.TerrainPage.TerrainPatch.Height >
					_terrainData.TerrainPage.TerrainPatch.Width )
					_viewport.DXViewport.Camera.FollowDistance =
						_terrainData.TerrainPage.TerrainPatch.Height * 2.0f;
				else
					_viewport.DXViewport.Camera.FollowDistance =
						_terrainData.TerrainPage.TerrainPatch.Width * 2.0f;

				_viewport.InitializeCamera();
			}
		}
		#endregion

		#region File Menu Actions
		/// <summary>
		/// Prompts the user to save data.
		/// </summary>
		/// <returns>The result of the dialog prompt.</returns>
		[LuaFunctionAttribute( "PromptToSave", "Prompts the user to save data." )]
		public DialogResult PromptToSave()
		{
			DialogResult result = MessageBox.Show(
				"Do you wish to save your work (unsaved data will be lost)?",
				"Unsaved Data!", MessageBoxButtons.YesNoCancel );

			if ( result == DialogResult.Yes )
				menuFile_SaveAs_Click();

			return result;
		}

		/// <summary>
		/// Creates a new TerrainPage for editing.
		/// </summary>
		[LuaFunctionAttribute( "NewTerrain", "Opens a dialog for creating a new TerrainPage." )]
		public void menuFile_New_Click()
		{
			if ( _terrainData.TerrainPage != null )
			{
				DialogResult result = PromptToSave();

				if ( result == DialogResult.Yes || result == DialogResult.No )
				{
					ClearTerrain();
					vertManip.CreateTerrainDialog();
					_projectName = null;
				}
			}
			else
			{
				vertManip.CreateTerrainDialog();
				_projectName = null;
			}
		}

		/// <summary>
		/// Opens an old TerrainPage for editing.
		/// </summary>
		[LuaFunctionAttribute( "OpenTerrain", "Opens an old TerrainPage for editing." )]
		public void menuFile_Open_Click()
		{
			bool proceed = true;

			if ( _terrainData.TerrainPage != null )
			{
				DialogResult result = PromptToSave();

				if ( result == DialogResult.Yes || result == DialogResult.No )
				{
					ClearTerrain();
					proceed = true;
				}
			}

			if ( proceed )
			{
				ImportTerrainProject.Driver project = new ImportTerrainProject.Driver();
				int index = -1;

				for ( int i = 0; i < _terrainData.PlugIns.FileImportPlugIns.Count; i++ )
				{
					if ( ( (PlugIn) _terrainData.PlugIns.FileImportPlugIns[i] ).GetName() == project.GetName() )
						index = i;
				}

				if ( _terrainData.RunPlugIn( index, DataInterfacing.PlugIns.PlugInTypes.Importing, this ) )
				{
					vertManip.EnableTerrainEditing( true );
					texManip.LoadTextures();
					camManip.ResetCamera();
					_projectName = project.ProjectName;
				}
			}
		}

		/// <summary>
		/// Closes the current TerrainPage.
		/// </summary>
		[LuaFunctionAttribute( "CloseTerrain", "Closes the current TerrainPage." )]
		public void menuFile_Close_Click()
		{
			if ( _terrainData.TerrainPage != null )
			{
				DialogResult result = PromptToSave();

				if ( result == DialogResult.Yes || result == DialogResult.No )
				{
					ClearTerrain();
					_terrainData.TerrainPage.Dispose();
					_terrainData.TerrainPage = null;
					_terrainData.RefreshAllBuffers();
					vertManip.RestoreDefaults();
				}
			}
			else
			{
				_terrainData.TerrainPage.Dispose();
				_terrainData.TerrainPage = null;
				_terrainData.RefreshAllBuffers();
				vertManip.RestoreDefaults();
			}
		}

		/// <summary>
		/// Saves the current TerrainPage.
		/// </summary>
		[LuaFunctionAttribute( "SaveTerrain", "Saves the current TerrainPage." )]
		public void menuFile_Save_Click()
		{
			if ( _terrainData.TerrainPage != null )
			{
				if ( _projectName != null )
				{
					ExportTerrainProject.Driver project = new ExportTerrainProject.Driver();
					int index = -1;

					for ( int i = 0; i < _terrainData.PlugIns.FileExportPlugIns.Count; i++ )
					{
						if ( ( (PlugIn) _terrainData.PlugIns.FileExportPlugIns[i] ).GetName() == project.GetName() )
							index = i;
					}

					if ( _terrainData.RunPlugInAuto( index, DataInterfacing.PlugIns.PlugInTypes.Exporting,
						this, _projectName ) )
						_projectName = project.ProjectName;
				}
				else
					menuFile_SaveAs_Click();
			}
			else
				MessageBox.Show( "No terrain has been created to save.", "Cannot Save Terrain",
					MessageBoxButtons.OK );
		}

		/// <summary>
		/// Saves the current TerrainPage with new parameters.
		/// </summary>
		[LuaFunctionAttribute( "SaveTerrainAs", "Saves the current TerrainPage with new parameters." )]
		public void menuFile_SaveAs_Click()
		{
			if ( _terrainData.TerrainPage != null )
			{
				ExportTerrainProject.Driver project = new ExportTerrainProject.Driver();
				int index = -1;

				for ( int i = 0; i < _terrainData.PlugIns.FileExportPlugIns.Count; i++ )
				{
					if ( ( (PlugIn) _terrainData.PlugIns.FileExportPlugIns[i] ).GetName() == project.GetName() )
						index = i;
				}

				if ( _terrainData.RunPlugIn( index, DataInterfacing.PlugIns.PlugInTypes.Exporting, this ) )
					_projectName = project.ProjectName;
			}
			else
				MessageBox.Show( "No terrain has been created to save.", "Cannot Save Terrain",
					MessageBoxButtons.OK );
		}

		/// <summary>
		/// Imports the current terrain using a method chosen by the user.
		/// </summary>
		[LuaFunctionAttribute( "ImportTerrain",
			"Imports the current terrain using a method chosen by the user." )]
		public void menuFile_Import_ImportTerrain_Click()
		{
			DialogResult result = DialogResult.OK;

			if ( _terrainData.TerrainPage != null )
			{
				result = PromptToSave();
			}

			if ( result != DialogResult.Cancel )
			{
				ClearTerrain();

				ImportSelection import = new ImportSelection();
				ArrayList plugins = _terrainData.PlugIns.FileImportPlugIns;
				string[] methods = new string[plugins.Count];
				int selection;

				for ( int i = 0; i < plugins.Count; i++ )
					methods[i] = ( (PlugIn) plugins[i] ).GetName();

				import.LoadImportNames( methods );
				import.StartPosition = FormStartPosition.CenterParent;

				if ( import.ShowDialog() == DialogResult.OK )
				{
					selection = import.ImportMethod;

					if ( _terrainData.RunPlugIn( selection, DataInterfacing.PlugIns.PlugInTypes.Importing, this ) )
					{
						vertManip.EnableTerrainEditing( true );
						texManip.LoadTextures();
					}
				}
			}
		}
		
		/// <summary>
		/// Loads a new plug-in used for importing.
		/// </summary>
		[LuaFunctionAttribute( "LoadImportPlugIn", "Loads a new plug-in used for importing." )]
		public void menuFile_Import_LoadMethod_Click()
		{
			_terrainData.LoadPlugIn( DataInterfacing.PlugIns.PlugInTypes.Importing );
		}

		/// <summary>
		/// Exports the current terrain using a method chosen by the user.
		/// </summary>
		[LuaFunctionAttribute( "ExportTerrain",
			"Exports the current terrain using a method chosen by the user." )]
		public void fileMenu_Export_ExportTerrain_Click()
		{
			if ( _terrainData.TerrainPage != null )
			{
				ExportSelection export = new ExportSelection();
				ArrayList plugins = _terrainData.PlugIns.FileExportPlugIns;
				string[] methods = new string[plugins.Count];
				int selection;

				for ( int i = 0; i < plugins.Count; i++ )
					methods[i] = ( (PlugIn) plugins[i] ).GetName();

				export.LoadExportNames( methods );
				export.StartPosition = FormStartPosition.CenterParent;

				if ( export.ShowDialog() == DialogResult.OK )
				{
					selection = export.ExportMethod;
					_terrainData.RunPlugIn( selection, DataInterfacing.PlugIns.PlugInTypes.Exporting, this );
				}
			}
			else
				MessageBox.Show( "No terrain has been created to export.", "Cannot Save Terrain",
					MessageBoxButtons.OK );
		}

		/// <summary>
		/// Loads a new plug-in used for exporting.
		/// </summary>
		[LuaFunctionAttribute( "LoadExportPlugIn", "Loads a new plug-in used for exporting." )]
		public void menuFile_Export_LoadMethod_Click()
		{
			_terrainData.LoadPlugIn( DataInterfacing.PlugIns.PlugInTypes.Exporting );
		}

		/// <summary>
		/// Exits the program.
		/// </summary>
		[LuaFunctionAttribute( "ExitProgram", "Exits the program." )]
		public void menuFile_Exit_Click()
		{
			if ( _terrainData.TerrainPage != null )
			{
				DialogResult result = MessageBox.Show(
					"Do you wish to save your work (unsaved data will be lost)?",
					"Unsaved Data!", MessageBoxButtons.YesNo );

				if ( result == DialogResult.Yes )
					menuFile_SaveAs_Click();

				this.Close();
			}
			else
				this.Close();
		}
		#endregion

		#region Edit Menu Actions
		/// <summary>
		/// Undoes the last action taken.
		/// </summary>
		[LuaFunctionAttribute( "UndoLastAction", "Undoes the last action taken." )]
		public void menuEdit_Undo_Click()
		{
			// Performs an "undo" action
			_terrainData.UndoLastPageAction();
			UndoTerrainAction();
		}

		/// <summary>
		/// Re-performs the last action taken.
		/// </summary>
		[LuaFunctionAttribute( "RedoLastAction", "Re-performs the last action taken." )]
		public void menuEdit_Redo_Click()
		{
			// Performs a "redo" action
			_terrainData.RedoLastPageAction();
			RedoTerrainAction();
		}

		/// <summary>
		/// Copies the currently selected vertex data (copies first selected vertex only).
		/// </summary>
		public void menuEdit_Copy_Click()
		{
			vertManip.CopyVertex();
		}

		/// <summary>
		/// Pastes copied vertex data onto the currently selected vertex (pastes one vertex only).
		/// </summary>
		public void menuEdit_Paste_Click()
		{
			vertManip.PasteVertex();
		}
		#endregion

		#region View Menu Actions
		/// <summary>
		/// Resets the camera to the center of the current terrain.
		/// </summary>
		public void menuView_Default_Click()
		{
			_dx.Camera.ResetCamera();
		}

		/// <summary>
		/// Selects all vertices in the TerrainPage.
		/// </summary>
		public void menuView_SelectAll_Click()
		{
			vertManip.SelectAllVertices();
		}

		/// <summary>
		/// Selects the non-selected vertices and un-selects currently selected vertices.
		/// </summary>
		public void menuView_SelectInverse_Click()
		{
			vertManip.SelectInverseVertices();
		}

		/// <summary>
		/// Sets the base color of the terrain to the lighting color.
		/// </summary>
		public void menuView_DefaultColor_Click()
		{
			lightManip.ColorTerrainByLighting();
		}

		/// <summary>
		/// Sets the base color of the terrain to coloring dependent upon vertex height.
		/// </summary>
		public void menuView_HeightColor_Click()
		{
			lightManip.ColorTerrainByHeight();
		}

		/// <summary>
		/// Increases the size of the rendered vertices.
		/// </summary>
		public void menuView_SizePlus_Click()
		{
			camManip.IncreaseVertexSize();
		}

		/// <summary>
		/// Decreases the size of the rendered vertices.
		/// </summary>
		[LuaFunctionAttribute( "DecreaseVertexSize", "Decreases the size of the rendered vertices." )]
		public void menuView_SizeMinus_Click()
		{
			camManip.DecreaseVertexSize();
		}

		/// <summary>
		/// Saves the current render frame to a file.
		/// </summary>
		[LuaFunctionAttribute( "ScreenCapture", "Saves the current render frame to a file." )]
		public void menuView_ScreenCapture_Click()
		{
			SaveFileDialog dlgSave = new SaveFileDialog();
			ImageFileFormat fileFormat = new ImageFileFormat();
			DialogResult result;
			bool valid = true;

			// Get filename to save
			dlgSave.Filter = "JPEG Files (*.jpg)|*.jpg|" +
				"Bitmap Files (*.bmp)|*.bmp|" +
				"PNG Files (*.png)|*.png|" +
				"TGA Files (*.tga)|*.tga";
			dlgSave.InitialDirectory = Path.GetDirectoryName( Application.ExecutablePath ) + "\\Screenshots";
			result = dlgSave.ShowDialog( this );

			if ( result == DialogResult.OK && dlgSave.FileName != null )
			{
				// Get the extension of the file to be saved
				switch ( Path.GetExtension( dlgSave.FileName ) )
				{
					case ".bmp":
						fileFormat = ImageFileFormat.Bmp;
						break;

					case ".jpg":
						fileFormat = ImageFileFormat.Jpg;
						break;

					case ".png":
						fileFormat = ImageFileFormat.Png;
						break;

					case ".tga":
						fileFormat = ImageFileFormat.Tga;
						break;

					default:
						valid = false;
						break;
				}

				// If the file extension is valid, save the screenshot
				if ( valid )
					_terrainData.TakeScreenshot( dlgSave.FileName, fileFormat );
			}
		}

		/// <summary>
		/// Opens a Lua scripting window.
		/// </summary>
		protected void menuView_Script_Click()
		{
			LuaScripting t = new LuaScripting(this);

			// Register Lua functionality
			_lua.RegisterLuaFunctions( t );

			t.LuaVM = _lua;
			t.Show();
		}
		#endregion

		#region Help Menu Actions
		/// <summary>
		/// Opens Terraingine help files.
		/// </summary>
		[LuaFunctionAttribute( "OpenHelp", "Opens Terraingine help files." )]
		public void menuHelp_Help_Click()
		{
			string file = "help.chm";

			if (File.Exists(file))
			{
				Process proc = new Process();

				proc.StartInfo.FileName = file;
				proc.Start();
			}
			else
				MessageBox.Show("File not found: " + file, "Error Opening Help File",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}

		/// <summary>
		/// Opens a window displaying program information.
		/// </summary>
		[LuaFunctionAttribute( "AboutTerraingine", "Opens a window displaying program information." )]
		public void menuHelp_About_Click()
		{
			AboutForm af = new AboutForm();

			af.ShowDialog( this );
		}
		#endregion

		#region Camera Menu Actions
		/// <summary>
		/// Activates camera rotation.
		/// </summary>
		public void menuCamera_Rotate_Click()
		{
			camManip.RotateCamera( true, !camManip.RotatePressed );
		}

		/// <summary>
		/// Activates camera panning.
		/// </summary>
		public void menuCamera_Pan_Click()
		{
			camManip.PanCamera( true, !camManip.PanPressed );
		}

		/// <summary>
		/// Activates camera zooming.
		/// </summary>
		public void menuCamera_Zoom_Click()
		{
			camManip.ZoomCamera( true, !camManip.ZoomPressed );
		}

		/// <summary>
		/// Resets the camera position.
		/// </summary>
		public void menuCamera_Reset_Click()
		{
			camManip.ResetCamera();
		}

		/// <summary>
		/// Activates wireframe rendering.
		/// </summary>
		public void menuCamera_Wireframe_Click()
		{
			camManip.WireframeTerrain( true, !camManip.WireframePressed );
		}

		/// <summary>
		/// Activates solid rendering.
		/// </summary>
		public void menuCamera_Solid_Click()
		{
			camManip.SolidTerrain( true, !camManip.SolidPressed );
		}
		#endregion

		#region Menu Events
		/// <summary>
		/// Creates a new TerrainPage for editing.
		/// </summary>
		private void menuFile_New_Click(object sender, System.EventArgs e)
		{
			menuFile_New_Click();
		}

		/// <summary>
		/// Opens an old TerrainPage for editing.
		/// </summary>
		private void menuFile_Open_Click(object sender, System.EventArgs e)
		{
			menuFile_Open_Click();
		}

		/// <summary>
		/// Closes the current TerrainPage.
		/// </summary>
		private void menuFile_Close_Click(object sender, System.EventArgs e)
		{
			menuFile_Close_Click();
		}

		/// <summary>
		/// Saves the current TerrainPage.
		/// </summary>
		private void menuFile_Save_Click(object sender, System.EventArgs e)
		{
			menuFile_Save_Click();
		}

		/// <summary>
		/// Saves the current TerrainPage with new parameters.
		/// </summary>
		private void menuFile_SaveAs_Click(object sender, System.EventArgs e)
		{
			menuFile_SaveAs_Click();
		}

		/// <summary>
		/// Imports the current terrain using a method chosen by the user.
		/// </summary>
		private void menuFile_Import_ImportTerrain_Click(object sender, System.EventArgs e)
		{
			menuFile_Import_ImportTerrain_Click();
		}
		
		/// <summary>
		/// Loads a new plug-in used for importing.
		/// </summary>
		private void menuFile_Import_LoadMethod_Click(object sender, System.EventArgs e)
		{
			menuFile_Import_LoadMethod_Click();
		}

		/// <summary>
		/// Exports the current terrain using a method chosen by the user.
		/// </summary>
		private void fileMenu_Export_ExportTerrain_Click(object sender, System.EventArgs e)
		{
			fileMenu_Export_ExportTerrain_Click();
		}

		/// <summary>
		/// Loads a new plug-in used for exporting.
		/// </summary>
		private void menuFile_Export_LoadMethod_Click(object sender, System.EventArgs e)
		{
			menuFile_Export_LoadMethod_Click();
		}

		/// <summary>
		/// Exits the program.
		/// </summary>
		private void menuFile_Exit_Click(object sender, System.EventArgs e)
		{
			menuFile_Exit_Click();
		}

		/// <summary>
		/// Undoes the last action taken.
		/// </summary>
		private void menuEdit_Undo_Click(object sender, System.EventArgs e)
		{
			menuEdit_Undo_Click();
		}

		/// <summary>
		/// Re-performs the last action taken.
		/// </summary>
		private void menuEdit_Redo_Click(object sender, System.EventArgs e)
		{
			menuEdit_Redo_Click();
		}

		/// <summary>
		/// Copies the currently selected vertex data (copies first selected vertex only).
		/// </summary>
		private void menuEdit_Copy_Click(object sender, System.EventArgs e)
		{
			menuEdit_Copy_Click();
		}

		/// <summary>
		/// Pastes copied vertex data onto the currently selected vertex (pastes one vertex only).
		/// </summary>
		private void menuEdit_Paste_Click(object sender, System.EventArgs e)
		{
			menuEdit_Paste_Click();
		}

		/// <summary>
		/// Resets the camera to the center of the current terrain.
		/// </summary>
		private void menuView_Default_Click(object sender, System.EventArgs e)
		{
			menuView_Default_Click();
		}

		/// <summary>
		/// Selects all vertices in the TerrainPage.
		/// </summary>
		private void menuView_SelectAll_Click(object sender, System.EventArgs e)
		{
			menuView_SelectAll_Click();
		}

		/// <summary>
		/// Selects the non-selected vertices and un-selects currently selected vertices.
		/// </summary>
		private void menuView_SelectInverse_Click(object sender, System.EventArgs e)
		{
			menuView_SelectInverse_Click();
		}

		/// <summary>
		/// Sets the base color of the terrain to the lighting color.
		/// </summary>
		public void menuView_DefaultColor_Click(object sender, System.EventArgs e)
		{
			menuView_DefaultColor_Click();
		}

		/// <summary>
		/// Sets the base color of the terrain to coloring dependent upon vertex height.
		/// </summary>
		public void menuView_HeightColor_Click(object sender, System.EventArgs e)
		{
			menuView_HeightColor_Click();
		}

		/// <summary>
		/// Increases the size of the rendered vertices.
		/// </summary>
		private void menuView_SizePlus_Click(object sender, System.EventArgs e)
		{
			menuView_SizePlus_Click();
		}

		/// <summary>
		/// Decreases the size of the rendered vertices.
		/// </summary>
		private void menuView_SizeMinus_Click(object sender, System.EventArgs e)
		{
			menuView_SizeMinus_Click();
		}

		/// <summary>
		/// Saves the current render frame to a file.
		/// </summary>
		private void menuView_ScreenCapture_Click(object sender, System.EventArgs e)
		{
			menuView_ScreenCapture_Click();
		}

		/// <summary>
		/// Opens a Lua scripting window.
		/// </summary>
		private void menuView_Script_Click(object sender, System.EventArgs e)
		{
			menuView_Script_Click();
		}

		/// <summary>
		/// Opens Terraingine help files.
		/// </summary>
		private void menuHelp_Help_Click(object sender, System.EventArgs e)
		{
			menuHelp_Help_Click();
		}

		/// <summary>
		/// Opens a window displaying program information.
		/// </summary>
		private void menuHelp_About_Click(object sender, System.EventArgs e)
		{
			menuHelp_About_Click();
		}

		/// <summary>
		/// Activates camera rotation.
		/// </summary>
		private void menuCamera_Rotate_Click(object sender, System.EventArgs e)
		{
			menuCamera_Rotate_Click();
		}

		/// <summary>
		/// Activates camera panning.
		/// </summary>
		private void menuCamera_Pan_Click(object sender, System.EventArgs e)
		{
			menuCamera_Pan_Click();
		}

		/// <summary>
		/// Activates camera zooming.
		/// </summary>
		private void menuCamera_Zoom_Click(object sender, System.EventArgs e)
		{
			menuCamera_Zoom_Click();
		}

		/// <summary>
		/// Resets the camera position.
		/// </summary>
		private void menuCamera_Reset_Click(object sender, System.EventArgs e)
		{
			menuCamera_Reset_Click();
		}

		/// <summary>
		/// Activates wireframe rendering.
		/// </summary>
		private void menuCamera_Wireframe_Click(object sender, System.EventArgs e)
		{
			menuCamera_Wireframe_Click();
		}

		/// <summary>
		/// Activates solid rendering.
		/// </summary>
		private void menuCamera_Solid_Click(object sender, System.EventArgs e)
		{
			menuCamera_Solid_Click();
		}
		#endregion

		#region Tab Controls
		/// <summary>
		/// Loads terrain history actions when the "History" tab is selected.
		/// </summary>
		private void tabControls_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ( tabControls.SelectedIndex == 2 )
			{
				vertManip.EnableVertexSelection( false );
				histManip.LoadHistoryActions();
			}
		}

		/// <summary>
		/// Updates how much of the vertex manipulation control is shown.
		/// </summary>
		private void vScrollVertices_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			Point p = vertManip.Location;

			p.Y = -vScrollVertices.Value;
			vertManip.Location = p;
		}
		
		/// <summary>
		/// Updates how much of the texture manipulation control is shown.
		/// </summary>
		private void vScrollTextures_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			Point p = texManip.Location;

			p.Y = -vScrollTextures.Value;
			texManip.Location = p;
		}
		
		/// <summary>
		/// Updates how much of the history manipulation control is shown.
		/// </summary>
		private void vScrollHistory_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			Point p = histManip.Location;

			p.Y = -vScrollHistory.Value;
			histManip.Location = p;
		}
		
		/// <summary>
		/// Updates how much of the light manipulation control is shown.
		/// </summary>
		private void vScrollLights_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			Point p = lightManip.Location;

			p.Y = -vScrollLights.Value;
			lightManip.Location = p;
		}
		#endregion

		#region Lua-Specific Functions
		/// <summary>
		/// Creates a piece of terrain with the specified parameters.
		/// </summary>
		/// <param name="rows">The number of rows in the TerrainPatch.</param>
		/// <param name="columns">The number of columns in the TerrainPatch.</param>
		/// <param name="height">The total height of the TerrainPatch.</param>
		/// <param name="width">The total width of the TerrainPatch.</param>
		/// <param name="name">The name of the TerrainPage.</param>
		[LuaFunctionAttribute( "CreateTerrain", "Creates a piece of terrain with the specified parameters.",
			 "The number of rows in the TerrainPatch.", "The number of columns in the TerrainPatch.",
			 "The total height of the TerrainPatch.", "The total width of the TerrainPatch.",
			 "The name of the TerrainPage." )]
		public void CreateTerrain( int rows, int columns, float height, float width, string name )
		{
			LoadTerrain( _terrainData.CreateTerrain( rows, columns, height, width, name ) );
		}

		/// <summary>
		/// Imports terrain using the specified plug-in.
		/// </summary>
		/// <param name="name">The name of the plug-in to use.</param>
		[LuaFunctionAttribute( "ImportTerrain", "Imports terrain using the specified plug-in.",
			 "The name of the plug-in to use." )]
		public void ImportTerrain( string name )
		{
			bool found = false;
			int count = 0;
			ArrayList plugins = _terrainData.PlugIns.FileImportPlugIns;

			while ( !found && count < plugins.Count )
			{
				if ( ( (PlugIn) plugins[count] ).GetName() == name )
					found = true;
				else
					count++;
			}

			if ( found )
			{
				if ( _terrainData.RunPlugIn( count, DataInterfacing.PlugIns.PlugInTypes.Importing, this ) )
				{
					vertManip.EnableTerrainEditing( true );
					texManip.LoadTextures();
				}
			}
		}

		/// <summary>
		/// Exports terrain using the specified plug-in.
		/// </summary>
		/// <param name="name">The name of the plug-in to use.</param>
		[LuaFunctionAttribute( "ExportTerrain", "Exports terrain using the specified plug-in.",
			 "The name of the plug-in to use." )]
		public void ExportTerrain( string name )
		{
			bool found = false;
			int count = 0;
			ArrayList plugins = _terrainData.PlugIns.FileExportPlugIns;

			while ( !found && count < plugins.Count )
			{
				if ( ( (PlugIn) plugins[count] ).GetName() == name )
					found = true;
				else
					count++;
			}

			if ( found )
			{
				if ( _terrainData.RunPlugIn( count, DataInterfacing.PlugIns.PlugInTypes.Exporting, this ) )
				{
					vertManip.EnableTerrainEditing( true );
					texManip.LoadTextures();
				}
			}
		}
		#endregion

		#region Other Form Callbacks
		/// <summary>
		/// Resizes and repositions the controls within the window when the Form is resized.
		/// </summary>
		private void MainForm_SizeChanged(object sender, System.EventArgs e)
		{
			Size newSize = new Size();
			Point newLocation = new Point();
			Size tabHeader = tabControls.Size - tpgVertices.Size;
			bool resume = true;

			// Make minimum size for MainForm
			if ( this.Size.Width < 500 )
			{
				newSize.Width = 500;
				newSize.Height = this.Size.Height;
				this.Size = newSize;
				resume = false;
			}

			if ( this.Size.Height < 352 )
			{
				newSize.Width = this.Size.Width;
				newSize.Height = 352;
				this.Size = newSize;
				resume = false;
			}

			if ( resume )
			{
				// Set size of top toolbar
				camManip.Width = this.ClientSize.Width;

				// Set size of tab controls
				newSize.Width = tabControls.Width;
				newSize.Height = this.ClientSize.Height - tabControls.Location.Y - statMain.Height;
				tabControls.Size = newSize;

				// Make the client area of each tab the same size
				tpgVertices.ClientSize = tabControls.Size - tabHeader;
				tpgTextures.ClientSize = tpgVertices.ClientSize;
				tpgHistory.ClientSize = tpgVertices.ClientSize;
				tpgLights.ClientSize = tpgVertices.ClientSize;

				// Set scrolling size of scroll bars
				vScrollVertices.Maximum = vertManip.Size.Height - tpgVertices.ClientSize.Height +
					vScrollVertices.LargeChange;
				vScrollTextures.Maximum = texManip.Size.Height - tpgVertices.ClientSize.Height +
					vScrollTextures.LargeChange;
				vScrollHistory.Maximum = histManip.Size.Height - tpgVertices.ClientSize.Height +
					vScrollHistory.LargeChange;
				vScrollLights.Maximum = lightManip.Size.Height - tpgVertices.ClientSize.Height +
					vScrollLights.LargeChange;

				// Set scroll bars to top position for convenience
				vScrollVertices.Minimum = 0;
				vScrollTextures.Minimum = 0;
				vScrollHistory.Minimum = 0;
				vScrollLights.Minimum = 0;
				vScrollVertices.Value = 0;
				vScrollTextures.Value = 0;
				vScrollHistory.Value = 0;
				vScrollLights.Value = 0;
				vScrollVertices_Scroll( this, null );
				vScrollTextures_Scroll( this, null );
				vScrollHistory_Scroll( this, null );
				vScrollLights_Scroll( this, null );

				// Set location of tab controls
				newLocation.X = this.ClientSize.Width - tabControls.Width;
				newLocation.Y = tabControls.Location.Y;
				tabControls.Location = newLocation;

				// Set size of scroll bars
				newSize.Width = vScrollVertices.Width;
				newSize.Height = tpgVertices.Height;
				vScrollVertices.Size = newSize;
				vScrollTextures.Size = newSize;
				vScrollHistory.Size = newSize;
				vScrollLights.Size = newSize;

				// Set size of DirectX viewport
				newSize.Width = this.ClientSize.Width - tabControls.Size.Width + 5;
				newSize.Height = this.ClientSize.Height - _viewport.Location.Y - statMain.Height + 10;
				_viewport.Size = newSize;

				// Set location of DirectX viewport
				newLocation.X = 0;
				newLocation.Y = _viewport.Location.Y;
				_viewport.Location = newLocation;

				// Resize DirectX device
				if ( _dx.Device != null )
				{
					_dx.ResizeWindow();
					SetupLights( this, e );
				}
			}
		}

		/// <summary>
		/// Performs mouse processing when the mouse is clicked over the main viewport.
		/// </summary>
		public virtual void pnlViewport_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( _terrainData.TerrainPage != null )
			{
				if ( _dx.Keyboard != null )
				{
					_dx.Keyboard.Update();

					if ( _dx.Keyboard.Keys[Key.LeftShift] || _dx.Keyboard.Keys[Key.RightShift] )
						_terrainData.EnableMultipleSelection = true;
					else
						_terrainData.EnableMultipleSelection = false;
				}

				if ( e.Button == MouseButtons.Left )
				{
					_dx.Camera.BeginMove();

					if ( _terrainData.EnableVertexMovement && !_terrainData.PauseVertexMovement &&
						!_dx.Mouse.CurrentButtons[1] )
					{
						_terrainData.BuildPickingRay( e.X, e.Y, _dx );
						_terrainData.SelectVertex( _dx, false );
						_terrainData.IsMoving = true;
					}
				}
				else if ( e.Button == MouseButtons.Right )
				{
					if ( vertManip.EnableVertexMovement.BackColor ==
						Color.FromKnownColor( KnownColor.ControlLight ) )
						_terrainData.EnableVertexMovement = true;
					else
						_terrainData.EnableVertexMovement = false;

					_terrainData.EnableMultipleSelection = false;

					if (_dx.Mouse.CurrentButtons[0])
					{
						_dx.Camera.CurrentMovement = QuaternionCamera.MovementType.Rotate;
						_terrainData.IsMoving = false;
					}
					else
						camManip.DisableCameraMovement();
				}
			
				vertManip.UpdateMaximumVertexHeight();
			}
		}

		/// <summary>
		/// Performs mouse processing when the mouse is clicked over the main viewport.
		/// </summary>
		public virtual void pnlViewport_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( _terrainData.TerrainPage != null &&
				_terrainData.EnableVertexMovement && !_terrainData.PauseVertexMovement )
			{
				_terrainData.BuildPickingRay( e.X, e.Y, _dx );
				_terrainData.SelectVertex( _dx, true );
				_terrainData.IsMoving = false;
				vertManip.UpdateMaximumVertexHeight();
			}
		}

		/// <summary>
		/// Displays notification that a drag-and-drop action is occurring.
		/// </summary>
		private void ViewportPanel_DragEnter(object sender, DragEventArgs e)
		{
			if ( e.Data.GetDataPresent( DataFormats.FileDrop, false ) == true )
				e.Effect = DragDropEffects.All;
		}

		/// <summary>
		/// Opens a drag-and-dropped file.
		/// </summary>
		private void ViewportPanel_DragDrop(object sender, DragEventArgs e)
		{
			bool proceed = true;
			string[] files = (string[]) e.Data.GetData( DataFormats.FileDrop );

			// Only accepts one file to load
			if ( files.Length > 0 && files[0] != null )
			{
				if ( _terrainData.TerrainPage != null )
				{
					DialogResult result = MessageBox.Show(
						"Do you wish to save your work (unsaved data will be lost)?",
						"Unsaved Data!", MessageBoxButtons.YesNoCancel );

					if ( result == DialogResult.Yes )
						menuFile_SaveAs_Click( sender, new EventArgs() );

					if ( result == DialogResult.Yes || result == DialogResult.No )
						proceed = true;
				}

				if ( proceed )
				{
					ImportTerrainProject.Driver project = new ImportTerrainProject.Driver();
					int index = -1;

					for ( int i = 0; i < _terrainData.PlugIns.FileImportPlugIns.Count; i++ )
					{
						if ( ( (PlugIn) _terrainData.PlugIns.FileImportPlugIns[i] ).GetName() == project.GetName() )
							index = i;
					}

					if ( _terrainData.RunPlugInAuto( index, DataInterfacing.PlugIns.PlugInTypes.Importing,
						this, files[0] ) )
					{
						vertManip.EnableTerrainEditing( true );
						_projectName = project.ProjectName;
					}
				}
			}
		}

		/// <summary>
		/// Performs mouse processing when the mouse wheel is scrolled over the main viewport.
		/// </summary>
		private void ViewportPanel_MouseWheel(object sender, MouseEventArgs e)
		{
			QuaternionCamera.MovementType moveType = _dx.Camera.CurrentMovement;
			bool moving = _dx.Camera.Moving;
			float truckSpeed = _dx.Camera.TruckSpeed;

			if (!moving)
				_dx.Camera.Moving = true;

			_dx.Camera.CurrentMovement = QuaternionCamera.MovementType.Truck;
			_dx.Camera.TruckSpeed = truckSpeed * 0.02f;	// UGLY! UGLY! Hackity hack-hack hack!

			// Zoom the camera based on mouse wheel movement
			_dx.Camera.Move(new Point(0, e.Delta));

			if (_dx.Camera.Moving != moving)
				_dx.Camera.Moving = moving;

			_dx.Camera.TruckSpeed = truckSpeed;
			_dx.Camera.CurrentMovement = moveType;
		}

		/// <summary>
		/// Un-toggles camera movement buttons.
		/// </summary>
		private void EnableVertexMovement_Click(object sender, EventArgs e)
		{
			if ( vertManip.EnableVertexMovement.BackColor == Color.FromKnownColor( KnownColor.ControlLight ) )
				camManip.DisableCameraMovement();
		}

		/// <summary>
		/// Safely disposes of the DirectX viewport.
		/// </summary>
		private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_terrainData.Dispose();
			_dx.Dispose();
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.statMain = new System.Windows.Forms.StatusBar();
			this.tabControls = new System.Windows.Forms.TabControl();
			this.tpgVertices = new System.Windows.Forms.TabPage();
			this.vertManip = new Voyage.Terraingine.VertexManipulation();
			this.vScrollVertices = new System.Windows.Forms.VScrollBar();
			this.tpgTextures = new System.Windows.Forms.TabPage();
			this.texManip = new Voyage.Terraingine.TextureManipulation();
			this.vScrollTextures = new System.Windows.Forms.VScrollBar();
			this.tpgHistory = new System.Windows.Forms.TabPage();
			this.histManip = new Voyage.Terraingine.HistoryManipulation();
			this.vScrollHistory = new System.Windows.Forms.VScrollBar();
			this.tpgLights = new System.Windows.Forms.TabPage();
			this.lightManip = new Voyage.Terraingine.LightManipulation();
			this.vScrollLights = new System.Windows.Forms.VScrollBar();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuFile = new System.Windows.Forms.MenuItem();
			this.menuFile_New = new System.Windows.Forms.MenuItem();
			this.menuFile_Open = new System.Windows.Forms.MenuItem();
			this.menuFile_Close = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuFile_Save = new System.Windows.Forms.MenuItem();
			this.menuFile_SaveAs = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuFile_Import = new System.Windows.Forms.MenuItem();
			this.menuFile_Import_ImportTerrain = new System.Windows.Forms.MenuItem();
			this.menuFile_Import_LoadMethod = new System.Windows.Forms.MenuItem();
			this.menuFile_Export = new System.Windows.Forms.MenuItem();
			this.fileMenu_Export_ExportTerrain = new System.Windows.Forms.MenuItem();
			this.menuFile_Export_LoadMethod = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			this.menuFile_Exit = new System.Windows.Forms.MenuItem();
			this.menuEdit = new System.Windows.Forms.MenuItem();
			this.menuEdit_Undo = new System.Windows.Forms.MenuItem();
			this.menuEdit_Redo = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuEdit_Copy = new System.Windows.Forms.MenuItem();
			this.menuEdit_Paste = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuView_Default = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuView_SelectAll = new System.Windows.Forms.MenuItem();
			this.menuView_SelectInverse = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuView_DefaultColor = new System.Windows.Forms.MenuItem();
			this.menuView_HeightColor = new System.Windows.Forms.MenuItem();
			this.menuItem11 = new System.Windows.Forms.MenuItem();
			this.menuView_SizePlus = new System.Windows.Forms.MenuItem();
			this.menuView_SizeMinus = new System.Windows.Forms.MenuItem();
			this.menuItem12 = new System.Windows.Forms.MenuItem();
			this.menuView_ScreenCapture = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuView_Script = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuHelp_Help = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.menuHelp_About = new System.Windows.Forms.MenuItem();
			this.menuCamera = new System.Windows.Forms.MenuItem();
			this.menuCamera_Rotate = new System.Windows.Forms.MenuItem();
			this.menuCamera_Pan = new System.Windows.Forms.MenuItem();
			this.menuCamera_Zoom = new System.Windows.Forms.MenuItem();
			this.menuCamera_Reset = new System.Windows.Forms.MenuItem();
			this.menuCamera_Wireframe = new System.Windows.Forms.MenuItem();
			this.menuCamera_Solid = new System.Windows.Forms.MenuItem();
			this.camManip = new Voyage.Terraingine.CameraManipulation();
			this.tabControls.SuspendLayout();
			this.tpgVertices.SuspendLayout();
			this.tpgTextures.SuspendLayout();
			this.tpgHistory.SuspendLayout();
			this.tpgLights.SuspendLayout();
			this.SuspendLayout();
			// 
			// _viewport
			// 
			this._viewport.Location = new System.Drawing.Point(0, 48);
			this._viewport.Name = "_viewport";
			this._viewport.Size = new System.Drawing.Size(504, 376);
			// 
			// statMain
			// 
			this.statMain.Location = new System.Drawing.Point(0, 419);
			this.statMain.Name = "statMain";
			this.statMain.Size = new System.Drawing.Size(696, 22);
			this.statMain.TabIndex = 13;
			// 
			// tabControls
			// 
			this.tabControls.Controls.Add(this.tpgVertices);
			this.tabControls.Controls.Add(this.tpgTextures);
			this.tabControls.Controls.Add(this.tpgHistory);
			this.tabControls.Controls.Add(this.tpgLights);
			this.tabControls.Location = new System.Drawing.Point(496, 56);
			this.tabControls.Name = "tabControls";
			this.tabControls.SelectedIndex = 0;
			this.tabControls.ShowToolTips = true;
			this.tabControls.Size = new System.Drawing.Size(200, 360);
			this.tabControls.TabIndex = 15;
			this.tabControls.SelectedIndexChanged += new System.EventHandler(this.tabControls_SelectedIndexChanged);
			// 
			// tpgVertices
			// 
			this.tpgVertices.Controls.Add(this.vertManip);
			this.tpgVertices.Controls.Add(this.vScrollVertices);
			this.tpgVertices.Location = new System.Drawing.Point(4, 22);
			this.tpgVertices.Name = "tpgVertices";
			this.tpgVertices.Size = new System.Drawing.Size(192, 334);
			this.tpgVertices.TabIndex = 0;
			this.tpgVertices.Text = "Vertices";
			this.tpgVertices.ToolTipText = "Modify Terrain Vertices";
			// 
			// vertManip
			// 
			this.vertManip.EnableDataUpdates = false;
			this.vertManip.Location = new System.Drawing.Point(0, 0);
			this.vertManip.Name = "vertManip";
			this.vertManip.Size = new System.Drawing.Size(176, 736);
			this.vertManip.TabIndex = 2;
			// 
			// vScrollVertices
			// 
			this.vScrollVertices.LargeChange = 300;
			this.vScrollVertices.Location = new System.Drawing.Point(176, 0);
			this.vScrollVertices.Maximum = 500;
			this.vScrollVertices.Name = "vScrollVertices";
			this.vScrollVertices.Size = new System.Drawing.Size(17, 336);
			this.vScrollVertices.SmallChange = 10;
			this.vScrollVertices.TabIndex = 1;
			this.vScrollVertices.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollVertices_Scroll);
			// 
			// tpgTextures
			// 
			this.tpgTextures.Controls.Add(this.texManip);
			this.tpgTextures.Controls.Add(this.vScrollTextures);
			this.tpgTextures.Location = new System.Drawing.Point(4, 22);
			this.tpgTextures.Name = "tpgTextures";
			this.tpgTextures.Size = new System.Drawing.Size(192, 334);
			this.tpgTextures.TabIndex = 1;
			this.tpgTextures.Text = "Textures";
			this.tpgTextures.ToolTipText = "Modify Terrain Textures";
			this.tpgTextures.Visible = false;
			// 
			// texManip
			// 
			this.texManip.EnableDataUpdates = false;
			this.texManip.Location = new System.Drawing.Point(0, 0);
			this.texManip.Name = "texManip";
			this.texManip.Size = new System.Drawing.Size(176, 712);
			this.texManip.TabIndex = 1;
			// 
			// vScrollTextures
			// 
			this.vScrollTextures.LargeChange = 300;
			this.vScrollTextures.Location = new System.Drawing.Point(176, 0);
			this.vScrollTextures.Maximum = 500;
			this.vScrollTextures.Name = "vScrollTextures";
			this.vScrollTextures.Size = new System.Drawing.Size(17, 336);
			this.vScrollTextures.SmallChange = 10;
			this.vScrollTextures.TabIndex = 0;
			this.vScrollTextures.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollTextures_Scroll);
			// 
			// tpgHistory
			// 
			this.tpgHistory.Controls.Add(this.histManip);
			this.tpgHistory.Controls.Add(this.vScrollHistory);
			this.tpgHistory.Location = new System.Drawing.Point(4, 22);
			this.tpgHistory.Name = "tpgHistory";
			this.tpgHistory.Size = new System.Drawing.Size(192, 334);
			this.tpgHistory.TabIndex = 2;
			this.tpgHistory.Text = "History";
			this.tpgHistory.ToolTipText = "Undo/Redo Terrain History";
			// 
			// histManip
			// 
			this.histManip.Location = new System.Drawing.Point(0, 0);
			this.histManip.Name = "histManip";
			this.histManip.Size = new System.Drawing.Size(176, 304);
			this.histManip.TabIndex = 2;
			// 
			// vScrollHistory
			// 
			this.vScrollHistory.LargeChange = 300;
			this.vScrollHistory.Location = new System.Drawing.Point(176, 0);
			this.vScrollHistory.Maximum = 500;
			this.vScrollHistory.Name = "vScrollHistory";
			this.vScrollHistory.Size = new System.Drawing.Size(17, 336);
			this.vScrollHistory.SmallChange = 10;
			this.vScrollHistory.TabIndex = 1;
			this.vScrollHistory.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollHistory_Scroll);
			// 
			// tpgLights
			// 
			this.tpgLights.Controls.Add(this.lightManip);
			this.tpgLights.Controls.Add(this.vScrollLights);
			this.tpgLights.Location = new System.Drawing.Point(4, 22);
			this.tpgLights.Name = "tpgLights";
			this.tpgLights.Size = new System.Drawing.Size(192, 334);
			this.tpgLights.TabIndex = 3;
			this.tpgLights.Text = "Lights";
			this.tpgLights.ToolTipText = "Adjust Terrain Lighting";
			// 
			// lightManip
			// 
			this.lightManip.EnableDataUpdates = false;
			this.lightManip.Location = new System.Drawing.Point(0, 0);
			this.lightManip.Name = "lightManip";
			this.lightManip.Size = new System.Drawing.Size(176, 248);
			this.lightManip.TabIndex = 1;
			// 
			// vScrollLights
			// 
			this.vScrollLights.LargeChange = 300;
			this.vScrollLights.Location = new System.Drawing.Point(176, 0);
			this.vScrollLights.Maximum = 500;
			this.vScrollLights.Name = "vScrollLights";
			this.vScrollLights.Size = new System.Drawing.Size(17, 336);
			this.vScrollLights.SmallChange = 10;
			this.vScrollLights.TabIndex = 0;
			this.vScrollLights.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollLights_Scroll);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuFile,
																					  this.menuEdit,
																					  this.menuItem5,
																					  this.menuItem6,
																					  this.menuCamera});
			// 
			// menuFile
			// 
			this.menuFile.Index = 0;
			this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuFile_New,
																					 this.menuFile_Open,
																					 this.menuFile_Close,
																					 this.menuItem7,
																					 this.menuFile_Save,
																					 this.menuFile_SaveAs,
																					 this.menuItem1,
																					 this.menuFile_Import,
																					 this.menuFile_Export,
																					 this.menuItem10,
																					 this.menuFile_Exit});
			this.menuFile.Text = "&File";
			// 
			// menuFile_New
			// 
			this.menuFile_New.Index = 0;
			this.menuFile_New.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.menuFile_New.Text = "&New...";
			this.menuFile_New.Click += new System.EventHandler(this.menuFile_New_Click);
			// 
			// menuFile_Open
			// 
			this.menuFile_Open.Index = 1;
			this.menuFile_Open.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.menuFile_Open.Text = "&Open...";
			this.menuFile_Open.Click += new System.EventHandler(this.menuFile_Open_Click);
			// 
			// menuFile_Close
			// 
			this.menuFile_Close.Index = 2;
			this.menuFile_Close.Shortcut = System.Windows.Forms.Shortcut.CtrlW;
			this.menuFile_Close.Text = "&Close";
			this.menuFile_Close.Click += new System.EventHandler(this.menuFile_Close_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 3;
			this.menuItem7.Text = "-";
			// 
			// menuFile_Save
			// 
			this.menuFile_Save.Index = 4;
			this.menuFile_Save.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.menuFile_Save.Text = "&Save Terrain Page";
			this.menuFile_Save.Click += new System.EventHandler(this.menuFile_Save_Click);
			// 
			// menuFile_SaveAs
			// 
			this.menuFile_SaveAs.Index = 5;
			this.menuFile_SaveAs.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftS;
			this.menuFile_SaveAs.Text = "Save Terrain Page &As...";
			this.menuFile_SaveAs.Click += new System.EventHandler(this.menuFile_SaveAs_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 6;
			this.menuItem1.Text = "-";
			// 
			// menuFile_Import
			// 
			this.menuFile_Import.Index = 7;
			this.menuFile_Import.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							this.menuFile_Import_ImportTerrain,
																							this.menuFile_Import_LoadMethod});
			this.menuFile_Import.Text = "&Import...";
			// 
			// menuFile_Import_ImportTerrain
			// 
			this.menuFile_Import_ImportTerrain.Index = 0;
			this.menuFile_Import_ImportTerrain.Text = "&Import Terrain...";
			this.menuFile_Import_ImportTerrain.Click += new System.EventHandler(this.menuFile_Import_ImportTerrain_Click);
			// 
			// menuFile_Import_LoadMethod
			// 
			this.menuFile_Import_LoadMethod.Index = 1;
			this.menuFile_Import_LoadMethod.Text = "&Load New Import Method...";
			this.menuFile_Import_LoadMethod.Click += new System.EventHandler(this.menuFile_Import_LoadMethod_Click);
			// 
			// menuFile_Export
			// 
			this.menuFile_Export.Index = 8;
			this.menuFile_Export.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							this.fileMenu_Export_ExportTerrain,
																							this.menuFile_Export_LoadMethod});
			this.menuFile_Export.Text = "&Export...";
			// 
			// fileMenu_Export_ExportTerrain
			// 
			this.fileMenu_Export_ExportTerrain.Index = 0;
			this.fileMenu_Export_ExportTerrain.Text = "&Export Terrain...";
			this.fileMenu_Export_ExportTerrain.Click += new System.EventHandler(this.fileMenu_Export_ExportTerrain_Click);
			// 
			// menuFile_Export_LoadMethod
			// 
			this.menuFile_Export_LoadMethod.Index = 1;
			this.menuFile_Export_LoadMethod.Text = "&Load New Export Method...";
			this.menuFile_Export_LoadMethod.Click += new System.EventHandler(this.menuFile_Export_LoadMethod_Click);
			// 
			// menuItem10
			// 
			this.menuItem10.Index = 9;
			this.menuItem10.Text = "-";
			// 
			// menuFile_Exit
			// 
			this.menuFile_Exit.Index = 10;
			this.menuFile_Exit.Text = "E&xit";
			this.menuFile_Exit.Click += new System.EventHandler(this.menuFile_Exit_Click);
			// 
			// menuEdit
			// 
			this.menuEdit.Index = 1;
			this.menuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuEdit_Undo,
																					 this.menuEdit_Redo,
																					 this.menuItem2,
																					 this.menuEdit_Copy,
																					 this.menuEdit_Paste});
			this.menuEdit.Text = "&Edit";
			// 
			// menuEdit_Undo
			// 
			this.menuEdit_Undo.Index = 0;
			this.menuEdit_Undo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
			this.menuEdit_Undo.Text = "&Undo Last Action";
			this.menuEdit_Undo.Click += new System.EventHandler(this.menuEdit_Undo_Click);
			// 
			// menuEdit_Redo
			// 
			this.menuEdit_Redo.Index = 1;
			this.menuEdit_Redo.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
			this.menuEdit_Redo.Text = "&Redo Last Action";
			this.menuEdit_Redo.Click += new System.EventHandler(this.menuEdit_Redo_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 2;
			this.menuItem2.Text = "-";
			// 
			// menuEdit_Copy
			// 
			this.menuEdit_Copy.Index = 3;
			this.menuEdit_Copy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			this.menuEdit_Copy.Text = "&Copy Vertex Height";
			this.menuEdit_Copy.Click += new System.EventHandler(this.menuEdit_Copy_Click);
			// 
			// menuEdit_Paste
			// 
			this.menuEdit_Paste.Index = 4;
			this.menuEdit_Paste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
			this.menuEdit_Paste.Text = "&Paste Vertex Height";
			this.menuEdit_Paste.Click += new System.EventHandler(this.menuEdit_Paste_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 2;
			this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuView_Default,
																					  this.menuItem3,
																					  this.menuView_SelectAll,
																					  this.menuView_SelectInverse,
																					  this.menuItem4,
																					  this.menuView_DefaultColor,
																					  this.menuView_HeightColor,
																					  this.menuItem11,
																					  this.menuView_SizePlus,
																					  this.menuView_SizeMinus,
																					  this.menuItem12,
																					  this.menuView_ScreenCapture,
																					  this.menuItem8,
																					  this.menuView_Script});
			this.menuItem5.Text = "&View";
			// 
			// menuView_Default
			// 
			this.menuView_Default.Index = 0;
			this.menuView_Default.Shortcut = System.Windows.Forms.Shortcut.CtrlD;
			this.menuView_Default.Text = "&Default View";
			this.menuView_Default.Click += new System.EventHandler(this.menuView_Default_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Text = "-";
			// 
			// menuView_SelectAll
			// 
			this.menuView_SelectAll.Index = 2;
			this.menuView_SelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
			this.menuView_SelectAll.Text = "&Select All Vertices";
			this.menuView_SelectAll.Click += new System.EventHandler(this.menuView_SelectAll_Click);
			// 
			// menuView_SelectInverse
			// 
			this.menuView_SelectInverse.Index = 3;
			this.menuView_SelectInverse.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
			this.menuView_SelectInverse.Text = "Select &Inverse Vertices";
			this.menuView_SelectInverse.Click += new System.EventHandler(this.menuView_SelectInverse_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 4;
			this.menuItem4.Text = "-";
			// 
			// menuView_DefaultColor
			// 
			this.menuView_DefaultColor.Index = 5;
			this.menuView_DefaultColor.Text = "Default Terrain &Coloring";
			this.menuView_DefaultColor.Click += new System.EventHandler(this.menuView_DefaultColor_Click);
			// 
			// menuView_HeightColor
			// 
			this.menuView_HeightColor.Index = 6;
			this.menuView_HeightColor.Text = "Terrain Coloring By &Height";
			this.menuView_HeightColor.Click += new System.EventHandler(this.menuView_HeightColor_Click);
			// 
			// menuItem11
			// 
			this.menuItem11.Index = 7;
			this.menuItem11.Text = "-";
			// 
			// menuView_SizePlus
			// 
			this.menuView_SizePlus.Index = 8;
			this.menuView_SizePlus.Shortcut = System.Windows.Forms.Shortcut.CtrlShift9;
			this.menuView_SizePlus.Text = "I&ncrease Vertex Size";
			this.menuView_SizePlus.Click += new System.EventHandler(this.menuView_SizePlus_Click);
			// 
			// menuView_SizeMinus
			// 
			this.menuView_SizeMinus.Index = 9;
			this.menuView_SizeMinus.Shortcut = System.Windows.Forms.Shortcut.CtrlShift0;
			this.menuView_SizeMinus.Text = "D&ecrease Vertex Size";
			this.menuView_SizeMinus.Click += new System.EventHandler(this.menuView_SizeMinus_Click);
			// 
			// menuItem12
			// 
			this.menuItem12.Index = 10;
			this.menuItem12.Text = "-";
			this.menuItem12.Visible = false;
			// 
			// menuView_ScreenCapture
			// 
			this.menuView_ScreenCapture.Index = 11;
			this.menuView_ScreenCapture.Text = "&Take Screen Capture";
			this.menuView_ScreenCapture.Visible = false;
			this.menuView_ScreenCapture.Click += new System.EventHandler(this.menuView_ScreenCapture_Click);
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 12;
			this.menuItem8.Text = "-";
			// 
			// menuView_Script
			// 
			this.menuView_Script.Index = 13;
			this.menuView_Script.Text = "&Open Scripting Window";
			this.menuView_Script.Click += new System.EventHandler(this.menuView_Script_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 3;
			this.menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuHelp_Help,
																					  this.menuItem9,
																					  this.menuHelp_About});
			this.menuItem6.Text = "&Help";
			// 
			// menuHelp_Help
			// 
			this.menuHelp_Help.Index = 0;
			this.menuHelp_Help.Shortcut = System.Windows.Forms.Shortcut.F1;
			this.menuHelp_Help.Text = "&View Help Topics...";
			this.menuHelp_Help.Click += new System.EventHandler(this.menuHelp_Help_Click);
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 1;
			this.menuItem9.Text = "-";
			// 
			// menuHelp_About
			// 
			this.menuHelp_About.Index = 2;
			this.menuHelp_About.Text = "&About Voyage Terrain Generator...";
			this.menuHelp_About.Click += new System.EventHandler(this.menuHelp_About_Click);
			// 
			// menuCamera
			// 
			this.menuCamera.Index = 4;
			this.menuCamera.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.menuCamera_Rotate,
																					   this.menuCamera_Pan,
																					   this.menuCamera_Zoom,
																					   this.menuCamera_Reset,
																					   this.menuCamera_Wireframe,
																					   this.menuCamera_Solid});
			this.menuCamera.Text = "Camera";
			this.menuCamera.Visible = false;
			// 
			// menuCamera_Rotate
			// 
			this.menuCamera_Rotate.Index = 0;
			this.menuCamera_Rotate.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftR;
			this.menuCamera_Rotate.Text = "Rotate";
			this.menuCamera_Rotate.Click += new System.EventHandler(this.menuCamera_Rotate_Click);
			// 
			// menuCamera_Pan
			// 
			this.menuCamera_Pan.Index = 1;
			this.menuCamera_Pan.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftP;
			this.menuCamera_Pan.Text = "Pan";
			this.menuCamera_Pan.Click += new System.EventHandler(this.menuCamera_Pan_Click);
			// 
			// menuCamera_Zoom
			// 
			this.menuCamera_Zoom.Index = 2;
			this.menuCamera_Zoom.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftZ;
			this.menuCamera_Zoom.Text = "Zoom";
			this.menuCamera_Zoom.Click += new System.EventHandler(this.menuCamera_Zoom_Click);
			// 
			// menuCamera_Reset
			// 
			this.menuCamera_Reset.Index = 3;
			this.menuCamera_Reset.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftQ;
			this.menuCamera_Reset.Text = "Reset";
			this.menuCamera_Reset.Click += new System.EventHandler(this.menuCamera_Reset_Click);
			// 
			// menuCamera_Wireframe
			// 
			this.menuCamera_Wireframe.Index = 4;
			this.menuCamera_Wireframe.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftW;
			this.menuCamera_Wireframe.Text = "Wireframe";
			this.menuCamera_Wireframe.Click += new System.EventHandler(this.menuCamera_Wireframe_Click);
			// 
			// menuCamera_Solid
			// 
			this.menuCamera_Solid.Index = 5;
			this.menuCamera_Solid.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftE;
			this.menuCamera_Solid.Text = "Solid";
			this.menuCamera_Solid.Click += new System.EventHandler(this.menuCamera_Solid_Click);
			// 
			// camManip
			// 
			this.camManip.EnableDataUpdates = false;
			this.camManip.Location = new System.Drawing.Point(0, 0);
			this.camManip.Name = "camManip";
			this.camManip.Size = new System.Drawing.Size(552, 56);
			this.camManip.TabIndex = 16;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(696, 441);
			this.Controls.Add(this.camManip);
			this.Controls.Add(this.tabControls);
			this.Controls.Add(this.statMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Location = new System.Drawing.Point(0, 0);
			this.Menu = this.mainMenu1;
			this.Name = "MainForm";
			this.Text = "Voyage Terrain Generator";
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlViewport_MouseDown);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
			this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlViewport_MouseUp);
			this.Controls.SetChildIndex(this._viewport, 0);
			this.Controls.SetChildIndex(this.statMain, 0);
			this.Controls.SetChildIndex(this.tabControls, 0);
			this.Controls.SetChildIndex(this.camManip, 0);
			this.tabControls.ResumeLayout(false);
			this.tpgVertices.ResumeLayout(false);
			this.tpgTextures.ResumeLayout(false);
			this.tpgHistory.ResumeLayout(false);
			this.tpgLights.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
