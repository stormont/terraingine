using System;
using System.IO;
using Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;

namespace Voyage.Terraingine.DXViewport
{
	/// <summary>
	/// A class to handle creation and usage of DirectX Effects.
	/// </summary>
	public class Effect
	{
		#region Enumerations
		/// <summary>
		/// Enumeration for the allowed effect creation types.
		/// </summary>
		public enum CreationType
		{
			/// <summary>
			/// Type for creating an effect file.
			/// </summary>
			File,
			
			/// <summary>
			/// Type for creating an effect stream.
			/// </summary>
			Stream,
			
			/// <summary>
			/// Type for creating an effect string.
			/// </summary>
			String
		}
		#endregion

		#region Data Members
		private Effect.CreationType	_createType;
		private D3D.Effect		_effect;
		private string			_filename;
		private Stream			_stream;
		private string			_sourceData;
		private D3D.Macro[]		_preprocessorDefines;
		private D3D.Include		_includeFile;
		private D3D.ShaderFlags	_flags;
		private D3D.EffectPool	_pool;
		private string			_errors;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the creation type for the Effect.
		/// </summary>
		public Effect.CreationType CreationMethod
		{
			get { return _createType; }
			set { _createType = value; }
		}

		/// <summary>
		/// Gets or sets the DirectX effect.
		/// </summary>
		public D3D.Effect DXEffect
		{
			get { return _effect; }
			set { _effect = value; }
		}

		/// <summary>
		/// Gets or sets the name of the Effect file.
		/// </summary>
		public string FileName
		{
			get { return _filename; }
			set { _filename = value; }
		}

		/// <summary>
		/// Gets or sets the stream data used for the Effect.
		/// </summary>
		public Stream Stream
		{
			get { return _stream; }
			set { _stream = value; }
		}

		/// <summary>
		/// Gets or sets the source data for the Effect.
		/// </summary>
		public string SourceData
		{
			get { return _sourceData; }
			set { _sourceData = value; }
		}

		/// <summary>
		/// Gets or sets the array of preprocessor macro definitions used during Effect creation.
		/// </summary>
		public D3D.Macro[] PreProcessorDefines
		{
			get { return _preprocessorDefines; }
			set { _preprocessorDefines = value; }
		}

		/// <summary>
		/// Gets or sets the Include object to use for handling #include directives during Effect creation.
		/// </summary>
		public D3D.Include IncludeFile
		{
			get { return _includeFile; }
			set { _includeFile = value; }
		}

		/// <summary>
		/// Gets or sets the shader compilation flags used during Effect creation.
		/// </summary>
		public D3D.ShaderFlags Flags
		{
			get { return _flags; }
			set { _flags = value; }
		}

		/// <summary>
		/// Gets or sets the EffectPool object to use for shared parameters.
		/// </summary>
		public D3D.EffectPool Pool
		{
			get { return _pool; }
			set { _pool = value; }
		}

		/// <summary>
		/// Gets the compilation errors returned during Effect creation.
		/// </summary>
		public string Errors
		{
			get { return _errors; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates the Effect object.
		/// </summary>
		public Effect()
		{
			_effect = null;
			Dispose();
		}

		/// <summary>
		/// Creates a DirectX Effect.
		/// </summary>
		/// <param name="device">The DirectX device that creates the effect.</param>
		/// <returns>Whether the Effect was created.</returns>
		public bool CreateEffect( D3D.Device device )
		{
			bool result = false;

			switch ( _createType )
			{
				case Effect.CreationType.File:
					result = CreateEffectFromFile( device );
					break;

				case Effect.CreationType.Stream:
					result = CreateEffectFromStream( device );
					break;

				case Effect.CreationType.String:
					result = CreateEffectFromString( device );
					break;
			}

			return result;
		}

		/// <summary>
		/// Creates a DirectX Effect from a file.
		/// </summary>
		/// <param name="device">The DirectX device that creates the effect.</param>
		/// <returns>Whether the Effect was created.</returns>
		public bool CreateEffectFromFile( D3D.Device device )
		{
			bool result = false;

			if ( _filename != null )
			{
				_effect = D3D.Effect.FromFile( device, _filename, _preprocessorDefines, _includeFile,
					_flags, _pool, out _errors );

				if ( _effect != null )
					result = true;
			}

			return result;
		}

		/// <summary>
		/// Creates a DirectX Effect from a data stream.
		/// </summary>
		/// <param name="device">The DirectX device that creates the effect.</param>
		/// <returns>Whether the Effect was created.</returns>
		public bool CreateEffectFromStream( D3D.Device device )
		{
			bool result = false;

			if ( _stream != null )
			{
				_effect = D3D.Effect.FromStream( device, _stream, _preprocessorDefines, _includeFile,
					_flags, _pool, out _errors );

				if ( _effect != null )
					result = true;
			}

			return result;
		}

		/// <summary>
		/// Creates a DirectX Effect from a string of source data.
		/// </summary>
		/// <param name="device">The DirectX device that creates the effect.</param>
		/// <returns>Whether the Effect was created.</returns>
		public bool CreateEffectFromString( D3D.Device device )
		{
			bool result = false;

			if ( _sourceData != null )
			{
				_effect = D3D.Effect.FromString( device, _sourceData, _preprocessorDefines, _includeFile,
					_flags, _pool, out _errors );

				if ( _effect != null )
					result = true;
			}

			return result;
		}

		/// <summary>
		/// Checks if the specified technique in the effect is valid.
		/// </summary>
		/// <param name="name">The name of the technique to validate.</param>
		/// <returns>Whether the technique is valid.</returns>
		public bool ValidateTechnique( string name )
		{
			bool result = false;
			D3D.EffectHandle technique = _effect.GetTechnique( name );

			if ( technique != null )
				result = _effect.IsTechniqueValid( technique );

			return result;
		}

		/// <summary>
		/// Disposes of the Effect object.
		/// </summary>
		public void Dispose()
		{
			if ( _effect != null )
			{
				_effect.Dispose();
				_effect = null;
			}

			_createType = Effect.CreationType.File;
			_filename = null;
			_stream = null;
			_sourceData = null;
			_preprocessorDefines = null;
			_includeFile = null;
			_flags = D3D.ShaderFlags.None;
			_pool = null;
			_errors = null;
		}
		#endregion
	}
}
