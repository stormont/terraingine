using System;
using LuaInterface;

namespace Voyage.LuaNetInterface
{
	/// <summary>
	/// A class for containing Lua function attributes.
	/// </summary>
	public class LuaFunctionAttribute : Attribute
	{
		#region Data Members
		private string _functionName;					// Name of the function
		private string _functionDocumentation;			// Documentation of the function
		private string[] _functionParameters = null;	// Documentation for the parameters
		#endregion

		#region Properties
		/// <summary>
		/// Gets the function name.
		/// </summary>
		public string FunctionName
		{
			get { return _functionName; }
		}

		/// <summary>
		/// Gets the function documentation.
		/// </summary>
		public string FunctionDocumentation
		{
			get { return _functionDocumentation; }
		}

		/// <summary>
		/// Gets the function parameters.
		/// </summary>
		public string[] FunctionParameters
		{
			get { return _functionParameters; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates a LuaFunctionAttribute for a function with a list of parameters.
		/// </summary>
		/// <param name="functionName">The name of the function.</param>
		/// <param name="functionDocumentation">The documentation of the function.</param>
		/// <param name="parameterDocumentation">The list of function parameters.</param>
		public LuaFunctionAttribute( string functionName, string functionDocumentation,
			params string[] parameterDocumentation )
		{
			_functionName = functionName;
			_functionDocumentation = functionDocumentation;
			_functionParameters = parameterDocumentation;
		}

		/// <summary>
		/// Creates a LuaFunctionAttribute for a function with a list of parameters.
		/// </summary>
		/// <param name="functionName">The name of the function.</param>
		/// <param name="functionDocumentation">The documentation of the function.</param>
		public LuaFunctionAttribute( string functionName, string functionDocumentation )
		{
			_functionName = functionName;
			_functionDocumentation = functionDocumentation;
		}
		#endregion
	}
}
