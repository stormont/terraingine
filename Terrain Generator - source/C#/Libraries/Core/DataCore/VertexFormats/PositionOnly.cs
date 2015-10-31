using System;
using System.Drawing;
using Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;

namespace Voyage.Terraingine.DataCore.VertexFormats
{
	/// <summary>
	/// A struct for providing a custom vertex format that contains only position information.
	/// </summary>
	public struct PositionOnly
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
		/// Format of the Direct3D vertex.
		/// </summary>
		public static readonly D3D.VertexFormats Format = D3D.VertexFormats.Position;
		
		/// <summary>
		/// Stride size of the Direct3D vertex.
		/// </summary>
		public static readonly int StrideSize = DXHelp.GetTypeSize( typeof( PositionOnly ) );
		
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
		#endregion

		#region Methods
		/// <summary>
		/// Creates a vertex with a position attribute.
		/// </summary>
		/// <param name="x">X-coordinate of the vertex position.</param>
		/// <param name="y">Y-coordinate of the vertex position.</param>
		/// <param name="z">Z-coordinate of the vertex position.</param>
		public PositionOnly( float x, float y, float z )
		{
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
		/// Creates a vertex with a position attribute.
		/// </summary>
		/// <param name="position">3D coordinates for the vertex position.</param>
		public PositionOnly( Vector3 position )
		{
			X = position.X;
			Y = position.Y;
			Z = position.Z;
		}

		/// <summary>
		/// Sets the position of the vertex.
		/// </summary>
		/// <param name="position">The new position of the vertex.</param>
		public void SetPosition( Vector3 position )
		{
			Position = position;
		}
		#endregion
	};
}
