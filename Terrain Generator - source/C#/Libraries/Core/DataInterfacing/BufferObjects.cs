using System;
using System.Drawing;
using Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine.DataInterfacing
{
	/// <summary>
	/// A class for building vertex and index buffers for terrain.
	/// </summary>
	public class BufferObjects
	{
		#region Data Members
		private DXViewport.Viewport	_viewport;
		private D3D.VertexBuffer	_vb, _vbVerts, _vbByHeight;
		private D3D.IndexBuffer	_ib, _ibVerts;
		private TerrainPage		_page;
		private int				_ibSize, _vbSize, _ibVertSize, _vbVertSize;
		private bool			_colorByHeight;
		private bool			_falloff;
		private float			_originalZoomFactor;
		private bool			_showSelectedVertices;
		private float			_softDistanceSquared;
		private bool			_softSelection;
		private float			_vertexSize;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the TerrainPage.
		/// </summary>
		public TerrainPage TerrainPage
		{
			get { return _page; }
			set { _page = value; }
		}

		/// <summary>
		/// Gets or sets the VertexBuffer for the TerrainPage.
		/// </summary>
		public D3D.VertexBuffer VertexBuffer
		{
			get { return _vb; }
			set { _vb = value; }
		}

		/// <summary>
		/// Gets or sets the VertexBuffer for the vertices of the TerrainPage.
		/// </summary>
		public D3D.VertexBuffer VertexBuffer_Vertices
		{
			get { return _vbVerts; }
			set { _vbVerts = value; }
		}

		/// <summary>
		/// Gets or sets the VertexBuffer for the TerrainPage (using shading by vertex height).
		/// </summary>
		public D3D.VertexBuffer VertexBuffer_ColoredByHeight
		{
			get { return _vbByHeight; }
			set { _vbByHeight = value; }
		}

		/// <summary>
		/// Gets or sets the IndexBuffer for the TerrainPage.
		/// </summary>
		public D3D.IndexBuffer IndexBuffer
		{
			get { return _ib; }
			set { _ib = value; }
		}

		/// <summary>
		/// Gets or sets the IndexBuffer for the vertices of the TerrainPage.
		/// </summary>
		public D3D.IndexBuffer IndexBuffer_Vertices
		{
			get { return _ibVerts; }
			set { _ibVerts = value; }
		}

		/// <summary>
		/// Gets or sets the size of the IndexBuffer.
		/// </summary>
		public int IndexBufferSize
		{
			get { return _ibSize; }
			set { _ibSize = value; }
		}

		/// <summary>
		/// Gets or sets the size of the VertexBuffer.
		/// </summary>
		public int VertexBufferSize
		{
			get { return _vbSize; }
			set { _vbSize = value; }
		}

		/// <summary>
		/// Gets or sets the size of the IndexBuffer for vertices.
		/// </summary>
		public int IndexBufferSize_Vertices
		{
			get { return _ibVertSize; }
			set { _ibVertSize = value; }
		}

		/// <summary>
		/// Gets or sets the size of the VertexBuffer for vertices.
		/// </summary>
		public int VertexBufferSize_Vertices
		{
			get { return _vbVertSize; }
			set { _vbVertSize = value; }
		}

		/// <summary>
		/// Gets or sets the main viewport DirectX interface.
		/// </summary>
		public DXViewport.Viewport MainViewport
		{
			get { return _viewport; }
			set { _viewport = value; }
		}

		/// <summary>
		/// Gets or sets if terrain is colored according to vertex height or uses default coloring.
		/// </summary>
		public bool ColorByHeight
		{
			get { return _colorByHeight; }
			set { _colorByHeight = value; }
		}

		/// <summary>
		/// Gets or sets if soft selection uses falloff.
		/// </summary>
		public bool Falloff
		{
			get { return _falloff; }
			set { _falloff = value; }
		}

		/// <summary>
		/// Gets or sets the original zoom factor of the camera.
		/// </summary>
		public float OriginalZoomFactor
		{
			get { return _originalZoomFactor; }
			set { _originalZoomFactor = value; }
		}

		/// <summary>
		/// Gets or sets if the selected vertices are shown.
		/// </summary>
		public bool ShowSelectedVertices
		{
			get { return _showSelectedVertices; }
			set { _showSelectedVertices = value; }
		}

		/// <summary>
		/// Gets or sets whether soft selection is enabled.
		/// </summary>
		public bool SoftSelection
		{
			get { return _softSelection; }
			set { _softSelection = value; }
		}

		/// <summary>
		/// Gets or sets the squared distance for soft selection.
		/// (Squared for quicker length comparisons.)
		/// </summary>
		public float SoftSelectionDistanceSquared
		{
			get { return _softDistanceSquared; }
			set { _softDistanceSquared = value; }
		}

		/// <summary>
		/// Gets or sets the base vertex size for rendering.
		/// </summary>
		public float VertexSize
		{
			get { return _vertexSize; }
			set { _vertexSize = value; }
		}
		#endregion

		#region Basic Methods
		/// <summary>
		/// Creates an object for building vertex and index buffers for terrain.
		/// </summary>
		public BufferObjects()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Clears the buffers used.
		/// </summary>
		public void ClearBuffers()
		{
			if ( _vb != null && !_vb.Disposed )
			{
				_vb.Dispose();
				_vb = null;
				_vbSize = 0;
			}

			if ( _vbVerts != null && !_vbVerts.Disposed )
			{
				_vbVerts.Dispose();
				_vbVerts = null;
				_vbVertSize = 0;
			}

			if ( _vbByHeight != null && !_vbByHeight.Disposed )
			{
				_vbByHeight.Dispose();
				_vbByHeight = null;
				_vbSize = 0;
			}
			
			if ( _ib != null && !_ib.Disposed )
			{
				_ib.Dispose();
				_ib = null;
				_ibSize = 0;
			}

			if ( _ibVerts != null && !_ibVerts.Disposed )
			{
				_ibVerts.Dispose();
				_ibVerts = null;
				_ibVertSize = 0;
			}
		}
		#endregion

		#region Buffer Building - Terrain Page
		/// <summary>
		/// Reloads the VertexBuffer for the TerrainPage.
		/// </summary>
		public void RefreshVertexBuffer_Page()
		{
			if ( _page != null )
			{
				if ( !_colorByHeight )
				{
					// Determine which list of vertices to build
					switch ( _viewport.Device.DeviceCaps.MaxSimultaneousTextures )
					{
						case 0:
							BuildVertices_PositionNormal();
							break;

						case 1:
							BuildVertices_PositionNormalTextured1();
							break;

						case 2:
							BuildVertices_PositionNormalTextured2();
							break;

						case 3:
							BuildVertices_PositionNormalTextured3();
							break;

						case 4:
							BuildVertices_PositionNormalTextured4();
							break;

						case 5:
							BuildVertices_PositionNormalTextured5();
							break;

						case 6:
							BuildVertices_PositionNormalTextured6();
							break;

						case 7:
							BuildVertices_PositionNormalTextured7();
							break;

						case 8:
						default:
							BuildVertices_PositionNormalTextured8();
							break;
					}
				}
				else
				{
					// Build non-textured vertex buffer
					GraphicsStream stream;
					int vertColor;
					D3D.CustomVertex.PositionNormalColored[] tri =
						new D3D.CustomVertex.PositionNormalColored[_page.TerrainPatch.NumVertices];

					for ( int i = 0; i < tri.Length; i++ )
					{
						tri[i].Position = _page.TerrainPatch.Vertices[i].Position;
						tri[i].Normal = _page.TerrainPatch.Vertices[i].Normal;
						vertColor = (int) ( ( tri[i].Position.Y / _page.MaximumVertexHeight ) * 255.0f );
						tri[i].Color = Color.FromArgb( vertColor, vertColor, vertColor ).ToArgb();
					}

					if ( _vbByHeight == null )
						_vbByHeight = new D3D.VertexBuffer( typeof( D3D.CustomVertex.PositionNormalColored ),
							tri.Length, _viewport.Device, D3D.Usage.Dynamic | D3D.Usage.WriteOnly,
							D3D.CustomVertex.PositionNormalColored.Format, D3D.Pool.Default );

					stream = _vbByHeight.Lock( 0, 0, D3D.LockFlags.Discard );
					stream.Write( tri );
					_vbByHeight.Unlock();
		
					_vbSize = tri.Length;
				}

				_page.TerrainPatch.RefreshBuffers = false;
			}
			else 
			{
				if ( _vb != null )
				{
					if ( !_vb.Disposed )
						_vb.Dispose();

					_vb = null;
					_vbSize = 0;
				}

				if ( _vbByHeight != null )
				{
					if ( !_vbByHeight.Disposed )
						_vbByHeight.Dispose();

					_vbByHeight = null;
					_vbSize = 0;
				}
			}
		}

		/// <summary>
		/// Reloads the IndexBuffer for the TerrainPage.
		/// </summary>
		public void RefreshIndexBuffer_Page()
		{
			if ( _page != null )
			{
				GraphicsStream stream;
				short[] indices = new short[_page.TerrainPatch.Indices.Length];

				for ( int i = 0; i < indices.Length; i++ )
					indices[i] = _page.TerrainPatch.Indices[i];

				if ( _ib != null )
				{
					_ib.Dispose();
					_ib = null;
				}

				_ib = new D3D.IndexBuffer( typeof( short ), indices.Length,
					_viewport.Device, D3D.Usage.WriteOnly, D3D.Pool.Managed );

				stream = _ib.Lock( 0, 0, D3D.LockFlags.None );
				stream.Write( indices );
				_ib.Unlock();

				_ibSize = indices.Length;
			}
			else if ( _ib != null )
			{
				if ( !_ib.Disposed )
					_ib.Dispose();

				_ib = null;
				_ibSize = 0;
			}
		}
		#endregion

		#region Vertex Buffer Building - Terrain Page
		/// <summary>
		/// Builds the vertex buffer using vertices with position and normal data.
		/// </summary>
		public void BuildVertices_PositionNormal()
		{
			GraphicsStream stream;
			DataCore.VertexFormats.PositionNormal[] tri =
				new DataCore.VertexFormats.PositionNormal[_page.TerrainPatch.NumVertices];

			for ( int i = 0; i < tri.Length; i++ )
			{
				tri[i].Position = _page.TerrainPatch.Vertices[i].Position;
				tri[i].Normal = _page.TerrainPatch.Vertices[i].Normal;
			}


			if ( _vb != null )
			{
				if ( !_vb.Disposed )
					_vb.Dispose();

				_vb = null;
			}

			_vb = new D3D.VertexBuffer( typeof( DataCore.VertexFormats.PositionNormal ),
				tri.Length, _viewport.Device, D3D.Usage.WriteOnly,
				DataCore.VertexFormats.PositionNormal.Format, D3D.Pool.Managed );

			stream = _vb.Lock( 0, 0, D3D.LockFlags.None );
			stream.Write( tri );
			_vb.Unlock();
		
			_vbSize = tri.Length;
		}

		/// <summary>
		/// Builds the vertex buffer using vertices with position, normal, and one set of texture data.
		/// </summary>
		public void BuildVertices_PositionNormalTextured1()
		{
			GraphicsStream stream;
			DataCore.VertexFormats.PositionNormalTextured1[] tri =
				new DataCore.VertexFormats.PositionNormalTextured1[_page.TerrainPatch.NumVertices];

			for ( int i = 0; i < tri.Length; i++ )
			{
				tri[i].Position = _page.TerrainPatch.Vertices[i].Position;
				tri[i].Normal = _page.TerrainPatch.Vertices[i].Normal;

				if ( _page.TerrainPatch.NumTextures > 0 )
					tri[i].SetTextureCoordinates( _page.TerrainPatch.GetTextureCoordinateList( 0 )[i], 0 );
				else
					tri[i].SetTextureCoordinates( new Vector2( 0f, 0f ), 0 );
			}

			if ( _vb != null )
			{
				if ( !_vb.Disposed )
					_vb.Dispose();

				_vb = null;
			}

			_vb = new D3D.VertexBuffer( typeof( DataCore.VertexFormats.PositionNormalTextured1 ),
				tri.Length, _viewport.Device, D3D.Usage.WriteOnly,
				DataCore.VertexFormats.PositionNormalTextured1.Format, D3D.Pool.Managed );

			stream = _vb.Lock( 0, 0, D3D.LockFlags.None );
			stream.Write( tri );
			_vb.Unlock();
		
			_vbSize = tri.Length;
		}

		/// <summary>
		/// Builds the vertex buffer using vertices with position, normal, and two sets of texture data.
		/// </summary>
		public void BuildVertices_PositionNormalTextured2()
		{
			GraphicsStream stream;
			DataCore.VertexFormats.PositionNormalTextured2[] tri =
				new DataCore.VertexFormats.PositionNormalTextured2[_page.TerrainPatch.NumVertices];

			for ( int i = 0; i < tri.Length; i++ )
			{
				tri[i].Position = _page.TerrainPatch.Vertices[i].Position;
				tri[i].Normal = _page.TerrainPatch.Vertices[i].Normal;

				if ( _page.TerrainPatch.NumTextures > 0 )
				{
					for ( int j = 0; j < _page.TerrainPatch.NumTextures && j < 2; j++ )
						tri[i].SetTextureCoordinates( _page.TerrainPatch.GetTextureCoordinateList( j )[i],
							j );
				}
				else
					tri[i].SetTextureCoordinates( new Vector2( 0f, 0f ), 0 );
			}

			if ( _vb != null )
			{
				if ( !_vb.Disposed )
					_vb.Dispose();

				_vb = null;
			}

			_vb = new D3D.VertexBuffer( typeof( DataCore.VertexFormats.PositionNormalTextured2 ),
				tri.Length, _viewport.Device, D3D.Usage.WriteOnly,
				DataCore.VertexFormats.PositionNormalTextured2.Format, D3D.Pool.Managed );

			stream = _vb.Lock( 0, 0, D3D.LockFlags.None );
			stream.Write( tri );
			_vb.Unlock();
		
			_vbSize = tri.Length;
		}

		/// <summary>
		/// Builds the vertex buffer using vertices with position, normal, and three sets of texture data.
		/// </summary>
		public void BuildVertices_PositionNormalTextured3()
		{
			GraphicsStream stream;
			DataCore.VertexFormats.PositionNormalTextured3[] tri =
				new DataCore.VertexFormats.PositionNormalTextured3[_page.TerrainPatch.NumVertices];

			for ( int i = 0; i < tri.Length; i++ )
			{
				tri[i].Position = _page.TerrainPatch.Vertices[i].Position;
				tri[i].Normal = _page.TerrainPatch.Vertices[i].Normal;

				if ( _page.TerrainPatch.NumTextures > 0 )
				{
					for ( int j = 0; j < _page.TerrainPatch.NumTextures && j < 3; j++ )
						tri[i].SetTextureCoordinates( _page.TerrainPatch.GetTextureCoordinateList( j )[i],
							j );
				}
				else
				{
					tri[i].SetTextureCoordinates( new Vector2( 0f, 0f ), 0 );
				}
			}

			if ( _vb != null )
			{
				if ( !_vb.Disposed )
					_vb.Dispose();

				_vb = null;
			}

			_vb = new D3D.VertexBuffer( typeof( DataCore.VertexFormats.PositionNormalTextured3 ),
				tri.Length, _viewport.Device, D3D.Usage.WriteOnly,
				DataCore.VertexFormats.PositionNormalTextured3.Format, D3D.Pool.Managed );

			stream = _vb.Lock( 0, 0, D3D.LockFlags.None );
			stream.Write( tri );
			_vb.Unlock();
		
			_vbSize = tri.Length;
		}

		/// <summary>
		/// Builds the vertex buffer using vertices with position, normal, and four sets of texture data.
		/// </summary>
		public void BuildVertices_PositionNormalTextured4()
		{
			GraphicsStream stream;
			DataCore.VertexFormats.PositionNormalTextured4[] tri =
				new DataCore.VertexFormats.PositionNormalTextured4[_page.TerrainPatch.NumVertices];

			for ( int i = 0; i < tri.Length; i++ )
			{
				tri[i].Position = _page.TerrainPatch.Vertices[i].Position;
				tri[i].Normal = _page.TerrainPatch.Vertices[i].Normal;

				if ( _page.TerrainPatch.NumTextures > 0 )
				{
					for ( int j = 0; j < _page.TerrainPatch.NumTextures && j < 4; j++ )
						tri[i].SetTextureCoordinates( _page.TerrainPatch.GetTextureCoordinateList( j )[i],
							j );
				}
				else
					tri[i].SetTextureCoordinates( new Vector2( 0f, 0f ), 0 );
			}

			if ( _vb != null )
			{
				if ( !_vb.Disposed )
					_vb.Dispose();

				_vb = null;
			}

			_vb = new D3D.VertexBuffer( typeof( DataCore.VertexFormats.PositionNormalTextured4 ),
				tri.Length, _viewport.Device, D3D.Usage.WriteOnly,
				DataCore.VertexFormats.PositionNormalTextured4.Format, D3D.Pool.Managed );

			stream = _vb.Lock( 0, 0, D3D.LockFlags.None );
			stream.Write( tri );
			_vb.Unlock();
		
			_vbSize = tri.Length;
		}

		/// <summary>
		/// Builds the vertex buffer using vertices with position, normal, and five sets of texture data.
		/// </summary>
		public void BuildVertices_PositionNormalTextured5()
		{
			GraphicsStream stream;
			DataCore.VertexFormats.PositionNormalTextured5[] tri =
				new DataCore.VertexFormats.PositionNormalTextured5[_page.TerrainPatch.NumVertices];

			for ( int i = 0; i < tri.Length; i++ )
			{
				tri[i].Position = _page.TerrainPatch.Vertices[i].Position;
				tri[i].Normal = _page.TerrainPatch.Vertices[i].Normal;

				if ( _page.TerrainPatch.NumTextures > 0 )
				{
					for ( int j = 0; j < _page.TerrainPatch.NumTextures && j < 5; j++ )
						tri[i].SetTextureCoordinates( _page.TerrainPatch.GetTextureCoordinateList( j )[i],
							j );
				}
				else
					tri[i].SetTextureCoordinates( new Vector2( 0f, 0f ), 0 );
			}

			if ( _vb != null )
			{
				if ( !_vb.Disposed )
					_vb.Dispose();

				_vb = null;
			}

			_vb = new D3D.VertexBuffer( typeof( DataCore.VertexFormats.PositionNormalTextured5 ),
				tri.Length, _viewport.Device, D3D.Usage.WriteOnly,
				DataCore.VertexFormats.PositionNormalTextured5.Format, D3D.Pool.Managed );

			stream = _vb.Lock( 0, 0, D3D.LockFlags.None );
			stream.Write( tri );
			_vb.Unlock();
		
			_vbSize = tri.Length;
		}

		/// <summary>
		/// Builds the vertex buffer using vertices with position, normal, and six sets of texture data.
		/// </summary>
		public void BuildVertices_PositionNormalTextured6()
		{
			GraphicsStream stream;
			DataCore.VertexFormats.PositionNormalTextured6[] tri =
				new DataCore.VertexFormats.PositionNormalTextured6[_page.TerrainPatch.NumVertices];

			for ( int i = 0; i < tri.Length; i++ )
			{
				tri[i].Position = _page.TerrainPatch.Vertices[i].Position;
				tri[i].Normal = _page.TerrainPatch.Vertices[i].Normal;

				if ( _page.TerrainPatch.NumTextures > 0 )
				{
					for ( int j = 0; j < _page.TerrainPatch.NumTextures && j < 6; j++ )
						tri[i].SetTextureCoordinates( _page.TerrainPatch.GetTextureCoordinateList( j )[i],
							j );
				}
				else
					tri[i].SetTextureCoordinates( new Vector2( 0f, 0f ), 0 );
			}

			if ( _vb != null )
			{
				if ( !_vb.Disposed )
					_vb.Dispose();

				_vb = null;
			}

			_vb = new D3D.VertexBuffer( typeof( DataCore.VertexFormats.PositionNormalTextured6 ),
				tri.Length, _viewport.Device, D3D.Usage.WriteOnly,
				DataCore.VertexFormats.PositionNormalTextured6.Format, D3D.Pool.Managed );

			stream = _vb.Lock( 0, 0, D3D.LockFlags.None );
			stream.Write( tri );
			_vb.Unlock();
		
			_vbSize = tri.Length;
		}

		/// <summary>
		/// Builds the vertex buffer using vertices with position, normal, and seven sets of texture data.
		/// </summary>
		public void BuildVertices_PositionNormalTextured7()
		{
			GraphicsStream stream;
			DataCore.VertexFormats.PositionNormalTextured7[] tri =
				new DataCore.VertexFormats.PositionNormalTextured7[_page.TerrainPatch.NumVertices];

			for ( int i = 0; i < tri.Length; i++ )
			{
				tri[i].Position = _page.TerrainPatch.Vertices[i].Position;
				tri[i].Normal = _page.TerrainPatch.Vertices[i].Normal;

				if ( _page.TerrainPatch.NumTextures > 0 )
				{
					for ( int j = 0; j < _page.TerrainPatch.NumTextures && j < 7; j++ )
						tri[i].SetTextureCoordinates( _page.TerrainPatch.GetTextureCoordinateList( j )[i],
							j );
				}
				else
					tri[i].SetTextureCoordinates( new Vector2( 0f, 0f ), 0 );
			}

			if ( _vb != null )
			{
				if ( !_vb.Disposed )
					_vb.Dispose();

				_vb = null;
			}

			_vb = new D3D.VertexBuffer( typeof( DataCore.VertexFormats.PositionNormalTextured7 ),
				tri.Length, _viewport.Device, D3D.Usage.WriteOnly,
				DataCore.VertexFormats.PositionNormalTextured7.Format, D3D.Pool.Managed );

			stream = _vb.Lock( 0, 0, D3D.LockFlags.None );
			stream.Write( tri );
			_vb.Unlock();
		
			_vbSize = tri.Length;
		}

		/// <summary>
		/// Builds the vertex buffer using vertices with position, normal, and eight sets of texture data.
		/// </summary>
		public void BuildVertices_PositionNormalTextured8()
		{
			GraphicsStream stream;
			DataCore.VertexFormats.PositionNormalTextured8[] tri =
				new DataCore.VertexFormats.PositionNormalTextured8[_page.TerrainPatch.NumVertices];

			for ( int i = 0; i < tri.Length; i++ )
			{
				tri[i].Position = _page.TerrainPatch.Vertices[i].Position;
				tri[i].Normal = _page.TerrainPatch.Vertices[i].Normal;

				if ( _page.TerrainPatch.NumTextures > 0 )
				{
					for ( int j = 0; j < _page.TerrainPatch.NumTextures && j < 8; j++ )
						tri[i].SetTextureCoordinates( _page.TerrainPatch.GetTextureCoordinateList( j )[i],
							j );
				}
				else
					tri[i].SetTextureCoordinates( new Vector2( 0f, 0f ), 0 );
			}

			if ( _vb != null )
			{
				if ( !_vb.Disposed )
					_vb.Dispose();

				_vb = null;
			}

			_vb = new D3D.VertexBuffer( typeof( DataCore.VertexFormats.PositionNormalTextured8 ),
				tri.Length, _viewport.Device, D3D.Usage.WriteOnly,
				DataCore.VertexFormats.PositionNormalTextured8.Format, D3D.Pool.Managed );

			stream = _vb.Lock( 0, 0, D3D.LockFlags.None );
			stream.Write( tri );
			_vb.Unlock();
		
			_vbSize = tri.Length;
		}
		#endregion

		#region Buffer Building - Vertices
		/// <summary>
		/// Reloads the VertexBuffer for the TerrainPage for displaying vertices.
		/// </summary>
		public void RefreshVertexBuffer_Vertices()
		{
			if ( _page != null )
			{
				GraphicsStream stream;
				D3D.CustomVertex.PositionColored[] vertices = new
					D3D.CustomVertex.PositionColored[_page.TerrainPatch.NumVertices * 4];
				D3D.CustomVertex.PositionNormal[] tri = _page.TerrainPatch.Vertices;
				int color = Color.Black.ToArgb();
				int red, blue;
				float zoomFactor = _viewport.Camera.FollowDistance / _originalZoomFactor;
				Vector3 position;
				float distance;
				float vDist = _page.TerrainPatch.NearestVertices * _vertexSize;

				for ( int i = 0; i < tri.Length; i++ )
				{
					if ( _showSelectedVertices && _page.TerrainPatch.SelectedVertices[i] )
						color = Color.Red.ToArgb();
					else if ( _showSelectedVertices && _softSelection &&
						_page.TerrainPatch.AreVerticesSelected )
					{
						position = _page.TerrainPatch.Vertices[i].Position;
						distance = _page.TerrainPatch.FindShortestDistanceToSelectedVertex( position );

						if ( distance <= _softDistanceSquared )
						{
							if ( _falloff )
							{
								red = Convert.ToInt32( Color.Red.R * ( 1 - distance / _softDistanceSquared ) );
								blue = Convert.ToInt32( Color.Blue.B * ( distance / _softDistanceSquared ) );
								color = Color.FromArgb( red, 0, blue ).ToArgb();
							}
							else
								color = Color.Red.ToArgb();
						}
						else
						{
							color = Color.Blue.ToArgb();
						}
					}
					else
					{
						color = Color.Blue.ToArgb();
					}

					vertices[i * 4].Position = new Vector3( tri[i].Position.X - ( vDist * zoomFactor ),
						tri[i].Position.Y, tri[i].Position.Z - ( vDist * zoomFactor ) );
					vertices[i * 4].Color = color;
					vertices[i * 4 + 1].Position = new Vector3( tri[i].Position.X - ( vDist * zoomFactor ),
						tri[i].Position.Y, tri[i].Position.Z + ( vDist * zoomFactor ) );
					vertices[i * 4 + 1].Color = color;
					vertices[i * 4 + 2].Position = new Vector3( tri[i].Position.X + ( vDist * zoomFactor ),
						tri[i].Position.Y, tri[i].Position.Z + ( vDist * zoomFactor ) );
					vertices[i * 4 + 2].Color = color;
					vertices[i * 4 + 3].Position = new Vector3( tri[i].Position.X + ( vDist * zoomFactor ),
						tri[i].Position.Y, tri[i].Position.Z - ( vDist * zoomFactor ) );
					vertices[i * 4 + 3].Color = color;
				}

				if ( _vbVerts != null )
				{
					if ( !_vbVerts.Disposed )
						_vbVerts.Dispose();

					_vbVerts = null;
				}

				_vbVerts = new D3D.VertexBuffer( typeof( D3D.CustomVertex.PositionColored ),
					vertices.Length, _viewport.Device, D3D.Usage.WriteOnly,
					D3D.CustomVertex.PositionColored.Format, D3D.Pool.Managed );

				stream = _vbVerts.Lock( 0, 0, D3D.LockFlags.None );
				stream.Write( vertices );
				_vbVerts.Unlock();
			
				_vbVertSize = vertices.Length;
				_page.TerrainPatch.RefreshVertices = false;
			}
			else if ( _vbVerts != null )
			{
				if ( !_vbVerts.Disposed )
					_vbVerts.Dispose();

				_vbVerts = null;
				_vbVertSize = 0;
			}
		}

		/// <summary>
		/// Reloads the IndexBuffer for the vertices of the TerrainPage.
		/// </summary>
		public void RefreshIndexBuffer_Vertices()
		{
			if ( _page != null )
			{
				GraphicsStream stream;
				short[] indices = new short[_page.TerrainPatch.NumVertices * 6];
				short vertCount = 0;

				for ( int i = 0; i < indices.Length; i += 6, vertCount += 4 )
				{
					indices[i] = vertCount;
					indices[i + 1] = (short) ( vertCount + 1 );
					indices[i + 2] = (short) ( vertCount + 2 );
					indices[i + 3] = (short) ( vertCount + 2 );
					indices[i + 4] = (short) ( vertCount + 3 );
					indices[i + 5] = vertCount;
				}

				if ( _ibVerts != null )
				{
					_ibVerts.Dispose();
					_ibVerts = null;
				}

				_ibVerts = new D3D.IndexBuffer( typeof( short ), indices.Length,
					_viewport.Device, D3D.Usage.WriteOnly, D3D.Pool.Managed );

				stream = _ibVerts.Lock( 0, 0, D3D.LockFlags.None );
				stream.Write( indices );
				_ibVerts.Unlock();

				_ibVertSize = indices.Length;
			}
			else if ( _ibVerts != null )
			{
				if ( !_ibVerts.Disposed )
					_ibVerts.Dispose();

				_ibVerts = null;
				_ibVertSize = 0;
			}
		}
		#endregion
	}
}
