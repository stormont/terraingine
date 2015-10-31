using System;
using System.Collections;
using Voyage.Terraingine.DataCore;

namespace Voyage.Terraingine.DataInterfacing
{
	/// <summary>
	/// Summary description for DataHistory.
	/// </summary>
	public class DataHistory
	{
		#region Data Members
		private Stack		_pageHistory;
		private Stack		_pageAction;
		private int			_maxPages;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the maximum size of the TerrainPage history stack.
		/// </summary>
		public int MaximumPageHistory
		{
			get { return _maxPages; }
			set
			{
				if ( value != _maxPages )
				{
					if ( value < _maxPages )
						CopyPageHistory( value );

					_maxPages = value;
				}
			}
		}

		/// <summary>
		/// Gets the descriptive actions for the TerrainPage history stack.
		/// </summary>
		public Stack PageHistoryActions
		{
			get { return _pageAction; }
		}

		/// <summary>
		/// Gets the size of the TerrainPage history stack.
		/// </summary>
		public int PageHistoryCount
		{
			get { return _pageHistory.Count; }
		}
		#endregion

		#region Members
		/// <summary>
		/// Creates an object for storing data history.
		/// </summary>
		public DataHistory()
		{
			_maxPages = 30;
			_pageHistory = new Stack( _maxPages );
			_pageAction = new Stack( _maxPages );
		}

		/// <summary>
		/// Pushes the specified TerrainPage onto the history stack.  If the maximum
		/// stack size has been reached, only the latest additions are kept.
		/// </summary>
		/// <param name="page">The TerrainPage to push onto the history stack.</param>
		/// <param name="action">The description for the action causing the TerrainPage change.</param>
		public void PushPage( TerrainPage page, string action )
		{
			if ( _pageHistory.Count < _maxPages )
			{
				// Push the latest TerrainPage onto the history stack
				_pageHistory.Push( page );
				_pageAction.Push( action );
			}
			else
			{
				// Maximum stack size has been reached.
				// Only store the latest TerrainPages in the history stack.
				CopyPageHistory( _maxPages - 1 );
				_pageHistory.Push( page );
				_pageAction.Push( action );
			}
		}

		/// <summary>
		/// Pops the last added TerrainPage from the history stack.
		/// </summary>
		/// <returns>The last added TerrainPage.</returns>
		public TerrainPage PopPage()
		{
			TerrainPage page = null;

			if ( _pageHistory.Count > 0 )
			{
				// Return the latest pushed TerrainPage
				page = ( TerrainPage ) _pageHistory.Pop();
				_pageAction.Pop();
			}

			return page;
		}

		/// <summary>
		/// Copies the specified number of the last TerrainPages pushed onto the history stack.
		/// </summary>
		/// <param name="numLatestPages">The number of TerrainPages to copy.</param>
		private void CopyPageHistory( int numLatestPages )
		{
			// No need to copy history if it will be larger than what already exists
			if ( numLatestPages < _pageHistory.Count )
			{
				Stack history = new Stack( numLatestPages );
				Stack actions = new Stack( numLatestPages );

				for ( int i = 0; i < numLatestPages; i++ )
				{
					history.Push( _pageHistory.Pop() );
					actions.Push( _pageAction.Pop() );
				}

				_pageHistory.Clear();
				_pageAction.Clear();

				for ( int i = 0; i < numLatestPages; i++ )
				{
					_pageHistory.Push( history.Pop() );
					_pageAction.Push( actions.Pop() );
				}
			}
		}

		/// <summary>
		/// Gets the action description of the TerrainPage last added to the history stack.
		/// </summary>
		/// <returns></returns>
		public string LastPageAction()
		{
			string result = null;

			if ( _pageAction.Count > 0 )
				result =( string )  _pageAction.Peek();

			return result;
		}

		/// <summary>
		/// Clears the history stacks.
		/// </summary>
		public void ClearHistory()
		{
			_pageHistory.Clear();
			_pageAction.Clear();
		}
		#endregion
	}
}
