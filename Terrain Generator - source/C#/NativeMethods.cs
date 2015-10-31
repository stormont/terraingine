using System;
using System.Runtime.InteropServices;

namespace Voyage.Terraingine
{
	/// <summary>
	/// Object for setting up a more efficient render loop.
	/// </summary>
	public class NativeMethods
	{
		/// <summary>
		/// Creates an object of type NativeMethods.
		/// </summary>
		public NativeMethods()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Defines the Message struct for the message pump.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Message
		{
			/// <summary>
			/// Pointer to the window.
			/// </summary>
			public IntPtr hWnd;

			/// <summary>
			/// Pointer to the message.
			/// </summary>
			public Int32 msg;			// This used to be of type WindowMessage, but was undefined

			/// <summary>
			/// Pointer to the WPARAM object.
			/// </summary>
			public IntPtr wParam;

			/// <summary>
			/// Pointer to the LPARAM object.
			/// </summary>
			public IntPtr lParam;

			/// <summary>
			/// Time the message is built.
			/// </summary>
			public uint time;

			/// <summary>
			/// Position of the message.
			/// </summary>
			public System.Drawing.Point p;
		}

		/// <summary>
		/// Checks if there are more messages remaining on the message pump.
		/// </summary>
		/// <returns>Whether another message is on the pump.</returns>
		[System.Security.SuppressUnmanagedCodeSecurity]		// We won't use this maliciously
		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool PeekMessage( out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags );
	}
}
