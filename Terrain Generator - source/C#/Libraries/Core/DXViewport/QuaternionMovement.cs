using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Voyage.Terraingine.DXViewport
{
	/// <summary>
	/// A quaternion-based movement object for DirectX.
	/// </summary>
	public class QuaternionMovement
	{
		#region Data Members
		/// <summary>
		/// A quaternion for holding orientation information.
		/// </summary>
		protected Quaternion	_orientation;

		/// <summary>
		/// A vector for holding position data.
		/// </summary>
		protected Vector3		_position;

		/// <summary>
		/// A vector for holding offset data.
		/// </summary>
		protected Vector3		_offset;

		/// <summary>
		/// A vector for holding scale values.
		/// </summary>
		protected Vector3		_scale;

		/// <summary>
		/// A value indicating distance from follow object.
		/// </summary>
		protected float			_followDistance;

		/// <summary>
		/// A vector indicating object velocity.
		/// </summary>
		protected Vector3		_velocity;

		/// <summary>
		/// A vector indicating the center of the object's rotation.
		/// </summary>
		protected Vector3		_rotationCenter;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the quaternion rotation of the object.
		/// </summary>
		public Quaternion Orientation
		{
			get { return _orientation; }
			set { _orientation = value; }
		}

		/// <summary>
		/// Gets or sets the position of the object.
		/// </summary>
		public Vector3 Position
		{
			get { return _position; }
			set { _position = value; }
		}

		/// <summary>
		/// Gets or sets the offset vector of the object.
		/// </summary>
		public Vector3 Offset
		{
			get { return _offset; }
			set { _offset = value; }
		}

		/// <summary>
		/// Gets or sets the scaling vector of the object.
		/// </summary>
		public Vector3 Scale
		{
			get { return _scale; }
			set { _scale = value; }
		}

		/// <summary>
		/// Gets or sets the distance the object follows its target.
		/// </summary>
		public float FollowDistance
		{
			get { return _followDistance; }
			set { _followDistance = value; }
		}

		/// <summary>
		/// Gets or sets the current velocity of the object.
		/// </summary>
		public Vector3 Velocity
		{
			get { return _velocity; }
			set { _velocity = value; }
		}

		/// <summary>
		/// Gets or sets the point around which the object rotates.
		/// </summary>
		public Vector3 RotationCenter
		{
			get { return _rotationCenter; }
			set { _rotationCenter = value; }
		}

		/// <summary>
		/// Gets the LookAt vector of the object.
		/// </summary>
		public Vector3 LookAt
		{
			get
			{
				Vector3 lookAt = new Vector3();
				lookAt = Position + LookVector * FollowDistance;

				if ( lookAt.Length() < 0.0001f )
					return Vector3.Empty;
				else
					return lookAt;
			}
		}

		/// <summary>
		/// Gets the offset position of the object.
		/// </summary>
		public Vector3 Origin
		{
			get
			{
				Vector3 result = Position + Offset;

				return result;
			}
		}

		/// <summary>
		/// Gets the object's Look vector.
		/// </summary>
		public Vector3 LookVector
		{
			get
			{
				Vector3 result = TransformVector( new Vector3( 0.0f, 0.0f, 1.0f ) );

				result.Normalize();
				return result;
			}
		}

		/// <summary>
		/// Gets the object's Up vector.
		/// </summary>
		public Vector3 UpVector
		{
			get
			{
				Vector3 result = TransformVector( new Vector3( 0.0f, 1.0f, 0.0f ) );

				result.Normalize();
				return result;
			}
		}

		/// <summary>
		/// Gets the object's Right vector.
		/// </summary>
		public Vector3 RightVector
		{
			get
			{
				Vector3 result = TransformVector( new Vector3( 1.0f, 0.0f, 0.0f ) );

				result.Normalize();
				return result;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates an object of type QuaternionMovement.
		/// </summary>
		public QuaternionMovement()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Moves the object along its Look vector.
		/// </summary>
		/// <param name="distance">Distance by which to move the object.</param>
		public void Move( float distance )
		{
			Vector3 look = LookVector;

			_position += look * distance;
		}

		/// <summary>
		/// Moves the object along its right vector.
		/// </summary>
		/// <param name="distance">Distance by which to move the object.</param>
		public void Strafe( float distance )
		{
			Vector3 right = RightVector;

			_offset += right * distance;
		}

		/// <summary>
		/// Moves the object along its up vector.
		/// </summary>
		/// <param name="distance">Distance by which to move the object.</param>
		public void Hover( float distance )
		{
			Vector3 up = UpVector;

			_offset += up * distance;
		}

		/// <summary>
		/// Create a Model matrix for the object.
		/// </summary>
		/// <returns>Model matrix for the object</returns>
		public Matrix ModelMatrix( Matrix world )
		{
			world = Matrix.Transformation( Vector3.Empty, Quaternion.Identity, _scale,
				_rotationCenter, _orientation, _position );
			world += Matrix.Translation( _offset );

			return world;
		}

		/// <summary>
		/// Get the absolute yaw, pitch, and roll angles of the object.
		/// </summary>
		/// <returns>A vector containing the three rotations.</returns>
		public Vector3 GetAbsoluteRotation()
		{
			Vector3 rotation = new Vector3();
			Vector3 xAxis = new Vector3( 1.0f, 0.0f, 0.0f );
			Vector3 yAxis = new Vector3( 0.0f, 1.0f, 0.0f );
			Vector3 zAxis = new Vector3( 0.0f, 0.0f, 1.0f );

			rotation.X = ( float ) Math.Acos( Vector3.Dot( xAxis, RightVector ) );
			rotation.Y = ( float ) Math.Acos( Vector3.Dot( yAxis, UpVector ) );
			rotation.Z = ( float ) Math.Acos( Vector3.Dot( zAxis, LookVector ) );

			return rotation;
		}

		/// <summary>
		/// Sets the rotation of the object based off of absolute yaw, pitch, and roll angles.
		/// </summary>
		/// <param name="yaw">Yaw (rotation around Y-axis) of the object.</param>
		/// <param name="pitch">Pitch (rotation around X-axis) of the object.</param>
		/// <param name="roll">Roll (rotation around Z-axis) of the object.</param>
		public void SetAbsoluteRotation( float yaw, float pitch, float roll )
		{
			_orientation = Quaternion.RotationYawPitchRoll( yaw, pitch, roll );
		}

		/// <summary>
		/// Rotate the object around its Y-axis (Yaw rotation).
		/// </summary>
		/// <param name="angle">Angle to rotate by.</param>
		public void RotateYaw( float angle )
		{
			Rotate( angle, new Vector3( 0.0f, 1.0f, 0.0f ) );
		}

		/// <summary>
		/// Rotate the object around its X-axis (Pitch rotation).
		/// </summary>
		/// <param name="angle">Angle to rotate by.</param>
		public void RotatePitch( float angle )
		{
			Rotate( angle, RightVector );
		}
		
		/// <summary>
		/// Rotate the object around its Z-axis (Roll rotation).
		/// </summary>
		/// <param name="angle">Angle to rotate by.</param>
		public void RotateRoll( float angle )
		{
			Rotate( angle, LookVector );
		}

		/// <summary>
		/// Rotate the object around the given axis.
		/// </summary>
		/// <param name="angle">Angle to rotate by.</param>
		/// <param name="axis">Axis to rotate around.</param>
		private void Rotate( float angle, Vector3 axis )
		{
			_orientation = QuaternionMath.RotateAxis( angle, axis, _orientation );
		}

		/// <summary>
		/// Rotate the object based on spherical coordinates.
		/// </summary>
		/// <param name="theta">XY-angle to rotate by.</param>
		/// <param name="phi">XZ-angle to rotate by.</param>
		/// <param name="firstPerson">Whether to do a first- or third-person rotation.</param>
		public void RotateSpherical( float theta, float phi, bool firstPerson )
		{
			if ( firstPerson )
			{
				RotateYaw( theta );
				RotatePitch( phi );
			}
			else
			{
				Vector3 lookAt = _position + LookVector * _followDistance;

				RotateYaw( theta );
				RotatePitch( phi );
				Position = lookAt - LookVector * _followDistance;
			}
		}

		/// <summary>
		/// Transforms the given axis by the object's orientation.
		/// </summary>
		/// <param name="axis">Axis to transform.</param>
		/// <returns>The transformed axis.</returns>
		private Vector3 TransformVector( Vector3 axis )
		{
			return QuaternionMath.TransformVectorByOrientation( axis, _orientation );
		}
		#endregion
	}
}
