using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine
{
	/// <summary>
	/// Driver class for a plug-in.
	/// </summary>
	public class PlugIn : System.Windows.Forms.Form
	{
		#region Enumerations
		/// <summary>
		/// Enumeration for different types of data desired from the calling program.
		/// </summary>
		public enum DesiredData
		{
			/// <summary>
			/// Plug-in desires lighting data.
			/// </summary>
			Lighting
		}
		#endregion

		#region Data Members
		/// <summary>
		/// List for holding the plug-in's desired data.
		/// </summary>
		protected ArrayList		_desiredData;

		/// <summary>
		/// List for holding the plug-in's received data.
		/// </summary>
		protected ArrayList		_receivedData;

		/// <summary>
		/// TerrainPage of the plug-in.
		/// </summary>
		protected TerrainPage	_page;

		/// <summary>
		/// Owner control of the plug-in.
		/// </summary>
		protected Control		_owner;

		/// <summary>
		/// Name of the plug-in.
		/// </summary>
		protected string		_name;

		/// <summary>
		/// Boolean for holding whether the plug-in was successful.
		/// </summary>
		protected bool			_success;

		/// <summary>
		/// Boolean for holding whether textures were modified by the plug-in.
		/// </summary>
		protected bool			_modifiedTextures;

		/// <summary>
		/// List for holding changed vertices.
		/// </summary>
		protected ArrayList		_textures;
		#endregion

		#region Methods
		/// <summary>
		/// Creates the Driver class.
		/// </summary>
		public PlugIn()
		{
			_name = "Plug-In";
			InitializeBase();
			this.CenterToParent();
		}

		/// <summary>
		/// Initialize the data of the base element.
		/// </summary>
		public virtual void InitializeBase()
		{
			_owner = null;
			_page = null;
			_success = false;
			_modifiedTextures = false;
			_textures = new ArrayList();
			_desiredData = new ArrayList();
			_receivedData = new ArrayList();
		}

		/// <summary>
		/// Sets the TerrainPage used by the plug-in.
		/// </summary>
		/// <param name="page">The TerrainPage to modify.</param>
		public virtual void SetPage( TerrainPage page )
		{
			_page = page;
		}

		/// <summary>
		/// Gets the TerrainPage used by the plug-in.
		/// </summary>
		/// <returns>The modified TerrainPage.</returns>
		public virtual TerrainPage GetPage()
		{
			return _page;
		}

		/// <summary>
		/// Sets the texture filenames used by the plug-in.
		/// </summary>
		/// <param name="textures">The texture filenames to modify.</param>
		public virtual void SetTextures( ArrayList textures )
		{
			_textures = textures;
		}

		/// <summary>
		/// Sets the received data given to the plug-in.
		/// </summary>
		/// <param name="data">The list of received data.</param>
		public virtual void SetReceivedData( ArrayList data )
		{
			_receivedData = data;
		}

		/// <summary>
		/// Sets the Control that uses the plug-in.
		/// </summary>
		/// <param name="owner">The Control that owns the plug-in.</param>
		public virtual void SetOwner( Control owner )
		{
			_owner = owner;
		}

		/// <summary>
		/// Gets the Control that uses the plug-in.
		/// </summary>
		/// <returns>The modified TerrainPage.</returns>
		public virtual Control GetOwner()
		{
			return _owner;
		}

		/// <summary>
		/// Gets the name of the plug-in.
		/// </summary>
		/// <returns>The name of the plug-in.</returns>
		public virtual string GetName()
		{
			return _name;
		}

		/// <summary>
		/// Gets if the plug-in was successfully run.
		/// </summary>
		/// <returns>Returns if the plug-in succeeded.</returns>
		public virtual bool GetSuccess()
		{
			return _success;
		}

		/// <summary>
		/// Gets the textures used by the plug-in.
		/// </summary>
		/// <returns>The modified textures.</returns>
		public virtual ArrayList GetTextures()
		{
			return _textures;
		}

		/// <summary>
		/// Gets the desired data requested by the plug-in.
		/// </summary>
		/// <returns>The list of desired data types.</returns>
		public virtual ArrayList GetDesiredData()
		{
			return _desiredData;
		}

		/// <summary>
		/// Gets if textures were modified by the plug-in.
		/// </summary>
		/// <returns>Whether textures were modified.</returns>
		public virtual bool GetModifiedTextures()
		{
			return _modifiedTextures;
		}

		/// <summary>
		/// Runs the plug-in.
		/// </summary>
		public virtual void Run()
		{
			// Perform the plug-in operations
		}

		/// <summary>
		/// Runs the plug-in as an automatic function.
		/// </summary>
		/// <param name="objects">Additional data sent to the plug-in.</param>
		public virtual void AutoRun( ArrayList objects )
		{
			// Perform the plug-in operations
		}
		#endregion
	}
}
