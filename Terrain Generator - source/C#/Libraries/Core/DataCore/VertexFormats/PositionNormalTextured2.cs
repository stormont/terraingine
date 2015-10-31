using System;
using System.Drawing;
using Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;

namespace Voyage.Terraingine.DataCore.VertexFormats
{
	/// <summary>
	/// A struct for providing a custom vertex format that provides vertex position and normal information.
	/// Also contains information for two sets of texture coordinates.
	/// </summary>
	public struct PositionNormalTextured2
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
		/// U-coordinate of the second texture.
		/// </summary>
		public float Tu2;
		
		/// <summary>
		/// V-coordinate of the second texture.
		/// </summary>
		public float Tv2;
		
		/// <summary>
		/// Format of the Direct3D vertex.
		/// </summary>
		public static readonly D3D.VertexFormats Format =
			D3D.VertexFormats.Position | D3D.VertexFormats.Normal | D3D.VertexFormats.Texture2;
		
		/// <summary>
		/// Stride size of the Direct3D vertex.
		/// </summary>
		public static readonly int StrideSize = DXHelp.GetTypeSize( typeof( PositionNormalTextured1 ) );
		
		/// <summary>
		/// Number of textures the vertex can hold.
		/// </summary>
		public static readonly int numTextures = 2;
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

		/// <summary>
		/// Gets or sets the texture coordinates for the second texture.
		/// </summary>
		public Vector2 Vector22
		{
			get { return new Vector2( Tu2, Tv2 ); }
			set
			{
				Tu2 = value.X;
				Tv2 = value.Y;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates a vertex with a position, a vertex normal, and two sets of texture coordinates.
		/// </summary>
		/// <param name="x">X-coordinate of the vertex position.</param>
		/// <param name="y">Y-coordinate of the vertex position.</param>
		/// <param name="z">Z-coordinate of the vertex position.</param>
		/// <param name="nX">X-coordinate of the vertex normal direction.</param>
		/// <param name="nY">Y-coordinate of the vertex normal direction.</param>
		/// <param name="nZ">Z-coordinate of the vertex normal direction.</param>
		/// <param name="u1">U-coordinate for the first texture.</param>
		/// <param name="v1">V-coordinate for the first texture.</param>
		/// <param name="u2">U-coordinate for the second texture.</param>
		/// <param name="v2">V-coordinate for the second texture.</param>
		public PositionNormalTextured2( float x, float y, float z, float nX, float nY, float nZ,
			float u1, float v1, float u2, float v2 )
		{
			X = x;
			Y = y;
			Z = z;

			Nx = nX;
			Ny = nY;
			Nz = nZ;

			Tu1 = u1;
			Tv1 = v1;

			Tu2 = u2;
			Tv2 = v2;
		}

		/// <summary>
		/// Creates a vertex with a position, a vertex normal, and two sets of texture coordinates.
		/// </summary>
		/// <param name="position">Position of the vertex.</param>
		/// <param name="normal">Vertex normal direction for the vertex.</param>
		/// <param name="texCoords1">Coordinates for the first texture of the vertex.</param>
		/// <param name="texCoords2">Coordinates for the second texture of the vertex.</param>
		public PositionNormalTextured2( Vector3 position, Vector3 normal, Vector2 texCoords1,
			Vector2 texCoords2 )
		{
			X = position.X;
			Y = position.Y;
			Z = position.Z;

			Nx = normal.X;
			Ny = normal.Y;
			Nz = normal.Z;

			Tu1 = texCoords1.X;
			Tv1 = texCoords1.Y;

			Tu2 = texCoords2.X;
			Tv2 = texCoords2.Y;
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

				case 2:
					texCoords = this.Vector22;
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

				case 2:
					this.Vector22 = texCoords;
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
