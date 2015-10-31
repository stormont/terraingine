using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataInterfacing;
using Voyage.Terraingine.DXViewport;
using Voyage.LuaNetInterface;

namespace Voyage.Terraingine
{
	/// <summary>
	/// A user control for manipulating lighting of terrain.
	/// </summary>
	public class LightManipulation : System.Windows.Forms.UserControl
	{
		#region Data Members
		private DataInterfacing.ViewportInterface	_viewport;
		private DataInterfacing.DataManipulation	_terrainData;
		private TerrainViewport		_owner;
		private DXViewport.Viewport	_dx;
		private bool				_updateData;

		private System.Windows.Forms.GroupBox grpLights_VertexColoring;
		private System.Windows.Forms.Button btnLights_HeightColor;
		private System.Windows.Forms.GroupBox grpLights_Primary;
		private System.Windows.Forms.NumericUpDown numLights_Angle;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox txtLights_PrimaryB;
		private System.Windows.Forms.TextBox txtLights_PrimaryG;
		private System.Windows.Forms.TextBox txtLights_PrimaryR;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TrackBar trkLights_PrimaryB;
		private System.Windows.Forms.TrackBar trkLights_PrimaryG;
		private System.Windows.Forms.TrackBar trkLights_PrimaryR;
		private System.Windows.Forms.Button btnLights_DefaultColor;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets whether the control allows data updates.
		/// </summary>
		public bool EnableDataUpdates
		{
			get { return _updateData; }
			set { _updateData = value; }
		}
		#endregion

		#region Basic Form Methods
		/// <summary>
		/// Creates a lighting manipulation user control.
		/// </summary>
		public LightManipulation()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
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
		/// Initializes the control's data members.
		/// </summary>
		/// <param name="owner">The TerrainViewport that contains the control.</param>
		public void Initialize( TerrainViewport owner )
		{
			// Shortcut variables for the DirectX viewport and the terrain data
			_owner = owner;
			_viewport = owner.MainViewport;
			_terrainData = owner.MainViewport.TerrainData;
			_dx = owner.MainViewport.DXViewport;

			// Initialize the control-specific data
			_updateData = true;

			// Register tooltips
			ToolTip t = new ToolTip();

			// Adjust Primary Light group tooltips
			t.SetToolTip( trkLights_PrimaryR, "Amount of red in the primary light" );
			t.SetToolTip( txtLights_PrimaryR, "Amount of red in the primary light" );
			t.SetToolTip( trkLights_PrimaryG, "Amount of green in the primary light" );
			t.SetToolTip( txtLights_PrimaryG, "Amount of green in the primary light" );
			t.SetToolTip( trkLights_PrimaryB, "Amount of blue in the primary light" );
			t.SetToolTip( txtLights_PrimaryB, "Amount of blue in the primary light" );

			// Vertex Coloring group tooltips
			t.SetToolTip( btnLights_DefaultColor, "Display normal terrain coloring" );
			t.SetToolTip( btnLights_HeightColor, "Display terrain vertex color by height" );
		}
		#endregion

		#region Adjust Primary Light
		/// <summary>
		/// Changes the red-color element of the primary light.
		/// </summary>
		private void txtLights_PrimaryR_TextChanged(object sender, System.EventArgs e)
		{
			if ( _updateData && trkLights_PrimaryR.Value.ToString().Length > 0 )
			{
				trkLights_PrimaryR.Value = Convert.ToInt32( txtLights_PrimaryR.Text );
				UpdatePrimaryLightColor();
			}
		}

		/// <summary>
		/// Changes the green-color element of the primary light.
		/// </summary>
		private void txtLights_PrimaryG_TextChanged(object sender, System.EventArgs e)
		{
			if ( _updateData && trkLights_PrimaryG.Value.ToString().Length > 0 )
			{
				trkLights_PrimaryG.Value = Convert.ToInt32( txtLights_PrimaryG.Text );
				UpdatePrimaryLightColor();
			}
		}

		/// <summary>
		/// Changes the blue-color element of the primary light.
		/// </summary>
		private void txtLights_PrimaryB_TextChanged(object sender, System.EventArgs e)
		{
			if ( _updateData && trkLights_PrimaryB.Value.ToString().Length > 0 )
			{
				trkLights_PrimaryB.Value = Convert.ToInt32( txtLights_PrimaryB.Text );
				UpdatePrimaryLightColor();
			}
		}

		/// <summary>
		/// Changes the red-color element of the primary light.
		/// </summary>
		private void trkLights_PrimaryR_Scroll(object sender, System.EventArgs e)
		{
			if ( _updateData )
			{
				txtLights_PrimaryR.Text = trkLights_PrimaryR.Value.ToString();
				UpdatePrimaryLightColor();
			}
		}

		/// <summary>
		/// Changes the green-color element of the primary light.
		/// </summary>
		private void trkLights_PrimaryG_Scroll(object sender, System.EventArgs e)
		{
			if ( _updateData )
			{
				txtLights_PrimaryG.Text = trkLights_PrimaryG.Value.ToString();
				UpdatePrimaryLightColor();
			}
		}

		/// <summary>
		/// Changes the blue-color element of the primary light.
		/// </summary>
		private void trkLights_PrimaryB_Scroll(object sender, System.EventArgs e)
		{
			if ( _updateData )
			{
				txtLights_PrimaryB.Text = trkLights_PrimaryB.Value.ToString();
				UpdatePrimaryLightColor();
			}
		}

		/// <summary>
		/// Changes the angle of the primary light around the terrain.
		/// </summary>
		private void numLights_Angle_ValueChanged(object sender, System.EventArgs e)
		{
			// Adjust the angle value to cycle between 0 and 360.
			if ( numLights_Angle.Value > 359 )
			{
				if ( _updateData )
				{
					_updateData = false;
					numLights_Angle.Value = 0;
					_updateData = true;
				}
				else
					numLights_Angle.Value = 0;
			}

			if ( numLights_Angle.Value < 0 )
			{
				if ( _updateData )
				{
					_updateData = false;
					numLights_Angle.Value = 359;
					_updateData = true;
				}
				else
					numLights_Angle.Value = 359;
			}

			UpdatePrimaryLightAngle();
		}
		#endregion

		#region Vertex Coloring
		/// <summary>
		/// Sets the base color of the terrain to the lighting color.
		/// </summary>
		private void btnLights_DefaultColor_Click(object sender, System.EventArgs e)
		{
			ColorTerrainByLighting();
		}

		/// <summary>
		/// Sets the base color of the terrain to coloring dependent upon vertex height.
		/// </summary>
		private void btnLights_HeightColor_Click(object sender, System.EventArgs e)
		{
			ColorTerrainByHeight();
		}
		#endregion

		#region Other Terrain Functions
		/// <summary>
		/// Updates the primary light color.
		/// </summary>
		private void UpdatePrimaryLightColor()
		{
			if ( _updateData )
			{
				Color color = _dx.Device.Lights[0].Diffuse;

				color = Color.FromArgb( Convert.ToInt32( txtLights_PrimaryR.Text ),
					Convert.ToInt32( txtLights_PrimaryG.Text ), Convert.ToInt32( txtLights_PrimaryB.Text ) );
				_dx.Device.Lights[0].Diffuse = color;
				_dx.Device.Lights[0].Update();
			}
		}

		/// <summary>
		/// Updates the direction of the primary light.
		/// </summary>
		private void UpdatePrimaryLightAngle()
		{
			if ( _updateData )
			{
				float angle = ( (float) numLights_Angle.Value ) * (float) Math.PI / 180.0f;
				Vector3 dir;

				dir.X = ( float ) -Math.Cos( angle );
				dir.Y = 0.0f;
				dir.Z = ( float ) -Math.Sin( angle );

				dir.Normalize();
				dir.Y = -1.0f;
				_dx.Device.Lights[0].Direction = new Vector3( dir.X, dir.Y, dir.Z );
				_dx.Device.Lights[0].Update();
			}
		}

		/// <summary>
		/// Initializes the primary light controls to the DirectX settings.
		/// </summary>
		[LuaFunctionAttribute( "InitializeLighting",
			 "Initializes the primary light controls to the DirectX settings." )]
		public void InitializePrimaryLight()
		{
			_updateData = false;
			txtLights_PrimaryR.Text = _dx.Device.Lights[0].Diffuse.R.ToString();
			txtLights_PrimaryG.Text = _dx.Device.Lights[0].Diffuse.G.ToString();
			txtLights_PrimaryB.Text = _dx.Device.Lights[0].Diffuse.B.ToString();
			trkLights_PrimaryR.Value = Convert.ToInt32( _dx.Device.Lights[0].Diffuse.R );
			trkLights_PrimaryG.Value = Convert.ToInt32( _dx.Device.Lights[0].Diffuse.G );
			trkLights_PrimaryB.Value = Convert.ToInt32( _dx.Device.Lights[0].Diffuse.B );
			numLights_Angle.Value = 45;
			_updateData = true;
		}

		/// <summary>
		/// Updates the primary light color.
		/// </summary>
		/// <param name="r">The red tint of the light.</param>
		/// <param name="g">The green tint of the light.</param>
		/// <param name="b">The blue tint of the light.</param>
		[LuaFunctionAttribute( "SetLightingColor",
			 "Updates the primary light color.", "The red tint of the light.",
			 "The green tint of the light.", "The blue tint of the light." )]
		public void UpdatePrimaryLightColor( int r, int g, int b )
		{
			trkLights_PrimaryR.Value = r;
			trkLights_PrimaryG.Value = g;
			trkLights_PrimaryB.Value = b;
			txtLights_PrimaryR.Text = r.ToString();
			txtLights_PrimaryG.Text = g.ToString();
			txtLights_PrimaryB.Text = b.ToString();

			UpdatePrimaryLightColor();
		}

		/// <summary>
		/// Updates the direction of the primary light.
		/// </summary>
		/// <param name="angle">The angle of the light.</param>
		[LuaFunctionAttribute( "SetLightingAngle",
			 "Updates the direction of the primary light.", "The angle of the light." )]
		public void UpdatePrimaryLightAngle( int angle )
		{
			numLights_Angle.Value = angle;
			UpdatePrimaryLightAngle();
		}

		/// <summary>
		/// Sets the base color of the terrain to the lighting color.
		/// </summary>
		[LuaFunctionAttribute( "ColorByLighting", "Sets the base color of the terrain to the lighting color." )]
		public void ColorTerrainByLighting()
		{
			_terrainData.BufferObjects.ColorByHeight = false;
			_terrainData.TerrainPage.TerrainPatch.RefreshBuffers = true;

			btnLights_DefaultColor.Enabled = false;
			btnLights_HeightColor.Enabled = true;
		}

		/// <summary>
		/// Sets the base color of the terrain to coloring dependent upon vertex height.
		/// </summary>
		[LuaFunctionAttribute( "ColorByHeight",
			 "Sets the base color of the terrain to coloring dependent upon vertex height." )]
		public void ColorTerrainByHeight()
		{
			_terrainData.BufferObjects.ColorByHeight = true;
			_terrainData.TerrainPage.TerrainPatch.RefreshBuffers = true;

			btnLights_DefaultColor.Enabled = true;
			btnLights_HeightColor.Enabled = false;
		}

		/// <summary>
		/// Gets the color of the lighting.
		/// </summary>
		/// <returns>The color of the lighting.</returns>
		[LuaFunctionAttribute( "GetLightingColor", "Gets the color of the lighting." )]
		public Color GetLightColor()
		{
			return Color.FromArgb( trkLights_PrimaryR.Value, trkLights_PrimaryG.Value,
				trkLights_PrimaryB.Value );
		}

		/// <summary>
		/// Gets the angle of the lighting.
		/// </summary>
		/// <returns>The angle of the lighting.</returns>
		[LuaFunctionAttribute( "GetLightingAngle", "Gets the angle of the lighting." )]
		public float GetLightAngle()
		{
			return (float) numLights_Angle.Value;
		}
		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.grpLights_VertexColoring = new System.Windows.Forms.GroupBox();
			this.btnLights_HeightColor = new System.Windows.Forms.Button();
			this.btnLights_DefaultColor = new System.Windows.Forms.Button();
			this.grpLights_Primary = new System.Windows.Forms.GroupBox();
			this.numLights_Angle = new System.Windows.Forms.NumericUpDown();
			this.label13 = new System.Windows.Forms.Label();
			this.txtLights_PrimaryB = new System.Windows.Forms.TextBox();
			this.txtLights_PrimaryG = new System.Windows.Forms.TextBox();
			this.txtLights_PrimaryR = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.trkLights_PrimaryB = new System.Windows.Forms.TrackBar();
			this.trkLights_PrimaryG = new System.Windows.Forms.TrackBar();
			this.trkLights_PrimaryR = new System.Windows.Forms.TrackBar();
			this.grpLights_VertexColoring.SuspendLayout();
			this.grpLights_Primary.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numLights_Angle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkLights_PrimaryB)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkLights_PrimaryG)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trkLights_PrimaryR)).BeginInit();
			this.SuspendLayout();
			// 
			// grpLights_VertexColoring
			// 
			this.grpLights_VertexColoring.Controls.Add(this.btnLights_HeightColor);
			this.grpLights_VertexColoring.Controls.Add(this.btnLights_DefaultColor);
			this.grpLights_VertexColoring.Location = new System.Drawing.Point(8, 160);
			this.grpLights_VertexColoring.Name = "grpLights_VertexColoring";
			this.grpLights_VertexColoring.Size = new System.Drawing.Size(160, 80);
			this.grpLights_VertexColoring.TabIndex = 3;
			this.grpLights_VertexColoring.TabStop = false;
			this.grpLights_VertexColoring.Text = "Vertex Coloring";
			// 
			// btnLights_HeightColor
			// 
			this.btnLights_HeightColor.Location = new System.Drawing.Point(8, 48);
			this.btnLights_HeightColor.Name = "btnLights_HeightColor";
			this.btnLights_HeightColor.Size = new System.Drawing.Size(144, 23);
			this.btnLights_HeightColor.TabIndex = 1;
			this.btnLights_HeightColor.Text = "Color by Vertex Height";
			this.btnLights_HeightColor.Click += new System.EventHandler(this.btnLights_HeightColor_Click);
			// 
			// btnLights_DefaultColor
			// 
			this.btnLights_DefaultColor.Enabled = false;
			this.btnLights_DefaultColor.Location = new System.Drawing.Point(8, 16);
			this.btnLights_DefaultColor.Name = "btnLights_DefaultColor";
			this.btnLights_DefaultColor.Size = new System.Drawing.Size(144, 23);
			this.btnLights_DefaultColor.TabIndex = 0;
			this.btnLights_DefaultColor.Text = "Default Coloring";
			this.btnLights_DefaultColor.Click += new System.EventHandler(this.btnLights_DefaultColor_Click);
			// 
			// grpLights_Primary
			// 
			this.grpLights_Primary.Controls.Add(this.numLights_Angle);
			this.grpLights_Primary.Controls.Add(this.label13);
			this.grpLights_Primary.Controls.Add(this.txtLights_PrimaryB);
			this.grpLights_Primary.Controls.Add(this.txtLights_PrimaryG);
			this.grpLights_Primary.Controls.Add(this.txtLights_PrimaryR);
			this.grpLights_Primary.Controls.Add(this.label12);
			this.grpLights_Primary.Controls.Add(this.label11);
			this.grpLights_Primary.Controls.Add(this.label10);
			this.grpLights_Primary.Controls.Add(this.trkLights_PrimaryB);
			this.grpLights_Primary.Controls.Add(this.trkLights_PrimaryG);
			this.grpLights_Primary.Controls.Add(this.trkLights_PrimaryR);
			this.grpLights_Primary.Location = new System.Drawing.Point(8, 8);
			this.grpLights_Primary.Name = "grpLights_Primary";
			this.grpLights_Primary.Size = new System.Drawing.Size(160, 144);
			this.grpLights_Primary.TabIndex = 2;
			this.grpLights_Primary.TabStop = false;
			this.grpLights_Primary.Text = "Adjust Primary Light";
			// 
			// numLights_Angle
			// 
			this.numLights_Angle.Location = new System.Drawing.Point(96, 112);
			this.numLights_Angle.Maximum = new System.Decimal(new int[] {
																			360,
																			0,
																			0,
																			0});
			this.numLights_Angle.Minimum = new System.Decimal(new int[] {
																			1,
																			0,
																			0,
																			-2147483648});
			this.numLights_Angle.Name = "numLights_Angle";
			this.numLights_Angle.Size = new System.Drawing.Size(56, 20);
			this.numLights_Angle.TabIndex = 10;
			this.numLights_Angle.ValueChanged += new System.EventHandler(this.numLights_Angle_ValueChanged);
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(8, 112);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(88, 16);
			this.label13.TabIndex = 9;
			this.label13.Text = "Direction Angle:";
			// 
			// txtLights_PrimaryB
			// 
			this.txtLights_PrimaryB.Location = new System.Drawing.Point(120, 80);
			this.txtLights_PrimaryB.Name = "txtLights_PrimaryB";
			this.txtLights_PrimaryB.Size = new System.Drawing.Size(32, 20);
			this.txtLights_PrimaryB.TabIndex = 8;
			this.txtLights_PrimaryB.Text = "";
			this.txtLights_PrimaryB.TextChanged += new System.EventHandler(this.txtLights_PrimaryB_TextChanged);
			// 
			// txtLights_PrimaryG
			// 
			this.txtLights_PrimaryG.Location = new System.Drawing.Point(120, 48);
			this.txtLights_PrimaryG.Name = "txtLights_PrimaryG";
			this.txtLights_PrimaryG.Size = new System.Drawing.Size(32, 20);
			this.txtLights_PrimaryG.TabIndex = 7;
			this.txtLights_PrimaryG.Text = "";
			this.txtLights_PrimaryG.TextChanged += new System.EventHandler(this.txtLights_PrimaryG_TextChanged);
			// 
			// txtLights_PrimaryR
			// 
			this.txtLights_PrimaryR.Location = new System.Drawing.Point(120, 16);
			this.txtLights_PrimaryR.Name = "txtLights_PrimaryR";
			this.txtLights_PrimaryR.Size = new System.Drawing.Size(32, 20);
			this.txtLights_PrimaryR.TabIndex = 6;
			this.txtLights_PrimaryR.Text = "";
			this.txtLights_PrimaryR.TextChanged += new System.EventHandler(this.txtLights_PrimaryR_TextChanged);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(8, 88);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(40, 16);
			this.label12.TabIndex = 2;
			this.label12.Text = "Blue:";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(8, 56);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(40, 16);
			this.label11.TabIndex = 1;
			this.label11.Text = "Green:";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(8, 24);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(40, 16);
			this.label10.TabIndex = 0;
			this.label10.Text = "Red:";
			// 
			// trkLights_PrimaryB
			// 
			this.trkLights_PrimaryB.Location = new System.Drawing.Point(40, 80);
			this.trkLights_PrimaryB.Maximum = 255;
			this.trkLights_PrimaryB.Name = "trkLights_PrimaryB";
			this.trkLights_PrimaryB.Size = new System.Drawing.Size(80, 45);
			this.trkLights_PrimaryB.TabIndex = 5;
			this.trkLights_PrimaryB.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trkLights_PrimaryB.Scroll += new System.EventHandler(this.trkLights_PrimaryB_Scroll);
			// 
			// trkLights_PrimaryG
			// 
			this.trkLights_PrimaryG.Location = new System.Drawing.Point(40, 48);
			this.trkLights_PrimaryG.Maximum = 255;
			this.trkLights_PrimaryG.Name = "trkLights_PrimaryG";
			this.trkLights_PrimaryG.Size = new System.Drawing.Size(80, 45);
			this.trkLights_PrimaryG.TabIndex = 4;
			this.trkLights_PrimaryG.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trkLights_PrimaryG.Scroll += new System.EventHandler(this.trkLights_PrimaryG_Scroll);
			// 
			// trkLights_PrimaryR
			// 
			this.trkLights_PrimaryR.Location = new System.Drawing.Point(40, 16);
			this.trkLights_PrimaryR.Maximum = 255;
			this.trkLights_PrimaryR.Name = "trkLights_PrimaryR";
			this.trkLights_PrimaryR.Size = new System.Drawing.Size(80, 45);
			this.trkLights_PrimaryR.TabIndex = 3;
			this.trkLights_PrimaryR.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trkLights_PrimaryR.Scroll += new System.EventHandler(this.trkLights_PrimaryR_Scroll);
			// 
			// LightManipulation
			// 
			this.Controls.Add(this.grpLights_VertexColoring);
			this.Controls.Add(this.grpLights_Primary);
			this.Name = "LightManipulation";
			this.Size = new System.Drawing.Size(176, 248);
			this.grpLights_VertexColoring.ResumeLayout(false);
			this.grpLights_Primary.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numLights_Angle)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkLights_PrimaryB)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkLights_PrimaryG)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trkLights_PrimaryR)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
