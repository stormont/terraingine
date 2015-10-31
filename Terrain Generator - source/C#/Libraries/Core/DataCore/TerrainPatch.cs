using System;
using System.Collections;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Voyage.Terraingine.DataCore
{
	/// <summary>
	/// Vertex, index, and material data for a piece of terrain.
	/// </summary>
	public class TerrainPatch
	{
		#region Data Members
		private int			_rows;
		private int			_columns;
		private int			_numVertices;
		private int			_numIndices;
		private short[]		_indices;
		private ArrayList	_textures;
		private bool		_refreshBuffers;
		private bool		_refreshVertices;
		private bool[]		_changedVertices;
		private bool[]		_selectedVertices;
		private float		_nearestVertices;
		private float		_width;
		private float		_height;
		private bool		_verticesSelected;
		private int			_numSelectedVertices;
		private int			_numChangedVertices;
		private int			_selectedTexture;
		private ArrayList	_texCoords;
		private CustomVertex.PositionNormal[]	_vertices;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the number of vertex rows in the TerrainPatch.
		/// </summary>
		public int Rows
		{
			get { return _rows; }
		}

		/// <summary>
		/// Gets the number of vertex columns in the TerrainPatch.
		/// </summary>
		public int Columns
		{
			get { return _columns; }
		}

		/// <summary>
		/// Gets the number of vertices in the TerrainPatch.
		/// </summary>
		public int NumVertices
		{
			get { return _numVertices; }
		}

		/// <summary>
		/// Gets the number of indices in the TerrainPatch.
		/// </summary>
		public int NumIndices
		{
			get { return _numIndices; }
		}

		/// <summary>
		/// Gets the number of textures in the TerrainPatch.
		/// </summary>
		public int NumTextures
		{
			get { return _textures.Count; }
		}

		/// <summary>
		/// Gets the vertices in the TerrainPatch.
		/// </summary>
		public CustomVertex.PositionNormal[] Vertices
		{
			get { return _vertices; }
			set { _vertices = value; }
		}

		/// <summary>
		/// Gets the indices in the TerrainPatch.
		/// </summary>
		public short[] Indices
		{
			get { return _indices; }
		}

		/// <summary>
		/// Gets the textures used in the TerrainPatch.
		/// </summary>
		public ArrayList Textures
		{
			get { return _textures; }
		}

		/// <summary>
		/// Gets the texture coordinates used in the TerrainPatch.
		/// </summary>
		public ArrayList TextureCoordinates
		{
			get { return _texCoords; }
		}

		/// <summary>
		/// Do visual buffers in a DirectX viewport need to be refreshed?
		/// (Caused by data being updated in the TerrainPatch.)
		/// </summary>
		public bool RefreshBuffers
		{
			get { return _refreshBuffers; }
			set { _refreshBuffers = value; }
		}

		/// <summary>
		/// Do visual buffers for vertex rendering in a DirectX viewport need to be refreshed?
		/// (Caused by data being updated in the TerrainPatch.)
		/// </summary>
		public bool RefreshVertices
		{
			get { return _refreshVertices; }
			set { _refreshVertices = value; }
		}

		/// <summary>
		/// Gets whether each individual vertex has been changed.
		/// </summary>
		public bool[] ChangedVertices
		{
			get { return _changedVertices; }
		}

		/// <summary>
		/// Gets or sets the selected vertices of the TerrainPatch.
		/// </summary>
		public bool[] SelectedVertices
		{
			get { return _selectedVertices; }
			set
			{
				_selectedVertices = value;
				_refreshVertices = true;
			}
		}

		/// <summary>
		/// Gets the smallest distance between vertices.
		/// </summary>
		public float NearestVertices
		{
			get { return _nearestVertices; }
		}

		/// <summary>
		/// Gets the height of the TerrainPatch.
		/// </summary>
		public float Height
		{
			get { return _height; }
		}

		/// <summary>
		/// Gets the width of the TerrainPatch.
		/// </summary>
		public float Width
		{
			get { return _width; }
		}

		/// <summary>
		/// Gets or sets if any vertices have been selected.
		/// </summary>
		public bool AreVerticesSelected
		{
			get { return _verticesSelected; }
			set { _verticesSelected = value; }
		}

		/// <summary>
		/// Gets the number of selected vertices.
		/// </summary>
		public int NumSelectedVertices
		{
			get { return _numSelectedVertices; }
		}

		/// <summary>
		/// Gets the number of vertices that need to be updated.
		/// </summary>
		public int NumChangedVertices
		{
			get { return _numChangedVertices; }
		}

		/// <summary>
		/// Gets or sets the index of the selected texture.
		/// </summary>
		public int SelectedTextureIndex
		{
			get { return _selectedTexture; }
			set
			{
				if ( value > -1 && value < _textures.Count )
					_selectedTexture = value;
			}
		}

		/// <summary>
		/// Gets or sets the data of the selected texture.
		/// </summary>
		public DataCore.Texture SelectedTexture
		{
			get { return GetTexture( _selectedTexture ); }
			set
			{
				if ( value != null )
					Textures[ _selectedTexture ] = value;
			}
		}

		/// <summary>
		/// Gets the height of each row.
		/// </summary>
		public float RowHeight
		{
			get { return _height / ( _rows - 1 ); }
		}

		/// <summary>
		/// Gets the width of each column.
		/// </summary>
		public float ColumnWidth
		{
			get { return _width / ( _columns - 1 ); }
		}
		#endregion

		#region Basic Methods
		/// <summary>
		/// Initialize a TerrainPatch object.
		/// </summary>
		public TerrainPatch()
		{
			Dispose();
		}

		/// <summary>
		/// Creates a member-wise copy of the specified TerrainPatch.
		/// </summary>
		/// <param name="patch">The TerrainPatch to copy.</param>
		public TerrainPatch( TerrainPatch patch )
		{
			if ( patch != null )
			{
				_rows				= patch._rows;
				_columns			= patch._columns;
				_numIndices			= patch._numIndices;
				_numVertices		= patch._numVertices;
				_refreshBuffers		= patch._refreshBuffers;
				_refreshVertices	= patch._refreshVertices;
				_nearestVertices	= patch._nearestVertices;
				_height				= patch._height;
				_width				= patch._width;
				_verticesSelected	= patch._verticesSelected;
				_numChangedVertices		= patch._numChangedVertices;
				_numSelectedVertices	= patch._numSelectedVertices;
				_selectedTexture		= patch._selectedTexture;
				
				// Copy list of texture coordinates
				_texCoords			= new ArrayList();

				for ( int i = 0; i < patch._texCoords.Count; i++ )
				{
					Vector2[] texCoords = new Vector2[( (Vector2[])
						patch._texCoords[i] ).Length];

					for ( int j = 0; j < texCoords.Length; j++ )
						texCoords[j] = new Vector2(
							( (Vector2[]) patch._texCoords[i] )[j].X,
							( (Vector2[]) patch._texCoords[i] )[j].Y);

					_texCoords.Add( texCoords );
				}

				// Copy textures
				_textures			= new ArrayList();

				for ( int i = 0; i < patch._textures.Count; i++ )
					_textures.Add( new Texture( (Texture) patch._textures[i] ) );

				// Copy vertices
				if ( patch._vertices != null )
				{
					_vertices = new CustomVertex.PositionNormal[patch._vertices.Length];

					for ( int i = 0; i < _vertices.Length; i++ )
					{
						_vertices[i].Position = patch._vertices[i].Position;
						_vertices[i].Normal = patch._vertices[i].Normal;
					}
				}
				else
					_vertices = null;

				// Copy indices
				if ( patch._indices != null )
				{
					_indices = new short[patch._indices.Length];

					for ( int i = 0; i < _indices.Length; i++ )
						_indices[i] = patch._indices[i];
				}
				else
					_indices = null;

				// Copy list of changed vertices
				if ( patch._changedVertices != null )
				{
					_changedVertices = new bool[patch._changedVertices.Length];

					for ( int i = 0; i < _changedVertices.Length; i++ )
						_changedVertices[i] = patch._changedVertices[i];
				}
				else
					_changedVertices = null;

				// Copy list of selected vertices
				if ( patch._selectedVertices != null )
				{
					_selectedVertices = new bool[patch._selectedVertices.Length];

					for ( int i = 0; i < _selectedVertices.Length; i++ )
						_selectedVertices[i] = patch._selectedVertices[i];
				}
				else
					_selectedVertices = null;
			}
			else
				Dispose();
		}

		/// <summary>
		/// Safely release the data held in the TerrainPatch object.
		/// </summary>
		public void Dispose()
		{
			_columns			= 0;
			_numIndices			= 0;
			_numVertices		= 0;
			_refreshBuffers		= false;
			_refreshVertices	= false;
			_rows				= 0;
			_vertices			= null;
			_indices			= null;
			_changedVertices	= null;
			_selectedVertices	= null;
			_nearestVertices	= -1.0f;
			_height				= 0f;
			_width				= 0f;
			_verticesSelected	= false;
			_textures			= new ArrayList();
			_texCoords			= new ArrayList();
			_selectedTexture	= -1;
			_numChangedVertices		= 0;
			_numSelectedVertices	= 0;

			for ( int i = 0; i < _textures.Count; i++ )
				( (DataCore.Texture) _textures[i] ).Dispose();

			_textures.Clear();
		}
		#endregion

		#region Building Terrain
		/// <summary>
		/// Create a new TerrainPatch.
		/// Rows and columns specify the number of vertices in the given directions.
		/// Scale indicates the size of the patch.
		/// </summary>
		/// <param name="rows">Number of rows to create.</param>
		/// <param name="columns">Number of columns to create.</param>
		public void CreatePatch( int rows, int columns )
		{
			CreatePatch( rows, columns, 1.0f, 1.0f );
		}

		/// <summary>
		/// Create a new TerrainPatch.
		/// Rows and columns specify the number of vertices in the given directions.
		/// Scale indicates the size of the patch.
		/// </summary>
		/// <param name="rows">Number of rows to create.</param>
		/// <param name="columns">Number of columns to create.</param>
		/// <param name="xScale">Distance from the first column to the last.</param>
		/// <param name="yScale">Distance from the first row to the last.</param>
		public void CreatePatch( int rows, int columns, float xScale, float yScale )
		{
			// Error check
			if ( rows < 2 )
				throw new Exception( "Rows must be greater than 1" );

			if ( columns < 2 )
				throw new Exception( "Columns must be greater than 1" );

			if ( xScale <= 0.0f )
				xScale = 1.0f;

			if ( yScale <= 0.0f )
				yScale = 1.0f;

			// Dispose existing data
			Dispose();

			// Set new data
			_rows = rows;
			_columns = columns;
			_height = yScale;
			_width = xScale;

			_vertices = new CustomVertex.PositionNormal[rows * columns];
			_indices = new short[NumFaces() * 3];
			_changedVertices = new bool[rows * columns];
			_selectedVertices = new bool[rows * columns];
			ResetChangedVertices();
			ResetSelectedVertices();

			_numVertices = rows * columns;
			_numIndices = NumFaces() * 3;

			// Calculate the distance between vertices along xy-plane
			float changeX = xScale / ( columns - 1 );
			float changeY = yScale / ( rows - 1 );

			if ( changeX < changeY )
				_nearestVertices = changeX;
			else
				_nearestVertices = changeY;

			// Specify starting vertex position
			int changeIndex = 0;
			int curRow = 0;

			// Calculate vertex positions
			for ( int i = 0; i < rows; i++ )
			{
				for ( int j = 0; j < columns; j++ )
				{
					_vertices[i * columns + j] = new CustomVertex.PositionNormal();
					_vertices[i * columns + j].Position =
						new Vector3( j * changeX, 0.0f, i * changeY );
					_vertices[i * columns + j].Normal = new Vector3( 0f, 1.0f, 0f );
				}
			}

			// Calculate indices list
			for ( int count = 0; count < _numIndices; count += 6 )
			{
				changeIndex = ( count / 6 ) + curRow;

				if ( ( ( int ) ( changeIndex + 1 ) / columns ) != curRow )
				{
					changeIndex++;
					curRow++;
				}

				_indices[count]		= ( short ) ( changeIndex );
				_indices[count + 1]	= ( short ) ( changeIndex + columns );
				_indices[count + 2]	= ( short ) ( changeIndex + columns + 1 );
				_indices[count + 3]	= ( short ) ( changeIndex + columns + 1 );
				_indices[count + 4]	= ( short ) ( changeIndex + 1 );
				_indices[count + 5]	= ( short ) ( changeIndex );
			}
		}

		/// <summary>
		/// Reset the data in the TerrainPage (not including the TerrainPatch).
		/// </summary>
		public void ResetData()
		{
			Vector3 position;

			ResetChangedVertices();
			ResetSelectedVertices();

			for ( int i = 0; i < _vertices.Length; i++ )
			{
				// Reset the vertex position
				position = _vertices[i].Position;
				position.Y = 0f;
				_vertices[i].Position = position;

				// Reset the vertex normal
				position = new Vector3( 0f, 1f, 0f );
				_vertices[i].Normal = position;
			}
		}

		/// <summary>
		/// Updates the current dimensions of the existing TerrainPatch.
		/// </summary>
		/// <param name="xScale">Distance from the first column to the last.</param>
		/// <param name="yScale">Distance from the first row to the last.</param>
		public void UpdateTerrainDimensions( float xScale, float yScale )
		{
			// Error check
			if ( xScale <= 0.0f )
				xScale = 1.0f;

			if ( yScale <= 0.0f )
				yScale = 1.0f;

			float yPosition;

			// Set new data
			_height = yScale;
			_width = xScale;

			// Calculate the distance between vertices along xy-plane
			float changeX = xScale / ( _columns - 1 );
			float changeY = yScale / ( _rows - 1 );

			if ( changeX < changeY )
				_nearestVertices = changeX;
			else
				_nearestVertices = changeY;

			// Update vertex positions
			for ( int j = 0; j < _rows; j++ )
			{
				for ( int i = 0; i < _columns; i++ )
				{
					yPosition = _vertices[ i + j * _columns ].Position.Y;
					_vertices[ i + j * _columns ].Position =
						new Vector3( i * changeX, yPosition, j * changeY );
				}
			}

			MarkAllChangedVertices();
		}
		#endregion

		#region Modified Vertices Handling
		/// <summary>
		/// Indicates that the specified index has been changed and should be updated in the
		/// top-level program.
		/// </summary>
		/// <param name="index">Index to update.</param>
		public void MarkChangedVertex( int index )
		{
			if ( index > -1 && index < _changedVertices.Length && !_changedVertices[index] )
			{
				_changedVertices[index] = true;
				_refreshBuffers = true;
				_numChangedVertices++;
			}
		}

		/// <summary>
		/// Marks all vertices as changed.
		/// </summary>
		public void MarkAllChangedVertices()
		{
			for ( int i = 0; i < _numVertices; i++ )
				_changedVertices[i] = true;

			_refreshBuffers = true;
			_numChangedVertices = _numVertices;
		}

		/// <summary>
		/// Resets all changed vertices to an "unchanged" setting.
		/// </summary>
		public void ResetChangedVertices()
		{
			for ( int count = 0; count < _changedVertices.Length; count++ )
				_changedVertices[count] = false;

			_refreshBuffers = false;
			_numChangedVertices = 0;
		}
		#endregion

		#region Vertex Selection
		/// <summary>
		/// Marks the specified vertex as selected.
		/// </summary>
		/// <param name="index">The index to select.</param>
		public void SelectVertex( int index )
		{
			if ( index > -1 && index < _selectedVertices.Length && !_selectedVertices[index] )
			{
				_selectedVertices[index] = true;
				_verticesSelected = true;
				_refreshVertices = true;
				_numSelectedVertices++;
			}
		}

		/// <summary>
		/// Removes the marking of a selected vertex.
		/// </summary>
		/// <param name="index">The index to un-select.</param>
		public void RemoveSelectedVertex( int index )
		{
			if ( index > -1 && index < _selectedVertices.Length && _selectedVertices[index] )
			{
				_selectedVertices[index] = false;
				_numSelectedVertices--;
				_refreshVertices = true;

				if ( _numSelectedVertices < 1 )
					_verticesSelected = false;
			}
		}

		/// <summary>
		/// Marks all vertices as unselected.
		/// </summary>
		public void ResetSelectedVertices()
		{
			for ( int count = 0; count < _selectedVertices.Length; count++ )
				_selectedVertices[count] = false;

			_verticesSelected = false;
			_numSelectedVertices = 0;
		}

		/// <summary>
		/// Gets a list of the selected vertex positions.
		/// </summary>
		/// <returns>The list of selected vertex positions.</returns>
		public Vector3[] GetSelectedVertexPositions()
		{
			Vector3[] positions = new Vector3[_numSelectedVertices];
			int count = 0;

			for ( int i = 0; i < _numVertices && count < _numSelectedVertices; i++ )
			{
				if ( _selectedVertices[i] )
				{
					positions[count] = _vertices[i].Position;
					count++;
				}
			}

			return positions;
		}

		/// <summary>
		/// Gets the indices of the selected vertices.
		/// </summary>
		/// <returns>The list of selected vertex indices.</returns>
		public int[] GetSelectedVertexIndices()
		{
			int[] vertices = new int[_numSelectedVertices];
			int count = 0;

			for ( int i = 0; i < _numVertices && count < _numSelectedVertices; i++ )
			{
				if ( _selectedVertices[i] )
				{
					vertices[count] = i;
					count++;
				}
			}

			return vertices;
		}

		/// <summary>
		/// Finds the shortest distance from the specified position on the TerrainPatch to the nearest
		/// selected vertex.
		/// </summary>
		/// <param name="position">Position to find shortest distance from.</param>
		/// <returns>The shortest distance to the nearest selected vertex.</returns>
		public float FindShortestDistanceToSelectedVertex( Vector3 position )
		{
			float distance = 0.0f;
			float tempDistance;
			Vector3 length;
			Vector3[] selectedPositions = GetSelectedVertexPositions();

			for ( int i = 0; i < selectedPositions.Length; i++ )
			{
				length = position - selectedPositions[i];
				length.Y = 0.0f;
				tempDistance = length.LengthSq();

				if ( tempDistance < distance || distance == 0.0f )
					distance = tempDistance;
			}

			return distance;
		}

		/// <summary>
		/// Finds the nearest vertex to the picking ray.
		/// </summary>
		/// <param name="ray">Picking ray used to select a vertex.</param>
		/// <param name="origin">Origin of the picking ray.</param>
		/// <returns>The index of the nearest vertex.</returns>
		public int FindNearestVertex( Vector3 ray, Vector3 origin )
		{
			int selectedVertex = -1;
			float distance;
			float nearestDistance = -1.0f;

			for ( int i = 0; i < _numVertices; i++ )
			{
				distance = VectorMath.Distance_PointToLine( ray, origin,
					_vertices[i].Position );

				if ( selectedVertex < 0 || distance < nearestDistance )
				{
					selectedVertex = i;
					nearestDistance = distance;
				}
			}

			return selectedVertex;
		}
		#endregion

		#region Textures
		/// <summary>
		/// Adds a texture to the list of textures used by the TerrainPatch.
		/// </summary>
		/// <param name="tex">The Texture to add to the TerrainPatch.</param>
		public void AddTexture( DataCore.Texture tex )
		{
			_textures.Add( tex );
			AddTextureCoordinates();
		}

		/// <summary>
		/// Adds a blank texture to the list of textures used by the TerrainPatch.
		/// </summary>
		public void AddBlankTexture()
		{
			Texture tex = new Texture();

			_textures.Add( tex );
			AddTextureCoordinates();
		}

		/// <summary>
		/// Removes the currently selected Texture from the list of textures used by the TerrainPatch.
		/// </summary>
		public void RemoveTexture()
		{
			RemoveTexture( _selectedTexture );
		}

		/// <summary>
		/// Removes the specified Texture from the list of textures used by the TerrainPatch.
		/// </summary>
		/// <param name="index">The index of the Texture to remove.</param>
		public void RemoveTexture( int index )
		{
			if ( index > -1 && index < _textures.Count )
			{
				( (DataCore.Texture) _textures[index] ).Dispose();
				_textures.RemoveAt( index );
				_texCoords.RemoveAt( index );
			}
		}

		/// <summary>
		/// Returns the currently selected Texture from the list of textures used by the TerrainPatch.
		/// </summary>
		/// <returns>The indicated texture returned (null if an invalid index is specified).</returns>
		public DataCore.Texture GetTexture()
		{
			return GetTexture( _selectedTexture );
		}

		/// <summary>
		/// Returns the specified Texture from the list of textures used by the TerrainPatch.
		/// </summary>
		/// <param name="index">The index of the Texture to return.</param>
		/// <returns>The indicated texture returned (null if an invalid index is specified).</returns>
		public DataCore.Texture GetTexture( int index )
		{
			DataCore.Texture texture = null;

			if ( index > -1 && index < _textures.Count )
				texture = (DataCore.Texture) Textures[index];

			return texture;
		}

		/// <summary>
		/// Sets the currently selected Texture from the list of textures used by the TerrainPatch.
		/// </summary>
		/// <param name="tex">The texture to store in the TerrainPatch.</param>
		public void SetTexture( DataCore.Texture tex )
		{
			if ( tex != null )
				Textures[ _selectedTexture ] = tex;
		}

		/// <summary>
		/// Sets the index of the currently selected Texture used by the TerrainPatch.
		/// </summary>
		/// <param name="index">The index of the Texture to select</param>
		public void SetTexture( int index )
		{
			if ( index > -1 && index < _textures.Count )
				_selectedTexture = index;
		}
		#endregion

		#region Texture Coordinates
		/// <summary>
		/// Adds a list of texture coordinates to the TerrainPatch.
		/// </summary>
		private void AddTextureCoordinates()
		{
			Vector2[] texCoords = new Vector2[_numVertices];
			int index;

			for ( int i = 0; i < texCoords.Length; i++ )
				texCoords[i] = new Vector2();

			index = _texCoords.Add( texCoords );
			SetTextureCoordinates( index );
		}

		/// <summary>
		/// Scales and shifts the texture coordinates for the currently selected texture.
		/// </summary>
		public void SetTextureCoordinates()
		{
			SetTextureCoordinates( _selectedTexture );
		}

		/// <summary>
		/// Scales and shifts the texture coordinates for the specified texture.
		/// </summary>
		/// <param name="index">Texture to update coordinates for.</param>
		public void SetTextureCoordinates( int index )
		{
			if ( index > -1 && index < _texCoords.Count )
			{
				Vector2[] texCoords = new Vector2[_numVertices];
				Vector2[] oldCoords = GetTextureCoordinateList( index );
				float uScale = ( (Texture) _textures[index] ).Scale.X;
				float vScale = ( (Texture) _textures[index] ).Scale.Y;
				float uShift = ( (Texture) _textures[index] ).Shift.X;
				float vShift = ( (Texture) _textures[index] ).Shift.Y;

				for ( int i = 0; i < _rows; i++ )
				{
					for ( int j = 0; j < _columns; j++ )
					{
						texCoords[i * _columns + j] = new Vector2();
						texCoords[i * _columns + j].X = ( ( (float) j ) / ( (float) ( _columns - 1 ) ) ) /
							uScale + uShift;
						texCoords[i * _columns + j].Y = ( ( (float) i ) / ( (float) ( _rows - 1 ) ) ) /
							vScale + vShift;
					}
				}

				_texCoords[index] = texCoords;
			}
		}

		/// <summary>
		/// Gets the texture coordinates for the currently selected texture.
		/// </summary>
		/// <returns>Coordinates for the texture.</returns>
		public Vector2[] GetTextureCoordinateList()
		{
			return GetTextureCoordinateList( _selectedTexture );
		}

		/// <summary>
		/// Gets the texture coordinates for the specified texture.
		/// </summary>
		/// <param name="index">Index of the texture to get coordinates for.</param>
		/// <returns>Coordinates for the texture.</returns>
		public Vector2[] GetTextureCoordinateList( int index )
		{
			if ( index > -1 && index < _texCoords.Count )
				return (Vector2[]) _texCoords[index];
			else
				return null;
		}

		/// <summary>
		/// Sets the texture coordinates for the currently selected texture.
		/// </summary>
		/// <param name="texCoords">Texture coordinates to set in the texture.</param>
		public void SetTextureCoordinateList( Vector2[] texCoords )
		{
			SetTextureCoordinateList( _selectedTexture, texCoords );
		}

		/// <summary>
		/// Sets the texture coordinates for the specified texture.
		/// </summary>
		/// <param name="index">Index of the texture to set coordinates for.</param>
		/// <param name="texCoords">Texture coordinates to set in the texture.</param>
		public void SetTextureCoordinateList( int index, Vector2[] texCoords )
		{
			if ( index > -1 && index < _texCoords.Count )
			{
				_texCoords.RemoveAt( index );
				_texCoords.Insert( index, texCoords );
			}
		}
		#endregion

		#region Other Methods
		/// <summary>
		/// Returns the number of triangle faces in the TerrainPatch.
		/// </summary>
		public int NumFaces()
		{
			return ( (_rows - 1) * (_columns - 1) * 2 );
		}

		/// <summary>
		/// Calculates the vertex normals for each TerrainPatch vertex.
		/// </summary>
		public void CalculateNormals()
		{
			Vector3 normal;

			for ( int i = 0; i < _rows; i++ )
			{
				for ( int j = 0; j < _columns; j++ )
				{
					normal = Vector3.Empty;

					// Check if there are two triangles below and to the left of the vertex
					if ( i > 0 && j > 0 )
					{
						// Add the weighted plane normals to the running total
						normal += VectorMath.GetWeightedPlaneNormal( _vertices[i * _columns + j].Position,
							_vertices[i * _columns + j - _columns].Position,
							_vertices[i * _columns + j - _columns - 1].Position );
						normal += VectorMath.GetWeightedPlaneNormal( _vertices[i * _columns + j].Position,
							_vertices[i * _columns + j - _columns - 1].Position,
							_vertices[i * _columns + j - 1].Position );
					}

					// Check if there is a triangle above and to the left of the vertex
					if ( i < _rows - 1 && j > 0 )
					{
						// Add the weighted plane normal to the running total
						normal += VectorMath.GetWeightedPlaneNormal( _vertices[i * _columns + j].Position,
							_vertices[i * _columns + j - 1].Position,
							_vertices[i * _columns + j + _columns].Position );
					}

					// Check if there is a triangle below and to the right of the vertex
					if ( i > 0 && j < _columns - 1 )
					{
						// Add the weighted plane normal to the running total
						normal += VectorMath.GetWeightedPlaneNormal( _vertices[i * _columns + j].Position,
							_vertices[i * _columns + j + 1].Position,
							_vertices[i * _columns + j - _columns].Position );
					}

					// Check if there are two triangles above and to the right of the vertex
					if ( i < _rows - 1 && j < _columns - 1 )
					{
						// Add the weighted plane normals to the running total
						normal += VectorMath.GetWeightedPlaneNormal( _vertices[i * _columns + j].Position,
							_vertices[i * _columns + j + _columns].Position,
							_vertices[i * _columns + j + _columns + 1].Position );
						normal += VectorMath.GetWeightedPlaneNormal( _vertices[i * _columns + j].Position,
							_vertices[i * _columns + j + _columns + 1].Position,
							_vertices[i * _columns + j + 1].Position );
					}

					normal.Normalize();
					_vertices[i * _columns + j].Normal = normal;
				}
			}

			_refreshBuffers = true;
		}

		/// <summary>
		/// Gets the specified XZ-point elevated to the height of the plane.
		/// </summary>
		/// <param name="v1">The first vertex to build the plane from.</param>
		/// <param name="v2">The second vertex to build the plane from.</param>
		/// <param name="v3">The third vertex to build the plane from.</param>
		/// <param name="origin">The point to elevate onto the plane.</param>
		/// <returns>The origin point elevated up to the height of the plane.</returns>
		public Vector3 GetPointOnPlane( int v1, int v2, int v3, Vector2 origin )
		{
			Vector3 point = Vector3.Empty;

			if ( v1 > -1 && v2 > -1 && v3 > -1 && v1 < _numVertices && v2 < _numVertices && v3 < _numVertices )
			{
				Vector3 p1 = _vertices[v1].Position;
				Vector3 p2 = _vertices[v2].Position;
				Vector3 p3 = _vertices[v3].Position;
				Vector2 basis = VectorMath.ConvertBasis( new Vector2( p2.X - p1.X, p2.Z - p1.Z ),
					new Vector2( p3.X - p1.X, p3.Z - p1.Z ), new Vector2( origin.X - p1.X, origin.Y - p1.Z ) );

				point = Vector3.BaryCentric( p1, p2, p3, basis.X, basis.Y );
			}

			return point;
		}

		/// <summary>
		/// Gets the normal on the given plane from the given point. If the given point is on a vertex,
		/// the normal returned will be the weighted vertex normal, rather than the plane normal.
		/// </summary>
		/// <param name="v1">The first vertex to build the plane from.</param>
		/// <param name="v2">The second vertex to build the plane from.</param>
		/// <param name="v3">The third vertex to build the plane from.</param>
		/// <param name="origin">The point to get the normal on the plane from.</param>
		/// <returns>The normal of the plane from the point.</returns>
		public Vector3 GetPointNormal( int v1, int v2, int v3, Vector3 origin )
		{
			Vector3 normal = Vector3.Empty;
			Vector3 norm1, norm2, norm3;
			float dist1, dist2, dist3, distTotal;

			if ( v1 > -1 && v2 > -1 && v3 > -1 && v1 < _numVertices && v2 < _numVertices && v3 < _numVertices )
			{
				dist1 = ( (Vector3) _vertices[v1].Position - origin ).Length();
				dist2 = ( (Vector3) _vertices[v2].Position - origin ).Length();
				dist3 = ( (Vector3) _vertices[v3].Position - origin ).Length();
				distTotal = dist1 + dist2 + dist3;

				norm1 = ( dist1 / distTotal ) * _vertices[v1].Normal;
				norm2 = ( dist2 / distTotal ) * _vertices[v2].Normal;
				norm3 = ( dist3 / distTotal ) * _vertices[v3].Normal;
				normal = norm1 + norm2 + norm3;
			}

			return normal;
		}

		/// <summary>
		/// Gets the index for the vertex at the specified position.
		/// </summary>
		/// <param name="position">The position to get the index from.</param>
		/// <returns>The index for the vertex at the specified position.</returns>
		public int GetVertexIndex( Vector3 position )
		{
			int vertex = -1;

			// Check that the vertex position is within the bounds of the terrain
			if ( position.X <= _width && position.Z <= _height )
			{
				int row = (int) ( position.X / ColumnWidth );
				int col = (int) ( position.Z / RowHeight );

				vertex = row * _columns + col;
			}

			return vertex;
		}
		#endregion
	}
}
