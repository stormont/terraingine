using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Voyage.Terraingine.ImportTerrainRaw
{
	/// <summary>
	/// An object for containing terrain parameters.
	/// </summary>
	public class Parameters : System.Windows.Forms.Form
	{
		#region Data Members

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numHeight;
		private System.Windows.Forms.NumericUpDown numWidth;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Properties
		/// <summary>
		/// Gets the terrain height specified by the user.
		/// </summary>
		public int TerrainHeight
		{
			get { return (int) numHeight.Value; }
		}

		/// <summary>
		/// Gets the terrain width specified by the user.
		/// </summary>
		public int TerrainWidth
		{
			get { return (int) numWidth.Value; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates an object for holding terrain parameters.
		/// </summary>
		public Parameters()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Center the form to the center of its parent
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
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.numHeight = new System.Windows.Forms.NumericUpDown();
			this.numWidth = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(144, 32);
			this.label1.TabIndex = 8;
			this.label1.Text = "Specify the size of the RAW image being opened:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(160, 32);
			this.label2.TabIndex = 9;
			this.label2.Text = "(Leave defaults at 0 to assume square image.)";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 80);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 16);
			this.label3.TabIndex = 2;
			this.label3.Text = "Height:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 112);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(40, 16);
			this.label4.TabIndex = 3;
			this.label4.Text = "Width:";
			// 
			// numHeight
			// 
			this.numHeight.Location = new System.Drawing.Point(64, 80);
			this.numHeight.Maximum = new System.Decimal(new int[] {
																	  255,
																	  0,
																	  0,
																	  0});
			this.numHeight.Name = "numHeight";
			this.numHeight.Size = new System.Drawing.Size(56, 20);
			this.numHeight.TabIndex = 4;
			// 
			// numWidth
			// 
			this.numWidth.Location = new System.Drawing.Point(64, 112);
			this.numWidth.Maximum = new System.Decimal(new int[] {
																	 255,
																	 0,
																	 0,
																	 0});
			this.numWidth.Name = "numWidth";
			this.numWidth.Size = new System.Drawing.Size(56, 20);
			this.numWidth.TabIndex = 5;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(128, 80);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(112, 24);
			this.label5.TabIndex = 6;
			this.label5.Text = "Specifies the number of terrain rows.";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(128, 112);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(112, 24);
			this.label6.TabIndex = 7;
			this.label6.Text = "Specifies the number of terrain columns.";
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(32, 160);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "OK";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(160, 160);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			// 
			// Parameters
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(248, 198);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.numWidth);
			this.Controls.Add(this.numHeight);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "Parameters";
			this.Text = "Specify RAW Image Parameters";
			((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
