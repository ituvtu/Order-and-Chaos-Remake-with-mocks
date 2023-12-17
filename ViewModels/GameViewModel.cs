using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ViewModelGame
{
	public class GameViewModel : INotifyPropertyChanged
	{
		public Game Game { get; private set; }

		public ObservableCollection<UIElement> Shapes { get; private set; }
		public string CurrentPlayerDisplay
		{
			get
			{
				return "Поточний гравець: " + Game.CurrentPlayer.ToString();
			}
		}
		public ICommand MakeMoveCircleCommand { get; private set; }
		public ICommand MakeMoveCrossCommand { get; private set; }
		public ICommand UpdateUICommand { get; private set; }
		// Команда для скасування кроку
		public ICommand UndoMoveCommand { get; private set; }

		// Команда для розпочатку нової гри
		public ICommand StartNewGameCommand { get; private set; }
		public GameViewModel()
		{
			Game = new Game();
			Shapes = new ObservableCollection<UIElement>();
			Game.BoardUpdated += OnBoardUpdated;
			MakeMoveCircleCommand = new RelayCommandWithParameters(MakeMoveCircle);
			MakeMoveCrossCommand = new RelayCommandWithParameters(MakeMoveCross);
			UpdateUICommand = new RelayCommand(UpdateUI);
			Game.GameWon += OnGameWon;
			UndoMoveCommand = new RelayCommand(UndoMove);
			StartNewGameCommand = new RelayCommand(StartNewGame);
			DrawGrid();
		}
		private void UndoMove()
		{
			// Логіка скасування кроку
			Game.UndoLastMove();
			OnPropertyChanged(nameof(CurrentPlayerDisplay));
		}

		private void StartNewGame()
		{
			// Логіка розпочатку нової гри
			Game.StartNewGame();
			OnPropertyChanged(nameof(CurrentPlayerDisplay));
		}
		private void UpdateUI()
		{
			// Логіка оновлення UI
			OnPropertyChanged(nameof(Shapes));
		}
		public void OnBoardUpdated()
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				Shapes.Clear();
				DrawGrid();
				for (int x = 0; x < Game.BoardWidth; x++)
				{
					for (int y = 0; y < Game.BoardHeight; y++)
					{
						var element = Game.GetBoard().GetValueAt(x, y);
						if (element != GameBoard.GameElement.Empty)
						{
							var shape = CreateShapeForElement(element, x, y);
							Shapes.Add(shape);
						}
					}
				}
			});

		}

		// Створює фігуру в залежності від елемента на полі
		private UIElement CreateShapeForElement(GameBoard.GameElement element, int x, int y)
		{
			switch (element)
			{
				case GameBoard.GameElement.Cross:
					return CreateCrossShape(x, y);
				case GameBoard.GameElement.Circle:
					return CreateCircleShape(x, y);
				default:
					return null;
			}
		}
		private void MakeMoveCircle(object parameter)
		{
			MakeMove(parameter, GameBoard.GameElement.Circle);
			OnPropertyChanged(nameof(CurrentPlayerDisplay));
		}
		private void DrawGrid()
		{
			int numCells = 6; // або інша кількість, що відповідає вашій грі
			double cellSize = 50; // замініть на розмір клітини, який ви використовуєте

			for (int i = 0; i <= numCells; i++)
			{
				// Горизонтальні лінії
				Line horizontalLine = new Line
				{
					X1 = 0,
					Y1 = i * cellSize,
					X2 = numCells * cellSize,
					Y2 = i * cellSize,
					Stroke = Brushes.Black,
					StrokeThickness = 1
				};
				Shapes.Add(horizontalLine);

				// Вертикальні лінії
				Line verticalLine = new Line
				{
					X1 = i * cellSize,
					Y1 = 0,
					X2 = i * cellSize,
					Y2 = numCells * cellSize,
					Stroke = Brushes.Black,
					StrokeThickness = 1
				};
				Shapes.Add(verticalLine);

			}
		}
		private void OnGameWon(Game.Player winner)
		{
			// Відображення повідомлення залежно від переможця
			string message = winner == Game.Player.Chaos ? "Перемога Хаосу!" : "Перемога Порядку!";
			MessageBox.Show(message, "Гра закінчена");
			StartNewGame();
		}

		private void MakeMoveCross(object parameter)
		{
			MakeMove(parameter, GameBoard.GameElement.Cross);
			OnPropertyChanged(nameof(CurrentPlayerDisplay));
		}

		private void MakeMove(object parameter, GameBoard.GameElement element)
		{
			var point = parameter as Point?;
			if (point == null) return;

			int x = (int)point.Value.X;
			int y = (int)point.Value.Y;

			if (Game.GetBoard().GetValueAt(x, y) == GameBoard.GameElement.Empty)
			{
				if (element == GameBoard.GameElement.Circle)
				{
					Game.MakeMoveCircle(x, y);
				}
				else
				{
					Game.MakeMoveCross(x, y);
				}


				// Оновити ігрове поле на інтерфейсі
				UpdateCanvas(x, y, element);
			}
		}

		private void UpdateCanvas(int x, int y, GameBoard.GameElement element)
		{
			UIElement shape = (element == GameBoard.GameElement.Cross) ? CreateCrossShape(x, y) : CreateCircleShape(x, y);

			Shapes.Add(shape);
			OnPropertyChanged(nameof(Shapes));
			OnBoardUpdated();
		}

		private UIElement CreateCrossShape(int x, int y)
		{
			int cellWidth = 50, cellHeight = 50;
			// Створення хрестика як комбінації двох ліній
			var cross = new Canvas
			{
				Width = cellWidth,
				Height = cellHeight
			};

			var line1 = new Line
			{
				X1 = 0,
				Y1 = 0,
				X2 = cellWidth,
				Y2 = cellHeight,
				Stroke = Brushes.Black,
				StrokeThickness = 2
			};

			var line2 = new Line
			{
				X1 = cellWidth,
				Y1 = 0,
				X2 = 0,
				Y2 = cellHeight,
				Stroke = Brushes.Black,
				StrokeThickness = 2
			};

			cross.Children.Add(line1);
			cross.Children.Add(line2);

			Canvas.SetLeft(cross, x * cellWidth);
			Canvas.SetTop(cross, y * cellHeight);

			return cross;
		}

		private UIElement CreateCircleShape(int x, int y)
		{
			int cellWidth = 50, cellHeight = 50;
			// Створення кола
			var circle = new Ellipse
			{
				Width = cellWidth,
				Height = cellHeight,
				Stroke = Brushes.Black,
				Fill = Brushes.Transparent,
				StrokeThickness = 2
			};

			Canvas.SetLeft(circle, x * cellWidth);
			Canvas.SetTop(circle, y * cellHeight);

			return circle;
		}

		// Імплементація INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			OnBoardUpdated();
		}
	}



	public class RelayCommand : ICommand
	{
		private readonly Action _execute;
		private readonly Func<bool> _canExecute;

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public RelayCommand(Action execute, Func<bool> canExecute = null)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute();
		}

		public void Execute(object parameter)
		{
			_execute();
		}
	}
	public class RelayCommandWithParameters : ICommand
	{
		private readonly Action<object> _execute;
		private readonly Func<object, bool> _canExecute;

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public RelayCommandWithParameters(Action<object> execute, Func<object, bool> canExecute = null)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}
	}
}