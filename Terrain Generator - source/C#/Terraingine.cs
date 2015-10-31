using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Voyage.Terraingine
{
	/// <summary>
	/// Entry class for the program.
	/// </summary>
	public class Terraingine
	{
		/// <summary>
		/// Creates a Terraingine object.
		/// </summary>
		public Terraingine()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Entry point for the program.
		/// </summary>
		static void Main()
		{
			try
			{
				using ( MainForm mainForm = new MainForm() )
				{
					Application.Idle += new EventHandler( mainForm.OnApplicationIdle );
					Application.Run( mainForm );
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
		}
	}
}
