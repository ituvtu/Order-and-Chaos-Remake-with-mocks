public interface IGame
{
	int Rows { get; }
	int Columns { get; }
	GameBoard.GameElement GetValueAt(int x, int y);
	void AddNode(GameBoard.Node newNode);
	string ToString();
}