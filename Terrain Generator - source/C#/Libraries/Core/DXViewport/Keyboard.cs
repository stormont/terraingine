using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX.DirectInput;

namespace Voyage.Terraingine.DXViewport
{
	/// <summary>
	/// Summary description for Keyboard.
	/// </summary>
	public class Keyboard
	{
		#region Data Members
		private Form			_window;
		private KeyboardState	_state;
		private Microsoft.DirectX.DirectInput.Device	_device;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the window to poll keyboard from.
		/// </summary>
		public Form Window
		{
			get { return _window; }
			set { Initialize( value ); }
		}

		/// <summary>
		/// Gets the last polled state of the keyboard.
		/// </summary>
		public KeyboardState Keys
		{
			get { return _state; }
		}

		/// <summary>
		/// Gets the current state of the keyboard.
		/// </summary>
		public KeyboardState CurrentKeys
		{
			get
			{
				Update();
				return _state;
			}
		}

		/// <summary>
		/// Gets the input device used to grab keyboard information.
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
		#endregion

		#region Methods
		/// <summary>
		/// Creates an object for polling the system keyboard.
		/// </summary>
		public Keyboard()
		{
		}

		/// <summary>
		/// Creates an object for polling the system keyboard.
		/// </summary>
		/// <param name="window">Window to poll keyboard from.</param>
		public Keyboard( Form window )
		{
			Initialize( window );
		}

		/// <summary>
		/// Disposes of the keyboard device.
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
		/// Initializes the keyboard device to poll from.
		/// </summary>
		/// <param name="window">Window to poll keyboard from.</param>
		public void Initialize( Form window )
		{
			Dispose();
			_window = window;
			_device = new Device( SystemGuid.Keyboard );

			if ( _device != null )
			{
				_device.SetCooperativeLevel( window.Handle,
					CooperativeLevelFlags.NonExclusive | CooperativeLevelFlags.Background );
				_device.Acquire();
			}
		}

		/// <summary>
		/// Updates the keyboard device.
		/// </summary>
		public void Update()
		{
			_state = _device.GetCurrentKeyboardState();
		}
		#endregion
	}
}
