using System;
using System.Drawing;
using Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;

namespace Voyage.Terraingine.DataCore.VertexFormats
{
	/// <summary>
	/// A struct for providing a custom vertex format that provides vertex position and normal information.
	/// Also contains information for one set of texture coordinates.
	/// </summary>
	public struct PositionNormalTextured1
	{
		#region Data Members
		/// <summary>
		/// X-coordinate of the vertex.
		/// </summary>
		public float X;
		
		/// <summary>
		/// Y-coordinate of the vertex.
		/// </summary>
		public float Y;

		/// <summary>
		/// Z-coordinate of the vertex.
		/// </summary>
		public float Z;
		
		/// <summary>
		/// X-coordinate of the vertex normal.
		/// </summary>
		public float Nx;
		
		/// <summary>
		/// Y-coordinate of the vertex normal.
		/// </summary>
		public float Ny;
		
		/// <summary>
		/// Z-coordinate of the vertex normal.
		/// </summary>
		public float Nz;

		/// <summary>
		/// U-coordinate of the first texture.
		/// </summary>
		public float Tu1;
		
		/// <summary>
		/// V-coordinate of the first texture.
		/// </summary>
		public float Tv1;
		
		/// <summary>
		/// Format of the Direct3D vertex.
		/// </summary>
		public static readonly D3D.VertexFormats Format =
			D3D.VertexFormats.Position | D3D.VertexFormats.Normal | D3D.VertexFormats.Texture1;
		
		/// <summary>
		/// Stride size of the Direct3D vertex.
		/// </summary>
		public static readonly int StrideSize = DXHelp.GetTypeSize( typeof( PositionNormalTextured1 ) );
		
		/// <summary>
		/// Number of textures the vertex can hold.
		/// </summary>
		public static readonly int numTextures = 1;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the position of the vertex.
		/// </summary>
		public Vector3 Position
		{
			get { return new Vector3( X, Y, Z ); }
			set
			{
				X = value.X;
				Y = value.Y;
				Z = value.Z;
			}
		}

		/// <summary>
		/// Gets or sets the vertex normal.
		/// </summary>
		public Vector3 Normal
		{
			get { return new Vector3( Nx, Ny, Nz ); }
			set
			{
				Nx = value.X;
				Ny = value.Y;
				Nz = value.Z;
			}
		}

		/// <summary>
		/// Gets or sets the texture coordinates for the first texture.
		/// </summary>
		public Vector2 Vector21
		{
			get { return new Vector2( Tu1, Tv1 ); }
			set
			{
				Tu1 = value.X;
				Tv1 = value.Y;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates a vertex with a position, a vertex normal, and one set of texture coordinates.
		/// </summary>
		/// <param name="x">X-coordinate of the vertex position.</param>
		/// <param name="y">Y-coordinate of the vertex position.</param>
		/// <param name="z">Z-coordinate of the vertex position.</param>
		/// <param name="nX">X-coordinate of the vertex normal direction.</param>
		/// <param name="nY">Y-coordinate of the vertex normal direction.</param>
		/// <param name="nZ">Z-coordinate of the vertex normal direction.</param>
		/// <param name="u">U-coordinate for the first texture.</param>
		/// <param name="v">V-coordinate for the first texture.</param>
		public PositionNormalTextured1( float x, float y, float z, float nX, float nY, float nZ,
			float u, float v )
		{
			X = x;
			Y = y;
			Z = z;

			Nx = nX;
			Ny = nY;
			Nz = nZ;

			Tu1 = u;
			Tv1 = v;
		}

		/// <summary>
		/// Creates a vertex with a position, a vertex normal, and one set of texture coordinates.
		/// </summary>
		/// <param name="position">Position of the vertex.</param>
		/// <param name="normal">Vertex normal direction for the vertex.</param>
		/// <param name="texCoords">Coordinates for the first texture of the vertex.</param>
		public PositionNormalTextured1( Vector3 position, Vector3 normal, Vector2 texCoords )
		{
			X = position.X;
			Y = position.Y;
			Z = position.Z;

			Nx = normal.X;
			Ny = normal.Y;
			Nz = normal.Z;

			Tu1 = texCoords.X;
			Tv1 = texCoords.Y;
		}

		/// <summary>
		/// Gets the texture coordinates for the specified texture.
		/// </summary>
		/// <param name="texture">Texture to get coordinates for.</param>
		/// <returns>Texture coordinates for the specified texture.</returns>
		public Vector2 GetVector2( int texture )
		{
			Vector2 texCoords;

			switch ( texture + 1 )
			{
				case 1:
					texCoords = this.Vector21;
					break;

				default:
					texCoords = new Vector2(0, 0);
					break;
			}

			return texCoords;
		}

		/// <summary>
		/// Sets the texture coordinates for the specified texture.
		/// </summary>
		/// <param name="texCoords">Texture coordinates for the texture.</param>
		/// <param name="texture">Texture to update.</param>
		public void SetTextureCoordinates( Vector2 texCoords, int texture )
		{
			switch ( texture + 1 )
			{
				case 1:
					this.Vector21 = texCoords;
					break;

				default:
					break;
			}
		}

		/// <summary>
		/// Sets the position of the vertex.
		/// </summary>
		/// <param name="position">The new position of the vertex.</param>
		public void SetPosition( Vector3 position )
		{
			Position = position;
		}

		/// <summary>
		/// Sets the normal of the vertex.
		/// </summary>
		/// <param name="normal">The new normal of the vertex.</param>
		public void SetNormal( Vector3 normal )
		{
			Normal = normal;
		}
		#endregion
	};
}
