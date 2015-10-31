using System;
using System.Drawing;
using System.IO;
using Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;

namespace Voyage.Terraingine.DataCore
{
	/// <summary>
	/// An object for storing texture data.
	/// </summary>
	public class Texture
	{
		#region Data Members
		private D3D.Texture	_texture;
		private string		_name;
		private string		_file;
		private string		_operationText;
		private bool		_mask;
		private bool		_render;
		private Vector2		_shift, _scale;
		private D3D.TextureOperation	_operation;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the DirectX texture object.
		/// </summary>
		public D3D.Texture DXTexture
		{
			get { return _texture; }
			set { _texture = value; }
		}

		/// <summary>
		/// Gets or sets the name of the texture.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets the filename of the texture.
		/// </summary>
		public string FileName
		{
			get { return _file; }
			set { _file = value; }
		}

		/// <summary>
		/// Gets or sets if the texture is used as a masking texture.
		/// </summary>
		public bool Mask
		{
			get { return _mask; }
			set { _mask = value; }
		}

		/// <summary>
		/// Gets or sets if the texture is to be rendered.
		/// </summary>
		public bool Render
		{
			get { return _render; }
			set { _render = value; }
		}

		/// <summary>
		/// Gets or sets the texture shift.
		/// </summary>
		public Vector2 Shift
		{
			get { return _shift; }
			set { _shift = value; }
		}

		/// <summary>
		/// Gets or sets the texture scale.
		/// </summary>
		public Vector2 Scale
		{
			get { return _scale; }
			set { _scale = value; }
		}

		/// <summary>
		/// Gets the texture blending operation used by the texture.
		/// </summary>
		public D3D.TextureOperation Operation
		{
			get { return _operation; }
			set { _operation = value; }
		}

		/// <summary>
		/// Gets or sets the text associated with the DirectX TextureOperation
		/// </summary>
		public string OperationText
		{
			get { return _operationText; }
			set { _operationText = value; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates a DirectX texture object.
		/// </summary>
		public Texture()
		{
			_texture = null;
			Initialize();
		}

		/// <summary>
		/// Creates a DirectX texture object.
		/// </summary>
		/// <param name="tex">Texture to contain in the object.</param>
		/// <param name="filename">Filename to the original texture file.</param>
		public Texture( D3D.Texture tex, string filename )
		{
			_texture = tex;
			Initialize();
			_name = filename;
			_file = filename;
		}

		/// <summary>
		/// Creates a copy of a Texture object.
		/// </summary>
		/// <param name="tex">Texture object to copy.</param>
		public Texture( Texture tex )
		{
			if ( tex != null )
			{
				_texture = tex.DXTexture;
				_name = tex._name;
				_file = tex._file;
				_mask = tex._mask;
				_render = tex._render;
				_shift = new Vector2( tex._shift.X, tex._shift.Y );
				_scale = new Vector2( tex._scale.X, tex._scale.Y );
				_operation = tex._operation;
				_operationText = tex._operationText;
			}
			else
			{
				_texture = null;
				Initialize();
			}
		}

		/// <summary>
		/// Safely disposes of the texture data.
		/// </summary>
		public void Dispose()
		{
			if ( _texture != null )
			{
				_texture.Dispose();
				_texture = null;
			}

			Initialize();
		}

		/// <summary>
		/// Initializes the additional information in the texture.
		/// </summary>
		public void Initialize()
		{
			_name = null;
			_file = null;
			_mask = false;
			_render = true;
			_shift = new Vector2( 0f, 0f );
			_scale = new Vector2( 1f, 1f );
			_operation = D3D.TextureOperation.SelectArg1;
			_operationText = null;
		}

		/// <summary>
		/// Gets the texture bitmap image data used by the texture.
		/// </summary>
		/// <returns>The bitmap used by the texture.</returns>
		public Bitmap GetImage()
		{
			StreamReader reader = new StreamReader( _file );
			Bitmap image = new Bitmap( reader.BaseStream );

			reader.Close();

			return image;
		}

		/// <summary>
		/// Loads the specified image into the texture.
		/// </summary>
		/// <param name="device">The DirectX device to load the texture into.</param>
		/// <param name="image">The image to load.</param>
		/// <param name="usage">The Direct3D Usage parameter for the image.</param>
		/// <param name="pool">The Direct3D Pool parameter for the image.</param>
		public void SetImage( D3D.Device device, Bitmap image, D3D.Usage usage, D3D.Pool pool )
		{
			if ( _texture != null )
				_texture.Dispose();

			_texture = new D3D.Texture( device, image, usage, pool );
		}
		#endregion
	}
}
