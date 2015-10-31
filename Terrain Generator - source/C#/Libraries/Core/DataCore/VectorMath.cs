using System;
using Microsoft.DirectX;

namespace Voyage.Terraingine.DataCore
{
	/// <summary>
	/// Object for performing mathematical computations.
	/// </summary>
	public class VectorMath
	{
		#region Methods
		/// <summary>
		/// Creates an object of type VectorMath.
		/// </summary>
		public VectorMath()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Gets the shortest distance between the given point and the given line.
		/// </summary>
		/// <param name="ray">Ray to build the line from.</param>
		/// <param name="origin">Origin point to build the line from.</param>
		/// <param name="point">Point to compare the distance from.</param>
		/// <returns>The shortest distance from the point.</returns>
		static public float Distance_PointToLine( Vector3 ray, Vector3 origin, Vector3 point )
		{
			if ( ray.Length() < 0.0f )
				return -1.0f;

			Vector3 pointVector = point - origin;
			double theta;
			float length = pointVector.Length();

			if ( length < 0.0f )
				return -1.0f;

			if ( double.IsNaN( Math.Acos( Vector3.Dot( ray, pointVector ) / ( ray.Length() * length ) ) ) )
				theta = 0;
			else
				theta = Math.Acos( Vector3.Dot( ray, pointVector ) / ( ray.Length() * length ) );

			return length * ( float ) Math.Sin( theta );
		}

		/// <summary>
		/// Get the normal vector to a plane created by three points.
		/// </summary>
		/// <param name="p1">The first point in the plane.</param>
		/// <param name="p2">The second point in the plane.</param>
		/// <param name="p3">The third point in the plane.</param>
		/// <returns>The unit normal vector to the plane formed by the three points.</returns>
		static public Vector3 GetPlaneNormal( Vector3 p1, Vector3 p2, Vector3 p3 )
		{
			Vector3 a = p2 - p1;
			Vector3 b = p3 - p1;
			Vector3 n = Vector3.Cross( a, b );

			n.Normalize();
			return n;
		}

		/// <summary>
		/// Get the normal vector to a plane created by three points.
		/// Normal vectors will be weighted by the size of the triangle formed by the three points.
		/// </summary>
		/// <param name="p1">The first point in the plane.</param>
		/// <param name="p2">The second point in the plane.</param>
		/// <param name="p3">The third point in the plane.</param>
		/// <returns>The non-unit normal vector to the plane formed by the three points.</returns>
		static public Vector3 GetWeightedPlaneNormal( Vector3 p1, Vector3 p2, Vector3 p3 )
		{
			Vector3 a = p2 - p1;
			Vector3 b = p3 - p1;
			Vector3 n = Vector3.Cross( a, b );
			float area = 0.5f * n.Length();

			return n;
		}

		/// <summary>
		/// Gets the intersection point of a line and a plane.
		/// </summary>
		/// <param name="p1">The first point forming the plane.</param>
		/// <param name="p2">The second point forming the plane.</param>
		/// <param name="p3">The third point forming the plane.</param>
		/// <param name="ray">The ray forming the intersecting line.</param>
		/// <param name="origin">The origin point of the intersecting ray.</param>
		/// <returns>The intersection point of the line and the plane.</returns>
		static public Vector3 GetLineIntersectPlane( Vector3 p1, Vector3 p2, Vector3 p3, Vector3 ray, Vector3 origin )
		{
			// N dot P + d = 0
			// P = 0 + t*V
			// d = -(N dot P)
			//
			// Therefore:
			//   t = (N dot P - N dot O) / (N dot V)
			// Solve P = 0 + t*V
			Vector3 normal = GetPlaneNormal( p1, p2, p3 );
			Vector3 line = ray - origin;
			float scalar = ( Vector3.Dot( normal, line ) - Vector3.Dot( normal, origin ) ) /
				Vector3.Dot( normal, ray );
			Vector3 point = origin + scalar * ray;

			return point;
		}

		/// <summary>
		/// Converts the bases for the specified vector from the basis definition vectors.
		/// </summary>
		/// <param name="b1">The first basis definition vector.</param>
		/// <param name="b2">The second basis definition vector.</param>
		/// <param name="w">The vector to convert bases on.</param>
		/// <returns>The new basis of the vector.</returns>
		static public Vector2 ConvertBasis( Vector2 b1, Vector2 b2, Vector2 w )
		{
			//
			// [w] = B^(-1) * w = [ B.M11 * w.X + B.M12 * w.Y ]
			//					  [ B.M21 * w.X + B.M22 * w.Y ]
			//
			Vector2 basis = Vector2.Empty;
			Matrix B = new Matrix();

			// Build inverse basis matrix
			B.M11 = b1.X;
			B.M21 = b1.Y;
			B.M12 = b2.X;
			B.M22 = b2.Y;
			B.M33 = 1;
			B.M44 = 1;

			B.Invert();

			// Determine basis
			basis.X = B.M11 * w.X + B.M12 * w.Y;
			basis.Y = B.M21 * w.X + B.M22 * w.Y;

			return basis;
		}

		/// <summary>
		/// Checks if the given point is in the specified triangle.
		/// </summary>
		/// <param name="p1">The first point forming the triangle.</param>
		/// <param name="p2">The second point forming the triangle.</param>
		/// <param name="p3">The third point forming the triangle.</param>
		/// <param name="point">The point to check if it is in the triangle.</param>
		/// <returns>Whether the point is in the triangle or not.</returns>
		static public bool IsPointInTriangle( Vector3 p1, Vector3 p2, Vector3 p3, Vector3 point )
		{
			if ( CheckIfSameSide( point, p1, p2, p3 ) && CheckIfSameSide( point, p2, p1, p3 ) &&
				CheckIfSameSide( point, p3, p1, p2 ) )
				return true;
			else
				return false;
		}

		/// <summary>
		/// Check if both points are on the same side of the line.
		/// </summary>
		/// <param name="p1">The first point to check.</param>
		/// <param name="p2">The second point to check.</param>
		/// <param name="a">The line begin point.</param>
		/// <param name="b">The line end point</param>
		/// <returns>Whether both points are on the same side of the line.</returns>
		static protected bool CheckIfSameSide( Vector3 p1, Vector3 p2, Vector3 a, Vector3 b )
		{
			Vector3 cp1 = Vector3.Cross( b - a, p1 - a );
			Vector3 cp2 = Vector3.Cross( b - a, p2 - a );

			if ( Vector3.Dot( cp1, cp2 ) >= 0 )
				return true;
			else
				return false;
		}
		#endregion
	}
}
