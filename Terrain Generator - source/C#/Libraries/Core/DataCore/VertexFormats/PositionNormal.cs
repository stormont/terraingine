using System;
using System.Drawing;
using Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;

namespace Voyage.Terraingine.DataCore.VertexFormats
{
	/// <summary>
	/// A struct for providing a custom vertex format that provides vertex position and normal information.
	/// </summary>
	public struct PositionNormal
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
		/// Format of the Direct3D vertex.
		/// </summary>
		public static readonly D3D.VertexFormats Format =
			D3D.VertexFormats.Position | D3D.VertexFormats.Normal;

		/// <summary>
		/// Stride size of the Direct3D vertex.
		/// </summary>
		public static readonly int StrideSize = DXHelp.GetTypeSize( typeof( PositionNormal ) );

		/// <summary>
		/// Number of textures the vertex can hold.
		/// </summary>
		public static readonly int numTextures = 0;
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
		#endregion

		#region Methods
		/// <summary>
		/// Creates a vertex with a position and a vertex normal.
		/// </summary>
		/// <param name="x">X-coordinate for the vertex position.</param>
		/// <param name="y">Y-coordinate for the vertex position.</param>
		/// <param name="z">Z-coordinate for the vertex position.</param>
		/// <param name="nX">X-coordinate for the vertex normal direction.</param>
		/// <param name="nY">Y-coordinate for the vertex normal direction.</param>
		/// <param name="nZ">Z-coordinate for the vertex normal direction.</param>
		public PositionNormal( float x, float y, float z, float nX, float nY, float nZ )
		{
			X = x;
			Y = y;
			Z = z;

			Nx = nX;
			Ny = nY;
			Nz = nZ;
		}

		/// <summary>
		/// Creates a vertex with a position and a vertex normal.
		/// </summary>
		/// <param name="position">Position of the vertex.</param>
		/// <param name="normal">Vertex normal direction for the vertex.</param>
		public PositionNormal( Vector3 position, Vector3 normal )
		{
			X = position.X;
			Y = position.Y;
			Z = position.Z;

			Nx = normal.X;
			Ny = normal.Y;
			Nz = normal.Z;
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
