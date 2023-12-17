using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ViewModelGame;

namespace OrderAndChaosGame
{
	public partial class MainWindow : Window
	{

		public MainWindow()
		{
			InitializeComponent();
			DataContext = new GameViewModel();
			this.UpdateUI();
		}


		private void GameCanvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			// Отримати позицію курсору миші на канвасі
			var point = e.GetPosition((Canvas)sender);

			// Обчислити координати клітинки на основі позиції курсору
			int x = (int)(point.X / 50); // Припускаючи, що ширина клітинки - 50
			int y = (int)(point.Y / 50); // Припускаючи, що висота клітинки - 50

			// Отримати ViewModel
			if (DataContext is GameViewModel viewModel)
			{
				// Створити точку з координатами клітинки
				Point cellPoint = new Point(x, y);

				// Визначити тип елементу, який потрібно створити, в залежності від натиснутої кнопки миші
				if (e.ChangedButton == MouseButton.Left)
				{
					// Виконати команду для створення кола
					viewModel.MakeMoveCircleCommand.Execute(cellPoint);
				}
				else if (e.ChangedButton == MouseButton.Right)
				{
					// Виконати команду для створення хрестика
					viewModel.MakeMoveCrossCommand.Execute(cellPoint);
				}
			}
		}



		private void UpdateUI()
		{
			// Метод для оновлення UI після зміни ігрового стану
			GameViewModel viewModel = DataContext as GameViewModel;
			// Припускаючи, що у вас є метод в ViewModel, який можна викликати для оновлення UI
			viewModel?.UpdateUICommand.Execute(null);
		}
	}
}