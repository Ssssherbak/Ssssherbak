using System;
using System.Collections.Generic;

namespace _2048
{
    public class Grid
    {
        private readonly Random _random;

        public int Size { get; private set; }
        public int[,] Cells { get; private set; }
        public int Score { get; private set; }

        public Grid(int size)
        {
            _random = new Random();

            Size = size;
            Cells = new int[Size,Size];
            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                    Cells[x, y] = 0;
            Score = 0;
        }

        public Grid(Grid grid)
        {
            Size = grid.Size;
            Score = grid.Score;
            Cells = new int[Size,Size];
            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                    Cells[x, y] = grid.Cells[x,y];
        }
        public bool MoveUp()
        {
            bool moved = false;
            for (int x = 0; x < Size; x++)
            {
                int y = 0;
                int endY = Size;
                while (y < endY)
                {
                    if (Cells[x, y] == 0)
                    {
                        for (int i = y; i < endY - 1; i++)
                        {
                            Cells[x, i] = Cells[x, i + 1];
                            if (Cells[x, i] > 0)
                                moved = true;
                        }
                        Cells[x, endY - 1] = 0;
                        endY -= 1;
                    }
                    else
                        y += 1;
                }
                y = 0;
                endY = Size;
                while (y != endY)
                {
                    if (y + 1 < Size && Cells[x, y] > 0 && Cells[x, y] == Cells[x, y + 1])
                    {
                        moved = true;
                        // Merge
                        Cells[x, y] *= 2;
                        // Score
                        Score += Cells[x, y];
                        for (int i = y + 1; i < endY - 1; i++)
                            Cells[x, i] = Cells[x, i + 1];
                        Cells[x, endY - 1] = 0;
                        y += 1;
                    }
                    else
                        y += 1;
                }
            }
            return moved;
        }

        public bool MoveRight()
        {
            bool moved = false;
            for (int y = 0; y < Size; y++)
            {
                int x = Size - 1;
                int endX = -1;
                while (x > endX)
                {
                    if (Cells[x, y] == 0) 
                    {
                        for (int i = x; i > endX + 1; i--)
                        {
                            Cells[i, y] = Cells[i - 1, y];
                            if (Cells[i, y] > 0)
                                moved = true;
                        }
                        Cells[endX + 1, y] = 0;
                        endX += 1;
                    }
                    else
                        x -= 1;
                }
                x = Size - 1;
                endX = -1;
                while (x != endX)
                {
                    if (x - 1 >= 0 && Cells[x, y] > 0 && Cells[x, y] == Cells[x - 1, y]) 
                    {
                        moved = true;
                        Cells[x, y] *= 2;
                        Score += Cells[x, y];
                        for (int i = x - 1; i > endX + 1; i--)
                            Cells[i, y] = Cells[i - 1, y];
                        Cells[endX + 1, y] = 0;
                        x -= 1;
                    }
                    else
                        x -= 1;
                }
            }
            return moved;
        }

        public bool MoveDown()
        {
            bool moved = false;
            for (int x = 0; x < Size; x++)
            {
                int y = Size - 1;
                int endY = -1;
                while (y > endY)
                {
                    if (Cells[x, y] == 0) 
                    {
                        for (int i = y; i > endY + 1; i--)
                        {
                            Cells[x, i] = Cells[x, i - 1];
                            if (Cells[x, i] > 0)
                                moved = true;
                        }
                        Cells[x, endY + 1] = 0;
                        endY += 1;
                    }
                    else
                        y -= 1;
                }
                y = Size - 1;
                endY = -1;

                while (y != endY)
                {
                    if (y - 1 >= 0 && Cells[x, y] > 0 && Cells[x, y] == Cells[x, y - 1]) 
                    {
                        moved = true;
                        Cells[x, y] *= 2;
                        Score += Cells[x, y];
                        for (int i = y - 1; i > endY + 1; i--)
                            Cells[x, i] = Cells[x, i - 1];
                        Cells[x, endY + 1] = 0;
                        y -= 1;
                    }
                    else
                        y -= 1;
                }
            }
            return moved;
        }

        public bool MoveLeft()
        {
            bool moved = false;
            for (int y = 0; y < Size; y++)
            {
                int x = 0;
                int endX = Size;
                while (x < endX)
                {
                    if (Cells[x, y] == 0)
                    {
                        for (int i = x; i < endX - 1; i++)
                        {
                            Cells[i, y] = Cells[i + 1, y];
                            if (Cells[i, y] > 0)
                                moved = true;
                        }
                        Cells[endX - 1, y] = 0;
                        endX -= 1;
                    }
                    else
                        x += 1;
                }
                x = 0;
                endX = Size;

                while (x != endX)
                {
                    if (x + 1 < Size && Cells[x, y] > 0 && Cells[x, y] == Cells[x + 1, y]) 
                    {
                        moved = true;

                        Cells[x, y] *= 2;

                        Score += Cells[x, y];

                        for (int i = x + 1; i < endX - 1; i++)
                            Cells[i, y] = Cells[i + 1, y];

                        Cells[endX - 1, y] = 0;
                        x += 1;
                    }
                    else
                        x += 1;
                }
            }
            return moved;
        }

        public bool AddRandomTile()
        {
            int x, y;
            bool hasAvailable = RandomAvailableCell(out x, out y);
            if (hasAvailable)
            {
                int value = _random.NextDouble() < 0.9 ? 2 : 4;
                Cells[x, y] = value;
            }
            return hasAvailable;
        }
        public IEnumerable<int> Flatten()
        {
            for (int row = 0; row < Cells.GetLength(0); row++)
                for (int col = 0; col < Cells.GetLength(1); col++)
                    yield return Cells[row, col];
        }

        public bool FindFarthestPosition(int startX, int startY, int stepX, int stepY, out int findX, out int findY)
        {
            findX = startX;
            findY = startY;
            while (true)
            {
                findX += stepX;
                findY += stepY;
                if (findX < 0 || findX >= Size || findY < 0 || findY >= Size)
                    return false;
                if (Cells[findX, findY] > 0)
                    return true;
            }
        }

        private bool RandomAvailableCell(out int x, out int y)
        {
            x = 0;
            y = 0;
            int count = 0;
            for (int yi = 0; yi < Size; yi++)
                for (int xi = 0; xi < Size; xi++)
                    if (Cells[xi, yi] == 0)
                        count++;
            if (count == 0)
                return false;
            int availableIndex = _random.Next(count);
            int index = 0;
            for (int yi = 0; yi < Size; yi++)
                for (int xi = 0; xi < Size; xi++)
                    if (Cells[xi, yi] == 0)
                        if (index == availableIndex)
                        {
                            x = xi;
                            y = yi;
                            return true;
                        }
                        else
                            index++;
            return true;
        }
    }
}