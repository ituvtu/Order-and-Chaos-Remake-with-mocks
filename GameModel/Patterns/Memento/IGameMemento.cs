public interface IGameMemento
{
	GameBoard SavedGameBoard { get; set; }
	void SaveCurrentGameState();
	void UndoLastMove();
	void ClearSavedStates();
	void SaveCurrentGameState(GameBoard savedGameBoard);
}