using System;
using System.Drawing;
using Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataCore;
using Voyage.Terraingine.DXViewport;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;
using Voyage.LuaNetInterface;

namespace Voyage.Terraingine.DataInterfacing
{
	/// <summary>
	/// Class for manipulating terrain data in Terraingine.
	/// </summary>
	public class DataManipulation
	{
		#region Data Members
		private DXViewport.Viewport	_viewport;
		private PlugIns			_plugins;
		private TerrainPage		_page, _pageCopy;
		private BufferObjects	_buffers;
		private Vector3			_ray;
		private float			_copyPosition;
		private bool			_moveVertex;
		private bool			_pauseMoveVertex;
		private float			_originalZoomFactor;
		private bool			_isMoving;
		private bool			_enableLighting;
		private bool			_renderTextures;
		private bool			_enableMultiSelect;
		private float			_softDistanceSquared;
		private bool			_softSelection;
		private bool			_falloff;
		private DataHistory		_history;
		private DataHistory		_redoHistory;
		private Version			_vertexShaderVersion;
		private Version			_pixelShaderVersion;
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
		/// Gets the main viewport DirectX interface.
		/// </summary>
		public DXViewport.Viewport MainViewport
		{
			get { return _viewport; }
		}

		/// <summary>
		/// Gets the plug-ins used.
		/// </summary>
		public PlugIns PlugIns
		{
			get { return _plugins; }
		}

		/// <summary>
		/// Gets or sets the buffer objects used.
		/// </summary>
		public BufferObjects BufferObjects
		{
			get { return _buffers; }
			set { _buffers = value; }
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
		/// Gets or sets the camera pick ray used to manipulate the terrain.
		/// </summary>
		public Vector3 CameraPickRay
		{
			get { return _ray; }
			set { _ray = value; }
		}

		/// <summary>
		/// Gets or sets the copied vertex position.
		/// </summary>
		public float VertexPositionCopy
		{
			get { return _copyPosition; }
			set { _copyPosition = value; }
		}

		/// <summary>
		/// Gets or sets whether vertex movement is enabled.
		/// </summary>
		public bool EnableVertexMovement
		{
			get { return _moveVertex; }
			set { _moveVertex = value; }
		}

		/// <summary>
		/// Gets or sets whether vertex movement is paused.
		/// </summary>
		public bool PauseVertexMovement
		{
			get { return _pauseMoveVertex; }
			set { _pauseMoveVertex = value; }
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
		/// Gets or sets if the vertices are currently being moved.
		/// </summary>
		public bool IsMoving
		{
			get { return _isMoving; }
			set { _isMoving = value; }
		}

		/// <summary>
		/// Gets or sets if lighting is used in rendering.
		/// </summary>
		public bool EnableLighting
		{
			get { return _enableLighting; }
			set { _enableLighting = value; }
		}

		/// <summary>
		/// Gets or sets whether multiple selection is enabled.
		/// </summary>
		public bool EnableMultipleSelection
		{
			get { return _enableMultiSelect; }
			set { _enableMultiSelect = value; }
		}

		/// <summary>
		/// Gets or sets if textures are rendered.
		/// </summary>
		public bool RenderTextures
		{
			get { return _renderTextures; }
			set { _renderTextures = value; }
		}

		/// <summary>
		/// Gets the history stacks for the data interface.
		/// </summary>
		public DataHistory DataHistory
		{
			get { return _history; }
		}

		/// <summary>
		/// Gets the "redo" history stacks for the data interface.
		/// </summary>
		public DataHistory RedoDataHistory
		{
			get { return _redoHistory; }
		}

		/// <summary>
		/// Gets the supported vertex shader version for the DirectX viewport.
		/// </summary>
		public Version SupportedVertexShaderVersion
		{
			get { return _vertexShaderVersion; }
		}

		/// <summary>
		/// Gets the supported vertex shader version for the DirectX viewport.
		/// </summary>
		public Version SupportedPixelShaderVersion
		{
			get { return _pixelShaderVersion; }
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates a DataManipulation object.
		/// </summary>
		public DataManipulation( DXViewport.Viewport viewport )
		{
			_buffers = new BufferObjects();
			_buffers.VertexSize = 0.25f;
			_buffers.ColorByHeight = false;
			_buffers.MainViewport = viewport;

			_viewport = viewport;
			_moveVertex = false;
			_pauseMoveVertex = false;
			_isMoving = false;
			_softSelection = false;
			_falloff = true;
			_enableLighting = true;
			_softDistanceSquared = 0.2f * 0.2f;		// Squared for quicker calculations
			_history = new DataHistory();
			_redoHistory = new DataHistory();
			_pageCopy = null;
			_renderTextures = true;
			_ray = Vector3.Empty;
			_copyPosition = 0f;

			_plugins = new PlugIns();

			// Set shader versions
			_vertexShaderVersion = _viewport.SupportedVertexShaderVersion;
			_pixelShaderVersion = _viewport.SupportedPixelShaderVersion;
		}

		/// <summary>
		/// Disposes of all data being used.
		/// </summary>
		public void Dispose()
		{
			// Clear terrain pages
			if ( _page != null )
			{
				_page.Dispose();
				_page = null;
			}
			
			if ( _pageCopy != null )
			{
				_pageCopy.Dispose();
				_pageCopy = null;
			}

			_buffers.ClearBuffers();

			// Clear history
			_history.ClearHistory();
			_redoHistory.ClearHistory();

			// Clear plug-ins
			foreach ( PlugIn p in _plugins.VertexPlugIns )
				p.Dispose();

			foreach ( PlugIn p in _plugins.TexturePlugIns )
				p.Dispose();

			foreach ( PlugIn p in _plugins.FileImportPlugIns )
				p.Dispose();

			foreach ( PlugIn p in _plugins.FileExportPlugIns )
				p.Dispose();

			_plugins.VertexPlugIns.Clear();
			_plugins.TexturePlugIns.Clear();
			_plugins.FileImportPlugIns.Clear();
			_plugins.FileExportPlugIns.Clear();
		}
		#endregion

		#region Terrain Control
		/// <summary>
		/// Gets the TerrainPage used.
		/// </summary>
		/// <returns>The TerrainPage being manipulated.</returns>
		[LuaFunctionAttribute( "GetTerrain", "Gets the TerrainPage used." )]
		public TerrainPage GetTerrain()
		{
			return _page;
		}

		/// <summary>
		/// Sets the TerrainPage used.
		/// </summary>
		/// <param name="page">The TerrainPage to manipulate.</param>
		[LuaFunctionAttribute( "SetTerrain", "Sets the TerrainPage used.",
			 "The TerrainPage to manipulate." )]
		public void SetTerrain( TerrainPage page )
		{
			_page = page;
		}

		/// <summary>
		/// Creates a TerrainPatch.
		/// </summary>
		/// <param name="rows">The number of rows in the TerrainPatch.</param>
		/// <param name="columns">The number of columns in the TerrainPatch.</param>
		/// <param name="height">The total height of the TerrainPatch.</param>
		/// <param name="width">The total width of the TerrainPatch.</param>
		/// <returns>The created TerrainPage.</returns>
		public TerrainPage CreateTerrain( int rows, int columns, float height, float width )
		{
			return CreateTerrain( rows, columns, height, width, null );
		}

		/// <summary>
		/// Creates a TerrainPatch.
		/// </summary>
		/// <param name="rows">The number of rows in the TerrainPatch.</param>
		/// <param name="columns">The number of columns in the TerrainPatch.</param>
		/// <param name="height">The total height of the TerrainPatch.</param>
		/// <param name="width">The total width of the TerrainPatch.</param>
		/// <param name="name">The name of the TerrainPage.</param>
		/// <returns>The created TerrainPage.</returns>
		public TerrainPage CreateTerrain( int rows, int columns, float height, float width, string name )
		{
			if ( _page != null )
				_page.Dispose();
			else
				_page = new TerrainPage();

			_page.TerrainPatch.CreatePatch( rows, columns, height, width );
			_page.TerrainPatch.RefreshBuffers = true;
			_page.Name = name;
			_buffers.RefreshIndexBuffer_Page();
			_buffers.RefreshIndexBuffer_Vertices();

			return _page;
		}

		/// <summary>
		/// Loads the specified TerrainPage and refreshes vertex/index buffers.
		/// </summary>
		/// <param name="page">The TerrainPage to load.</param>
		public void LoadTerrain( TerrainPage page )
		{
			StoreLastPage( page, "Load New Terrain" );
			_page.TerrainPatch.RefreshBuffers = true;
			_buffers.RefreshIndexBuffer_Page();
			_buffers.RefreshIndexBuffer_Vertices();
		}
		#endregion

		#region Viewing Terrain
		/// <summary>
		/// Refresh all vertex and index buffers.
		/// </summary>
		[LuaFunctionAttribute( "RefreshAllBuffers", "Refresh all vertex and index buffers." )]
		public void RefreshAllBuffers()
		{
			// Set buffer states
			_buffers.TerrainPage = _page;
			_buffers.Falloff = _falloff;
			_buffers.OriginalZoomFactor = _originalZoomFactor;
			_buffers.SoftSelection = _softSelection;
			_buffers.SoftSelectionDistanceSquared = _softDistanceSquared;

			// Build the buffers
			_buffers.RefreshVertexBuffer_Page();
			_buffers.RefreshIndexBuffer_Page();

			if ( _page != null && _page.TerrainPatch.NumVertices < _viewport.Device.DeviceCaps.MaxVertexIndex )
			{
				_buffers.RefreshVertexBuffer_Vertices();
				_buffers.RefreshIndexBuffer_Vertices();
			}
		}

		/// <summary>
		/// Refresh all vertex buffers.
		/// </summary>
		[LuaFunctionAttribute( "RefreshVertexBuffers", "Refresh all vertex buffers." )]
		public void RefreshVertexBuffers()
		{
			// Set buffer states
			_buffers.TerrainPage = _page;
			_buffers.Falloff = _falloff;
			_buffers.OriginalZoomFactor = _originalZoomFactor;
			_buffers.SoftSelection = _softSelection;
			_buffers.SoftSelectionDistanceSquared = _softDistanceSquared;

			// Build the buffers
			_buffers.RefreshVertexBuffer_Page();

			if ( _page != null && _page.TerrainPatch.NumVertices < _viewport.Device.DeviceCaps.MaxVertexIndex )
				_buffers.RefreshVertexBuffer_Vertices();
		}

		/// <summary>
		/// Refreshes the index buffer for the terrain.
		/// </summary>
		[LuaFunctionAttribute( "RefreshIndexBuffer", "Refreshes the index buffer for the terrain." )]
		public void RefreshIndexBuffer()
		{
			// Set buffer states
			_buffers.TerrainPage = _page;
			_buffers.Falloff = _falloff;
			_buffers.OriginalZoomFactor = _originalZoomFactor;
			_buffers.SoftSelection = _softSelection;
			_buffers.SoftSelectionDistanceSquared = _softDistanceSquared;

			// Build the buffers
			_buffers.RefreshIndexBuffer_Page();
		}

		/// <summary>
		/// Refresh the vertex buffer for vertices.
		/// </summary>
		[LuaFunctionAttribute( "RefreshVertexBuffer_Vertices", "Refresh the vertex buffer for vertices." )]
		public void RefreshVertexBuffer_Vertices()
		{
			// Set buffer states
			_buffers.TerrainPage = _page;
			_buffers.Falloff = _falloff;
			_buffers.OriginalZoomFactor = _originalZoomFactor;
			_buffers.SoftSelection = _softSelection;
			_buffers.SoftSelectionDistanceSquared = _softDistanceSquared;

			if ( _page != null && _page.TerrainPatch.NumVertices < _viewport.Device.DeviceCaps.MaxVertexIndex )
				_buffers.RefreshVertexBuffer_Vertices();
		}
		#endregion

		#region Vertex Manipulation
		/// <summary>
		/// Updates the height and width of the TerrainPatch.
		/// </summary>
		/// <param name="height">New height of the TerrainPatch.</param>
		/// <param name="width">New width of the TerrainPatch.</param>
		public void UpdateTerrainDimensions( float height, float width )
		{
			StoreCurrentPage( "Update Terrain Dimensions" );
			_page.TerrainPatch.UpdateTerrainDimensions( height, width );
		}

		/// <summary>
		/// Updates the number of rows and columns in a TerrainPatch by creating
		/// a new version of the TerrainPatch.
		/// </summary>
		/// <param name="rows">Number of rows in the new TerrainPatch.</param>
		/// <param name="columns">Number of columns in the new TerrainPatch.</param>
		public void UpdateTerrainSize( int rows, int columns )
		{
			float height = _page.TerrainPatch.Height;
			float width = _page.TerrainPatch.Width;

			StoreCurrentPage( "Update Terrain Size" );

			if ( _page != null )
				_page.Dispose();
			else
				_page = new TerrainPage();

			_page.TerrainPatch.CreatePatch( rows, columns, height, width );
			RefreshIndexBuffer();
			_page.TerrainPatch.RefreshBuffers = true;
		}

		/// <summary>
		/// Updates the maximum allowed vertex height in a TerrainPage.
		/// </summary>
		/// <param name="height">The maximum allowed vertex height.</param>
		public void UpdateMaximumTerrainHeight( float height )
		{
			StoreCurrentPage( "Update Maximum Terrain Height" );
			_page.MaximumVertexHeight = height;
		}

		/// <summary>
		/// Programatically sets the height of the selected vertices to the specified value.
		/// </summary>
		/// <param name="height">The height to set the vertices to.</param>
		/// <param name="saveState">Whether to save the terrain state.</param>
		public void SetVertexHeight( float height, bool saveState )
		{
			Vector3[] positions = _page.TerrainPatch.GetSelectedVertexPositions();

			if ( positions.Length > 0 && height != positions[0].Y )
			{
				if ( saveState )
					StoreCurrentPage( "Vertex Movement" );

				_page.SetSelectedVerticesHeight( _softSelection, height );
			}
		}

		/// <summary>
		/// Pastes copied vertex data onto the currently selected vertex (pastes one vertex only).
		/// </summary>
		/// <param name="height">The new height of the vertex.</param>
		public void PasteSelectedVertexPosition( float height )
		{
			SetVertexHeight( height, true );
		}

		/// <summary>
		/// Copies the height of the first of the currently selected vertices.
		/// </summary>
		public void CopySelectedVertexPosition()
		{
			Vector3[] positions = _page.TerrainPatch.GetSelectedVertexPositions();

			if ( positions.Length > 0 )
				_copyPosition = positions[0].Y;
		}

		/// <summary>
		/// Selects a vertex in the terrain from the given origin.
		/// </summary>
		/// <param name="viewport">The viewport to build the picking ray from.</param>
		/// <param name="endSelection">Whether to use multiple selection.</param>
		public void SelectVertex( DXViewport.Viewport viewport, bool endSelection )
		{
			_page.SelectVertex( _ray, viewport.Camera.Position -
				viewport.Camera.Offset, endSelection, _enableMultiSelect );
		}

		/// <summary>
		/// Selects the specified vertex.
		/// </summary>
		/// <param name="index">Index of the vertex.</param>
		/// <param name="endSelection">Whether to enable clearing selected vertices if one is not selected.</param>
		public void SelectVertex( int index, bool endSelection )
		{
			_page.SelectVertex( index, endSelection, _enableMultiSelect );
		}

		/// <summary>
		/// Builds a picking ray from the viewport camera.
		/// </summary>
		/// <param name="x">X-position in the viewport.</param>
		/// <param name="y">Y-position in the viewport.</param>
		/// <param name="viewport">The DirectX viewport to use.</param>
		[LuaFunctionAttribute( "BuildPickingRay",
			 "Builds a picking ray from the viewport camera.",
			 "X-position in the viewport.", "Y-position in the viewport.",
			 "The DirectX viewport to use." )]
		public void BuildPickingRay( int x, int y, DXViewport.Viewport viewport )
		{
			_ray = viewport.Camera.PickingRay( x, y, viewport.Window.Width, viewport.Window.Height );
		}

		/// <summary>
		/// Moves the selected vertices.
		/// </summary>
		public void MoveVertices()
		{
			if ( _moveVertex )
			{
				// Movement is done; store page history
				if ( _viewport.Mouse.Buttons[0] == false )
					MoveVertices( true, 0.0f );
				else if ( _isMoving && _page.TerrainPatch.AreVerticesSelected )
				{
					float distChange = -_viewport.Mouse.Location.Y * 0.005f *
						( _viewport.Camera.FollowDistance / _originalZoomFactor );

					MoveVertices( false, distChange );
				}
			}
		}

		/// <summary>
		/// Moves the selected vertices.
		/// </summary>
		/// <param name="endMovement">Whether to end vertex movement.</param>
		/// <param name="distChange">The change in distance to move selected vertices.</param>
		public void MoveVertices( bool endMovement, float distChange )
		{
			if ( _moveVertex )
			{
				// Movement is done; store page history
				if ( endMovement )
				{
					if ( _pageCopy != null )
					{
						StoreGenericPage( _pageCopy, "Vertex Movement" );
						_pageCopy.Dispose();
						_pageCopy = null;
					}
				}
				else // Vertices are still being moved
				{
					if ( _pageCopy == null )
						_pageCopy = new TerrainPage( _page );

					_page.MoveSelectedVertices( _softSelection, distChange, _falloff,
						_softDistanceSquared );
				}
			}
		}
		#endregion

		#region Texture Manipulation
		/// <summary>
		/// Shifts the currently selected texture by the indicated distance.
		/// </summary>
		public void ShiftTexture()
		{
			if ( _pageCopy == null )
				_pageCopy = new TerrainPage( _page );

			_page.TerrainPatch.SetTextureCoordinates();
		}

		/// <summary>
		/// Scales the currently selected texture by the indicated distance.
		/// </summary>
		public void ScaleTexture()
		{
			if ( _pageCopy == null )
				_pageCopy = new TerrainPage( _page );

			_page.TerrainPatch.SetTextureCoordinates();
		}

		/// <summary>
		/// Ends texture movement if any textures have been modified.
		/// </summary>
		public void EndTextureMovement()
		{
			if ( _pageCopy != null )
			{
				StoreGenericPage( _pageCopy, "Texture Movement" );
				_pageCopy.Dispose();
				_pageCopy = null;
			}
		}
		#endregion

		#region Rendering
		/// <summary>
		/// Selects the vertex format for rendering that can be handled by the video card.
		/// </summary>
		private void SelectVertexFormat()
		{
			switch ( _viewport.Device.DeviceCaps.MaxSimultaneousTextures )
			{
				case 0:
					_viewport.Device.VertexFormat = DataCore.VertexFormats.PositionNormal.Format;
					break;

				case 1:
					_viewport.Device.VertexFormat = DataCore.VertexFormats.PositionNormalTextured1.Format;
					break;

				case 2:
					_viewport.Device.VertexFormat = DataCore.VertexFormats.PositionNormalTextured2.Format;
					break;

				case 3:
					_viewport.Device.VertexFormat = DataCore.VertexFormats.PositionNormalTextured3.Format;
					break;

				case 4:
					_viewport.Device.VertexFormat = DataCore.VertexFormats.PositionNormalTextured4.Format;
					break;

				case 5:
					_viewport.Device.VertexFormat = DataCore.VertexFormats.PositionNormalTextured5.Format;
					break;

				case 6:
					_viewport.Device.VertexFormat = DataCore.VertexFormats.PositionNormalTextured6.Format;
					break;

				case 7:
					_viewport.Device.VertexFormat = DataCore.VertexFormats.PositionNormalTextured7.Format;
					break;

				case 8:
				default:
					_viewport.Device.VertexFormat = DataCore.VertexFormats.PositionNormalTextured8.Format;
					break;
			}
		}

		/// <summary>
		/// Renders the terrain.
		/// </summary>
		public void RenderTerrain()
		{
			if ( _page != null )
			{
				if ( _page.TerrainPatch.RefreshBuffers )
				{
					RefreshAllBuffers();
					_page.TerrainPatch.RefreshBuffers = false;
					_page.TerrainPatch.RefreshVertices = false;
				}

				if ( _buffers.VertexBuffer != null || _buffers.VertexBuffer_ColoredByHeight != null )
				{
					D3D.Material mat = new D3D.Material();

					mat.Diffuse = Color.White;
					_viewport.Device.Material = mat;

					if ( _enableLighting && !_viewport.Device.RenderState.Lighting && !_buffers.ColorByHeight )
						_viewport.Device.RenderState.Lighting = true;
					else if ( _buffers.ColorByHeight && _viewport.Device.RenderState.Lighting )
						_viewport.Device.RenderState.Lighting = false;

					if ( _viewport.Device.RenderState.FillMode == D3D.FillMode.WireFrame &&
						_viewport.Device.RenderState.CullMode != D3D.Cull.None )
						_viewport.Device.RenderState.CullMode = D3D.Cull.None;

					if ( ( _buffers.ColorByHeight && _buffers.VertexBuffer_ColoredByHeight != null ) ||
						_buffers.VertexBuffer == null )
					{
						_viewport.Device.VertexFormat = D3D.CustomVertex.PositionNormalColored.Format;
						_viewport.Device.SetStreamSource( 0, _buffers.VertexBuffer_ColoredByHeight, 0 );
					}
					else
					{
						SelectVertexFormat();
						_viewport.Device.SetStreamSource( 0, _buffers.VertexBuffer, 0 );
					}

					_viewport.Device.Transform.World = _viewport.Camera.WorldIdentity;

					// Set the textures to render
					if ( _renderTextures && _page.TerrainPatch.NumTextures > 0 && !_buffers.ColorByHeight )
					{
						for ( int i = 0; i < _page.TerrainPatch.NumTextures &&
							i < _viewport.Device.DeviceCaps.MaxSimultaneousTextures; i++ )
						{
							if ( ( (DataCore.Texture) _page.TerrainPatch.Textures[i] ).Render )
							{
								_viewport.Device.SetTexture( i,
									( (DataCore.Texture) _page.TerrainPatch.Textures[i] ).DXTexture );
								SetTextureOperation( i );

								if ( _viewport.Device.RenderState.AlphaBlendEnable )
									_viewport.Device.RenderState.AlphaBlendEnable = false;
							}
						}
					}
					else
					{
						_viewport.Device.SetTexture( 0, null );
						_viewport.Device.TextureState[0].ColorOperation = D3D.TextureOperation.Disable;
					}

					if ( _buffers.IndexBuffer != null )
					{
						_viewport.Device.Indices = _buffers.IndexBuffer;
						_viewport.Device.DrawIndexedPrimitives( D3D.PrimitiveType.TriangleList, 0, 0,
							_buffers.VertexBufferSize, 0, _buffers.IndexBufferSize / 3 );
					}
					else
						_viewport.Device.DrawPrimitives( D3D.PrimitiveType.TriangleList, 0,
							_buffers.VertexBufferSize );
				}
			}
		}

		/// <summary>
		/// Renders the highlighted terrain vertices.
		/// </summary>
		public void RenderVertices()
		{
			if ( _buffers.VertexBuffer_Vertices != null && _page.TerrainPatch.NumVertices < 75 * 75 )
			{
				if ( _page.TerrainPatch.RefreshVertices || _buffers.VertexBuffer_Vertices == null )
				{
					RefreshVertexBuffer_Vertices();
					_page.TerrainPatch.RefreshVertices = false;
				}

				Matrix world = _viewport.Camera.WorldIdentity;
				world.Translate( _viewport.Camera.Offset + new Vector3( 0f, 0.0001f, 0f ) );
				_viewport.Device.RenderState.FillMode = D3D.FillMode.Solid;
				_viewport.Device.SetStreamSource( 0, _buffers.VertexBuffer_Vertices, 0 );

				if ( _vertexShaderVersion >= new Version( 1, 1 ) &&
					_viewport.Effects.Count > 1 && _viewport.Effects[1] != null )
				{
					D3D.Effect effect = ( (Effect) _viewport.Effects[1] ).DXEffect;
					int passes;

					// Set Effect technique
					effect.Technique = effect.GetTechnique( "TVertices" );

					// Set camera matrices
					effect.SetValue( "World", world );
					effect.SetValue( "View", _viewport.Camera.View );
					effect.SetValue( "Projection", _viewport.Camera.Projection );

					// Begin the effect rendering
					passes = effect.Begin( D3D.FX.None );

					// Render each pass
					for ( int i = 0; i < passes; i++ )
					{
						effect.BeginPass( i );

						// Render vertices with or without index buffer
						if ( _buffers.IndexBuffer_Vertices != null )
						{
							_viewport.Device.Indices = _buffers.IndexBuffer_Vertices;
							_viewport.Device.DrawIndexedPrimitives( D3D.PrimitiveType.TriangleList, 0, 0,
								_buffers.VertexBufferSize_Vertices, 0, _buffers.IndexBufferSize_Vertices / 3 );
						}
						else
							_viewport.Device.DrawPrimitives( D3D.PrimitiveType.TriangleList, 0,
								_buffers.VertexBufferSize_Vertices );

						effect.EndPass();
					}

					// End the effect rendering
					effect.End();
				}
				else
				{
					// Keeping Z-Buffer enabled will hide vertices obscured by terrain
					//_viewport.Device.RenderState.ZBufferEnable = false;
					_viewport.Device.Transform.World = world;

					if ( _viewport.Device.RenderState.Lighting )
						_viewport.Device.RenderState.Lighting = false;

					if ( _buffers.IndexBuffer_Vertices != null )
					{
						_viewport.Device.Indices = _buffers.IndexBuffer_Vertices;
						_viewport.Device.DrawIndexedPrimitives( D3D.PrimitiveType.TriangleList, 0, 0,
							_buffers.VertexBufferSize_Vertices, 0, _buffers.IndexBufferSize_Vertices / 3 );
					}
					else
						_viewport.Device.DrawPrimitives( D3D.PrimitiveType.TriangleList, 0,
							_buffers.VertexBufferSize_Vertices / 3 );

					//_viewport.Device.RenderState.ZBufferEnable = true;
				}
			}
		}

		/// <summary>
		/// Sets the texture drawing operation for the specified texture state in the DirectX device.
		/// </summary>
		/// <param name="texture">Texture state to update.</param>
		private void SetTextureOperation( int texture )
		{
			DataCore.Texture tex = (DataCore.Texture) _page.TerrainPatch.Textures[texture];
			D3D.TextureOperation operation = tex.Operation;

			switch ( operation )
			{
				case D3D.TextureOperation.Add:
				case D3D.TextureOperation.AddSigned:
				case D3D.TextureOperation.AddSmooth:
				case D3D.TextureOperation.BlendTextureAlpha:
				case D3D.TextureOperation.DotProduct3:
				case D3D.TextureOperation.Modulate:
				case D3D.TextureOperation.Modulate2X:
				case D3D.TextureOperation.Subtract:
					_viewport.Device.TextureState[texture].ColorArgument1 = D3D.TextureArgument.Current;
					_viewport.Device.TextureState[texture].ColorArgument2 = D3D.TextureArgument.TextureColor;
					_viewport.Device.TextureState[texture].ColorOperation = operation;
					break;

				case D3D.TextureOperation.BlendFactorAlpha:
					_viewport.Device.RenderState.BlendOperation = D3D.BlendOperation.Subtract;
					_viewport.Device.RenderState.SourceBlend = D3D.Blend.SourceAlpha;
					_viewport.Device.RenderState.DestinationBlend = D3D.Blend.DestinationAlpha;
					_viewport.Device.RenderState.AlphaBlendEnable = true;
					_viewport.Device.RenderState.BlendFactor = Color.FromArgb( 50, Color.Black );
					_viewport.Device.TextureState[texture].ColorArgument1 = D3D.TextureArgument.Current;
					_viewport.Device.TextureState[texture].ColorArgument2 = D3D.TextureArgument.TextureColor;
					_viewport.Device.TextureState[texture].ColorOperation = operation;
					break;

				case D3D.TextureOperation.SelectArg1:
				default:
					_viewport.Device.TextureState[texture].ColorArgument1 = D3D.TextureArgument.TextureColor;
					_viewport.Device.TextureState[texture].ColorOperation = D3D.TextureOperation.SelectArg1;
					break;
			}
		}

		/// <summary>
		/// Gets the Direct3D TextureOperation associated with the specified string.
		/// </summary>
		/// <param name="operation">String to get the TextureOperation from.</param>
		/// <returns>The TextureOperation being used.</returns>
		public D3D.TextureOperation GetTextureOperation( string operation )
		{
			D3D.TextureOperation result;

			switch ( operation )
			{
				case "Add Color":
					result = D3D.TextureOperation.Add;
					break;

				case "AddSigned":
					result = D3D.TextureOperation.AddSigned;
					break;

				case "AddSmooth":
					result = D3D.TextureOperation.AddSmooth;
					break;

				case "DotProduct3":
					result = D3D.TextureOperation.DotProduct3;
					break;

				case "Modulate":
					result = D3D.TextureOperation.Modulate;
					break;

				case "Modulate2X":
					result = D3D.TextureOperation.Modulate2X;
					break;

				case "Subtract Color":
					result = D3D.TextureOperation.Subtract;
					break;

				case "Use Texture Alpha":
					result = D3D.TextureOperation.BlendTextureAlpha;
					break;

				case "Blend Alpha":
					result = D3D.TextureOperation.BlendFactorAlpha;
					break;

				case "Normal (Default)":
				case "SelectArg1":
				default:
					result = D3D.TextureOperation.SelectArg1;
					break;
			}

			return result;
		}

		/// <summary>
		/// Saves the current rendering to a screenshot file.
		/// </summary>
		/// <param name="filename">The name of the file to create or overwrite.</param>
		/// <param name="fileFormat">The image format of the image to create.</param>
		public void TakeScreenshot( string filename, D3D.ImageFileFormat fileFormat )
		{
			D3D.Surface buffer = _viewport.Device.GetBackBuffer( 0, 0, D3D.BackBufferType.Mono );

			D3D.SurfaceLoader.Save( filename, fileFormat, buffer );
		}
		#endregion

		#region Running Plug-Ins
		/// <summary>
		/// Loads the plug-ins associated with the application.
		/// </summary>
		[LuaFunctionAttribute( "LoadPlugIns", "Loads the plug-ins associated with the application." )]
		public void LoadPlugIns()
		{
			_plugins.LoadPlugIns();
		}

		/// <summary>
		/// Loads a plug-in of the specified type.
		/// </summary>
		/// <param name="type">The type of plug-in to load.</param>
		[LuaFunctionAttribute( "LoadPlugIn", "Loads a plug-in of the specified type.",
			"The type of plug-in to load." )]
		public void LoadPlugIn( DataInterfacing.PlugIns.PlugInTypes type )
		{
			_plugins.LoadPlugIn( type );
		}

		/// <summary>
		/// Runs the specified plug-in from the list of plug-ins.
		/// </summary>
		/// <param name="index">The plug-in to run.</param>
		/// <param name="type">The type of plug-in to run.</param>
		/// <param name="owner">The Form calling the plug-in.</param>
		public bool RunPlugIn( int index, DataInterfacing.PlugIns.PlugInTypes type, Form owner )
		{
			return RunPlugInAuto( index, type, owner, null );
		}

		/// <summary>
		/// Runs the specified plug-in from the list of plug-ins.
		/// </summary>
		/// <param name="index">The plug-in to run.</param>
		/// <param name="type">The type of plug-in to run.</param>
		/// <param name="owner">The Form calling the plug-in.</param>
		/// <param name="filename">The filename to load into the plug-in.</param>
		public bool RunPlugInAuto( int index, DataInterfacing.PlugIns.PlugInTypes type, Form owner,
			string filename )
		{
			bool success = false;
			ArrayList plugins = null;

			switch ( type )
			{
				case DataInterfacing.PlugIns.PlugInTypes.Textures:
					plugins = _plugins.TexturePlugIns;
					break;
						
				case DataInterfacing.PlugIns.PlugInTypes.Importing:
					plugins = _plugins.FileImportPlugIns;
					break;
						
				case DataInterfacing.PlugIns.PlugInTypes.Exporting:
					plugins = _plugins.FileExportPlugIns;
					break;

				case DataInterfacing.PlugIns.PlugInTypes.Vertices:
				default:
					plugins = _plugins.VertexPlugIns;
					break;
			}

			if ( index > -1 && index < plugins.Count )
			{
				try
				{
					PlugIn plugin = (PlugIn) plugins[index];
					ArrayList textures = new ArrayList();
					DataCore.Texture tex, texCopy;

					if ( _page != null )
					{
						foreach ( DataCore.Texture t in _page.TerrainPatch.Textures )
							textures.Add( t.Name );
					}

					if ( type != DataInterfacing.PlugIns.PlugInTypes.Importing )
						plugin.SetPage( new TerrainPage( _page ) );

					plugin.StartPosition = FormStartPosition.CenterParent;
					plugin.SetOwner( owner );
					plugin.SetTextures( textures );
					LoadDesiredPlugInData( plugin );

					if ( filename == null )
						plugin.Run();
					else
					{
						ArrayList objects = new ArrayList();

						objects.Add( filename );
						plugin.AutoRun( objects );
					}

					success = plugin.GetSuccess();

					if ( success )
					{
						StoreLastPage( plugin.GetPage(), plugin.GetName() );

						if ( plugin.GetPage() != null && plugin.GetModifiedTextures() )
						{
							textures = plugin.GetTextures();

							// Build new texture details
							for ( int i = 0; i < textures.Count; i++ )
							{
								texCopy = (Texture) textures[i];
								tex = (DataCore.Texture) _page.TerrainPatch.Textures[i];

								if ( tex.DXTexture != null && !tex.DXTexture.Disposed )
									tex.DXTexture.Dispose();

								tex.DXTexture = D3D.TextureLoader.FromFile( _viewport.Device, texCopy.FileName );
								tex.Name = texCopy.Name;
								tex.FileName = texCopy.FileName;
								tex.OperationText = texCopy.OperationText;
								tex.Operation = GetTextureOperation( tex.OperationText );
								tex.Mask = texCopy.Mask;
								tex.Scale = texCopy.Scale;
								tex.Shift = texCopy.Shift;

								_page.TerrainPatch.Textures[i] = tex;
							}
						}

						_page.TerrainPatch.CalculateNormals();
						_buffers.RefreshIndexBuffer_Page();
					}
				}
				catch ( Exception e )
				{
					string message = "An exception has been thrown!\n\n";

					message += "Source: " + e.Source + "\n";
					message += "Error: " + e.Message;

					MessageBox.Show( null, message, "Error Running Application", MessageBoxButtons.OK,
						MessageBoxIcon.Error );
				}
				finally
				{
				}
			}

			return success;
		}

		/// <summary>
		/// Loads the plug-in's desired data into the plug-in.
		/// </summary>
		/// <param name="plugin">The plug-in to load data into.</param>
		protected void LoadDesiredPlugInData( PlugIn plugin )
		{
			ArrayList data = new ArrayList();
			PlugIn.DesiredData d;

			for ( int i = 0; i < plugin.GetDesiredData().Count; i++ )
			{
				d = (PlugIn.DesiredData) plugin.GetDesiredData()[i];

				switch ( d )
				{
					case PlugIn.DesiredData.Lighting:
						data.Add( _viewport.Device.Lights[0].Direction );
						break;
				}
			}

			plugin.SetReceivedData( data );
		}
		#endregion

		#region History
		/// <summary>
		/// Stores the current TerrainPage in the history stack.
		/// </summary>
		/// <param name="action">The action description for what caused the change.</param>
		[LuaFunctionAttribute( "StoreCurrentPage",
			 "Stores the current TerrainPage in the history stack.",
			 "The action description for what caused the change." )]
		public void StoreCurrentPage( string action )
		{
			if ( _page != null )
				_history.PushPage( new TerrainPage( _page ), action );
			else
				_history.PushPage( null, action );

			_redoHistory.ClearHistory();
		}

		/// <summary>
		/// Stores the specified TerrainPage in the history stack before replacing the it
		/// with the new TerrainPage.
		/// </summary>
		/// <param name="page">The new TerrainPage to manipulate.</param>
		/// <param name="action">The action description for what caused the change.</param>
		[LuaFunctionAttribute( "StoreLastPage",
			 "Stores the specified TerrainPage in the history stack before replacing the it with the new TerrainPage.",
			 "The new TerrainPage to manipulate.", "The action description for what caused the change." )]
		public void StoreLastPage( TerrainPage page, string action )
		{
			if ( _page != null )
				_history.PushPage( _page, action );
			else
				_history.PushPage( null, action );

			_redoHistory.ClearHistory();
			_page = page;
		}

		/// <summary>
		/// Stores the specified TerrainPage in the history stack.
		/// </summary>
		/// <param name="page">The new TerrainPage to manipulate.</param>
		/// <param name="action">The action description for what caused the change.</param>
		[LuaFunctionAttribute( "StoreGenericPage", "Stores the specified TerrainPage in the history stack.",
			 "The new TerrainPage to manipulate.", "The action description for what caused the change." )]
		public void StoreGenericPage( TerrainPage page, string action )
		{
			if ( page != null )
				_history.PushPage( new TerrainPage( page ), action );
			else
				_history.PushPage( null, action );

			_redoHistory.ClearHistory();
		}

		/// <summary>
		/// Restores the last TerrainPage pushed onto the history stack.
		/// </summary>
		public void UndoLastPageAction()
		{
			if ( _history.PageHistoryCount > 0 )
			{
				if ( _page != null )
					_redoHistory.PushPage( new TerrainPage( _page ), _history.LastPageAction() );
				else
					_redoHistory.PushPage( null, _history.LastPageAction() );

				_page = _history.PopPage();
				RefreshIndexBuffer();

				if ( _page != null )
					_page.TerrainPatch.RefreshBuffers = true;
			}
		}

		/// <summary>
		/// Restores the last TerrainPage pushed onto the "redo" history stack.
		/// </summary>
		public void RedoLastPageAction()
		{
			if ( _redoHistory.PageHistoryCount > 0 )
			{
				if ( _page != null )
					_history.PushPage( new TerrainPage( _page ), _redoHistory.LastPageAction() );
				else
					_history.PushPage( null, _redoHistory.LastPageAction() );

				_page = _redoHistory.PopPage();
				RefreshIndexBuffer();

				if ( _page != null )
					_page.TerrainPatch.RefreshBuffers = true;
			}
		}
		#endregion
	}
}
