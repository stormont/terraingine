using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX.DirectInput;

namespace Voyage.Terraingine.DXViewport
{
	/// <summary>
	/// Summary description for Mouse.
	/// </summary>
	public class Mouse
	{
		#region Data Members
		private Form		_window;
		private MouseState	_state;
		private bool[]		_buttons;
		private Microsoft.DirectX.DirectInput.Device	_device;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the window to poll mouse from.
		/// </summary>
		public Form Window
		{
			get { return _window; }
			set { Initialize( value ); }
		}

		/// <summary>
		/// Gets the last polled state of the mouse.
		/// </summary>
		public MouseState State
		{
			get { return _state; }
		}

		/// <summary>
		/// Gets the last polled location of the mouse.
		/// </summary>
		public Point Location
		{
			get
			{
				Point p = Point.Empty;

				p.X = _state.X;
				p.Y = _state.Y;

				return p;
			}
		}

		/// <summary>
		/// Gets the current polled location of the mouse.
		/// </summary>
		public Point CurrentLocation
		{
			get
			{
				Point p = Point.Empty;

				Update();
				p.X = _state.X;
				p.Y = _state.Y;

				return p;
			}
		}

		/// <summary>
		/// Gets the last polled state of the mouse buttons.
		/// </summary>
		public bool[] Buttons
		{
			get { return _buttons; }
		}

		/// <summary>
		/// Gets the current state of the mouse buttons.
		/// </summary>
		public bool[] CurrentButtons
		{
			get
			{
				Update();
				return _buttons;
			}
		}

		/// <summary>
		/// Gets the input device used to grab mouse information.
		/// </summary>
		public Microsoft.DirectX.DirectInput.Device Device
		{
			get { return _device; }
		}

		/// <summary>
		/// Gets whether the input device has been properly initialized.
		/// </summary>
		public bool Initialized
		{
			get
			{
				if ( _device != null )
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets if the left mouse button is pressed.
		/// </summary>
		public bool LeftButton
		{
			get { return _buttons[0]; }
		}

		/// <summary>
		/// Gets if the right mouse button is pressed.
		/// </summary>
		public bool RightButton
		{
			get { return _buttons[1]; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates an object for polling the system mouse.
		/// </summary>
		public Mouse()
		{
		}

		/// <summary>
		/// Creates an object for polling the system mouse.
		/// </summary>
		/// <param name="window">Window to poll mouse from.</param>
		public Mouse( Form window )
		{
			Initialize( window );
		}

		/// <summary>
		/// Disposes of the mouse device.
		/// </summary>
		public void Dispose()
		{
			if ( _device != null )
			{
				_device.Dispose();
				_device = null;
			}
		}

		/// <summary>
		/// Initializes the mouse device to poll from.
		/// </summary>
		/// <param name="window">Window to poll mouse from.</param>
		public void Initialize( Form window )
		{
			Dispose();
			_window = window;
			_device = new Device( SystemGuid.Mouse );

			if ( _device != null )
			{
				_device.Properties.AxisModeAbsolute = false;
				_device.SetCooperativeLevel( window.Handle,
					CooperativeLevelFlags.NonExclusive | CooperativeLevelFlags.Background );
				_device.Acquire();
			}
		}

		/// <summary>
		/// Updates the mouse device.
		/// </summary>
		public void Update()
		{
			_state = _device.CurrentMouseState;
			_buttons = new bool[_state.GetMouseButtons().Length];
			UpdatePressedButtons();
		}

		/// <summary>
		/// Updates the state of the which buttons are pressed.
		/// </summary>
		protected void UpdatePressedButtons()
		{
			byte[] buttons = _state.GetMouseButtons();

			for ( int i = 0; i < buttons.Length; i++ )
			{
				if ( buttons[i] != 0 )
					_buttons[i] = true;
				else
					_buttons[i] = false;
			}
		}
		#endregion
	}
}
