using System;
using System.Collections;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;
using System.Diagnostics;

namespace Voyage.Terraingine.DXViewport
{
	/// <summary>
	/// A DirectX viewport wrapper.
	/// </summary>
	public class Viewport
	{
		#region Data Members
		private Device				_device;
		private Control				_window;
		private Form				_parent;
		private Color				_clearColor;
		private QuaternionCamera	_camera;
		private Mouse				_mouse;
		private Keyboard			_keyboard;
		private bool				_deviceLost;
		private PresentParameters	_presentParams;
		private ArrayList			_effects;

		// Frames per second counters
		private int					_fps;
		private DateTime			_lastRender;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the DirectX device.
		/// </summary>
		public Device Device
		{
			get { return _device; }
		}

		/// <summary>
		/// Gets or sets the window DirectX is loaded in.
		/// </summary>
		public Control Window
		{
			get { return _window; }
			set
			{
				ReleaseDevice();
				_window = value;
				InitializeDevice();
			}
		}

		/// <summary>
		/// Gets the parent window of the DirectX control.
		/// </summary>
		public Form ParentWindow
		{
			get { return _parent; }
		}

		/// <summary>
		/// Gets or sets the background color of the DirectX window.
		/// </summary>
		public Color ClearColor
		{
			get { return _clearColor; }
			set { _clearColor = value; }
		}

		/// <summary>
		/// Gets or sets the maximum number of frames rendered per second.
		/// </summary>
		public int FramesPerSecond
		{
			get { return _fps; }
			set { _fps = value; }
		}

		/// <summary>
		/// Gets or sets the quaternion camera used by the DirectX window.
		/// </summary>
		public QuaternionCamera Camera
		{
			get { return _camera; }
			set { _camera = value; }
		}

		/// <summary>
		/// Gets the mouse device as polled by the viewport window.
		/// </summary>
		public Mouse Mouse
		{
			get { return _mouse; }
		}

		/// <summary>
		/// Gets the keyboard device as polled by the viewport window.
		/// </summary>
		public Keyboard Keyboard
		{
			get { return _keyboard; }
		}

		/// <summary>
		/// Gets if the Direct3D device has been lost.
		/// </summary>
		public bool LostDevice
		{
			get { return _deviceLost; }
		}

		/// <summary>
		/// Gets or sets the present parameters used by the device.
		/// </summary>
		public PresentParameters PresentParameters
		{
			get { return _presentParams; }
			set { _presentParams = value; }
		}

		/// <summary>
		/// Gets or sets the list of DirectX Effects usable in rendering.
		/// </summary>
		public ArrayList Effects
		{
			get { return _effects; }
			set { _effects = value; }
		}

		/// <summary>
		/// Gets the supported version for Vertex Shaders.
		/// </summary>
		public Version SupportedVertexShaderVersion
		{
			get { return Manager.GetDeviceCaps( 0, DeviceType.Hardware ).VertexShaderVersion; }
		}

		/// <summary>
		/// Gets the supported version for Pixel Shaders.
		/// </summary>
		public Version SupportedPixelShaderVersion
		{
			get { return Manager.GetDeviceCaps( 0, DeviceType.Hardware ).PixelShaderVersion; }
		}

		/// <summary>
		/// Gets the time at the last rendering from the previous rendering.
		/// </summary>
		public DateTime LastRenderTime
		{
			get { return _lastRender; }
		}
		#endregion

		#region Initialization And Basic Methods
		/// <summary>
		/// Creates a DirectX viewport window.
		/// </summary>
		public Viewport()
		{
			_camera = null;
			_effects = new ArrayList();
		}

		/// <summary>
		/// Creates a DirectX viewport window.
		/// </summary>
		/// <param name="window">Window to load DirectX into.</param>
		/// <param name="parent">Parent form of the DirectX control.</param>
		public Viewport( Control window, Form parent )
		{
			_camera = null;
			_effects = new ArrayList();

			InitializeDXWindow( window, parent );
		}

		/// <summary>
		/// Safely releases the data within the viewport.
		/// </summary>
		public void Dispose()
		{
			ReleaseDevice();
			Camera.Dispose();

			// Clean up user interface devices
			Mouse.Dispose();
			Keyboard.Dispose();

			// Clean up Effects used
			foreach ( Effect e in _effects )
				e.Dispose();

			_effects.Clear();
		}

		/// <summary>
		/// Initializes the DirectX window.
		/// </summary>
		/// <param name="window">Window to load DirectX into.</param>
		/// <param name="parent">Parent form of the DirectX control.</param>
		public void InitializeDXWindow( Control window, Form parent )
		{
			_window = window;
			_parent = parent;
			_clearColor = Color.Blue;
			_fps = 60;
			_lastRender = DateTime.Now;
			InitializeDevice();
		}
		#endregion

		#region Device Handling
		/// <summary>
		/// Initialize the DirectX device.
		/// </summary>
		/// <returns>Whether the initialization succeeded.</returns>
		private bool InitializeDevice()
		{
			if ( _device != null && !_device.Disposed )
				ReleaseDevice();

			try
			{
				_presentParams = new PresentParameters();

				_presentParams.Windowed = true;
				_presentParams.SwapEffect = SwapEffect.Discard;
				_presentParams.EnableAutoDepthStencil = true;
				_presentParams.AutoDepthStencilFormat = DepthFormat.D16;

				// A window must be specified to load into
				// NOTE: If the adapter is updated to become dynamic, make sure to update
				// all adapter references
				if ( _window != null )
					_device = new Device( 0, DeviceType.Hardware, _window.Handle,
						CreateFlags.HardwareVertexProcessing, _presentParams );
				else
					return false;

				_device.DeviceReset += new EventHandler( this.OnDeviceReset );
				_device.DeviceLost += new EventHandler( this.OnDeviceLost );

				InitializeCamera();
				OnDeviceReset( this, new System.EventArgs() );

				_deviceLost = false;

				return true;
			}
			catch ( DirectXException e )
			{
				ThrowException( e, true );

				if ( _device != null )
				{
					_device.Dispose();
					_device = null;
				}

				_deviceLost = true;

				return false;
			}
		}

		/// <summary>
		/// The Direct3D device has been reset; re-initialize information related to
		/// DirectX.
		/// </summary>
		protected void OnDeviceReset( object sender, EventArgs e )
		{
			ResetCamera();

			try
			{
				_mouse = new Mouse( _parent );
				_keyboard = new Keyboard( _parent );

				foreach ( Effect fx in _effects )
					fx.CreateEffect( _device );
			}
			catch ( DirectXException exc )
			{
				ThrowException( exc, true );

				if ( _mouse != null )
				{
					_mouse.Dispose();
					_mouse = null;
				}

				if ( _keyboard != null )
				{
					_keyboard.Dispose();
					_keyboard = null;
				}
			}
		}

		/// <summary>
		/// Disposes of data lost when the DirectX device was lost.
		/// Note:  Managed index/vertex buffers are unaffected.
		/// </summary>
		protected void OnDeviceLost( object sender, EventArgs e )
		{
			// We would normally dispose of any vertex/index buffers here.
			// For the purposes of this library, the user should overload
			// the "_device.DeviceLost" property manually, as shown below:
			//
			// _device.DeviceLost += new EventHandler( this.OnDeviceLost );
			//
			// This overloading should be done in the user-side code.
			// Index/Vertex buffers created using "Pool.Managed" will be
			// unaffected.
		}

		/// <summary>
		/// Attemps to recover a lost Direct3D device.
		/// </summary>
		private void RecoverDevice()
		{
			try
			{
				_device.TestCooperativeLevel();
			}
			catch ( DeviceLostException )
			{
				// Do nothing
			}
			catch ( DeviceNotResetException )
			{
				try
				{
					_device.Reset( _presentParams );
					_deviceLost = false;

					Debug.WriteLine( "Device successfully reset." );
				}
				catch ( DeviceLostException )
				{
					// Do nothing
				}
			}
		}

		/// <summary>
		/// Releases the DirectX device.
		/// </summary>
		protected void ReleaseDevice()
		{
			_device.Dispose();
		}
		#endregion

		#region Rendering
		/// <summary>
		/// Checks if it is time to render again (based on the Viewport's frames per second).
		/// </summary>
		/// <returns>Whether rendering should occur.</returns>
		public bool IsTimeToRender()
		{
			DateTime now = DateTime.Now;
			TimeSpan time = now - _lastRender;
			bool render = false;
			double ms = time.Milliseconds + time.Seconds * 1000 + time.Minutes * 60000 +
				time.Hours * 3600000;

			if ( ms > ( 1000f / _fps ) )
			{
				_lastRender = now;
				render = true;
			}

			return render;
		}

		/// <summary>
		/// Begins rendering the DirectX scene.
		/// </summary>
		public void BeginRender()
		{
			if ( !_device.Disposed )
			{
				if ( _deviceLost )
				{
					RecoverDevice();
				}

				if ( !_deviceLost )
				{
					try
					{
						if ( _presentParams.EnableAutoDepthStencil == true )
							_device.Clear( ClearFlags.Target | ClearFlags.ZBuffer, _clearColor, 1.0f, 0 );
						else
							_device.Clear( ClearFlags.Target, _clearColor, 1.0f, 0 );

						_device.BeginScene();

						_device.Transform.View = _camera.View;
						_device.Transform.World = UpdateCamera();
					}
					catch ( DeviceLostException )
					{
						_deviceLost = true;
						Debug.WriteLine( "Device was lost." );
					}
					catch ( DirectXException e )
					{
						ThrowException( e, false );
					}
				}
			}
		}

		/// <summary>
		/// Ends rendering the DirectX scene.
		/// </summary>
		public void EndRender()
		{
			if ( !_device.Disposed )
			{
				try
				{
					_device.EndScene();
					_device.Present();
				}
				catch ( DeviceLostException )
				{
					_deviceLost = true;
				}
				catch ( DirectXException e )
				{
					ThrowException( e, false );
				}
			}
		}
		#endregion

		#region Camera
		/// <summary>
		/// Initializes the camera used in the DirectX window.
		/// </summary>
		private void InitializeCamera()
		{
			if ( _camera == null )
			{
				Vector3 camEye = new Vector3( 0.0f, 0.0f, 1.0f );
				Vector3 camTarget = new Vector3( 0.0f, 0.0f, 0.0f );

				_camera = new QuaternionCamera();
				_camera.SetViewParameters( camEye, camTarget );
			}
		}

		/// <summary>
		/// Updates the world matrix of the camera.
		/// </summary>
		/// <returns>World transform of the camera.</returns>
		private Matrix UpdateCamera()
		{
			Matrix world = Matrix.Identity;
			Vector3 offset = new Vector3();

			if ( _mouse != null && _camera.Moving && _mouse.Buttons[0] == false )
				_camera.EndMove();

			if ( _camera.Moving )
				_camera.Move( _mouse.Location );

			offset = _camera.Offset;
			world = Matrix.Translation( _camera.Offset );

			return world;
		}

		/// <summary>
		/// Resets the camera projection parameters.
		/// </summary>
		public void ResetCamera()
		{
			_camera.SetProjectionParameters( _camera.FieldOfView,
				( float ) _window.Width / _window.Height, _camera.NearPlane, _camera.FarPlane );
			_device.Transform.Projection = _camera.Projection;
		}

		/// <summary>
		/// Sets the world matrix of the DirectX device to create a billboard effect.
		/// </summary>
		public Matrix GetBillboardWorldMatrix()
		{
			Vector3 normal = _camera.LookVector;
			Vector3 right = _camera.RightVector;
			Vector3 up = _camera.UpVector;
			Matrix billboardMatrix = new Matrix();

			// Set Right vector
			billboardMatrix.M11 = right.X;
			billboardMatrix.M12 = right.Y;
			billboardMatrix.M13 = right.Z;

			// Set Up vector
			billboardMatrix.M21 = up.X;
			billboardMatrix.M22 = up.Y;
			billboardMatrix.M23 = up.Z;

			// Set Look vector
			billboardMatrix.M31 = normal.X;
			billboardMatrix.M32 = normal.Y;
			billboardMatrix.M33 = normal.Z;
			billboardMatrix.M44 = 1;

			return billboardMatrix;
		}
		#endregion

		#region Other Methods
		/// <summary>
		/// Alert the user of a DirectX exception.
		/// </summary>
		/// <param name="e">Exception thrown.</param>
		/// <param name="displayMessageBox">Whether or not to display a message box warning.</param>
		private void ThrowException( DirectXException e, bool displayMessageBox )
		{
			string message = "";
			
			message += "Error: " + e.Message + "\n";
			message += "\nDirectX Error Message: " + e.ErrorString + "\n";
			message += "\nSource: " + e.Source;

			if ( displayMessageBox )
			{
				MessageBox.Show( message,
					"Voyage.Terraingine.DXViewport.Viewport.InitializeDevice()",
					MessageBoxButtons.OK, MessageBoxIcon.Error );
			}

			Debug.WriteLine( message );
		}

		/// <summary>
		/// Resize the DirectX window.
		/// </summary>
		public void ResizeWindow()
		{
			// Device reset is necessary to prevent line-thickness (pixellation) stretching
			_presentParams.BackBufferHeight = _window.Height;
			_presentParams.BackBufferWidth = _window.Width;

			if ( _window.Height > 0 && _window.Width > 0 )
				_device.Reset( _presentParams );
		}
		#endregion
	}
}
