using System;
public class BoardIterator : IBoardIterator
{
	#region Fields and Properties

	private readonly GameBoard gameBoard;
	private int currentIndex = -1;
	public object Current
	{
		get
		{
			if (currentIndex == -1 || currentIndex >= gameBoard.Rows * gameBoard.Columns)
				throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");

			int row = currentIndex / gameBoard.Columns;
			int col = currentIndex % gameBoard.Columns;

			return gameBoard.GetValueAt(row, col);
		}
	}
	public int CurrentX
	{
		get
		{
			return currentIndex / gameBoard.Columns;
		}
	}

	public int CurrentY
	{
		get
		{
			return currentIndex % gameBoard.Columns;
		}
	}
	#endregion

	#region Constructors
	public BoardIterator(GameBoard board)
	{
		gameBoard = board ?? throw new ArgumentNullException(nameof(board));
	}
	#endregion

	#region Public Methods
	public bool MoveNext()
	{
		currentIndex++;
		return currentIndex < gameBoard.Rows * gameBoard.Columns;
	}

	public void Reset()
	{
		currentIndex = -1;
	}
	#endregion

}
