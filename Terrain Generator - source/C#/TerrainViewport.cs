using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Voyage.Terraingine.DataInterfacing;

namespace Voyage.Terraingine
{
	/// <summary>
	/// Summary description for TerrainViewport.
	/// </summary>
	public class TerrainViewport : System.Windows.Forms.Form
	{
		#region Data Members
		/// <summary>
		/// Viewport used for rendering.
		/// </summary>
		protected Voyage.Terraingine.DataInterfacing.ViewportInterface _viewport;

		private System.ComponentModel.Container components = null;

		#endregion

		#region Properties
		/// <summary>
		/// Gets if the application has any messages left to process.
		/// </summary>
		protected virtual bool AppStillIdle
		{
			get
			{
				NativeMethods.Message msg;
				return !NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
			}
		}

		/// <summary>
		/// Gets the main viewport DirectX interface.
		/// </summary>
		public virtual DataInterfacing.ViewportInterface MainViewport
		{
			get { return _viewport; }
		}
		#endregion

		#region Basic Form Methods
		/// <summary>
		/// Creates a Form of type TerrainViewport.
		/// </summary>
		public TerrainViewport()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_viewport.CreateViewport( this );
			_viewport.InitializeViewport_Default();
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

		#region Other Methods
		/// <summary>
		/// Runs further processing when the application is idle.
		/// </summary>
		public virtual void OnApplicationIdle(object sender, EventArgs e)
		{
			// Render frames during idle time (no messages are waiting)
			while ( AppStillIdle )
			{
				if ( _viewport.DXViewport.IsTimeToRender() && _viewport.BeginRender() )
				{
					_viewport.PreRender();
					_viewport.RenderSceneElements();
					_viewport.EndRender();
				}
			}
		}

		/// <summary>
		/// Gets the center point of the form.
		/// </summary>
		/// <returns>The center of the form.</returns>
		public Point GetFormCenter()
		{
			Rectangle r = this.Bounds;

			return new Point( r.X + r.Width / 2, r.Y + r.Height / 2 );
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._viewport = new Voyage.Terraingine.DataInterfacing.ViewportInterface();
			this.SuspendLayout();
			// 
			// _viewport
			// 
			this._viewport.Location = new System.Drawing.Point(0, 0);
			this._viewport.Name = "_viewport";
			this._viewport.Owner = null;
			this._viewport.Size = new System.Drawing.Size(120, 120);
			this._viewport.TabIndex = 0;
			this._viewport.TerrainData = null;
			// 
			// TerrainViewport
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(208, 182);
			this.Controls.Add(this._viewport);
			this.Name = "TerrainViewport";
			this.Text = "Terrain Viewport";
			this.ResumeLayout(false);

		}
		#endregion    
	}
}
