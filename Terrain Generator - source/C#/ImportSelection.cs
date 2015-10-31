using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Voyage.Terraingine
{
	/// <summary>
	/// A Form for selected which import method to load terrain with.
	/// </summary>
	public class ImportSelection : System.Windows.Forms.Form
	{
		#region Data Members

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox lstMethods;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Properties
		/// <summary>
		/// Gets the selected import method.
		/// </summary>
		public int ImportMethod
		{
			get { return lstMethods.SelectedIndex; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates an import selection form.
		/// </summary>
		public ImportSelection()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.CenterToParent();
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
		/// Selects the double-clicked import method.
		/// </summary>
		private void lstMethods_DoubleClick(object sender, System.EventArgs e)
		{
			if ( lstMethods.SelectedIndex > -1 )
			{
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		/// <summary>
		/// Selects an import method and enables the "OK" button.
		/// </summary>
		private void lstMethods_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ( lstMethods.SelectedIndex > -1 )
				btnOK.Enabled = true;
		}

		/// <summary>
		/// Loads the import methods from which the user will choose from.
		/// </summary>
		/// <param name="methods">The list of import methods.</param>
		public void LoadImportNames( string[] methods )
		{
			foreach ( string s in methods )
				lstMethods.Items.Add( s );
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.lstMethods = new System.Windows.Forms.ListBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(224, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Select a method by which to import terrain:";
			// 
			// lstMethods
			// 
			this.lstMethods.Location = new System.Drawing.Point(8, 24);
			this.lstMethods.Name = "lstMethods";
			this.lstMethods.Size = new System.Drawing.Size(168, 134);
			this.lstMethods.TabIndex = 3;
			this.lstMethods.DoubleClick += new System.EventHandler(this.lstMethods_DoubleClick);
			this.lstMethods.SelectedIndexChanged += new System.EventHandler(this.lstMethods_SelectedIndexChanged);
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Enabled = false;
			this.btnOK.Location = new System.Drawing.Point(184, 32);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(96, 23);
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "OK";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(184, 64);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(96, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			// 
			// ImportSelection
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 166);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.lstMethods);
			this.Controls.Add(this.label1);
			this.Name = "ImportSelection";
			this.Text = "Select Import Method";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
