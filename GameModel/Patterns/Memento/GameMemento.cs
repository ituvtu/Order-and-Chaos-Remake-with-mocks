using System.Collections.Generic;

public class GameMemento : IGameMemento
{
	#region Fields and Properties
	public GameBoard SavedGameBoard { get; set; }
	private readonly Stack<GameBoard> savedGameBoards = new Stack<GameBoard>();
	#endregion

	#region Public Methods
	public GameMemento(GameBoard gameBoard)
	{
		// Копіюємо стан гри для збереження
		SavedGameBoard = new GameBoard(gameBoard.Rows, gameBoard.Columns);
		for (int i = 0; i < gameBoard.Rows; i++)
		{
			for (int j = 0; j < gameBoard.Columns; j++)
			{
				SavedGameBoard.SetValueAt(i, j, gameBoard.GetValueAt(i, j));
			}
		}

		// Зберігаємо стан гри в стек

		savedGameBoards.Push(new GameBoard(SavedGameBoard.Rows, SavedGameBoard.Columns));

	}

	// Метод для збереження поточного стану гри у стеку
	public void SaveCurrentGameState()
	{
		// Зберігаємо стан гри в стек
		savedGameBoards.Push(new GameBoard(SavedGameBoard.Rows, SavedGameBoard.Columns));

	}
	public void ClearSavedStates()
	{
		savedGameBoards.Clear();
	}
	// Метод для скасування останнього кроку та відновлення попереднього стану гри
	public void UndoLastMove()
	{
		if (savedGameBoards.Count > 1)
		{
			
			savedGameBoards.Pop(); // Видаляємо останнє збережене поле
			SavedGameBoard = savedGameBoards.Peek(); // Відновлюємо попередній стан гри

		}
	}

	public void SaveCurrentGameState(GameBoard currentBoard)
	{
		// Зберігаємо копію поточного стану гри
		savedGameBoards.Push(new GameBoard(currentBoard.Rows, currentBoard.Columns));
		for (int i = 0; i < currentBoard.Rows; i++)
		{
			for (int j = 0; j < currentBoard.Columns; j++)
			{
				savedGameBoards.Peek().SetValueAt(i, j, currentBoard.GetValueAt(i, j));
			}
		}
	}

	#endregion
}
