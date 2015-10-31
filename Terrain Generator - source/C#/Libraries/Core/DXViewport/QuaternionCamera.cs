using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Diagnostics;

namespace Voyage.Terraingine.DXViewport
{
	/// <summary>
	/// A quaternion-based camera class for DirectX.
	/// </summary>
	public class QuaternionCamera : QuaternionMovement
	{
		#region Enumerators
		/// <summary>
		/// Describes the current movement type of the camera.
		/// </summary>
		public enum MovementType
		{
			/// <summary>
			/// No movement allowed.
			/// </summary>
			None,

			/// <summary>
			/// Panning (strafing) movement allowed.
			/// </summary>
			Pan,
			
			/// <summary>
			/// Rotation movement allowed.
			/// </summary>
			Rotate,
			
			/// <summary>
			/// Trucking (zooming) movement allowed.
			/// </summary>
			Truck
		}
		#endregion

		#region Data Members
		// QuaternionCamera matrices
		private Matrix		_projection;
		private Matrix		_view;
		private Matrix		_world;

		// QuaternionCamera states
		private bool			_enableRotation;
		private bool			_enableMovement;
		private bool			_firstPerson;
		private bool			_chaseCam;
		private bool			_isMoving;
		private DXViewport.QuaternionCamera.MovementType	_curMovement;

		// Variables for resetting the camera to its start location
		private bool			_isResetting;
		private DateTime		_beganReset;
		private DateTime		_timeToReset;
		private QuaternionMovement	_resetTarget;
		private QuaternionMovement	_resetStartPosition;

		// Camera movement speeds
		private float			_rotateSpeed;
		private float			_panSpeed;
		private float			_truckSpeed;

		// Camera projection parameters
		private float			_fov;
		private float			_aspectRatio;
		private float			_nearPlane;
		private float			_farPlane;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the camera projection matrix
		/// </summary>
		public Matrix Projection
		{
			get { return _projection; }
			set { _projection = value; }
		}

		/// <summary>
		/// Gets or sets the camera view matrix
		/// </summary>
		public Matrix View
		{
			get { return _view; }
			set { _view = value; }
		}

		/// <summary>
		/// Gets or sets the camera world matrix
		/// </summary>
		public Matrix World
		{
			get { return _world; }
			set { _world = value; }
		}

		/// <summary>
		/// Gets or sets if the camera has rotation enabled.
		/// </summary>
		public bool EnableRotation
		{
			get { return _enableRotation; }
			set { _enableRotation = value; }
		}

		/// <summary>
		/// Gets or sets if the camera has movement enabled.
		/// </summary>
		public bool EnableMovement
		{
			get { return _enableMovement; }
			set { _enableMovement = value; }
		}

		/// <summary>
		/// Gets or sets if the camera is in first-person mode.
		/// </summary>
		public bool FirstPerson
		{
			get { return _firstPerson; }
			set { _firstPerson = value; }
		}

		/// <summary>
		/// Gets or sets if the camera is in chase-cam mode.
		/// </summary>
		public bool ChaseCam
		{
			get { return _chaseCam; }
			set { _chaseCam = value; }
		}

		/// <summary>
		/// Gets or sets if the camera is currently moving.
		/// </summary>
		public bool Moving
		{
			get { return _isMoving; }
			set { _isMoving = value; }
		}

		/// <summary>
		/// Gets or sets the current MovementType of the camera.
		/// </summary>
		public DXViewport.QuaternionCamera.MovementType CurrentMovement
		{
			get { return _curMovement; }
			set { _curMovement = value; }
		}
		
		/// <summary>
		/// Gets or sets if the camera is currently resetting itself.
		/// </summary>
		public bool Resetting
		{
			get { return _isResetting; }
			set { _isResetting = value; }
		}

		/// <summary>
		/// Gets when the last camera reset began.
		/// </summary>
		public DateTime ResetBegan
		{
			get { return _beganReset; }
		}

		/// <summary>
		/// Gets or sets the time it takes the camera to reset.
		/// </summary>
		public DateTime TimeToReset
		{
			get { return _timeToReset; }
			set { _timeToReset = value; }
		}

		/// <summary>
		/// Gets or sets the camera's reset target position and orientation.
		/// </summary>
		public QuaternionMovement ResetTarget
		{
			get { return _resetTarget; }
			set { _resetTarget = value; }
		}

		/// <summary>
		/// Gets the camera's position at the beginning of the last reset.
		/// </summary>
		public QuaternionMovement ResetStartPosition
		{
			get { return _resetStartPosition; }
		}

		/// <summary>
		/// Gets or sets the rotation speed of the camera.
		/// </summary>
		public float RotationSpeed
		{
			get { return _rotateSpeed; }
			set { _rotateSpeed = value; }
		}

		/// <summary>
		/// Gets or sets the panning (strafing) speed of the camera.
		/// </summary>
		public float PanSpeed
		{
			get { return _panSpeed; }
			set { _panSpeed = value; }
		}

		/// <summary>
		/// Gets or sets the trucking (zooming) speed of the camera.
		/// </summary>
		public float TruckSpeed
		{
			get { return _truckSpeed; }
			set { _truckSpeed = value; }
		}

		/// <summary>
		/// Gets or sets the field of view of the camera.
		/// </summary>
		public float FieldOfView
		{
			get { return _fov; }
			set { _fov = value; }
		}

		/// <summary>
		/// Gets or sets the aspect ratio of the camera.
		/// </summary>
		public float AspectRatio
		{
			get { return _aspectRatio; }
			set { _aspectRatio = value; }
		}

		/// <summary>
		/// Gets or sets the near plane of the camera.
		/// </summary>
		public float NearPlane
		{
			get { return _nearPlane; }
			set { _nearPlane = value; }
		}

		/// <summary>
		/// Gets or sets the far plane of the camera.
		/// </summary>
		public float FarPlane
		{
			get { return _farPlane; }
			set { _farPlane = value; }
		}

		/// <summary>
		/// Returns the World identity matrix relative to the camera's offset position.
		/// </summary>
		public Matrix WorldIdentity
		{
			get
			{
				Matrix world = Matrix.Identity;

				world.Translate( Offset );

				return world;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates an object of type QuaternionCamera.
		/// </summary>
		public QuaternionCamera()
		{
			_view = new Matrix();
			_projection = new Matrix();
			_world = new Matrix();

			_view = Matrix.Identity;
			_projection = Matrix.Identity;
			_world = Matrix.Identity;

			_resetTarget = new QuaternionMovement();
			_resetStartPosition = new QuaternionMovement();

			_enableRotation = true;
			_enableMovement = true;
			_firstPerson = true;
			_chaseCam = false;
			_isMoving = false;
			_curMovement = DXViewport.QuaternionCamera.MovementType.None;

			_isResetting = false;
			_beganReset = new DateTime( 0 );
			_timeToReset = DateTime.MinValue.AddSeconds( 1 );

			_rotateSpeed = ( float ) Math.PI / 180.0f;	// 1 degree
			_panSpeed = 0.01f;
			_truckSpeed = 0.1f;

			_fov = ( float ) Math.PI / 4;
			_aspectRatio = 1.333333f;
			_nearPlane = 1.0f;
			_farPlane = 1000.0f;
		}

		/// <summary>
		/// Sets the viewing parameters for the camera.
		/// </summary>
		/// <param name="eyePoint">Position of the camera.</param>
		/// <param name="lookAtPoint">Target position the camera is looking at.</param>
		public void SetViewParameters( Vector3 eyePoint, Vector3 lookAtPoint )
		{
			Vector3 lookAt = lookAtPoint - eyePoint;
			Vector3 up = new Vector3( 0.0f, 1.0f, 0.0f );
			Matrix world = new Matrix();
			Vector3 zBasis;
			float yaw, length, pitch;

			Position = eyePoint;
			FollowDistance = lookAt.Length();
			_view = Matrix.LookAtLH( eyePoint, lookAtPoint, up );

			// Calculate the world matrix
			world = Matrix.Invert( _view );

//			// Can the following steps be done more directly with a DirectX Quaternion/Matrix function?
//
//			// The axis basis vectors and camera position are stored inside the 
//			// position matrix in the 4 rows of the camera's world matrix.
//			// To figure out the yaw/pitch of the camera, we just need the Z basis vector.
//			// (According to Microsoft DirectX Sample Framework)
			zBasis = new Vector3( world.M31, world.M32, world.M33 );
			yaw = ( float ) Math.Atan2( zBasis.X, zBasis.Z );
			length = ( float ) Math.Sqrt( zBasis.Z * zBasis.Z + zBasis.X * zBasis.X );
			pitch = ( float ) -Math.Atan2( zBasis.Y, length );

			Orientation = Quaternion.RotationYawPitchRoll( yaw, pitch, 0.0f );
		}

		/// <summary>
		/// Sets the projection parameters of the camera.
		/// </summary>
		/// <param name="fov">Sets the Field-of-View angle for the camera.</param>
		/// <param name="aspectRatio">Aspect ratio for the camera.</param>
		/// <param name="nearPlane">Near viewing plane.</param>
		/// <param name="farPlane">Far viewing plane.</param>
		public void SetProjectionParameters( float fov, float aspectRatio, float nearPlane, float farPlane )
		{
			_fov = fov;
			_aspectRatio = aspectRatio;
			_nearPlane = nearPlane;
			_farPlane = farPlane;

			_projection = Matrix.PerspectiveFovLH( fov, aspectRatio, nearPlane, farPlane );
		}

		/// <summary>
		/// Moves the camera based upon time passed since last move.
		/// </summary>
		public void FrameMove()
		{
			if ( _isResetting )
			{
				DateTime time = DateTime.Now;

				// Percentage of reset move remaining, going from 1.0 to 0.0
				float percentFromDone = 1 - ( ( float ) ( time.Ticks - _beganReset.Ticks ) /
					( float ) _timeToReset.Ticks );

				Offset = _resetTarget.Position * ( 1 - percentFromDone ) +
					_resetStartPosition.Position * percentFromDone;

				if ( time.Ticks - _beganReset.Ticks >= _timeToReset.Ticks )
				{
					_isResetting = false;
					_beganReset = DateTime.MinValue;
					Offset = _resetTarget.Position;
				}
			}
		}

		/// <summary>
		/// Moves the camera based upon the camera's current MovementType.
		/// At the default rotation speed, each point in X or Y indicates a single
		/// degree of rotation.
		/// </summary>
		/// <param name="p">Change in x and y (screen coordinates).</param>
		public void Move( Point p )
		{
			if ( _isMoving && ( p.X != 0 || p.Y != 0 ) )
			{
				switch ( _curMovement )
				{
					case DXViewport.QuaternionCamera.MovementType.Pan:
						Pan( p );
						break;
					
					case DXViewport.QuaternionCamera.MovementType.Truck:
						Truck( p );
						break;
					
					case DXViewport.QuaternionCamera.MovementType.Rotate:
						Rotate( p );
						break;
					
					case DXViewport.QuaternionCamera.MovementType.None:
					default:
						break;
				}

				ResetViewMatrix();
			}
		}

		/// <summary>
		/// Begins a camera move.
		/// </summary>
		public void BeginMove()
		{
			_isMoving = true;
		}

		/// <summary>
		/// Ends a camera move.
		/// </summary>
		public void EndMove()
		{
			_isMoving = false;
		}

		/// <summary>
		/// Rotate the camera.
		/// </summary>
		/// <param name="p">Change in x and y (screen coordinates).</param>
		public void Rotate( Point p )
		{
			if ( _enableRotation )
				RotateSpherical( p.X * _rotateSpeed, p.Y * _rotateSpeed, _firstPerson );
		}

		/// <summary>
		/// Pan (strafe) the camera.
		/// </summary>
		/// <param name="p">Change in x and y (screen coordinates).</param>
		public void Pan( Point p )
		{
			if ( _enableMovement )
			{
				Hover( -p.Y * _panSpeed );
				Strafe( p.X * _panSpeed );
			}
		}

		/// <summary>
		/// Truck (zoom) the camera.
		/// </summary>
		/// <param name="p">Change in x and y (screen coordinates).</param>
		public void Truck( Point p )
		{
			if ( _enableMovement )
			{
				float distance = p.Y * _truckSpeed;

				if ( ( _followDistance - distance ) < 0 )
					distance = _followDistance;

				if ( distance != 0 )
				{
					Move( distance );
					_followDistance -= distance;
				}
			}
		}

		/// <summary>
		/// Reset the camera's world/view matrices.
		/// </summary>
		public void ResetViewMatrix()
		{
			Vector3 lookAt = LookVector;
			Vector3 up = UpVector;

			lookAt.Normalize();
			up.Normalize();

			_view = Matrix.LookAtLH( Position, lookAt, up );
		}

		/// <summary>
		/// Sets the camera's reset position to the current camera parameters.
		/// </summary>
		public void InitializeReset()
		{
			_resetTarget.Position = Offset;
		}

		/// <summary>
		/// Resets the camera's position, orientation, and look-at target.
		/// </summary>
		public void ResetCamera()
		{
			_resetStartPosition.Position = Offset;
			_resetStartPosition.FollowDistance = FollowDistance;
			_isResetting = true;
			_beganReset = DateTime.Now;
		}

		/// <summary>
		/// Safely releases the data within the camera.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Returns a ray for picking objects relative to the camera's current location.
		/// </summary>
		/// <param name="x">X-deviation in screen coordinates from the center of the DirectX viewport.</param>
		/// <param name="y">Y-deviation in screen coordinates from the center of the DirectX viewport.</param>
		/// <param name="windowWidth">The width of the DirectX viewport.</param>
		/// <param name="windowHeight">The height of the DirectX viewport.</param>
		/// <returns>Picking ray for selecting objects relative to the camera.</returns>
		public Vector3 PickingRay( int x, int y, int windowWidth, int windowHeight )
		{
			// Determine the deviation relative to the camera's Right and Up vectors
			float deltaRight = ( ( ( 2.0f * x ) / windowWidth ) - 1.0f ) / _projection.M11;
			float deltaUp = ( ( ( -2.0f * y ) / windowHeight ) + 1.0f ) / _projection.M22;

			// The picking ray is a deviation from the Look vector
			Vector3 ray = LookVector + UpVector * deltaUp + RightVector * deltaRight;

			// Make sure to normalize the result
			ray.Normalize();

			return ray;
		}
		#endregion
	}
}
