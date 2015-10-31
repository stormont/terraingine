using System;
using System.Drawing;
using Microsoft.DirectX;

namespace Voyage.Terraingine.DataCore
{
	/// <summary>
	/// World data for a piece of terrain.
	/// </summary>
	public class TerrainPage
	{
		#region Data Members
		private string			_name;
		private Vector3			_position;
		private Quaternion		_rotation;
		private bool			_renderable;
		private TerrainPatch	_patch;
		private Vector3			_scale;
		private float			_maxVertexHeight;
		#endregion

		#region Properties
		/// <summary>
		/// Accesses the name of the TerrainPage.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Get the local position of the TerrainPage.
		/// </summary>
		public Vector3 Position
		{
			get{ return _position; }
			set{ _position = value; }
		}

		/// <summary>
		/// Get the local rotation of the TerrainPage.
		/// </summary>
		public Quaternion Rotation
		{
			get{ return _rotation; }
			set{ _rotation = value; }
		}

		/// <summary>
		/// Is the TerrainPage renderable?
		/// </summary>
		public bool Renderable
		{
			get{ return _renderable; }
			set{ _renderable = value; }
		}

		/// <summary>
		/// Get the TerrainPatch of the TerrainPage.
		/// </summary>
		public TerrainPatch TerrainPatch
		{
			get{ return _patch; }
			set{ _patch = value; }
		}

		/// <summary>
		/// Gets or sets the scale of the TerrainPage.
		/// </summary>
		public Vector3 Scale
		{
			get { return _scale; }
			set { _scale = value; }
		}

		/// <summary>
		/// Gets or sets the maximum vertex height for the TerrainPatch.
		/// </summary>
		public float MaximumVertexHeight
		{
			get { return _maxVertexHeight; }
			set
			{
				Vector3 position;

				_maxVertexHeight = value;
				_patch.RefreshBuffers = true;

				for ( int i = 0; i < _patch.Rows; i++ )
				{
					for ( int j = 0; j < _patch.Columns; j++ )
					{
						position = _patch.Vertices[i * _patch.Rows + j].Position;

						if ( position.Y > _maxVertexHeight )
						{
							position.Y = _maxVertexHeight;
							_patch.Vertices[i * _patch.Rows + j].Position = position;
						}
					}
				}

				_patch.CalculateNormals();
			}
		}
		#endregion

		#region Basic Methods
		/// <summary>
		/// Initialize a TerrainPage object.
		/// </summary>
		public TerrainPage()
		{
			Dispose();
			_patch = new TerrainPatch();
		}

		/// <summary>
		/// Creates a member-wise copy of the specified TerrainPage.
		/// </summary>
		/// <param name="page">The TerrainPage to copy.</param>
		public TerrainPage( TerrainPage page )
		{
			if ( page != null )
			{
				_name = page._name;
				_position = page._position;
				_scale = page._scale;
				_rotation = page._rotation;
				_renderable = page._renderable;
				_maxVertexHeight = page._maxVertexHeight;
				_patch = new TerrainPatch( page._patch );
			}
			else
			{
				Dispose();
				_patch = new TerrainPatch();
			}
		}

		/// <summary>
		/// Safely release the data held in the TerrainPage object.
		/// </summary>
		public void Dispose()
		{
			ResetData();

			if ( _patch != null )
				_patch.Dispose();
		}

		/// <summary>
		/// Reset the data in the TerrainPage (not including the TerrainPatch).
		/// </summary>
		public void ResetData()
		{
			_name = null;
			_position = new Vector3( 0f, 0f, 0f );
			_scale = new Vector3( 1.0f, 1.0f, 1.0f );
			_rotation = Quaternion.Identity;
			_renderable = true;
			_maxVertexHeight = 1.0f;
		}

		/// <summary>
		/// Gets the model matrix for the TerrainPage.
		/// </summary>
		/// <returns>The model matrix for the TerrainPage.</returns>
		public Matrix TerrainModelMatrix()
		{
			return Matrix.Transformation( Vector3.Empty, Quaternion.Identity, _scale,
				Vector3.Empty, _rotation, _position );
		}
		#endregion

		#region Vertex Selection And Distances
		/// <summary>
		/// Selects the nearest vertex to the picking ray.
		/// </summary>
		/// <param name="ray">Picking ray used to select a vertex.</param>
		/// <param name="origin">Origin of the picking ray.</param>
		/// <param name="endSelection">Whether to enable clearing selected vertices if one is not selected.</param>
		/// <param name="multiSelect">Whether to allow selection of multiple vertices.</param>
		public void SelectVertex( Vector3 ray, Vector3 origin, bool endSelection,
			bool multiSelect )
		{
			bool previouslySelected = false;
			int selectedVertex = _patch.FindNearestVertex( ray, origin + _position );
			float distance = VectorMath.Distance_PointToLine( ray, origin,
				_patch.Vertices[selectedVertex].Position + _position );

			if ( distance > _patch.NearestVertices / 2 )
				selectedVertex = -1;

			// If multiple-vertex selection is not allowed
			if ( !multiSelect )
			{
				// Has the vertex been previously selected?
				if ( selectedVertex > -1 && _patch.SelectedVertices[selectedVertex] )
					previouslySelected = true;

				// Is this a "begin" selection?
				if ( !endSelection )
				{
					// If the vertex has not been previously selected, clear all selected vertices
					if ( !previouslySelected )
						_patch.ResetSelectedVertices();

					// If the vertex is valid, select it
					if ( selectedVertex > -1 )
						_patch.SelectVertex( selectedVertex );
				}
			}
			else if ( !endSelection )
			{
				// Multiple-vertex selection is allowed and a vertex was selected
				// Only select new vertices on a "begin" selection
				_patch.SelectVertex( selectedVertex );
			}

			_patch.RefreshVertices = true;
		}

		/// <summary>
		/// Selects the specified vertex.
		/// </summary>
		/// <param name="index">Index of the vertex.</param>
		/// <param name="endSelection">Whether to enable clearing selected vertices if one is not selected.</param>
		/// <param name="multiSelect">Whether to allow selection of multiple vertices.</param>
		public void SelectVertex( int index, bool endSelection, bool multiSelect )
		{
			bool previouslySelected = false;

			// If multiple-vertex selection is not allowed
			if ( !multiSelect )
			{
				// Has the vertex been previously selected?
				if ( index > -1 && _patch.SelectedVertices[index] )
					previouslySelected = true;

				// Is this a "begin" selection?
				if ( !endSelection )
				{
					// If the vertex has not been previously selected, clear all selected vertices
					if ( !previouslySelected )
						_patch.ResetSelectedVertices();

					// If the vertex is valid, select it
					if ( index > -1 )
						_patch.SelectVertex( index );
				}
			}
			else if ( !endSelection )
			{
				// Multiple-vertex selection is allowed and a vertex was selected
				// Only select new vertices on a "begin" selection
				_patch.SelectVertex( index );
			}

			_patch.RefreshVertices = true;
		}

		/// <summary>
		/// Gets the normal of the nearest vertex to the intersecting ray.
		/// </summary>
		/// <param name="ray">The intersecting ray.</param>
		/// <param name="point">The point from which to draw the ray.</param>
		/// <returns>The normal of the nearest vertex.</returns>
		public Vector3 GetNearestVertexNormal( Vector3 ray, Vector3 point )
		{
			int vertex = _patch.FindNearestVertex( ray, point + _position );
			return _patch.Vertices[vertex].Normal;
		}

		/// <summary>
		/// Gets the normal of the triangle where the ray intersects the terrain.
		/// </summary>
		/// <param name="ray">The intersecting ray.</param>
		/// <param name="point">The point from which to draw the ray.</param>
		/// <param name="threshold">The distance threshold under which to get the nearest vertex normal.</param>
		/// <returns>The normal of the intersected triangle.</returns>
		public Vector3 GetIntersectNormal( Vector3 ray, Vector3 point, float threshold )
		{
			// Adjust ray to TerrainPage coordinate space
			ray.X += _position.X * _scale.X;
			ray.Y += _position.Y * _scale.Y;
			ray.Z += _position.Z * _scale.Z;

			Vector3 normal = Vector3.Empty;
			int vertex = _patch.FindNearestVertex( ray, point );
			Vector3 length;
			float thresholdSquared = threshold * threshold;

			// Check if the nearest vertex is under the threshold distance
			length = ray - _patch.Vertices[vertex].Position;

			if ( length.LengthSq() < thresholdSquared )
				normal = _patch.Vertices[vertex].Normal;

			return normal;
		}
		#endregion

		#region Vertex Movement
		/// <summary>
		/// Moves the selected vertices in the TerrainPage.
		/// </summary>
		/// <param name="distChange">The distance to move the selected vertices.</param>
		public void MoveSelectedVertices( float distChange )
		{
			MoveSelectedVertices( false, distChange, false, 0f );
		}

		/// <summary>
		/// Moves the selected vertices in the TerrainPage.
		/// </summary>
		/// <param name="enableSoftSelection">Whether to use soft selection.</param>
		/// <param name="distChange">The distance to move the selected vertices.</param>
		/// <param name="useFalloff">Whether to use falloff in blending the vertices.</param>
		/// <param name="softDistSquared">The squared soft selection term.</param>
		public void MoveSelectedVertices( bool enableSoftSelection, float distChange, bool useFalloff,
			float softDistSquared )
		{
			Vector3 position;

			for ( int i = 0; i < _patch.NumVertices; i++ )
			{
				if ( _patch.SelectedVertices[i] )
				{
					position = _patch.Vertices[i].Position;
					position.Y += distChange;

					if ( position.Y < 0.0f )
						position.Y = 0.0f;
					else if ( position.Y > _maxVertexHeight )
						position.Y = _maxVertexHeight;

					_patch.Vertices[i].Position = position;
				}
			}

			if ( enableSoftSelection )
				MoveSoftSelection( distChange, useFalloff, softDistSquared );

			_patch.CalculateNormals();
		}

		/// <summary>
		/// Moves the vertices selected using soft selection.
		/// </summary>
		/// <param name="distChange">The distance to move the selected vertices.</param>
		/// <param name="useFalloff">Whether to use falloff in blending the vertices.</param>
		/// <param name="softDistSquared">The squared soft selection term.</param>
		private void MoveSoftSelection( float distChange, bool useFalloff, float softDistSquared )
		{
			Vector3 position;
			float distance;
					
			for ( int i = 0; i < _patch.NumVertices; i++ )
			{
				if ( !_patch.SelectedVertices[i] )
				{
					position = _patch.Vertices[i].Position;
					distance = _patch.FindShortestDistanceToSelectedVertex( position );

					if ( distance <= softDistSquared )
					{
						if ( useFalloff )
							position.Y += distChange * ( 1 - distance / softDistSquared );
						else
							position.Y += distChange;

						if ( position.Y < 0.0f )
							position.Y = 0.0f;
						else if ( position.Y > _maxVertexHeight )
							position.Y = _maxVertexHeight;

						_patch.Vertices[i].Position = position;
					}
				}
			}
		}

		/// <summary>
		/// Moves the selected vertices in the TerrainPage.  Does not affect soft-selected vertices.
		/// </summary>
		/// <param name="enableSoftSelection">Whether to use soft selection.</param>
		/// <param name="height">The height to set the selected vertices to.</param>
		public void SetSelectedVerticesHeight( bool enableSoftSelection, float height )
		{
			Vector3 position;

			for ( int i = 0; i < _patch.NumVertices; i++ )
			{
				if ( _patch.SelectedVertices[i] )
				{
					position = _patch.Vertices[i].Position;
					position.Y = height;

					if ( position.Y < 0.0f )
						position.Y = 0.0f;
					else if ( position.Y > _maxVertexHeight )
						position.Y = _maxVertexHeight;

					_patch.Vertices[i].Position = position;
				}
			}

			_patch.CalculateNormals();
		}
		#endregion

		#region Other Methods
		/// <summary>
		/// Gets the plane on the terrain (defined by three points,
		/// with the fourth point as the 2D point elevated onto the plane) the given point is in.
		/// </summary>
		/// <param name="point">The point in the terrain.</param>
		/// <param name="v1">The first vertex in the plane.</param>
		/// <param name="v2">The second vertex in the plane.</param>
		/// <param name="v3">The third vertex in the plane.</param>
		/// <param name="point3d">The 3D elevated point in the terrain.</param>
		public bool GetPlane( Vector2 point, out int v1, out int v2, out int v3,
			out Vector3 point3d )
		{
			v1 = -1;
			v2 = -1;
			v3 = -1;
			point3d = Vector3.Empty;
			bool result = false;

			// Check if the point is within the bounds of the terrain
			if ( point.X < _position.X || point.X > _patch.Width ||
				point.Y < _position.Z || point.Y > _patch.Height )
				return result;

			GetPlane( point.X, point.Y, out v1, out v2, out v3 );

			// rows & cols determines the lower-left-hand corner of the quad
			// Check if the point is in the lower-right triangle of the quad
			point3d = _patch.GetPointOnPlane( v1, v2, v3, point );

			if ( !VectorMath.IsPointInTriangle( _patch.Vertices[v1].Position,
				_patch.Vertices[v2].Position, _patch.Vertices[v3].Position, point3d ) )
			{
				// The point is within the upper-left triangle
				// Return plane as counter-clockwise triangle, with the hypotenuse formed by the first
				// and last points
				v3 = v1;
				v2 = v1 + _patch.Columns;
				v1 = v1 + _patch.Columns + 1;

				if ( VectorMath.IsPointInTriangle( _patch.Vertices[v1].Position,
					_patch.Vertices[v2].Position, _patch.Vertices[v3].Position, point3d ) )
					result = true;
				else
					result = false;
			}
			else
				result = true;

			return result;
		}

		/// <summary>
		/// Gets the plane on the terrain (defined by three points) the given point is in.
		/// </summary>
		/// <param name="point">The point in the terrain.</param>
		/// <param name="v1">The first vertex in the plane.</param>
		/// <param name="v2">The second vertex in the plane.</param>
		/// <param name="v3">The third vertex in the plane.</param>
		public bool GetPlane( Vector3 point, out int v1, out int v2, out int v3 )
		{
			v1 = -1;
			v2 = -1;
			v3 = -1;
			bool result = false;

			// Check if the point is within the bounds of the terrain
			if ( point.X < _position.X || point.X > _patch.Width || point.Y < _position.Y ||
				point.Z < _position.Z || point.Z > _patch.Height )
				return false;

			GetPlane( point.X, point.Y, out v1, out v2, out v3 );

			if ( !VectorMath.IsPointInTriangle( _patch.Vertices[v1].Position,
				_patch.Vertices[v2].Position, _patch.Vertices[v3].Position, point ) )
			{
				// The point is within the upper-left triangle
				// Return plane as counter-clockwise triangle, with the hypotenuse formed by the first
				// and last points
				v3 = v1;
				v2 = v1 + _patch.Columns;
				v1 = v1 + _patch.Columns + 1;

				if ( VectorMath.IsPointInTriangle( _patch.Vertices[v1].Position,
					_patch.Vertices[v2].Position, _patch.Vertices[v3].Position, point ) )
					result = true;
				else
					result = false;
			}
			else
				result = true;

			return result;
		}

		/// <summary>
		/// Gets the vertices that make up the plane at the specified point in the terrain.
		/// </summary>
		/// <param name="xPos">The X-coordinate in the terrain from which to find the plane.</param>
		/// <param name="zPos">The Z-coordinate in the terrain from which to find the plane.</param>
		/// <param name="v1">The first vertex of the plane.</param>
		/// <param name="v2">The second vertex of the plane.</param>
		/// <param name="v3">The third vertex of the plane.</param>
		/// <returns></returns>
		private bool GetPlane( float xPos, float zPos, out int v1, out int v2, out int v3 )
		{
			bool result = false;

			// Determine the quad the point is in
			int rows = (int) ( zPos / _patch.RowHeight );
			int cols = (int) ( xPos / _patch.ColumnWidth );

			// If the point is right on the edge of the terrain, set to the last quad
			if ( rows == _patch.Rows )
				rows--;

			if ( cols == _patch.Columns )
				cols--;

			v1 = rows * _patch.Columns + cols;
			v2 = rows * _patch.Columns + cols + 1;
			v3 = (rows + 1) * _patch.Columns + cols + 1;

			return result;
		}
		#endregion
	}
}
