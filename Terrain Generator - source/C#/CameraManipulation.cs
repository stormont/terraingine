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
using Voyage.LuaNetInterface;

namespace Voyage.Terraingine
{
	/// <summary>
	/// A user control for manipulating the viewport camera.
	/// </summary>
	public class CameraManipulation : System.Windows.Forms.UserControl
	{
		#region Data Members
		private DataInterfacing.ViewportInterface	_viewport;
		private DataInterfacing.DataManipulation	_terrainData;
		private TerrainViewport			_owner;
		private DXViewport.Viewport		_dx;
		private bool					_updateData;

		private System.Windows.Forms.ImageList imgLstCamera;
		private System.Windows.Forms.ToolBar toolCamera;
		private System.Windows.Forms.ToolBarButton toolBtnRotate;
		private System.Windows.Forms.ToolBarButton toolBtnPan;
		private System.Windows.Forms.ToolBarButton toolBtnZoom;
		private System.Windows.Forms.ToolBarButton toolBtnSolid;
		private System.Windows.Forms.ToolBarButton toolBtnWireframe;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarResetCam;
		private System.ComponentModel.IContainer components;

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
		/// Gets if the rotate button is pressed.
		/// </summary>
		public bool RotatePressed
		{
			get { return toolBtnRotate.Pushed; }
		}

		/// <summary>
		/// Gets if the pan button is pressed.
		/// </summary>
		public bool PanPressed
		{
			get { return toolBtnPan.Pushed; }
		}

		/// <summary>
		/// Gets if the zoom button is pressed.
		/// </summary>
		public bool ZoomPressed
		{
			get { return toolBtnZoom.Pushed; }
		}

		/// <summary>
		/// Gets if the solid button is pressed.
		/// </summary>
		public bool SolidPressed
		{
			get { return toolBtnSolid.Pushed; }
		}

		/// <summary>
		/// Gets if the wireframe button is pressed.
		/// </summary>
		public bool WireframePressed
		{
			get { return toolBtnWireframe.Pushed; }
		}
		#endregion

		#region Basic Form Methods
		/// <summary>
		/// Creates a camera manipulation user control.
		/// </summary>
		public CameraManipulation()
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
			_updateData = true;
		}
		#endregion

		#region Toolbar
		/// <summary>
		/// Perform a toolbar click action.
		/// </summary>
		private void toolCamera_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			switch ( e.Button.Text )
			{
				case "Rotate":
					RotateCamera( toolBtnRotate.Pushed, false );
					break;

				case "Pan":
					PanCamera( toolBtnPan.Pushed, false );
					break;

				case "Zoom":
					ZoomCamera( toolBtnZoom.Pushed, false );
					break;

				case "Reset Camera":
					ResetCamera();
					break;

				case "Solid":
					SolidTerrain( toolBtnSolid.Pushed, false );
					break;

				case "Wireframe":
					WireframeTerrain( toolBtnWireframe.Pushed, false );
					break;

				default:
					break;
			}
		}
		#endregion

		#region Camera Movement
		/// <summary>
		/// Rotates the camera.
		/// </summary>
		/// <param name="pushed">Indicates if the button was already active.</param>
		/// <param name="toggle">Indicates whether to visually toggle the button.</param>
		public void RotateCamera( bool pushed, bool toggle )
		{
			// Note: Toggle buttons use inverse logic
			if ( pushed )
			{
				_terrainData.PauseVertexMovement = true;
				_dx.Camera.CurrentMovement = DXViewport.QuaternionCamera.MovementType.Rotate;
			}
			else
			{
				_terrainData.PauseVertexMovement = false;
				_dx.Camera.CurrentMovement = DXViewport.QuaternionCamera.MovementType.None;
			}

			if ( toggle )
				toolBtnRotate.Pushed = !toolBtnRotate.Pushed;

			toolBtnPan.Pushed = false;
			toolBtnZoom.Pushed = false;
		}

		/// <summary>
		/// Pans the camera.
		/// </summary>
		/// <param name="pushed">Indicates if the button was already active.</param>
		/// <param name="toggle">Indicates whether to visually toggle the button.</param>
		public void PanCamera( bool pushed, bool toggle )
		{
			// Note: Toggle buttons use inverse logic
			if ( pushed )
			{
				_terrainData.PauseVertexMovement = true;
				_dx.Camera.CurrentMovement = DXViewport.QuaternionCamera.MovementType.Pan;
			}
			else
			{
				_terrainData.PauseVertexMovement = false;
				_dx.Camera.CurrentMovement = DXViewport.QuaternionCamera.MovementType.None;
			}

			if ( toggle )
				toolBtnPan.Pushed = !toolBtnRotate.Pushed;

			toolBtnRotate.Pushed = false;
			toolBtnZoom.Pushed = false;
		}

		/// <summary>
		/// Zooms the camera.
		/// </summary>
		/// <param name="pushed">Indicates if the button was already active.</param>
		/// <param name="toggle">Indicates whether to visually toggle the button.</param>
		public void ZoomCamera( bool pushed, bool toggle )
		{
			// Note: Toggle buttons use inverse logic
			if ( pushed )
			{
				_terrainData.PauseVertexMovement = true;
				_dx.Camera.CurrentMovement = DXViewport.QuaternionCamera.MovementType.Truck;
			}
			else
			{
				_terrainData.PauseVertexMovement = false;
				_dx.Camera.CurrentMovement = DXViewport.QuaternionCamera.MovementType.None;
			}

			if ( toggle )
				toolBtnZoom.Pushed = !toolBtnRotate.Pushed;

			toolBtnRotate.Pushed = false;
			toolBtnPan.Pushed = false;
		}

		/// <summary>
		/// Displays the TerrainPage in solid form.
		/// </summary>
		/// <param name="pushed">Indicates if the button was already active.</param>
		/// <param name="toggle">Indicates whether to visually toggle the button.</param>
		public void SolidTerrain( bool pushed, bool toggle )
		{
			// Note: Toggle buttons use inverse logic
			if ( pushed )
			{
				_viewport.FillMode = FillMode.Solid;
				_viewport.CullMode = Cull.CounterClockwise;
				toolBtnWireframe.Pushed = false;
			}
			else
			{
				_viewport.FillMode = FillMode.WireFrame;
				_viewport.CullMode = Cull.None;
				toolBtnWireframe.Pushed = true;
			}

			if ( toggle )
				toolBtnSolid.Pushed = !toolBtnWireframe.Pushed;
		}

		/// <summary>
		/// Displays the TerrainPage in wireframe form.
		/// </summary>
		/// <param name="pushed">Indicates if the button was already active.</param>
		/// <param name="toggle">Indicates whether to visually toggle the button.</param>
		public void WireframeTerrain( bool pushed, bool toggle )
		{
			// Note: Toggle buttons use inverse logic
			if ( pushed )
			{
				_viewport.FillMode = FillMode.WireFrame;
				_viewport.CullMode = Cull.None;
				toolBtnSolid.Pushed = false;
			}
			else
			{
				_viewport.FillMode = FillMode.Solid;
				_viewport.CullMode = Cull.CounterClockwise;
				toolBtnSolid.Pushed = true;
			}

			if ( toggle )
				toolBtnWireframe.Pushed = !toolBtnSolid.Pushed;
		}

		/// <summary>
		/// Resets the camera position.
		/// </summary>
		[LuaFunctionAttribute( "ResetCamera", "Resets the camera to the center of the current terrain." )]
		public void ResetCamera()
		{
			if ( _terrainData.TerrainPage != null )
			{
				Vector3 position = _terrainData.TerrainPage.Position;
				float distance;

				if ( _terrainData.TerrainPage.TerrainPatch.Height > _terrainData.TerrainPage.TerrainPatch.Width )
					distance = ( float ) _terrainData.TerrainPage.TerrainPatch.Height * 2.0f;
				else
					distance = ( float ) _terrainData.TerrainPage.TerrainPatch.Width * 2.0f;

				position.X -= _terrainData.TerrainPage.TerrainPatch.Width / 2.0f;
				position.Z -= _terrainData.TerrainPage.TerrainPatch.Height / 2.0f;
				_dx.Camera.ResetTarget.Position = position;
				_dx.Camera.ResetTarget.FollowDistance = distance;
				_dx.Camera.ResetCamera();
			}
		}

		/// <summary>
		/// Displays the TerrainPage in solid form.
		/// </summary>
		[LuaFunctionAttribute( "ViewSolidTerrain", "Displays the TerrainPage in solid form." )]
		public void ViewSolidTerrain()
		{
			toolBtnSolid.Pushed = true;
			SolidTerrain( false, true );
		}

		/// <summary>
		/// Displays the TerrainPage in wireframe form.
		/// </summary>
		[LuaFunctionAttribute( "ViewWireframeTerrain", "Displays the TerrainPage in wireframe form." )]
		public void ViewWireframeTerrain()
		{
			toolBtnWireframe.Pushed = true;
			WireframeTerrain( false, true );
		}

		/// <summary>
		/// Disables camera movement.
		/// </summary>
		[LuaFunctionAttribute( "DisableCamera", "Disables camera movement." )]
		public void DisableCameraMovement()
		{
			if ( toolBtnRotate.Pushed )
			{
				_dx.Camera.CurrentMovement = DXViewport.QuaternionCamera.MovementType.None;
				toolBtnRotate.Pushed = false;
			}

			if ( toolBtnPan.Pushed )
			{
				_dx.Camera.CurrentMovement = DXViewport.QuaternionCamera.MovementType.None;
				toolBtnPan.Pushed = false;
			}

			if ( toolBtnZoom.Pushed )
			{
				_dx.Camera.CurrentMovement = DXViewport.QuaternionCamera.MovementType.None;
				toolBtnZoom.Pushed = false;
			}

			_terrainData.PauseVertexMovement = false;
		}
		#endregion

		#region Lua-Specific Functions
		/// <summary>
		/// Increases the size of the rendered vertices.
		/// </summary>
		[LuaFunctionAttribute( "IncreaseVertexSize", "Increases the size of the rendered vertices." )]
		public void IncreaseVertexSize()
		{
			_terrainData.BufferObjects.VertexSize *= 1.5f;

			if ( _terrainData.BufferObjects.VertexSize > 0.495f )
				_terrainData.BufferObjects.VertexSize = 0.495f;

			_terrainData.TerrainPage.TerrainPatch.RefreshVertices = true;
		}

		/// <summary>
		/// Decreases the size of the rendered vertices.
		/// </summary>
		[LuaFunctionAttribute( "DecreaseVertexSize", "Decreases the size of the rendered vertices." )]
		public void DecreaseVertexSize()
		{
			_terrainData.BufferObjects.VertexSize /= 1.5f;

			if ( _terrainData.BufferObjects.VertexSize == 0f )
				_terrainData.BufferObjects.VertexSize = 0.005f;

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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CameraManipulation));
			this.imgLstCamera = new System.Windows.Forms.ImageList(this.components);
			this.toolCamera = new System.Windows.Forms.ToolBar();
			this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
			this.toolBtnRotate = new System.Windows.Forms.ToolBarButton();
			this.toolBtnPan = new System.Windows.Forms.ToolBarButton();
			this.toolBtnZoom = new System.Windows.Forms.ToolBarButton();
			this.toolBarResetCam = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
			this.toolBtnSolid = new System.Windows.Forms.ToolBarButton();
			this.toolBtnWireframe = new System.Windows.Forms.ToolBarButton();
			this.SuspendLayout();
			// 
			// imgLstCamera
			// 
			this.imgLstCamera.ImageSize = new System.Drawing.Size(40, 40);
			this.imgLstCamera.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgLstCamera.ImageStream")));
			this.imgLstCamera.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// toolCamera
			// 
			this.toolCamera.AutoSize = false;
			this.toolCamera.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						  this.toolBarButton2,
																						  this.toolBtnRotate,
																						  this.toolBtnPan,
																						  this.toolBtnZoom,
																						  this.toolBarResetCam,
																						  this.toolBarButton1,
																						  this.toolBtnSolid,
																						  this.toolBtnWireframe});
			this.toolCamera.ButtonSize = new System.Drawing.Size(40, 40);
			this.toolCamera.DropDownArrows = true;
			this.toolCamera.ImageList = this.imgLstCamera;
			this.toolCamera.Location = new System.Drawing.Point(0, 0);
			this.toolCamera.Name = "toolCamera";
			this.toolCamera.ShowToolTips = true;
			this.toolCamera.Size = new System.Drawing.Size(456, 56);
			this.toolCamera.TabIndex = 1;
			this.toolCamera.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolCamera_ButtonClick);
			// 
			// toolBarButton2
			// 
			this.toolBarButton2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBtnRotate
			// 
			this.toolBtnRotate.ImageIndex = 0;
			this.toolBtnRotate.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.toolBtnRotate.Text = "Rotate";
			this.toolBtnRotate.ToolTipText = "Rotate Camera (Ctrl+Shift+R)";
			// 
			// toolBtnPan
			// 
			this.toolBtnPan.ImageIndex = 1;
			this.toolBtnPan.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.toolBtnPan.Text = "Pan";
			this.toolBtnPan.ToolTipText = "Pan Camera (Ctrl+Shift+P)";
			// 
			// toolBtnZoom
			// 
			this.toolBtnZoom.ImageIndex = 2;
			this.toolBtnZoom.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.toolBtnZoom.Text = "Zoom";
			this.toolBtnZoom.ToolTipText = "Zoom Camera (Ctrl+Shift+Z)";
			// 
			// toolBarResetCam
			// 
			this.toolBarResetCam.ImageIndex = 3;
			this.toolBarResetCam.Text = "Reset Camera";
			this.toolBarResetCam.ToolTipText = "Reset the camera position (Ctrl+Shift+Q)";
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBtnSolid
			// 
			this.toolBtnSolid.ImageIndex = 4;
			this.toolBtnSolid.Pushed = true;
			this.toolBtnSolid.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.toolBtnSolid.Text = "Solid";
			this.toolBtnSolid.ToolTipText = "Display Solid Terrain (Ctrl+Shift+E)";
			// 
			// toolBtnWireframe
			// 
			this.toolBtnWireframe.ImageIndex = 5;
			this.toolBtnWireframe.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.toolBtnWireframe.Text = "Wireframe";
			this.toolBtnWireframe.ToolTipText = "Display Wireframe Terrain (Ctrl+Shift+W)";
			// 
			// CameraManipulation
			// 
			this.Controls.Add(this.toolCamera);
			this.Name = "CameraManipulation";
			this.Size = new System.Drawing.Size(456, 56);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
