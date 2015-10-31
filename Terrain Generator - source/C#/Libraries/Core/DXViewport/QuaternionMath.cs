using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Voyage.Terraingine.DXViewport
{
	/// <summary>
	/// Performs common Quaternion math functions.
	/// </summary>
	public class QuaternionMath
	{
		#region Methods
		/// <summary>
		/// Creates an object for performing quaternion math.
		/// </summary>
		public QuaternionMath()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Rotate an orientation around an axis.
		/// </summary>
		/// <param name="angle">Angle to rotate by.</param>
		/// <param name="axis">Axis to rotate around.</param>
		/// <param name="orientation">Orientation to rotate.</param>
		/// <returns>Final orientation.</returns>
		static public Quaternion RotateAxis( float angle, Vector3 axis, Quaternion orientation )
		{
			Quaternion rotation = new Quaternion();

			rotation = Quaternion.RotationAxis( axis, angle );
			orientation *= rotation;
			orientation = Quaternion.Normalize( orientation );

			return orientation;
		}

		/// <summary>
		/// Transform an axis by an orientation.
		/// </summary>
		/// <param name="axis">Axis to transform.</param>
		/// <param name="orientation">Orientation to transform by.</param>
		/// <returns>Transformed axis.</returns>
		static public Vector3 TransformVectorByOrientation( Vector3 axis, Quaternion orientation )
		{
			Matrix rotation = new Matrix();
			Vector3 newAxis = new Vector3();

			rotation = Matrix.RotationQuaternion( orientation );
			newAxis.X = axis.X * rotation.M11 + axis.Y * rotation.M21 + axis.Z * rotation.M31 + rotation.M41;
			newAxis.Y = axis.X * rotation.M12 + axis.Y * rotation.M22 + axis.Z * rotation.M32 + rotation.M42;
			newAxis.Z = axis.X * rotation.M13 + axis.Y * rotation.M23 + axis.Z * rotation.M33 + rotation.M43;

			return newAxis;
		}
		#endregion
	}
}
