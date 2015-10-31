using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

// Class-specific using directives
using D3DFont = Microsoft.DirectX.Direct3D.Font;
using WinFont = System.Drawing.Font;

namespace Voyage.Terraingine.DXViewport
{
	/// <summary>
	/// A 2D font object in DirectX.
	/// </summary>
	public class Font2D
	{
		#region Data Members
		private D3DFont	_font;
		private WinFont	_winFont;
		private Color	_color;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the Direct3D font object.
		/// </summary>
		public D3DFont D3DFont
		{
			get { return _font; }
		}

		/// <summary>
		/// Gets the Windows font used.
		/// </summary>
		public WinFont WindowsFont
		{
			get { return _winFont; }
		}

		/// <summary>
		/// Gets or sets the color of the text drawn.
		/// </summary>
		public Color Color
		{
			get { return _color; }
			set { _color = value; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates a 2D Direct3D font object.
		/// </summary>
		/// <param name="device">DirectX device to attach the font to.</param>
		public Font2D( Device device )
		{
			_color = Color.Yellow;
			_winFont = new WinFont( System.Drawing.FontFamily.GenericSerif, 12 );
			CreateFont( device );
		}
		
		/// <summary>
		/// Creates a 2D Direct3D font object.
		/// </summary>
		/// <param name="device">DirectX device to attach the font to.</param>
		/// <param name="font">Windows font to draw with.</param>
		public Font2D( Device device, WinFont font )
		{
			_color = Color.Yellow;
			_winFont = font;
			CreateFont( device );
		}

		/// <summary>
		/// Creates the Direct3D font object.
		/// </summary>
		/// <param name="device">DirectX device to attach the font to.</param>
		public void CreateFont( Device device )
		{
			_font = new D3DFont( device, _winFont );
		}

		/// <summary>
		/// Changes the font used by the Direct3D font object.
		/// </summary>
		/// <param name="device">DirectX device to attach the font to.</param>
		/// <param name="font">Windows font to change to.</param>
		public void ChangeFont( Device device, WinFont font )
		{
			_winFont = font;
			CreateFont( device );
		}

		/// <summary>
		/// Draws the specified text using the Direct3D font object.
		/// </summary>
		/// <param name="text">Text to draw.</param>
		/// <param name="rect">Rectangle to draw the text in.</param>
		public void DrawText( string text, Rectangle rect )
		{
			Sprite sprite = null;
			DrawTextFormat format = DrawTextFormat.Top | DrawTextFormat.Left | DrawTextFormat.WordBreak;

			RenderText( sprite, text, rect, format );
		}

		/// <summary>
		/// Draws the specified text using the Direct3D font object.
		/// </summary>
		/// <param name="text">Text to draw.</param>
		/// <param name="rect">Rectangle to draw the text in.</param>
		/// <param name="sprite">Sprite background for the text.</param>
		public void DrawText( string text, Rectangle rect, Sprite sprite )
		{
			DrawTextFormat format = DrawTextFormat.Top | DrawTextFormat.Left | DrawTextFormat.WordBreak;

			RenderText( sprite, text, rect, format );
		}

		/// <summary>
		/// Draws the specified text using the Direct3D font object.
		/// </summary>
		/// <param name="text">Text to draw.</param>
		/// <param name="rect">Rectangle to draw the text in.</param>
		/// <param name="sprite">Sprite background for the text.</param>
		/// <param name="format">Text formatting options.</param>
		public void DrawText( string text, Rectangle rect, Sprite sprite, DrawTextFormat format )
		{
			RenderText( sprite, text, rect, format );
		}

		/// <summary>
		/// Draws the specified text using the Direct3D font object.
		/// </summary>
		/// <param name="text">Text to draw.</param>
		/// <param name="rect">Rectangle to draw the text in.</param>
		/// <param name="format">Text formatting options.</param>
		public void DrawText( string text, Rectangle rect, DrawTextFormat format )
		{
			Sprite sprite = null;

			RenderText( sprite, text, rect, format );
		}

		/// <summary>
		/// Cleanly releases the Direct3D font.
		/// </summary>
		public void Dispose()
		{
			_font = null;
			_winFont = null;
		}

		/// <summary>
		/// Draws the specified text using the Direct3D font object.
		/// </summary>
		/// <param name="text">Text to draw.</param>
		/// <param name="rect">Rectangle to draw the text in.</param>
		/// <param name="sprite">Sprite background for the text.</param>
		/// <param name="format">Text formatting options.</param>
		private void RenderText( Sprite sprite, string text, Rectangle rect, DrawTextFormat format )
		{
			_font.DrawText( sprite, text, rect, format, _color.ToArgb() );
		}
		#endregion
	}
}
