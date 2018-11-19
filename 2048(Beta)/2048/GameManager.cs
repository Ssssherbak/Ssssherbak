using System;

namespace _2048
{
    public class GameManager
    {
        private readonly Random _random;

        public int Size { get; private set; }
        public int StartTileCount { get; private set; }
        public Grid Grid { get; private set; }
        public int Moves { get; private set; }

        public event EventHandler GridModified;

        public GameManager(int size, int startTileCount)
        {
            _random = new Random();
            Size = size;
            StartTileCount = startTileCount;
        }

        public void Start()
        {
            Grid = new Grid(Size);
            Moves = 0;
            AddStartTiles();
            if (GridModified != null)
                GridModified(this, new EventArgs());
        }

        public void Test(params int[] cells)
        {
            Grid = new Grid(Size);
            for(int i = 0; i < cells.Length; i++)
            {
                int x = i%Size;
                int y = i/Size;
                Grid.Cells[x, y] = cells[i];
            }
        }

        public bool Move(Directions direction)
        {
            bool moved = false;
            switch(direction)
            {
                case Directions.Up:
                    moved = Grid.MoveUp();
                    break;
                case Directions.Right:
                    moved = Grid.MoveRight();
                    break;
                case Directions.Down:
                    moved = Grid.MoveDown();
                    break;
                case Directions.Left:
                    moved = Grid.MoveLeft();
                    break;
            }
            if (moved)
            {
                Moves++;
                bool tileAdded = Grid.AddRandomTile();
                if (GridModified != null)
                    GridModified(this, new EventArgs());
                return tileAdded;
            }
            return true;
        }

        private void AddStartTiles()
        {
            for (int i = 0; i < StartTileCount; i++)
                Grid.AddRandomTile();
        }
    }
}
