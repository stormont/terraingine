using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Voyage.LuaNetInterface;

namespace Voyage.Terraingine
{
	/// <summary>
	/// A class for running Lua scripts through a WinForms "console".
	/// </summary>
	public class LuaScripting : System.Windows.Forms.Form
	{
		#region Data Members
		private LuaVirtualMachine		_lua;
		private MainForm				_owner;

		private System.Windows.Forms.RichTextBox txtScript;
		private System.Windows.Forms.RichTextBox txtResult;
		private System.Windows.Forms.Button btnRun;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnHelp;
		private System.Windows.Forms.Label lblResult;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the Lua virtual machine.
		/// </summary>
		public LuaVirtualMachine LuaVM
		{
			get { return _lua; }
			set { _lua = value; }
		}
		#endregion

		#region Basic Form Methods
		/// <summary>
		/// Creates an object for running Lua scripts.
		/// </summary>
		/// <param name="owner">The owner MainForm of the window.</param>
		public LuaScripting( MainForm owner )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_owner = owner;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Runs the entered script.
		/// </summary>
		private void btnRun_Click(object sender, System.EventArgs e)
		{
			if ( _lua != null )
			{
				txtResult.Text = "";

				try
				{
					_owner.DataInterface.StoreCurrentPage( "Lua Script" );
					_lua.Lua.DoString( txtScript.Text );
				}
				catch ( Exception ex )
				{
					txtResult.Text += ex.Message;
				}
				finally
				{
					_owner.UpdateStates();
				}
			}
			else
				MessageBox.Show( "Lua not initialized", "Lua Error", MessageBoxButtons.OK );
		}

		/// <summary>
		/// Saves the entered script.
		/// </summary>
		private void btnSave_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog saveDlg = new SaveFileDialog();
			DialogResult result;

			// Set SaveFileDialog settings
			saveDlg.Filter = "Lua Script Files (*.lua)|*.lua|All files (*.*)|*.*" ;
			saveDlg.InitialDirectory = Path.GetDirectoryName( Application.ExecutablePath ) + "\\Scripts";

			// Show dialog and store result
			result = saveDlg.ShowDialog( this );

			// Save data
			if ( result == DialogResult.OK && saveDlg.FileName != null )
			{
				StreamWriter writer = new StreamWriter( saveDlg.FileName );

				// Store entered text
				writer.Write( txtScript.Text );
				writer.Close();
			}
		}

		/// <summary>
		/// Loads a previously saved script.
		/// </summary>
		private void btnLoad_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog openDlg = new OpenFileDialog();
			DialogResult result;

			// Set OpenFileDialog settings
			openDlg.Filter = "Lua Script Files (*.lua)|*.lua|All files (*.*)|*.*" ;
			openDlg.InitialDirectory = Path.GetDirectoryName( Application.ExecutablePath ) + "\\Scripts";

			// Show dialog and store result
			result = openDlg.ShowDialog( this );

			// Open data
			if ( result == DialogResult.OK && openDlg.FileName != null )
			{
				StreamReader reader = new StreamReader( openDlg.FileName );

				// Display stored text
				txtScript.Text = reader.ReadToEnd();
				reader.Close();
			}
		}

		/// <summary>
		/// Removes Lua-registered functions of the class on closing.
		/// </summary>
		private void LuaScripting_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if ( _lua != null )
			{
				_lua.Functions.Remove( "help" );
				_lua.Functions.Remove( "helpcmd" );
				_lua.Functions.Remove( "display" );
			}
		}

		/// <summary>
		/// Displays Lua-registered help function.
		/// </summary>
		private void btnHelp_Click(object sender, System.EventArgs e)
		{
			txtResult.Clear();
			_lua.Lua.DoString( "help()" );
		}
		
		/// <summary>
		/// Resizes the script toolbar window.
		/// </summary>
		private void LuaScripting_SizeChanged(object sender, System.EventArgs e)
		{
			int textWidth = ( this.Width - 34 ) / 2;
			int textHeight = this.Height - 122;
			int spacer = 8;
			Point p;
			Size s = new Size();

			// Set minimum TextBox sizes
			if ( textWidth < 200 )
				textWidth = 200;

			if ( textHeight < 100 )
				textHeight = 100;

			// Set TextBox sizes
			s.Width = textWidth;
			s.Height = textHeight;
			txtScript.Size = s;
			txtResult.Size = s;

			// Set Result TextBox location
			p = txtResult.Location;
			p.X = spacer + textWidth + spacer;
			txtResult.Location = p;

			// Set Results Label location
			p = txtResult.Location;
			p.Y -= spacer * 2;
			lblResult.Location = p;

			// Set Run Button location
			p = txtScript.Location;
			p.Y += textHeight + spacer;
			btnRun.Location = p;

			// Set Save Button location
			p = btnRun.Location;
			p.X += textWidth - btnSave.Width;
			btnSave.Location = p;

			// Set Help Button location
			p = btnSave.Location;
			p.X += textWidth + btnSave.Width + spacer - btnHelp.Width;
			btnHelp.Location = p;
		}
		#endregion

		#region LuaMethods
		/// <summary>
		/// Displays available commands to the console.
		/// </summary>
		[LuaFunctionAttribute( "help", "List available commands." )]
		public void Help()
		{
			string[] results;
			int counter;

			// Display stand-alone commands
			if ( _lua.Functions.Count > 0 )
			{
				results = new string[_lua.Functions.Count];
				counter = 0;

				txtResult.Text += "HELP:\n=====\nCommands and packages available through Lua interface.\n" +
					"\nAvailable commands:\n";

				IDictionaryEnumerator funcs = _lua.Functions.GetEnumerator();

				while ( funcs.MoveNext() )
				{
					results[counter] = ( (LuaFunctionDescriptor) funcs.Value ).FunctionHeader;
					counter++;
				}

				Array.Sort( results );

				for ( int i = 0; i < counter; i++ )
					txtResult.Text += results[i] + "\n";
			}

			// Display packages
			if ( _lua.Packages.Count > 0 )
			{
				results = new string[_lua.Packages.Count];
				counter = 0;

				txtResult.Text += "\nAvailable packages:\n";

				IDictionaryEnumerator pkgs = _lua.Packages.GetEnumerator();

				while ( pkgs.MoveNext() )
				{
					LuaPackageDescriptor desc = (LuaPackageDescriptor) pkgs.Value;

					results[counter] += desc.PackageName + " - " + desc.PackageDocumentation;
					counter++;
				}

				Array.Sort( results );

				for ( int i = 0; i < counter; i++ )
					txtResult.Text += results[i] + "\n";
			}

			txtResult.Text += "\n";
		}

		/// <summary>
		/// Displays information about the specified command to the console.
		/// </summary>
		/// <param name="command">The command to display information for.</param>
		[LuaFunctionAttribute( "helpcmd", "Show help for a given command.",
			 "Command to get help for (put in quotes)." )]
		public void Help( string command )
		{
			// Check if the command is a function name
			if ( _lua.Functions.Contains( command ) )
			{
				LuaFunctionDescriptor func = (LuaFunctionDescriptor) _lua.Functions[command];

				// Display basic help message
				txtResult.Text += "Help for " + command + ":\n=========";

				for ( int i = 0; i <= command.Length; i++ )
					txtResult.Text += "=";

				txtResult.Text += "\n" + func.FunctionFullDocumentation + "\n\n";

				return;
			}

			// Check if the command is a package name
			if ( command.IndexOf( "." ) == -1 )
			{
				// Check if the package exists
				if ( _lua.Packages.ContainsKey( command ) )
				{
					LuaPackageDescriptor pkg = (LuaPackageDescriptor) _lua.Packages[command];

					DisplayPackageHelp( pkg );

					return;
				}
				else
				{
					txtResult.Text += "No such function or package: " + command + "\n\n";

					return;
				}
			}

			// Determine the path to the function name
			string[] parts = command.Split( '.' );

			// Check if the package exists
			if ( !_lua.Packages.ContainsKey( parts[0] ) )
			{
				txtResult.Text += "No such function or package: " + command + "\n\n";

				return;
			}
			
			LuaPackageDescriptor desc = (LuaPackageDescriptor) _lua.Packages[ parts[0] ];

			// Check if the function exists within the package
			if ( !desc.HasFunction( parts[1] ) )
			{
				txtResult.Text += "Package " + parts[0] + " doesn't have a " + parts[1] + " function.\n\n";

				return;
			}

			// Display basic help message
			txtResult.Text += "Help for " + command + ":\n=========";

			for ( int i = 0; i <= command.Length; i++ )
				txtResult.Text += "=";

			txtResult.Text += "\n" + desc.PackageName + "." + desc.WriteHelp( parts[1] ) + "\n\n";
		}

		/// <summary>
		/// Displays the specified text strings to the output window.
		/// </summary>
		/// <param name="text">The text to be displayed.</param>
		[LuaFunctionAttribute( "print", "Displays the specified text string to the output window.",
			 "The text to be displayed." )]
		public void Print( string text )
		{
			txtResult.Text += text + "\n";
		}
		#endregion

		#region Non-Lua Methods
		/// <summary>
		/// Displays help for the LuaPackageDescriptor to the output window.
		/// </summary>
		/// <param name="package">The package to display help for.</param>
		public void DisplayPackageHelp( LuaPackageDescriptor package )
		{
			IDictionaryEnumerator functions = package.Functions.GetEnumerator();
			string[] results = new string[package.Functions.Count];
			int counter = 0;

			txtResult.Text += package.PackageName + ":\n";

			for ( int i = 0; i <= package.PackageName.Length; i++ )
				txtResult.Text += "=";
			
			txtResult.Text += "\n" + package.PackageDocumentation + "\n\n" + "Available commands:\n";

			// Display functions within the package
			while ( functions.MoveNext() )
			{
				results[counter] = ( (LuaFunctionDescriptor) functions.Value ).FunctionHeader;
				counter++;
			}

			Array.Sort( results );

			for ( int i = 0; i < counter; i++ )
				txtResult.Text += results[i] + "\n";

			txtResult.Text += "\n";
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtScript = new System.Windows.Forms.RichTextBox();
			this.txtResult = new System.Windows.Forms.RichTextBox();
			this.btnRun = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnLoad = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.lblResult = new System.Windows.Forms.Label();
			this.btnHelp = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtScript
			// 
			this.txtScript.DetectUrls = false;
			this.txtScript.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.txtScript.Location = new System.Drawing.Point(8, 56);
			this.txtScript.Name = "txtScript";
			this.txtScript.Size = new System.Drawing.Size(344, 408);
			this.txtScript.TabIndex = 0;
			this.txtScript.Text = "";
			this.txtScript.WordWrap = false;
			// 
			// txtResult
			// 
			this.txtResult.DetectUrls = false;
			this.txtResult.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.txtResult.Location = new System.Drawing.Point(360, 56);
			this.txtResult.Name = "txtResult";
			this.txtResult.ReadOnly = true;
			this.txtResult.Size = new System.Drawing.Size(344, 408);
			this.txtResult.TabIndex = 2;
			this.txtResult.Text = "";
			this.txtResult.WordWrap = false;
			// 
			// btnRun
			// 
			this.btnRun.Location = new System.Drawing.Point(8, 472);
			this.btnRun.Name = "btnRun";
			this.btnRun.Size = new System.Drawing.Size(88, 23);
			this.btnRun.TabIndex = 1;
			this.btnRun.Text = "Run Script";
			this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(264, 472);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(88, 23);
			this.btnSave.TabIndex = 3;
			this.btnSave.Text = "Save Script";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnLoad
			// 
			this.btnLoad.Location = new System.Drawing.Point(8, 8);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(88, 23);
			this.btnLoad.TabIndex = 4;
			this.btnLoad.Text = "Load Script";
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 5;
			this.label1.Text = "Script:";
			// 
			// lblResult
			// 
			this.lblResult.Location = new System.Drawing.Point(360, 40);
			this.lblResult.Name = "lblResult";
			this.lblResult.Size = new System.Drawing.Size(100, 16);
			this.lblResult.TabIndex = 6;
			this.lblResult.Text = "Results:";
			// 
			// btnHelp
			// 
			this.btnHelp.Location = new System.Drawing.Point(632, 472);
			this.btnHelp.Name = "btnHelp";
			this.btnHelp.TabIndex = 7;
			this.btnHelp.Text = "View Help";
			this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
			// 
			// LuaScripting
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(714, 504);
			this.Controls.Add(this.btnHelp);
			this.Controls.Add(this.lblResult);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnLoad);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnRun);
			this.Controls.Add(this.txtResult);
			this.Controls.Add(this.txtScript);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "LuaScripting";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Lua Scripting Interface";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.LuaScripting_Closing);
			this.SizeChanged += new System.EventHandler(this.LuaScripting_SizeChanged);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
