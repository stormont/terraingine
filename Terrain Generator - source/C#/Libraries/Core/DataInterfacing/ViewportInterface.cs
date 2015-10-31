using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using Voyage.Terraingine.DXViewport;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine.DataInterfacing
{
	/// <summary>
	/// A UserControl for interfacing with terrain data and rendering the data to a DirectX viewport.
	/// </summary>
	public class ViewportInterface : System.Windows.Forms.UserControl
	{
		#region Data Members
		private DXViewport.Viewport	_viewport;
		private DataManipulation		_terrainData;
		private Form					_owner;
		private FillMode				_fillMode;
		private Cull					_cullMode;

		private System.Windows.Forms.Panel pnlMainViewport;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the Form that contains this control.
		/// </summary>
		public virtual Form Owner
		{
			get { return _owner; }
			set { _owner = value; }
		}

		/// <summary>
		/// Gets the main viewport DirectX interface.
		/// </summary>
		public virtual DXViewport.Viewport DXViewport
		{
			get { return _viewport; }
		}

		/// <summary>
		/// Gets or sets the terrain data used by the viewport.
		/// </summary>
		public DataManipulation TerrainData
		{
			get { return _terrainData; }
			set { _terrainData = value; }
		}

		/// <summary>
		/// Gets or sets the FillMode used by the DirectX viewport.
		/// </summary>
		public FillMode FillMode
		{
			get { return _fillMode; }
			set { _fillMode = value; }
		}

		/// <summary>
		/// Gets or sets the CullMode used by the DirectX viewport.
		/// </summary>
		public Cull CullMode
		{
			get { return _cullMode; }
			set { _cullMode = value; }
		}

		/// <summary>
		/// Gets or sets the panel used by the DirectX viewport.
		/// </summary>
		public Panel ViewportPanel
		{
			get { return pnlMainViewport; }
			set { pnlMainViewport = value; }
		}
		#endregion

		#region Basic Control Members
		/// <summary>
		/// Creates an interface viewport between the terrain data and DirectX.
		/// </summary>
		public ViewportInterface()
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
		/// Creates the viewport interface.
		/// </summary>
		/// <param name="owner">The form the viewport interface is contained within.</param>
		public void CreateViewport( Form owner )
		{
			_owner = owner;
			_fillMode = FillMode.WireFrame;
			_cullMode = Cull.CounterClockwise;
		}

		/// <summary>
		/// Sets the size of the viewport.
		/// </summary>
		/// <param name="s">The new size of the viewport.</param>
		public virtual void SetViewportSize( Size s )
		{
			// The UserControl must be at least the same size as the viewport
			this.Size = s;
			pnlMainViewport.Size = s;
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Initializes the DXViewport and the DataManipulation classes
		/// using default configuration settings.
		/// </summary>
		public virtual void InitializeViewport_Default()
		{
			InitializeViewport();
			InitializeDXDefaults();
			InitializeCamera();
		}

		/// <summary>
		/// Initializes the DXViewport and the DataManipulation classes.
		/// Call InitializeDXDefaults() and InitializeCamera() afterwards,
		/// or implement their equivalent.
		/// </summary>
		public virtual void InitializeViewport()
		{
			_viewport = new DXViewport.Viewport();
			_viewport.InitializeDXWindow( pnlMainViewport, _owner );
			_terrainData = new DataManipulation( _viewport );
			
			_viewport.Device.DeviceReset += new EventHandler( this.OnDeviceReset );
			OnDeviceReset( _owner, new System.EventArgs() );
		}

		/// <summary>
		/// Sets up additional data members in the form.
		/// Call InitializeCamera() afterwards, or implement its equivalent.
		/// </summary>
		public void InitializeDXDefaults()
		{
			// Final DXViewport initialization
			_viewport.ClearColor = Color.Blue;
			_viewport.Camera.FirstPerson = false;

			// Final terrain initialization
			_terrainData.EnableVertexMovement = false;
			_terrainData.SoftSelection = false;
			_terrainData.SoftSelectionDistanceSquared = 0.2f * 0.2f;

			_terrainData.OriginalZoomFactor = _viewport.Camera.FollowDistance;
		}

		/// <summary>
		/// Initializes the camera in the DirectX viewport.
		/// </summary>
		public virtual void InitializeCamera()
		{
			Vector3 eye		= Vector3.Empty;
			Vector3 lookAt	= Vector3.Empty;
			Vector3 offset	= Vector3.Empty;
			Point p			= new Point( 0, 45 );

			_viewport.Camera.FirstPerson		= false;
			_viewport.Camera.CurrentMovement	= QuaternionCamera.MovementType.Rotate;

			eye.Z += -_viewport.Camera.FollowDistance;
			_viewport.Camera.SetViewParameters( eye, lookAt );

			if ( _terrainData.TerrainPage != null )
			{
				offset.X = -_terrainData.TerrainPage.TerrainPatch.Width / 2.0f;
				offset.Z = -_terrainData.TerrainPage.TerrainPatch.Height / 2.0f;
				_viewport.Camera.Offset = offset;
			}

			_viewport.Camera.BeginMove();
			_viewport.Camera.Move( p );
			_viewport.Camera.EndMove();
			_viewport.Camera.CurrentMovement = QuaternionCamera.MovementType.None;

			// Change the default camera projection near/far planes
			_viewport.Camera.NearPlane = 0.1f;
			_viewport.Camera.FarPlane = 100.0f;
			_viewport.ResetCamera();
		}

		/// <summary>
		/// Resets application-specific render states when the Direct3D device is reset.
		/// </summary>
		public virtual void OnDeviceReset( object sender, System.EventArgs e )
		{
			_viewport.Device.RenderState.CullMode = _cullMode;
			_viewport.Device.RenderState.CullMode = Cull.CounterClockwise;

			if ( _terrainData.TerrainPage != null )
			{
				_terrainData.TerrainPage.TerrainPatch.RefreshBuffers = true;
				_terrainData.TerrainPage.TerrainPatch.RefreshVertices = true;
			}
		}
		#endregion

		#region Rendering
		/// <summary>
		/// Performs necessary pre-rendering functions.
		/// </summary>
		public virtual void PreRender()
		{
			if ( _viewport.Mouse != null )
			{
				_viewport.Mouse.Update();
				_viewport.Camera.FrameMove();
				_terrainData.MoveVertices();
			}
		}

		/// <summary>
		/// Renders the DirectX viewport.
		/// </summary>
		/// <returns>Whether the frame has begun rendering.</returns>
		public virtual bool BeginRender()
		{
			bool result = false;

			if ( _viewport.Device != null )
			{
				// Only render if the viewport is visible
				if ( pnlMainViewport.Visible &&
					pnlMainViewport.Size.Height > 0 &&
					pnlMainViewport.Size.Width > 0 )
				{
					_viewport.BeginRender();
					result = true;
				}
			}

			return result;
		}

		/// <summary>
		/// Ends the rendering of the frame.
		/// </summary>
		public virtual void EndRender()
		{
			_viewport.EndRender();
		}

		/// <summary>
		/// Renders the elements in the scene.
		/// </summary>
		public virtual void RenderSceneElements()
		{
			if ( !_viewport.LostDevice && _terrainData.TerrainPage != null )
				RenderTerrain();
		}

		/// <summary>
		/// Renders the terrain used in the main viewport.
		/// </summary>
		public virtual void RenderTerrain()
		{
			if ( _viewport.Device.RenderState.FillMode != _fillMode )
				_viewport.Device.RenderState.FillMode = _fillMode;

			_viewport.Device.RenderState.CullMode = Cull.CounterClockwise;
			_terrainData.RenderTerrain();
		}
		#endregion

		#region Other Form Callbacks
		/// <summary>
		/// Automatically updates the new size of the viewport when the size of the UserControl is changed.
		/// </summary>
		private void ViewportInterface_SizeChanged(object sender, System.EventArgs e)
		{
			pnlMainViewport.Size = new Size( this.Size.Width - 20, this.Size.Height - 20 );
		}
		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnlMainViewport = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// pnlMainViewport
			// 
			this.pnlMainViewport.AllowDrop = true;
			this.pnlMainViewport.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlMainViewport.Location = new System.Drawing.Point(8, 8);
			this.pnlMainViewport.Name = "pnlMainViewport";
			this.pnlMainViewport.Size = new System.Drawing.Size(100, 100);
			this.pnlMainViewport.TabIndex = 1;
			// 
			// ViewportInterface
			// 
			this.Controls.Add(this.pnlMainViewport);
			this.Name = "ViewportInterface";
			this.Size = new System.Drawing.Size(120, 120);
			this.SizeChanged += new System.EventHandler(this.ViewportInterface_SizeChanged);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
