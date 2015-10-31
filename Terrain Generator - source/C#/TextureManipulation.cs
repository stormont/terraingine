using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Collections.Specialized;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataInterfacing;
using Voyage.Terraingine.DXViewport;
using Voyage.LuaNetInterface;

namespace Voyage.Terraingine
{
	/// <summary>
	/// A user control for manipulating terrain textures.
	/// </summary>
	public class TextureManipulation : System.Windows.Forms.UserControl
	{
		#region Data Members
		private DataInterfacing.ViewportInterface	_viewport;
		private DataInterfacing.DataManipulation	_terrainData;
		private NameValueCollection	_textureAlgorithms;
		private TerrainViewport		_owner;
		private DXViewport.Viewport	_dx;
		private bool					_updateData;

		private System.Windows.Forms.GroupBox grpTextures_Operation;
		private System.Windows.Forms.ComboBox cmbTextures_Operation;
		private System.Windows.Forms.GroupBox grpTextures_Name;
		private System.Windows.Forms.Button btnTextures_Name;
		private System.Windows.Forms.TextBox txtTextures_Name;
		private System.Windows.Forms.GroupBox grpTextures_Sizing;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown numTextures_vScale;
		private System.Windows.Forms.NumericUpDown numTextures_uScale;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown numTextures_vShift;
		private System.Windows.Forms.NumericUpDown numTextures_uShift;
		private System.Windows.Forms.GroupBox grpTextures_Textures;
		private System.Windows.Forms.Button btnTextures_DisableAll;
		private System.Windows.Forms.Button btnTextures_Disable;
		private System.Windows.Forms.Button btnTextures_MoveDown;
		private System.Windows.Forms.Button btnTextures_MoveUp;
		private System.Windows.Forms.Button btnTextures_RemoveTex;
		private System.Windows.Forms.TreeView treeTextures;
		private System.Windows.Forms.Button btnTextures_AddTex;
		private System.Windows.Forms.GroupBox grpTextureAlgorithms;
		private System.Windows.Forms.Button btnLoadAlgorithm;
		private System.Windows.Forms.Button btnRunAlgorithm;
		private System.Windows.Forms.ListBox lstAlgorithms;

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
		#endregion

		#region Basic Form Methods
		/// <summary>
		/// Creates a texture manipulation user control.
		/// </summary>
		public TextureManipulation()
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
			_textureAlgorithms = new NameValueCollection();
			_updateData = true;

			// Register tooltips
			ToolTip t = new ToolTip();

			// Textures group tooltips
			t.SetToolTip( btnTextures_AddTex, "Add a texture to the terrain" );
			t.SetToolTip( btnTextures_RemoveTex, "Remove the selected texture from the terrain" );
			t.SetToolTip( btnTextures_MoveUp, "Move the selected texture up one level" );
			t.SetToolTip( btnTextures_MoveDown, "Move the selected texture down one level" );
			t.SetToolTip( btnTextures_Disable, "Disable the selected texture" );
			t.SetToolTip( btnTextures_DisableAll, "Disable all textures" );

			// Texture Name group tooltips
			t.SetToolTip( btnTextures_Name, "Change the name of the selected texture" );
		}
		#endregion

		#region Event Methods
		/// <summary>
		/// Displays notification that a drag-and-drop action is occurring.
		/// </summary>
		private void treeTextures_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if ( e.Data.GetDataPresent( DataFormats.FileDrop, false ) == true )
				e.Effect = DragDropEffects.All;
		}

		/// <summary>
		/// Opens a drag-and-dropped texture.
		/// </summary>
		private void treeTextures_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			string[] files = (string[]) e.Data.GetData( DataFormats.FileDrop );
			DialogResult result = DialogResult.OK;

			if ( _terrainData.TerrainPage.TerrainPatch.NumTextures >=
				_viewport.DXViewport.Device.DeviceCaps.MaxSimultaneousTextures )
			{
				result = MessageBox.Show( "The maximum number of textures for your video card have " +
					"been loaded. Additional textures will not be rendered.\n\nDo you wish to proceed?",
					"Maximum Textures Loaded", MessageBoxButtons.OKCancel );

				if ( result == DialogResult.OK )
				{
					foreach ( string s in files )
						LoadTexture( s );
				}
			}
		}

		/// <summary>
		/// Changes the selection of the current texture.
		/// </summary>
		private void treeTextures_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			treeTextures_AfterSelect();
		}

		/// <summary>
		/// Adds a texture to the TerrainPage.
		/// Supports the following file formats: .bmp, .dds, .dib, .hdr, .jpg, .pfm, .png, .ppm, .tga
		/// </summary>
		private void btnTextures_AddTex_Click(object sender, System.EventArgs e)
		{
			btnTextures_AddTex_Click();
		}

		/// <summary>
		/// Removes a texture from the TerrainPage.
		/// </summary>
		private void btnTextures_RemoveTex_Click(object sender, System.EventArgs e)
		{
			btnTextures_RemoveTex_Click();
		}

		/// <summary>
		/// Moves the selected texture up one layer on the TerrainPatch.
		/// </summary>
		private void btnTextures_MoveUp_Click(object sender, System.EventArgs e)
		{
			btnTextures_MoveUp_Click();
		}

		/// <summary>
		/// Moves the selected texture down one layer on the TerrainPatch.
		/// </summary>
		private void btnTextures_MoveDown_Click(object sender, System.EventArgs e)
		{
			btnTextures_MoveDown_Click();
		}

		/// <summary>
		/// Disables the rendering of the selected texture.
		/// </summary>
		private void btnTextures_Disable_Click(object sender, System.EventArgs e)
		{
			btnTextures_Disable_Click();
		}

		/// <summary>
		/// Disables the rendering of all textures.
		/// </summary>
		private void btnTextures_DisableAll_Click(object sender, System.EventArgs e)
		{
			btnTextures_DisableAll_Click();
		}
		
		/// <summary>
		/// Renames the currently selected texture.
		/// </summary>
		private void txtTextures_Name_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if ( e.KeyCode == Keys.Enter )
				btnTextures_Name_Click();
		}

		/// <summary>
		/// Renames the currently selected texture.
		/// </summary>
		private void btnTextures_Name_Click(object sender, System.EventArgs e)
		{
			btnTextures_Name_Click();
		}
		
		/// <summary>
		/// Changes the texture blending operation for the selected texture when rendering.
		/// </summary>
		private void cmbTextures_Operation_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			cmbTextures_Operation_SelectedIndexChanged();
		}

		/// <summary>
		/// Shifts the U-coordinates of the current texture.
		/// </summary>
		private void numTextures_uShift_ValueChanged(object sender, System.EventArgs e)
		{
			ShiftValuesChanged( false );
		}

		/// <summary>
		/// Shifts the U-coordinates of the current texture.
		/// </summary>
		private void numTextures_uShift_Leave(object sender, System.EventArgs e)
		{
			ShiftValuesChanged( true );
		}

		/// <summary>
		/// Shifts the V-coordinates of the current texture.
		/// </summary>
		private void numTextures_vShift_ValueChanged(object sender, System.EventArgs e)
		{
			ShiftValuesChanged( false );
		}

		/// <summary>
		/// Shifts the V-coordinates of the current texture.
		/// </summary>
		private void numTextures_vShift_Leave(object sender, System.EventArgs e)
		{
			ShiftValuesChanged( true );
		}

		/// <summary>
		/// Scales the U-coordinates of the current texture.
		/// </summary>
		private void numTextures_uScale_ValueChanged(object sender, System.EventArgs e)
		{
			ScaleValuesChanged( false );
		}

		/// <summary>
		/// Scales the U-coordinates of the current texture.
		/// </summary>
		private void numTextures_uScale_Leave(object sender, System.EventArgs e)
		{
			ScaleValuesChanged( true );
		}

		/// <summary>
		/// Scales the V-coordinates of the current texture.
		/// </summary>
		private void numTextures_vScale_ValueChanged(object sender, System.EventArgs e)
		{
			ScaleValuesChanged( false );
		}

		/// <summary>
		/// Scales the V-coordinates of the current texture.
		/// </summary>
		private void numTextures_vScale_Leave(object sender, System.EventArgs e)
		{
			ScaleValuesChanged( true );
		}
		/// <summary>
		/// Enables the texture manipulation "Run Algorithm" button if an item is selected.
		/// </summary>
		private void lstAlgorithms_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			lstAlgorithms_SelectedIndexChanged();
		}

		/// <summary>
		/// Runs the texture manipulation "Run Algorithm" button if an item is being clicked.
		/// </summary>
		private void lstAlgorithms_DoubleClick(object sender, System.EventArgs e)
		{
			lstAlgorithms_DoubleClick();
		}

		/// <summary>
		/// Runs the selected texture manipulation algorithm.
		/// </summary>
		private void btnRunAlgorithm_Click(object sender, System.EventArgs e)
		{
			btnRunAlgorithm_Click();
		}

		/// <summary>
		/// Loads a new texture manipulation plug-in.
		/// </summary>
		private void btnLoadAlgorithm_Click(object sender, System.EventArgs e)
		{
			btnLoadAlgorithm_Click();
		}
		#endregion

		#region Textures
		/// <summary>
		/// Changes the selection of the current texture.
		/// </summary>
		public void treeTextures_AfterSelect()
		{
			if ( treeTextures.SelectedNode != null )
				SetTextureSelectedState( treeTextures.Nodes.Count - treeTextures.SelectedNode.Index - 1 );
			else
				SetTextureSelectedState( -1 );
		}

		/// <summary>
		/// Adds a texture to the TerrainPage.
		/// Supports the following file formats: .bmp, .dds, .dib, .hdr, .jpg, .pfm, .png, .ppm, .tga
		/// </summary>
		public void btnTextures_AddTex_Click()
		{
			if ( _terrainData.TerrainPage != null )
			{
				OpenFileDialog dlg = new OpenFileDialog();
				DialogResult result = DialogResult.OK;

				if ( _terrainData.TerrainPage.TerrainPatch.NumTextures >=
					_viewport.DXViewport.Device.DeviceCaps.MaxSimultaneousTextures )
				{
					result = MessageBox.Show( "The maximum number of textures for your video card have " +
						"been loaded. Additional textures will not be rendered.\n\nDo you wish to proceed?",
						"Maximum Textures Loaded", MessageBoxButtons.OKCancel );
				}

				if ( result == DialogResult.OK )
				{
					dlg.InitialDirectory = Application.ExecutablePath.Substring( 0,
						Application.ExecutablePath.LastIndexOf( "\\" ) ) + "\\Images";
					result = dlg.ShowDialog( this );

					if ( result == DialogResult.OK && dlg.FileName != null )
						LoadTexture( dlg.FileName );
				}
			}
			else
				MessageBox.Show( "Must have terrain to load texture onto!", "Cannot Load Texture",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
		}

		/// <summary>
		/// Removes a texture from the TerrainPage.
		/// </summary>
		public void btnTextures_RemoveTex_Click()
		{
			if ( _terrainData.TerrainPage.TerrainPatch.SelectedTextureIndex > -1 )
			{
				_terrainData.StoreCurrentPage( "Remove Texture" );
				_terrainData.TerrainPage.TerrainPatch.RemoveTexture();
				treeTextures.Nodes.Remove( treeTextures.SelectedNode );

				if ( treeTextures.Nodes.Count == 0 )
					treeTextures_AfterSelect( this, new TreeViewEventArgs( null ) );
			}
		}

		/// <summary>
		/// Moves the selected texture up one layer on the TerrainPatch.
		/// </summary>
		public void btnTextures_MoveUp_Click()
		{
			if ( treeTextures.SelectedNode.Index > 0 )
			{
				TreeNode node = treeTextures.SelectedNode;
				int index = treeTextures.SelectedNode.Index;
				int startIndex = treeTextures.SelectedNode.Index;
				object tex;

				treeTextures.Nodes.RemoveAt( treeTextures.SelectedNode.Index );
				treeTextures.Nodes.Insert( index - 1, node );

				_terrainData.StoreCurrentPage( "Change Texture Layer" );
				index = treeTextures.Nodes.Count - index - 1;
				tex = _terrainData.TerrainPage.TerrainPatch.Textures[index];
				_terrainData.TerrainPage.TerrainPatch.Textures[index] =
					_terrainData.TerrainPage.TerrainPatch.Textures[index + 1];
				_terrainData.TerrainPage.TerrainPatch.Textures[index + 1] = tex;

				treeTextures.SelectedNode = treeTextures.Nodes[startIndex - 1];
			}
		}

		/// <summary>
		/// Moves the selected texture down one layer on the TerrainPatch.
		/// </summary>
		public void btnTextures_MoveDown_Click()
		{
			if ( treeTextures.SelectedNode.Index < treeTextures.Nodes.Count - 1 )
			{
				TreeNode node = treeTextures.SelectedNode;
				int index = treeTextures.SelectedNode.Index;
				int startIndex = treeTextures.SelectedNode.Index;
				object tex;

				treeTextures.Nodes.RemoveAt( treeTextures.SelectedNode.Index );
				treeTextures.Nodes.Insert( index + 1, node );

				_terrainData.StoreCurrentPage( "Change Texture Layer" );
				index = treeTextures.Nodes.Count - index - 1;
				tex = _terrainData.TerrainPage.TerrainPatch.Textures[index];
				_terrainData.TerrainPage.TerrainPatch.Textures[index] =
					_terrainData.TerrainPage.TerrainPatch.Textures[index - 1];
				_terrainData.TerrainPage.TerrainPatch.Textures[index - 1] = tex;

				treeTextures.SelectedNode = treeTextures.Nodes[startIndex + 1];
			}
		}

		/// <summary>
		/// Disables the rendering of the selected texture.
		/// </summary>
		public void btnTextures_Disable_Click()
		{
			if ( _terrainData.TerrainPage.TerrainPatch.SelectedTextureIndex > -1 )
			{
				DataCore.Texture texture = _terrainData.TerrainPage.TerrainPatch.GetTexture();

				texture.Render = !texture.Render;

				if ( btnTextures_Disable.Text == "Disable Texture" )
					btnTextures_Disable.Text = "Enable Texture";
				else
					btnTextures_Disable.Text = "Disable Texture";
			}
		}

		/// <summary>
		/// Disables the rendering of all textures.
		/// </summary>
		public void btnTextures_DisableAll_Click()
		{
			EnableAllTextures( _terrainData.RenderTextures );
		}
		
		/// <summary>
		/// Loads a texture file into the TerrainPage.
		/// </summary>
		/// <param name="filename">Filename of the texture to load.</param>
		[LuaFunctionAttribute( "LoadTexture", "Loads a texture file into the TerrainPage.",
			"The filename of the texture to load." )]
		public void LoadTexture( string filename )
		{
			if ( _terrainData.TerrainPage != null )
			{
				try
				{
					TreeNode node = new TreeNode( filename );
					int index = _terrainData.TerrainPage.TerrainPatch.Textures.Count;

					_terrainData.StoreCurrentPage( "Add Texture" );
					_terrainData.TerrainPage.TerrainPatch.AddTexture( new DataCore.Texture( 
						TextureLoader.FromFile( _dx.Device, filename ), filename ) );
					_terrainData.TerrainPage.TerrainPatch.RefreshBuffers = true;
					( (DataCore.Texture) _terrainData.TerrainPage.TerrainPatch.Textures[index] ).Name =
						filename;

					treeTextures.Nodes.Insert( 0, node );
					treeTextures.SelectedNode = treeTextures.Nodes[0];
					txtTextures_Name.Focus();
					txtTextures_Name.SelectAll();	// Selects all text in the textbox for easy renaming.
				}
				catch
				{
					MessageBox.Show( "The texture could not be loaded.", "Error Loading Texture",
						MessageBoxButtons.OK );
				}
				finally
				{
				}
			}
			else
			{
				MessageBox.Show( "The terrain must be created before textures can be loaded.",
					"Error Loading Texture", MessageBoxButtons.OK );
			}
		}

		/// <summary>
		/// Removes the current texture from the TerrainPage.
		/// </summary>
		[LuaFunctionAttribute( "RemoveTexture", "Removes the current texture from the TerrainPage." )]
		public void RemoveTexture()
		{
			if ( treeTextures.SelectedNode.Index > -1 )
				btnTextures_RemoveTex_Click();
		}

		/// <summary>
		/// Moves the current texture up one layer on the TerrainPatch.
		/// </summary>
		[LuaFunctionAttribute( "MoveTextureUp",
			"Moves the current texture up one layer on the TerrainPatch." )]
		public void MoveTextureUp()
		{
			if ( treeTextures.SelectedNode.Index > -1 )
				btnTextures_MoveUp_Click();
		}

		/// <summary>
		/// Moves the current texture down one layer on the TerrainPatch.
		/// </summary>
		[LuaFunctionAttribute( "MoveTextureDown",
			 "Moves the current texture down one layer on the TerrainPatch." )]
		public void MoveTextureDown()
		{
			if ( treeTextures.SelectedNode.Index > -1 )
				btnTextures_MoveDown_Click();
		}

		/// <summary>
		/// Disables the current texture in the TerrainPatch.
		/// </summary>
		[LuaFunctionAttribute( "DisableTexture", "Disables the current texture in the TerrainPatch." )]
		public void DisableTexture()
		{
			if ( treeTextures.SelectedNode.Index > -1 )
				btnTextures_Disable_Click();
		}

		/// <summary>
		/// Enables or disables the rendering of all textures.
		/// </summary>
		/// <param name="enable">Whether to enable or disable texture rendering.</param>
		[LuaFunctionAttribute( "EnableAllTextures", "Enables or disables the rendering of all textures.",
			 "Whether to enable or disable texture rendering." )]
		public void EnableAllTextures( bool enable )
		{
			if ( enable )
			{
				_terrainData.RenderTextures = false;
				btnTextures_DisableAll.Text = "Enable All Textures";
				btnTextures_DisableAll.BackColor = Color.FromKnownColor( KnownColor.ControlLight );
			}
			else
			{
				_terrainData.RenderTextures = true;
				btnTextures_DisableAll.Text = "Disable All Textures";
				btnTextures_DisableAll.BackColor = Color.FromKnownColor( KnownColor.Control );
			}
		}
		#endregion

		#region Texture Name
		/// <summary>
		/// Renames the currently selected texture.
		/// </summary>
		public void btnTextures_Name_Click()
		{
			SetTextureName( txtTextures_Name.Text );
		}

		/// <summary>
		/// Renames the specified texture.
		/// </summary>
		/// <param name="name">The new name of the texture.</param>
		[LuaFunctionAttribute( "SetTextureName", "Renames the specified texture.",
			 "The new name of the texture." )]
		public void SetTextureName( string name )
		{
			if ( _terrainData.TerrainPage.TerrainPatch.SelectedTextureIndex > -1 )
			{
				DataCore.Texture texture = _terrainData.TerrainPage.TerrainPatch.GetTexture();

				texture.Name = txtTextures_Name.Text;
				treeTextures.SelectedNode.Text = name;
			}
		}
		#endregion

		#region Texture Operation
		/// <summary>
		/// Changes the texture blending operation for the selected texture when rendering.
		/// </summary>
		public void cmbTextures_Operation_SelectedIndexChanged()
		{
			if ( _terrainData.TerrainPage.TerrainPatch.SelectedTextureIndex > -1 )
				SetTextureOperation( cmbTextures_Operation.Items[cmbTextures_Operation.SelectedIndex].ToString() );
		}

		/// <summary>
		/// Changes the texture blending operation for the specified texture when rendering.
		/// </summary>
		/// <param name="operation">The name of the operation to set.</param>
		[LuaFunctionAttribute( "SetTextureOperation",
			"Changes the texture blending operation for the specified texture when rendering.",
			"The name of the operation to set." )]
		public void SetTextureOperation( string operation )
		{
			if ( _terrainData.TerrainPage.TerrainPatch.SelectedTextureIndex > -1 )
			{
				DataCore.Texture texture = _terrainData.TerrainPage.TerrainPatch.GetTexture();

				if ( HasOperation( operation ) )
				{
					_terrainData.StoreCurrentPage( "Change Texture Blending" );
					texture.Operation = _terrainData.GetTextureOperation( operation );
					texture.OperationText = operation;
					_terrainData.TerrainPage.TerrainPatch.RefreshBuffers = true;
				}
			}
		}

		/// <summary>
		/// Checks if the list of operations contains the specified operation.
		/// </summary>
		/// <param name="name">The name of the operation to search for.</param>
		/// <returns>Whether the operation was found.</returns>
		private bool HasOperation( string name )
		{
			bool found = false;
			int count = 0;

			while ( !found && count < cmbTextures_Operation.Items.Count )
			{
				if ( cmbTextures_Operation.Items[count].ToString() == name )
					found = true;
				else
					count++;
			}

			return found;
		}
		#endregion

		#region Adjust Texture Size
		/// <summary>
		/// Shifts the selected texture by the amount indicated by the numeric spinners.
		/// </summary>
		/// <param name="endTextureMovement">Whether to end texture movement.</param>
		public void ShiftValuesChanged( bool endTextureMovement )
		{
			if ( _updateData )
			{
				ShiftValuesChanged();

				if ( endTextureMovement )
					_terrainData.EndTextureMovement();
			}
		}
		
		/// <summary>
		/// Scales the selected texture by the amount indicated by the numeric spinners.
		/// </summary>
		/// <param name="endTextureMovement">Whether to end texture movement.</param>
		public void ScaleValuesChanged( bool endTextureMovement )
		{
			if ( _updateData )
			{
				ScaleValuesChanged();

				if ( endTextureMovement )
					_terrainData.EndTextureMovement();
			}
		}

		/// <summary>
		/// Shifts the selected texture by the amount indicated by the numeric spinners.
		/// </summary>
		private void ShiftValuesChanged()
		{
			if ( _terrainData.TerrainPage.TerrainPatch.SelectedTextureIndex > -1 )
			{
				DataCore.Texture tex = _terrainData.TerrainPage.TerrainPatch.GetTexture();
				Vector2 shift = tex.Shift;

				shift.X = ( float ) -numTextures_uShift.Value;
				shift.Y =( float ) -numTextures_vShift.Value;
				tex.Shift = shift;
				_terrainData.ShiftTexture();
				_terrainData.TerrainPage.TerrainPatch.RefreshBuffers = true;
			}
		}

		/// <summary>
		/// Scales the selected texture by the amount indicated by the numeric spinners.
		/// </summary>
		private void ScaleValuesChanged()
		{
			if ( _terrainData.TerrainPage.TerrainPatch.SelectedTextureIndex > -1 )
			{
				DataCore.Texture tex = _terrainData.TerrainPage.TerrainPatch.GetTexture();
				Vector2 scale = tex.Scale;

				scale.X = ( float ) numTextures_uScale.Value;
				scale.Y = ( float ) numTextures_vScale.Value;
				tex.Scale = scale;
				_terrainData.ScaleTexture();
				_terrainData.TerrainPage.TerrainPatch.RefreshBuffers = true;
			}
		}

		/// <summary>
		/// Sets the scaling values for the current texture.
		/// </summary>
		/// <param name="uScale">The new U-scale for the texture.</param>
		/// <param name="vScale">The new V-scale for the texture.</param>
		[LuaFunctionAttribute( "SetTextureScale",
			 "Sets the scaling values for the current texture.",
			 "The new U-scale for the texture.", "The new V-scale for the texture." )]
		public void SetTextureScale( float uScale, float vScale )
		{
			_updateData = false;
			numTextures_uScale.Value = Convert.ToDecimal( uScale );
			numTextures_vScale.Value = Convert.ToDecimal( vScale );
			_updateData = true;

			ScaleValuesChanged();
		}

		/// <summary>
		/// Sets the shift values for the current texture.
		/// </summary>
		/// <param name="uShift">The new U-shift for the texture.</param>
		/// <param name="vShift">The new V-shift for the texture.</param>
		[LuaFunctionAttribute( "SetTextureShift",
			 "Sets the shift values for the current texture.",
			 "The new U-shift for the texture.", "The new V-shift for the texture." )]
		public void SetTextureShift( float uShift, float vShift )
		{
			_updateData = false;
			numTextures_uShift.Value = Convert.ToDecimal( uShift );
			numTextures_vShift.Value = Convert.ToDecimal( vShift );
			_updateData = true;

			ShiftValuesChanged();
		}
		#endregion

		#region Algorithms
		/// <summary>
		/// Enables the texture manipulation "Run Algorithm" button if an item is selected.
		/// </summary>
		public void lstAlgorithms_SelectedIndexChanged()
		{
			if ( lstAlgorithms.SelectedIndex > -1 )
				btnRunAlgorithm.Enabled = true;
			else
				btnRunAlgorithm.Enabled = false;
		}

		/// <summary>
		/// Runs the texture manipulation "Run Algorithm" button if an item is being clicked.
		/// </summary>
		public void lstAlgorithms_DoubleClick()
		{
			if ( lstAlgorithms.SelectedIndex > -1 )
			{
				btnRunAlgorithm.Enabled = true;
				btnRunAlgorithm_Click();
			}
			else
				btnRunAlgorithm.Enabled = false;
		}

		/// <summary>
		/// Runs the selected texture manipulation algorithm.
		/// </summary>
		public void btnRunAlgorithm_Click()
		{
			if ( lstAlgorithms.SelectedIndex > -1 )
			{
				_terrainData.RunPlugIn( lstAlgorithms.SelectedIndex,
					DataInterfacing.PlugIns.PlugInTypes.Textures, _owner );
				LoadTextures();
			}
		}

		/// <summary>
		/// Loads a new texture manipulation plug-in.
		/// </summary>
		public void btnLoadAlgorithm_Click()
		{
			_terrainData.LoadPlugIn( DataInterfacing.PlugIns.PlugInTypes.Textures );
			LoadAlgorithms();
		}

		/// <summary>
		/// Loads the list of texture manipulation algorithms.
		/// </summary>
		public void LoadAlgorithms()
		{
			_textureAlgorithms.Clear();
			lstAlgorithms.Items.Clear();

			for ( int i = 0; i < _terrainData.PlugIns.TexturePlugIns.Count; i++ )
				_textureAlgorithms.Add( i.ToString(),
					( (PlugIn) _terrainData.PlugIns.TexturePlugIns[i] ).GetName() );

			foreach ( string key in _textureAlgorithms.Keys )
				lstAlgorithms.Items.Add( _textureAlgorithms.GetValues( key )[0] );

			lstAlgorithms.SelectedIndex = -1;
		}

		/// <summary>
		/// Runs the texture manipulation plug-in specified.
		/// </summary>
		/// <param name="name">The name of the plug-in to run.</param>
		[LuaFunctionAttribute( "RunPlugIn", "Runs the texture manipulation plug-in specified.",
			 "The name of the plug-in to run." )]
		public void RunAlgorithm( string name )
		{
			bool found = false;
			int count = 0;

			while ( !found && lstAlgorithms.Items[count].ToString() != name )
				count++;

			if ( found )
				_terrainData.RunPlugIn( count, DataInterfacing.PlugIns.PlugInTypes.Textures, _owner );
			else
				MessageBox.Show( "Texture manipulation plug-in " + name + " could not be found! " );
		}

		/// <summary>
		/// Runs the texture manipulation plug-in specified in automatic mode.
		/// </summary>
		/// <param name="name">The name of the plug-in to run.</param>
		/// <param name="filename">The name of the file to load into the plug-in.</param>
		[LuaFunctionAttribute( "RunAutoPlugIn",
			 "Runs the texture manipulation plug-in specified in automatic mode.",
			 "The name of the plug-in to run.", "The name of the file to load into the plug-in." )]
		public void RunAutoAlgorithm( string name, string filename )
		{
			bool found = false;
			int count = 0;

			while ( !found && lstAlgorithms.Items[count].ToString() != name )
				count++;

			if ( found )
				_terrainData.RunPlugInAuto( count, DataInterfacing.PlugIns.PlugInTypes.Textures,
					_owner, filename );
			else
				MessageBox.Show( "Texture manipulation plug-in " + name + " could not be found! " );
		}

		/// <summary>
		/// Loads a new texture manipulation plug-in.
		/// </summary>
		[LuaFunctionAttribute( "LoadPlugIn", "Loads a new texture manipulation plug-in." )]
		public void LoadAlgorithm()
		{
			_terrainData.LoadPlugIn( DataInterfacing.PlugIns.PlugInTypes.Textures );
			LoadAlgorithms();
		}
		#endregion

		#region Other Terrain Functions
		/// <summary>
		/// Loads the textures of the TerrainPage into the Textures tab control.
		/// </summary>
		public void LoadTextures()
		{
			TreeNode node;
			DataCore.Texture tex;

			treeTextures.Nodes.Clear();

			if ( _terrainData.TerrainPage != null )
			{
				for ( int i = 0; i < _terrainData.TerrainPage.TerrainPatch.Textures.Count; i++ )
				{
					tex = ( (DataCore.Texture) _terrainData.TerrainPage.TerrainPatch.Textures[i] );
					node = new TreeNode( tex.Name );
					treeTextures.Nodes.Insert( 0, node );
				}

				if ( treeTextures.Nodes.Count > 0 )
					treeTextures.SelectedNode = treeTextures.Nodes[0];

				treeTextures_AfterSelect();
			}
		}

		/// <summary>
		/// Sets window states for selecting a new texture.
		/// </summary>
		/// <param name="index">The texture being selected.</param>
		private void SetTextureSelectedState( int index )
		{
			if ( index > -1 && index < _terrainData.TerrainPage.TerrainPatch.Textures.Count )
			{
				DataCore.Texture tex;

				_terrainData.TerrainPage.TerrainPatch.SelectedTextureIndex = index;
				tex = _terrainData.TerrainPage.TerrainPatch.GetTexture();

				btnTextures_RemoveTex.Enabled = true;
				btnTextures_MoveUp.Enabled = true;
				btnTextures_MoveDown.Enabled = true;
				btnTextures_Disable.Enabled = true;

				if ( tex.Render )
					btnTextures_Disable.Text = "Disable Texture";
				else
					btnTextures_Disable.Text = "Enable Texture";

				txtTextures_Name.Text = tex.Name;

				grpTextures_Name.Enabled = true;
				grpTextures_Operation.Enabled = true;
				grpTextures_Sizing.Enabled = true;
				grpTextureAlgorithms.Enabled = true;

				if ( tex.OperationText != null && tex.OperationText.Length > 0 )
				{
					cmbTextures_Operation.SelectAll();
					cmbTextures_Operation.SelectedText = tex.OperationText;
					cmbTextures_Operation.SelectedValue = tex.OperationText;
				}
				else
				{
					cmbTextures_Operation.SelectAll();
					cmbTextures_Operation.SelectedText = "";
					cmbTextures_Operation.SelectedValue = null;
				}

				_updateData = false;
				numTextures_uShift.Value = Convert.ToDecimal( tex.Shift.X );
				numTextures_vShift.Value = Convert.ToDecimal( tex.Shift.Y );
				numTextures_uScale.Value = Convert.ToDecimal( tex.Scale.X );
				numTextures_vScale.Value = Convert.ToDecimal( tex.Scale.Y );
				_updateData = true;
				_terrainData.TerrainPage.TerrainPatch.SetTexture( index );
			}
			else
			{
				_terrainData.TerrainPage.TerrainPatch.SelectedTextureIndex = -1;

				btnTextures_RemoveTex.Enabled = false;
				btnTextures_MoveUp.Enabled = false;
				btnTextures_MoveDown.Enabled = false;
				btnTextures_Disable.Enabled = false;

				txtTextures_Name.Text = null;
				cmbTextures_Operation.SelectedIndex = -1;

				grpTextures_Name.Enabled = false;
				grpTextures_Operation.Enabled = false;
				grpTextures_Sizing.Enabled = false;
				grpTextureAlgorithms.Enabled = false;
			}
		}

		/// <summary>
		/// Selects a texture from the TerrainPage.
		/// </summary>
		/// <param name="index">The index of the texture to select.</param>
		[LuaFunctionAttribute( "SelectTexture", "Selects a texture from the TerrainPage.",
			 "The name of the texture to select." )]
		public void SelectTexture( int index )
		{
			if ( index > -1 && index < _terrainData.TerrainPage.TerrainPatch.Textures.Count )
				treeTextures.SelectedNode = treeTextures.Nodes[ treeTextures.Nodes.Count - index - 1];
		}

		/// <summary>
		/// Selects a texture from the TerrainPage.
		/// </summary>
		/// <param name="name">The name of the texture to select.</param>
		[LuaFunctionAttribute( "SelectTexture", "Selects a texture from the TerrainPage.",
			 "The name of the texture to select." )]
		public void SelectTexture( string name )
		{
			bool found = false;
			int count = 0;

			while ( !found && count < treeTextures.Nodes.Count )
			{
				if ( treeTextures.Nodes[count].Text == name )
					found = true;
				else
					count++;
			}

			if ( found )
				SelectTexture( treeTextures.Nodes.Count - count - 1 );
		}
		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TextureManipulation));
			this.grpTextures_Operation = new System.Windows.Forms.GroupBox();
			this.cmbTextures_Operation = new System.Windows.Forms.ComboBox();
			this.grpTextures_Name = new System.Windows.Forms.GroupBox();
			this.btnTextures_Name = new System.Windows.Forms.Button();
			this.txtTextures_Name = new System.Windows.Forms.TextBox();
			this.grpTextures_Sizing = new System.Windows.Forms.GroupBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.numTextures_vScale = new System.Windows.Forms.NumericUpDown();
			this.numTextures_uScale = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.numTextures_vShift = new System.Windows.Forms.NumericUpDown();
			this.numTextures_uShift = new System.Windows.Forms.NumericUpDown();
			this.grpTextures_Textures = new System.Windows.Forms.GroupBox();
			this.btnTextures_DisableAll = new System.Windows.Forms.Button();
			this.btnTextures_Disable = new System.Windows.Forms.Button();
			this.btnTextures_MoveDown = new System.Windows.Forms.Button();
			this.btnTextures_MoveUp = new System.Windows.Forms.Button();
			this.btnTextures_RemoveTex = new System.Windows.Forms.Button();
			this.treeTextures = new System.Windows.Forms.TreeView();
			this.btnTextures_AddTex = new System.Windows.Forms.Button();
			this.grpTextureAlgorithms = new System.Windows.Forms.GroupBox();
			this.btnLoadAlgorithm = new System.Windows.Forms.Button();
			this.btnRunAlgorithm = new System.Windows.Forms.Button();
			this.lstAlgorithms = new System.Windows.Forms.ListBox();
			this.grpTextures_Operation.SuspendLayout();
			this.grpTextures_Name.SuspendLayout();
			this.grpTextures_Sizing.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numTextures_vScale)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numTextures_uScale)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numTextures_vShift)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numTextures_uShift)).BeginInit();
			this.grpTextures_Textures.SuspendLayout();
			this.grpTextureAlgorithms.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpTextures_Operation
			// 
			this.grpTextures_Operation.Controls.Add(this.cmbTextures_Operation);
			this.grpTextures_Operation.Enabled = false;
			this.grpTextures_Operation.Location = new System.Drawing.Point(8, 352);
			this.grpTextures_Operation.Name = "grpTextures_Operation";
			this.grpTextures_Operation.Size = new System.Drawing.Size(160, 48);
			this.grpTextures_Operation.TabIndex = 23;
			this.grpTextures_Operation.TabStop = false;
			this.grpTextures_Operation.Text = "Texture Operation";
			// 
			// cmbTextures_Operation
			// 
			this.cmbTextures_Operation.Items.AddRange(new object[] {
																	   "Add Color",
																	   "AddSigned",
																	   "AddSmooth",
																	   "Blend Alpha",
																	   "DotProduct3",
																	   "Modulate",
																	   "Modulate2X",
																	   "Normal (Default)",
																	   "Subtract Color",
																	   "Use Texture Alpha"});
			this.cmbTextures_Operation.Location = new System.Drawing.Point(8, 16);
			this.cmbTextures_Operation.MaxDropDownItems = 30;
			this.cmbTextures_Operation.Name = "cmbTextures_Operation";
			this.cmbTextures_Operation.Size = new System.Drawing.Size(144, 21);
			this.cmbTextures_Operation.Sorted = true;
			this.cmbTextures_Operation.TabIndex = 0;
			this.cmbTextures_Operation.SelectedIndexChanged += new System.EventHandler(this.cmbTextures_Operation_SelectedIndexChanged);
			// 
			// grpTextures_Name
			// 
			this.grpTextures_Name.Controls.Add(this.btnTextures_Name);
			this.grpTextures_Name.Controls.Add(this.txtTextures_Name);
			this.grpTextures_Name.Enabled = false;
			this.grpTextures_Name.Location = new System.Drawing.Point(8, 272);
			this.grpTextures_Name.Name = "grpTextures_Name";
			this.grpTextures_Name.Size = new System.Drawing.Size(160, 72);
			this.grpTextures_Name.TabIndex = 22;
			this.grpTextures_Name.TabStop = false;
			this.grpTextures_Name.Text = "Texture Name";
			// 
			// btnTextures_Name
			// 
			this.btnTextures_Name.Location = new System.Drawing.Point(8, 40);
			this.btnTextures_Name.Name = "btnTextures_Name";
			this.btnTextures_Name.Size = new System.Drawing.Size(144, 23);
			this.btnTextures_Name.TabIndex = 1;
			this.btnTextures_Name.Text = "Rename Texture";
			this.btnTextures_Name.Click += new System.EventHandler(this.btnTextures_Name_Click);
			// 
			// txtTextures_Name
			// 
			this.txtTextures_Name.Location = new System.Drawing.Point(8, 16);
			this.txtTextures_Name.Name = "txtTextures_Name";
			this.txtTextures_Name.Size = new System.Drawing.Size(144, 20);
			this.txtTextures_Name.TabIndex = 0;
			this.txtTextures_Name.Text = "";
			this.txtTextures_Name.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtTextures_Name_KeyUp);
			// 
			// grpTextures_Sizing
			// 
			this.grpTextures_Sizing.Controls.Add(this.label9);
			this.grpTextures_Sizing.Controls.Add(this.label8);
			this.grpTextures_Sizing.Controls.Add(this.numTextures_vScale);
			this.grpTextures_Sizing.Controls.Add(this.numTextures_uScale);
			this.grpTextures_Sizing.Controls.Add(this.label7);
			this.grpTextures_Sizing.Controls.Add(this.label6);
			this.grpTextures_Sizing.Controls.Add(this.numTextures_vShift);
			this.grpTextures_Sizing.Controls.Add(this.numTextures_uShift);
			this.grpTextures_Sizing.Enabled = false;
			this.grpTextures_Sizing.Location = new System.Drawing.Point(8, 408);
			this.grpTextures_Sizing.Name = "grpTextures_Sizing";
			this.grpTextures_Sizing.Size = new System.Drawing.Size(160, 128);
			this.grpTextures_Sizing.TabIndex = 21;
			this.grpTextures_Sizing.TabStop = false;
			this.grpTextures_Sizing.Text = "Adjust Texture Size";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(16, 96);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(48, 16);
			this.label9.TabIndex = 23;
			this.label9.Text = "V Scale:";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(16, 72);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(48, 16);
			this.label8.TabIndex = 22;
			this.label8.Text = "U Scale:";
			// 
			// numTextures_vScale
			// 
			this.numTextures_vScale.DecimalPlaces = 3;
			this.numTextures_vScale.Increment = new System.Decimal(new int[] {
																				 1,
																				 0,
																				 0,
																				 196608});
			this.numTextures_vScale.Location = new System.Drawing.Point(88, 96);
			this.numTextures_vScale.Minimum = new System.Decimal(new int[] {
																			   1,
																			   0,
																			   0,
																			   196608});
			this.numTextures_vScale.Name = "numTextures_vScale";
			this.numTextures_vScale.Size = new System.Drawing.Size(56, 20);
			this.numTextures_vScale.TabIndex = 21;
			this.numTextures_vScale.Value = new System.Decimal(new int[] {
																			 1,
																			 0,
																			 0,
																			 0});
			this.numTextures_vScale.ValueChanged += new System.EventHandler(this.numTextures_vScale_ValueChanged);
			this.numTextures_vScale.Leave += new System.EventHandler(this.numTextures_vScale_Leave);
			// 
			// numTextures_uScale
			// 
			this.numTextures_uScale.DecimalPlaces = 3;
			this.numTextures_uScale.Increment = new System.Decimal(new int[] {
																				 1,
																				 0,
																				 0,
																				 196608});
			this.numTextures_uScale.Location = new System.Drawing.Point(88, 72);
			this.numTextures_uScale.Minimum = new System.Decimal(new int[] {
																			   1,
																			   0,
																			   0,
																			   196608});
			this.numTextures_uScale.Name = "numTextures_uScale";
			this.numTextures_uScale.Size = new System.Drawing.Size(56, 20);
			this.numTextures_uScale.TabIndex = 20;
			this.numTextures_uScale.Value = new System.Decimal(new int[] {
																			 10,
																			 0,
																			 0,
																			 65536});
			this.numTextures_uScale.ValueChanged += new System.EventHandler(this.numTextures_uScale_ValueChanged);
			this.numTextures_uScale.Leave += new System.EventHandler(this.numTextures_uScale_Leave);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(16, 40);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(48, 16);
			this.label7.TabIndex = 19;
			this.label7.Text = "V Shift:";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 16);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(48, 16);
			this.label6.TabIndex = 18;
			this.label6.Text = "U Shift:";
			// 
			// numTextures_vShift
			// 
			this.numTextures_vShift.DecimalPlaces = 3;
			this.numTextures_vShift.Increment = new System.Decimal(new int[] {
																				 1,
																				 0,
																				 0,
																				 196608});
			this.numTextures_vShift.Location = new System.Drawing.Point(88, 40);
			this.numTextures_vShift.Maximum = new System.Decimal(new int[] {
																			   10,
																			   0,
																			   0,
																			   0});
			this.numTextures_vShift.Minimum = new System.Decimal(new int[] {
																			   10,
																			   0,
																			   0,
																			   -2147483648});
			this.numTextures_vShift.Name = "numTextures_vShift";
			this.numTextures_vShift.Size = new System.Drawing.Size(56, 20);
			this.numTextures_vShift.TabIndex = 17;
			this.numTextures_vShift.ValueChanged += new System.EventHandler(this.numTextures_vShift_ValueChanged);
			this.numTextures_vShift.Leave += new System.EventHandler(this.numTextures_vShift_Leave);
			// 
			// numTextures_uShift
			// 
			this.numTextures_uShift.DecimalPlaces = 3;
			this.numTextures_uShift.Increment = new System.Decimal(new int[] {
																				 1,
																				 0,
																				 0,
																				 196608});
			this.numTextures_uShift.Location = new System.Drawing.Point(88, 16);
			this.numTextures_uShift.Maximum = new System.Decimal(new int[] {
																			   10,
																			   0,
																			   0,
																			   0});
			this.numTextures_uShift.Minimum = new System.Decimal(new int[] {
																			   10,
																			   0,
																			   0,
																			   -2147483648});
			this.numTextures_uShift.Name = "numTextures_uShift";
			this.numTextures_uShift.Size = new System.Drawing.Size(56, 20);
			this.numTextures_uShift.TabIndex = 16;
			this.numTextures_uShift.ValueChanged += new System.EventHandler(this.numTextures_uShift_ValueChanged);
			this.numTextures_uShift.Leave += new System.EventHandler(this.numTextures_uShift_Leave);
			// 
			// grpTextures_Textures
			// 
			this.grpTextures_Textures.Controls.Add(this.btnTextures_DisableAll);
			this.grpTextures_Textures.Controls.Add(this.btnTextures_Disable);
			this.grpTextures_Textures.Controls.Add(this.btnTextures_MoveDown);
			this.grpTextures_Textures.Controls.Add(this.btnTextures_MoveUp);
			this.grpTextures_Textures.Controls.Add(this.btnTextures_RemoveTex);
			this.grpTextures_Textures.Controls.Add(this.treeTextures);
			this.grpTextures_Textures.Controls.Add(this.btnTextures_AddTex);
			this.grpTextures_Textures.Location = new System.Drawing.Point(8, 8);
			this.grpTextures_Textures.Name = "grpTextures_Textures";
			this.grpTextures_Textures.Size = new System.Drawing.Size(160, 256);
			this.grpTextures_Textures.TabIndex = 20;
			this.grpTextures_Textures.TabStop = false;
			this.grpTextures_Textures.Text = "Textures";
			// 
			// btnTextures_DisableAll
			// 
			this.btnTextures_DisableAll.Location = new System.Drawing.Point(8, 224);
			this.btnTextures_DisableAll.Name = "btnTextures_DisableAll";
			this.btnTextures_DisableAll.Size = new System.Drawing.Size(144, 23);
			this.btnTextures_DisableAll.TabIndex = 22;
			this.btnTextures_DisableAll.Text = "Disable All Textures";
			this.btnTextures_DisableAll.Click += new System.EventHandler(this.btnTextures_DisableAll_Click);
			// 
			// btnTextures_Disable
			// 
			this.btnTextures_Disable.Enabled = false;
			this.btnTextures_Disable.Location = new System.Drawing.Point(8, 192);
			this.btnTextures_Disable.Name = "btnTextures_Disable";
			this.btnTextures_Disable.Size = new System.Drawing.Size(144, 23);
			this.btnTextures_Disable.TabIndex = 21;
			this.btnTextures_Disable.Text = "Disable Texture";
			this.btnTextures_Disable.Click += new System.EventHandler(this.btnTextures_Disable_Click);
			// 
			// btnTextures_MoveDown
			// 
			this.btnTextures_MoveDown.Enabled = false;
			this.btnTextures_MoveDown.Image = ((System.Drawing.Image)(resources.GetObject("btnTextures_MoveDown.Image")));
			this.btnTextures_MoveDown.Location = new System.Drawing.Point(136, 160);
			this.btnTextures_MoveDown.Name = "btnTextures_MoveDown";
			this.btnTextures_MoveDown.Size = new System.Drawing.Size(16, 16);
			this.btnTextures_MoveDown.TabIndex = 20;
			this.btnTextures_MoveDown.Click += new System.EventHandler(this.btnTextures_MoveDown_Click);
			// 
			// btnTextures_MoveUp
			// 
			this.btnTextures_MoveUp.Enabled = false;
			this.btnTextures_MoveUp.Image = ((System.Drawing.Image)(resources.GetObject("btnTextures_MoveUp.Image")));
			this.btnTextures_MoveUp.Location = new System.Drawing.Point(112, 160);
			this.btnTextures_MoveUp.Name = "btnTextures_MoveUp";
			this.btnTextures_MoveUp.Size = new System.Drawing.Size(16, 16);
			this.btnTextures_MoveUp.TabIndex = 19;
			this.btnTextures_MoveUp.Click += new System.EventHandler(this.btnTextures_MoveUp_Click);
			// 
			// btnTextures_RemoveTex
			// 
			this.btnTextures_RemoveTex.Enabled = false;
			this.btnTextures_RemoveTex.Image = ((System.Drawing.Image)(resources.GetObject("btnTextures_RemoveTex.Image")));
			this.btnTextures_RemoveTex.Location = new System.Drawing.Point(32, 160);
			this.btnTextures_RemoveTex.Name = "btnTextures_RemoveTex";
			this.btnTextures_RemoveTex.Size = new System.Drawing.Size(16, 16);
			this.btnTextures_RemoveTex.TabIndex = 17;
			this.btnTextures_RemoveTex.Click += new System.EventHandler(this.btnTextures_RemoveTex_Click);
			// 
			// treeTextures
			// 
			this.treeTextures.AllowDrop = true;
			this.treeTextures.FullRowSelect = true;
			this.treeTextures.HideSelection = false;
			this.treeTextures.ImageIndex = -1;
			this.treeTextures.Indent = 15;
			this.treeTextures.Location = new System.Drawing.Point(8, 16);
			this.treeTextures.Name = "treeTextures";
			this.treeTextures.SelectedImageIndex = -1;
			this.treeTextures.Size = new System.Drawing.Size(144, 136);
			this.treeTextures.TabIndex = 0;
			this.treeTextures.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeTextures_AfterSelect);
			this.treeTextures.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeTextures_DragEnter);
			this.treeTextures.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeTextures_DragDrop);
			// 
			// btnTextures_AddTex
			// 
			this.btnTextures_AddTex.Image = ((System.Drawing.Image)(resources.GetObject("btnTextures_AddTex.Image")));
			this.btnTextures_AddTex.Location = new System.Drawing.Point(8, 160);
			this.btnTextures_AddTex.Name = "btnTextures_AddTex";
			this.btnTextures_AddTex.Size = new System.Drawing.Size(16, 16);
			this.btnTextures_AddTex.TabIndex = 16;
			this.btnTextures_AddTex.Click += new System.EventHandler(this.btnTextures_AddTex_Click);
			// 
			// grpTextureAlgorithms
			// 
			this.grpTextureAlgorithms.Controls.Add(this.btnLoadAlgorithm);
			this.grpTextureAlgorithms.Controls.Add(this.btnRunAlgorithm);
			this.grpTextureAlgorithms.Controls.Add(this.lstAlgorithms);
			this.grpTextureAlgorithms.Enabled = false;
			this.grpTextureAlgorithms.Location = new System.Drawing.Point(8, 544);
			this.grpTextureAlgorithms.Name = "grpTextureAlgorithms";
			this.grpTextureAlgorithms.Size = new System.Drawing.Size(160, 160);
			this.grpTextureAlgorithms.TabIndex = 24;
			this.grpTextureAlgorithms.TabStop = false;
			this.grpTextureAlgorithms.Text = "Algorithms";
			// 
			// btnLoadAlgorithm
			// 
			this.btnLoadAlgorithm.Location = new System.Drawing.Point(8, 128);
			this.btnLoadAlgorithm.Name = "btnLoadAlgorithm";
			this.btnLoadAlgorithm.Size = new System.Drawing.Size(144, 23);
			this.btnLoadAlgorithm.TabIndex = 2;
			this.btnLoadAlgorithm.Text = "Load New Algorithm";
			this.btnLoadAlgorithm.Click += new System.EventHandler(this.btnLoadAlgorithm_Click);
			// 
			// btnRunAlgorithm
			// 
			this.btnRunAlgorithm.Enabled = false;
			this.btnRunAlgorithm.Location = new System.Drawing.Point(8, 96);
			this.btnRunAlgorithm.Name = "btnRunAlgorithm";
			this.btnRunAlgorithm.Size = new System.Drawing.Size(144, 23);
			this.btnRunAlgorithm.TabIndex = 1;
			this.btnRunAlgorithm.Text = "Run Algorithm";
			this.btnRunAlgorithm.Click += new System.EventHandler(this.btnRunAlgorithm_Click);
			// 
			// lstAlgorithms
			// 
			this.lstAlgorithms.Location = new System.Drawing.Point(8, 16);
			this.lstAlgorithms.Name = "lstAlgorithms";
			this.lstAlgorithms.Size = new System.Drawing.Size(144, 69);
			this.lstAlgorithms.TabIndex = 0;
			this.lstAlgorithms.DoubleClick += new System.EventHandler(this.lstAlgorithms_DoubleClick);
			this.lstAlgorithms.SelectedIndexChanged += new System.EventHandler(this.lstAlgorithms_SelectedIndexChanged);
			// 
			// TextureManipulation
			// 
			this.Controls.Add(this.grpTextureAlgorithms);
			this.Controls.Add(this.grpTextures_Operation);
			this.Controls.Add(this.grpTextures_Name);
			this.Controls.Add(this.grpTextures_Sizing);
			this.Controls.Add(this.grpTextures_Textures);
			this.Name = "TextureManipulation";
			this.Size = new System.Drawing.Size(176, 712);
			this.grpTextures_Operation.ResumeLayout(false);
			this.grpTextures_Name.ResumeLayout(false);
			this.grpTextures_Sizing.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numTextures_vScale)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numTextures_uScale)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numTextures_vShift)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numTextures_uShift)).EndInit();
			this.grpTextures_Textures.ResumeLayout(false);
			this.grpTextureAlgorithms.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
