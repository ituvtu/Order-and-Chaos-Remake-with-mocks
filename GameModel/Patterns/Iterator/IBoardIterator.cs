public interface IBoardIterator
{
	object Current { get; }
	int CurrentX { get; }
	int CurrentY { get; }

	bool MoveNext();
	void Reset();
}