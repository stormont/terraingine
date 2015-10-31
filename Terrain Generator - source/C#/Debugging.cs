using System;
using System.Diagnostics;

namespace Voyage.Terraingine
{
	/// <summary>
	/// Class for performing common debugging tasks.
	/// </summary>
	public class Debugging
	{
		#region Data Members
		private DateTime	_time;
		private string		_source;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the current time held by the Debugging object.
		/// </summary>
		public DateTime Time
		{
			get { return _time; }
			set { _time = value; }
		}

		/// <summary>
		/// Gets or sets the source function holding the Debugging object.
		/// </summary>
		public string Source
		{
			get { return _source; }
			set { _source = value; }
		}
		#endregion

		/// <summary>
		/// Creates an instance of the debugging class.
		/// </summary>
		public Debugging()
		{
			_time = DateTime.Now;
			_source = null;
		}

		/// <summary>
		/// Creates an instance of the debugging class.
		/// </summary>
		/// <param name="source">Source function creating the Debugging object.</param>
		/// <param name="assert">Whether to assert a base value upon creation.</param>
		public Debugging( string source, bool assert )
		{
			_time = DateTime.Now;
			_source = source;

			if ( assert )
				AssertMilliSeconds( DateTime.MinValue );
		}

		/// <summary>
		/// Asserts the currently held time to the debugging output window.
		/// </summary>
		public void AssertTime()
		{
			_time = DateTime.Now;

			Debug.Indent();
			Debug.WriteLine( _source + " Time: " + _time.ToShortTimeString() );
			Debug.Unindent();
		}

		/// <summary>
		/// Asserts the specified time to the debugging output window.
		/// </summary>
		/// <param name="time">Time to assert.</param>
		public void AssertTime( DateTime time )
		{
			Debug.Indent();
			Debug.WriteLine( _source + " Time: " + time.ToShortTimeString() );
			Debug.Unindent();
		}

		/// <summary>
		/// Asserts the currently held time ticks to the debugging output window.
		/// </summary>
		public void AssertTicks()
		{
			DateTime now = DateTime.Now;
			long ticks = now.Ticks - _time.Ticks;

			Debug.Indent();
			Debug.WriteLine( _source + " Ticks: " + ticks.ToString() );
			Debug.Unindent();

			_time = now;
		}

		/// <summary>
		/// Asserts the specified number of time ticks to the debugging output window.
		/// </summary>
		/// <param name="ticks">Number of ticks to assert.</param>
		public void AssertTicks( long ticks )
		{
			Debug.Indent();
			Debug.WriteLine( _source + " Ticks: " + ticks.ToString() );
			Debug.Unindent();
		}

		/// <summary>
		/// Asserts the currently held milliseconds to the debugging output window.
		/// </summary>
		public void AssertMilliSeconds()
		{
			DateTime now = DateTime.Now;
			TimeSpan span = DateTime.Now - _time;
			long ms = span.Milliseconds + span.Seconds * 1000 + span.Minutes * 60000 + span.Hours * 3600000;

			Debug.Indent();
			Debug.WriteLine( _source + " Milliseconds: " + ms.ToString() );
			Debug.Unindent();

			_time = now;
		}

		/// <summary>
		/// Asserts the specified number of milliseconds to the debugging output window.
		/// </summary>
		/// <param name="time">DateTime to assert.</param>
		public void AssertMilliSeconds( DateTime time )
		{
			long ms = time.Millisecond + time.Second * 1000 + time.Minute * 60000 + time.Hour * 3600000;

			Debug.Indent();
			Debug.WriteLine( _source + " Milliseconds: " + ms.ToString() );
			Debug.Unindent();
		}

		/// <summary>
		/// Asserts the specified message to the debugging output window.
		/// </summary>
		/// <param name="message">The message to assert.</param>
		public void AssertMessage( string message )
		{
			Debug.WriteLine( message );
		}
	}
}
