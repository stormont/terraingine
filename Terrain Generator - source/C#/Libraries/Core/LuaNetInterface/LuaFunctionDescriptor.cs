using System;
using System.Collections;

namespace Voyage.LuaNetInterface
{
	/// <summary>
	/// A class for describing a Lua function.
	/// </summary>
	public class LuaFunctionDescriptor
	{
		#region Data Members
		private string _functionName;					// Name of the function
		private string _functionDocumentation;			// Documentation for the function
		private ArrayList _functionParameters;			// The list of function parameters
		private ArrayList _functionParamDocumentation;	// Documentation for the list of function parameters
		private string _functionDocumentationString;	// The function documentation string
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of the function.
		/// </summary>
		public string FunctionName
		{
			get { return _functionName; }
		}

		/// <summary>
		/// Gets the documentation for the function.
		/// </summary>
		public string FunctionDocumentation
		{
			get { return _functionDocumentation; }
		}

		/// <summary>
		/// Gets the list of function parameters.
		/// </summary>
		public ArrayList FunctionParameters
		{
			get { return _functionParameters; }
		}

		/// <summary>
		/// Gets the list of documentation for the parameters.
		/// </summary>
		public ArrayList FunctionParamDocumentation
		{
			get { return _functionParamDocumentation; }
		}

		/// <summary>
		/// Gets the first line of the function header.
		/// </summary>
		public string FunctionHeader
		{
			get
			{
				if ( _functionDocumentationString.IndexOf( "\n" ) == -1 )
					return _functionDocumentationString;

				return _functionDocumentationString.Substring( 0, _functionDocumentationString.IndexOf( "\n" ) );
			}
		}

		/// <summary>
		/// Gets the full function header.
		/// </summary>
		public string FunctionFullDocumentation
		{
			get { return _functionDocumentationString; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates a LuaFunctionDescriptor for a descriptor of a function with a list of parameters.
		/// </summary>
		/// <param name="functionName">The name of the function.</param>
		/// <param name="functionDocumentation">The documentation of the function.</param>
		/// <param name="parameterList">The list of function parameters.</param>
		/// <param name="parameterDocumentation">The list of parameter documentation.</param>
		public LuaFunctionDescriptor( string functionName, string functionDocumentation,
			ArrayList parameterList, ArrayList parameterDocumentation )
		{
			string funcHeader = functionName + "(%params%) - " + functionDocumentation;
			string funcBody = "\n\n";
			string funcParams = "";
			bool first = true;

			_functionName = functionName;
			_functionDocumentation = functionDocumentation;
			_functionParameters = parameterList;
			_functionParamDocumentation = parameterDocumentation;

			// Build the function documentation string
			for ( int i = 0; i < parameterList.Count; i++ )
			{
				if ( !first )
					funcParams += ", ";

				funcParams += parameterList[i];
				funcBody += "\t" + parameterList[i] + "\t\t" + parameterDocumentation[i] + "\n";

				first = false;
			}

			funcBody = funcBody.Substring( 0, funcBody.Length - 1 );

			if ( first )
				funcBody = funcBody.Substring( 0, funcBody.Length - 1 );

			_functionDocumentationString = funcHeader.Replace( "%params%", funcParams ) + funcBody;
		}
		#endregion
	}
}
