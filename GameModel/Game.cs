using System;
using static GameBoard;
public class Game : IGame
{
	#region Fields and Properties

	private readonly IGameMemento _memento;

	private GameBoard gameBoard;
	public int BoardWidth => gameBoard.Columns;
	public int BoardHeight => gameBoard.Rows;
	public event Action BoardUpdated;
	public enum Player
	{
		Chaos,  // Хаос
		Order   // Порядок
	}
	public Player CurrentPlayer { get; private set; } // Поточний гравець

	public void SwitchPlayer()
	{
		CurrentPlayer = (CurrentPlayer == Player.Chaos) ? Player.Order : Player.Chaos;

	}


	public int Rows => gameBoard.Rows;
	public int Columns => gameBoard.Columns;

	#endregion

	#region Events

	public event Action<Player> GameWon; // Подія з передачею Player як переможця

	#endregion

	#region Constructor

	public Game()
	{
		int rows = 6;
		int columns = 6;
		_memento = new GameMemento(new GameBoard(rows, columns));
		gameBoard = _memento.SavedGameBoard;
		BoardUpdated?.Invoke();
	}

	#endregion

	#region Public Methods

	public void StartNewGame()
	{
		gameBoard.ResetBoard();
		CurrentPlayer = Player.Order;
		BoardUpdated?.Invoke();
		// Додаткова ініціалізація за потреби
	}
	public void UndoLastMove()
	{
		// Викликаємо метод UndoLastMove() з GameMemento
		_memento.UndoLastMove();

		// Оновлюємо gameBoard до останнього збереженого стану з _memento
		gameBoard = _memento.SavedGameBoard;

		// Переключаємо гравця назад, якщо це необхідно
		SwitchPlayer();
		BoardUpdated?.Invoke();
		// Можливо, вам потрібно оновити інтерфейс або зробити інші дії після відміни ходу
		// Наприклад, сповістити ViewModel про зміну стану гри
	}

	
	private void ClearMementoStack()
	{
		_memento.ClearSavedStates();
	}
	public void MakeMoveCross(int x, int y)
	{
		gameBoard.SetValueAt(x, y, GameBoard.GameElement.Cross);
		SaveCurrentGameState();
		var result = gameBoard.CheckForWin(GameBoard.GameElement.Cross);
		HandleMoveResult(gameBoard.CheckForWin(GameBoard.GameElement.Cross));
		SwitchPlayer();
		if (result == GameResult.OrderWin)
		{
			GameWon?.Invoke(Player.Order);
			ClearMementoStack();
		}
		else if (gameBoard.IsBoardFull())
		{
			GameWon?.Invoke(Player.Chaos);
			ClearMementoStack();
		}

		BoardUpdated?.Invoke();
	}

	public void MakeMoveCircle(int x, int y)
	{
		gameBoard.SetValueAt(x, y, GameBoard.GameElement.Circle);
		SaveCurrentGameState();
		var result = gameBoard.CheckForWin(GameBoard.GameElement.Circle);
		HandleMoveResult(gameBoard.CheckForWin(GameBoard.GameElement.Circle));
		SwitchPlayer();
		if (result == GameResult.OrderWin)
		{
			GameWon?.Invoke(Player.Order);
			ClearMementoStack();
		}
		else if (gameBoard.IsBoardFull())
		{
			GameWon?.Invoke(Player.Chaos);
			ClearMementoStack();
		}

		BoardUpdated?.Invoke();
	}

	private void SaveCurrentGameState()
	{
		_memento.SaveCurrentGameState(gameBoard);
	}



	public void AddNode(GameBoard.Node newNode)
	{
		gameBoard.AddNode(newNode);
	}

	public void RestoreFromMemento(IGameMemento memento)
	{
		if (memento == null)
			throw new ArgumentNullException(nameof(memento));

		gameBoard = memento.SavedGameBoard;
		BoardUpdated?.Invoke();
	}

	#endregion

	#region Private Methods
	

	private void HandleMoveResult(GameResult result)
	{
		if (result == GameResult.OrderWin)
		{
			GameWon?.Invoke(CurrentPlayer); // Порядок перемагає
		}
	}


	protected virtual void OnGameWon(Player playerValue)
	{
		GameWon?.Invoke(playerValue);
	}

	#endregion

	#region Interface Implementations

	public GameBoard.GameElement GetValueAt(int x, int y)
	{
		return gameBoard.GetValueAt(x, y);
	}

	public string GetBoardAsString()
	{
		return gameBoard.ToString();
	}

	public Player GetCurrentPlayer()
	{
		return CurrentPlayer;
	}

	public GameBoard GetBoard()
	{
		return gameBoard;
	}

	public GameMemento CreateMemento()
	{
		return new GameMemento(gameBoard);
	}

	#endregion
}
