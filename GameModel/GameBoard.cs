using System;
using System.Text;
public enum GameResult
{
	OrderWin, // Перемога порядку
	ChaosWin, // Перемога хаосу
	NoWin     // Немає переможця (гра продовжується)
}
public class GameBoard
{
	#region Fields
	private Node boardData; // Поле, що буде використовуватись для представлення ігрового поля
	#endregion

	#region Nested Classes
	public enum GameElement
	{
		Empty = 0,
		Cross = 1,
		Circle = 2
	}

	public class Node
	{
		public int X { get; set; }
		public int Y { get; set; }
		public GameElement Value { get; set; } // Тепер використовуємо GameElement
		public Node Next { get; set; }

		public Node(int x, int y, GameElement value)
		{
			X = x;
			Y = y;
			Value = value;
			Next = null;
		}
	}

	#endregion

	#region Constructors
	public GameBoard(int rows, int columns)
	{
		Rows = rows;
		Columns = columns;
		boardData = null; // Початкове ігрове поле пусте
	}
	#endregion

	#region Properties
	public int Rows { get; private set; }
	public int Columns { get; private set; }
	#endregion

	#region Public Methods
	// Отримати значення комірки за її координатами
	public GameElement GetValueAt(int x, int y)
	{
		if (boardData != null)
		{
			Node node = FindNode(x, y);
			if (node != null)
			{
				return node.Value;
			}
		}
		return 0; // Повертаємо 0 для нульових значень або якщо значення не знайдено.
	}

	// Встановити значення комірки за її координатами
	public void SetValueAt(int x, int y, GameElement element)
	{
		if (x < 0 || x >= Rows || y < 0 || y >= Columns)
		{
			throw new IndexOutOfRangeException("Invalid row or column index.");
		}

		if (boardData == null)
		{
			boardData = new Node(x, y, element);
		}
		else
		{
			Node node = FindNode(x, y);
			if (node != null)
			{
				node.Value = element;
			}
			else
			{
				Node newNode = new Node(x, y, element)
				{
					Next = boardData
				};
				boardData = newNode;
			}
		}
	}

	// Додати новий вузол (ход) на гру
	public void AddNode(Node newNode)
	{
		if (newNode.X < 0 || newNode.X >= Rows || newNode.Y < 0 || newNode.Y >= Columns)
		{
			throw new IndexOutOfRangeException("Invalid row or column index.");
		}

		if (boardData != null)
		{
			Node node = FindNode(newNode.X, newNode.Y);
			if (node != null)
			{
				node.Value = newNode.Value;
			}
			else
			{
				newNode.Next = boardData;
				boardData = newNode;
			}
		}
		else
		{
			boardData = new Node(newNode.X, newNode.Y, newNode.Value);
		}
	}

	// Метод для виведення ігрового поля на консоль
	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < Rows; i++)
		{
			for (int j = 0; j < Columns; j++)
			{
				sb.Append(GetValueAt(i, j) + " ");
			}
			sb.AppendLine(); // Перехід на новий рядок для виводу наступного рядка ігрового поля
		}

		return sb.ToString();
	}
	public void ResetBoard()
	{
		// Очистити пов'язаний список, встановивши boardData на null
		boardData = null;

		// Опціонально, якщо вам потрібно зберегти стан ігрового поля до ресету,
		// ви могли б додати логіку тут для збереження стану до ресету, 
		// наприклад, використовуючи мементо або інший механізм.
	}
	
	#endregion

	#region Private Methods
	private Node FindNode(int x, int y)
	{
		Node currentNode = boardData;
		while (currentNode != null)
		{
			if (currentNode.X == x && currentNode.Y == y)
			{
				return currentNode;
			}
			currentNode = currentNode.Next;
		}
		return null;
	}

	public GameResult CheckForWin(GameElement winElement)
	{
		if (CheckVerticalLines(winElement) || CheckHorizontalLines(winElement))
			return GameResult.OrderWin;

		if (IsBoardFull())
			return GameResult.ChaosWin;

		return GameResult.NoWin;
	}

	private bool CheckVerticalLines(GameElement winElement)
	{
		for (int y = 0; y < Columns; y++)
		{
			if (CheckVerticalLine(y, winElement))
				return true;
		}

		return false;
	}

	private bool CheckHorizontalLines(GameElement winElement)
	{
		for (int x = 0; x < Rows; x++)
		{
			if (CheckHorizontalLine(x, winElement))
				return true;
		}

		return false;
	}

	private bool CheckVerticalLine(int y, GameElement winElement)
	{
		IBoardIterator iterator = new BoardIterator(this);

		int consecutiveCount = 0;

		while (iterator.MoveNext())
		{
			int currentX = iterator.CurrentX;

			if (currentX == y)
			{
				if ((GameElement)iterator.Current == winElement)
				{
					consecutiveCount++;
					if (consecutiveCount == 5)
						return true;
				}
				else
				{
					consecutiveCount = 0;
				}
			}
		}

		return false;
	}

	private bool CheckHorizontalLine(int x, GameElement winElement)
	{
		IBoardIterator iterator = new BoardIterator(this);

		int consecutiveCount = 0;

		while (iterator.MoveNext())
		{
			int currentY = iterator.CurrentY;

			if (currentY == x)
			{
				if ((GameElement)iterator.Current == winElement)
				{
					consecutiveCount++;
					if (consecutiveCount == 5)
						return true;
				}
				else
				{
					consecutiveCount = 0;
				}
			}
		}

		return false;
	}



	public bool IsBoardFull()
	{
		IBoardIterator iterator = new BoardIterator(this);
		while (iterator.MoveNext())
		{
			if ((int)iterator.Current == 0)
				return false;
		}

		return true;
	}

	#endregion
}
