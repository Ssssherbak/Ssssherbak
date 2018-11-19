using System;
using System.Collections.Generic;
using System.Linq;

namespace _2048
{
    public class AI
    {
        public const double Epsilon = 0.00001;
        public const double SmoothWeight = 0.1;
        public const double Mono2Weight = 1.0;
        public const double EmptyWeight = 2.7;
        public const double MaxWeight = 1.0;

        private class Transition
        {
            public Grid Grid { get; set; }
            public Directions Direction { get; set; }
        }

        public List<Directions> GetBestMoves(Grid root)
        {
            double bestRating = double.MaxValue;
            List<Directions> bestMoves = new List<Directions>();

            List<Transition> moves = GetAllTransition(root);
            foreach (Transition transition in moves)
            {
                double rating = AlphaBetaRate(transition.Grid, 10, double.MaxValue, double.MinValue, false);

                if (Math.Abs(rating - bestRating) < Epsilon)
                    bestMoves.Add(transition.Direction);
                else if (rating < bestRating)
                {
                    bestMoves = new List<Directions>
                        {
                            transition.Direction
                        };
                    bestRating = rating;
                }
            }
            return bestMoves;
        }

        public List<Directions> GetBestMoves2(Grid root)
        {
            List<Directions> bests = new List<Directions>();
            for (int depth = 0; depth < 10; depth++)
            {
                Tuple<double, Directions, int, int> best = AlphaBetaRate2(root, 0, double.MinValue, double.MaxValue, true, 0, 0);
                if (best.Item2 == Directions.None)
                    break;
                bests.Add(best.Item2);
            }
            return bests; 
        }
        private double AlphaBetaRate(Grid root, int depth, double alpha, double beta, bool player)
        {
            if (depth == 0)
                return Rate(root);

            if (player)
            {
                List<Transition> moves = GetAllTransition(root);
                if (moves.Count == 0)
                    return double.MaxValue;

                foreach (Transition st in moves)
                {
                    alpha = Math.Min(alpha, AlphaBetaRate(st.Grid, depth - 1, alpha, beta, false));
                    if (beta >= alpha)
                        break;
                }
                return alpha;
            }
            else
            {
                IEnumerable<Grid> moves = GetAllRandom(root);

                foreach (Grid st in moves)
                {
                    beta = Math.Max(beta, AlphaBetaRate(st, depth - 1, alpha, beta, true));
                    if (beta <= alpha)
                        break;
                }
                return beta;
            }
        }

        private double Rate(Grid grid)
        {
            List<Transition> moves = GetAllTransition(grid);
            if (moves.Count == 0)
                return double.MaxValue;
            int numZero = 0;
            Dictionary<int, int> count = new Dictionary<int, int>();
            for (int y = 0; y < grid.Size; y++)
            {
                for (int x = 0; x < grid.Size; x++)
                {
                    if (grid.Cells[x, y] == 0)
                    {
                        numZero++;
                        continue;
                    }
                    int cur;
                    if (!count.TryGetValue(grid.Cells[x, y], out cur))
                        count[grid.Cells[x, y]] = 1;
                    else
                        count[grid.Cells[x, y]] = cur + 1;
                }
            }

            int numNonZero = grid.Size*grid.Size - numZero;
            double entropy = count.Keys.Select(k => (double) count[k]/numNonZero).Aggregate<double, double>(0, (current, freq) => current - freq*Math.Log(freq));
            entropy /= Math.Log(grid.Size*grid.Size);

            return numNonZero + entropy;
        }

        private Tuple<double, Directions, int, int> AlphaBetaRate2(Grid root, int depth, double alpha, double beta, bool player, int positions, int cutoffs)
        {
            double bestScore = 0;
            Directions bestMove = Directions.None;
            Tuple<double, Directions, int, int> result;
            if (player)
            {
                bestScore = alpha;
                List<Transition> transitions = GetAllTransition(root);
                foreach (Transition transition in transitions)
                {

                    positions++;

                    if (depth == 0)
                        result = new Tuple<double, Directions, int, int>(Rate2(transition.Grid), transition.Direction, 0, 0);
                    else
                    {
                        result = AlphaBetaRate2(transition.Grid, depth - 1, bestScore, beta, false, positions, cutoffs);
                        positions = result.Item3;
                        cutoffs = result.Item4;
                    }

                    if (result.Item1 > bestScore)
                    {
                        bestScore = result.Item1;
                        bestMove = transition.Direction;
                    }
                    if (bestScore > beta)
                    {
                        cutoffs++;
                        return new Tuple<double, Directions, int, int>(beta, bestMove, positions, cutoffs);
                    }
                }
            }
            else
            {
                bestScore = beta;
                List<Tuple<int, int, double>>[] scores = new List<Tuple<int, int, double>>[2]; // 0: 2  1: 4
                List<Tuple<int, int>> frees = GetFree(root).ToList();
                scores[0] = new List<Tuple<int, int, double>>(); // 2
                scores[1] = new List<Tuple<int, int, double>>(); // 4
                foreach (Tuple<int, int> xy in frees)
                {
                    root.Cells[xy.Item1, xy.Item2] = 2;
                    double value2 = -Smoothness(root) + Islands(root);
                    scores[0].Add(new Tuple<int, int, double>(xy.Item1, xy.Item2, value2));
                    root.Cells[xy.Item1, xy.Item2] = 4;
                    double value4 = -Smoothness(root) + Islands(root);
                    scores[1].Add(new Tuple<int, int, double>(xy.Item1, xy.Item2, value4));
                    root.Cells[xy.Item1, xy.Item2] = 0;
                }
                List<Tuple<int, int, int>> candidates = new List<Tuple<int, int, int>>();
                double maxScore = Math.Max(scores[0].Max(x => x.Item3), scores[1].Max(x => x.Item3));
                for (int i = 0; i < 2; i++)
                    candidates.AddRange(scores[i]
                                            .Where(x => Math.Abs(x.Item3 - maxScore) < Epsilon)
                                            .Select(x => new Tuple<int, int, int>(x.Item1, x.Item2, i == 0 ? 2 : 4)));
                foreach (Tuple<int, int, int> candidate in candidates)
                {
                    Grid newGrid = new Grid(root);
                    newGrid.Cells[candidate.Item1, candidate.Item2] = candidate.Item3;
                    positions++;
                    result = AlphaBetaRate2(newGrid, depth, alpha, bestScore, true, positions, cutoffs);
                    positions = result.Item3;
                    cutoffs = result.Item4;

                    if (result.Item1 < bestScore)
                        bestScore = result.Item1;
                    if (bestScore < alpha)
                    {
                        cutoffs++;
                        return new Tuple<double, Directions, int, int>(alpha, Directions.None, positions, cutoffs);
                    }
                }
            }
            return new Tuple<double, Directions, int, int>(bestScore, bestMove, positions, cutoffs);
        }

        private double Rate2(Grid grid)
        {
            IEnumerable<Tuple<int, int>> emptyCells = GetFree(grid);

            return Smoothness(grid)*SmoothWeight
                   + Monotonicity2(grid)*Mono2Weight
                   + Math.Log(emptyCells.Count())*EmptyWeight
                   + MaxValue(grid)*MaxWeight;
        }
        private double Smoothness(Grid grid)
        {
            return 0;
            double smoothness = 0;
            for (int x = 0; x < grid.Size; x++)
                for (int y = 0; y < grid.Size; y++)
                    if (grid.Cells[x, y] > 0)
                    {
                        double value = Math.Log(grid.Cells[x, y])/Math.Log(2);
                        smoothness -= SmoothnessDirection(grid, value, x, y, 1, 0); // right
                        smoothness -= SmoothnessDirection(grid, value, x, y, 0, 1); // down
                    }
            return smoothness;
        }

        private double SmoothnessDirection(Grid grid, double value, int x, int y, int stepX, int stepY)
        {
            int targetX, targetY;
            bool found = grid.FindFarthestPosition(x, y, stepX, stepY, out targetX, out targetY);
            if (found && grid.Cells[targetX, targetY] > 0)
            {
                int target = grid.Cells[targetX, targetY];
                double targetValue = Math.Log(target)/Math.Log(2);
                return Math.Abs(value - targetValue);
            }
            return 0;
        }
        private double Monotonicity2(Grid grid)
        {
            return 0;
            double[] totals = {0, 0, 0, 0};

            for (int x = 0; x < grid.Size; x++)
            {
                int current = 0;
                int next = current + 1;
                while (next < grid.Size)
                {
                    while (next < grid.Size && grid.Cells[x, next] == 0)
                        next++;
                    if (next >= grid.Size)
                        next--;
                    double currentValue = grid.Cells[x, current] > 0
                                              ? Math.Log(grid.Cells[x, current])/Math.Log(2)
                                              : 0;
                    double nextValue = grid.Cells[x, next] > 0
                                           ? Math.Log(grid.Cells[x, next])/Math.Log(2)
                                           : 0;
                    if (currentValue > nextValue)
                        totals[0] += nextValue - currentValue;
                    else if (nextValue > currentValue)
                        totals[1] += currentValue - nextValue;
                    current = next;
                    next++;
                }
            }
            for (int y = 0; y < grid.Size; y++)
            {
                int current = 0;
                int next = current + 1;
                while (next < grid.Size)
                {
                    while (next < grid.Size && grid.Cells[next, y] == 0)
                        next++;
                    if (next >= grid.Size)
                        next--;
                    double currentValue = grid.Cells[current, y] > 0
                                              ? Math.Log(grid.Cells[current, y])/Math.Log(2)
                                              : 0;
                    double nextValue = grid.Cells[next, y] > 0
                                           ? Math.Log(grid.Cells[next, y])/Math.Log(2)
                                           : 0;
                    if (currentValue > nextValue)
                        totals[2] += nextValue - currentValue;
                    else if (nextValue > currentValue)
                        totals[3] += currentValue - nextValue;
                    current = next;
                    next++;
                }
            }
            return Math.Max(totals[0], totals[1]) + Math.Max(totals[2], totals[3]);
        }

        private double MaxValue(Grid grid)
        {
            int max = grid.Flatten().Max();
            return max == 0 ? 0 : Math.Log(max)/Math.Log(2);
        }

        private void Mark(Grid grid, int x, int y, int value, bool[,] mark)
        {
            if (grid.Cells[x, y] > 0 && grid.Cells[x, y] == value && !mark[x, y])
            {
                // left
                if (x - 1 >= 0)
                    Mark(grid, x - 1, y, value, mark);
                // right
                if (x + 1 < grid.Size)
                    Mark(grid, x + 1, y, value, mark);
                // down
                if (y - 1 >= 0)
                    Mark(grid, x, y - 1, value, mark);
                // up
                if (y + 1 < grid.Size)
                    Mark(grid, x, y + 1, value, mark);
            }
        }

        private double Islands(Grid grid)
        {
            double islands = 0;
            bool[,] mark = new bool[grid.Size,grid.Size];
            for (int y = 0; y < grid.Size; y++)
                for (int x = 0; x < grid.Size; x++)
                    if (grid.Cells[x, y] > 0 && !mark[x, y])
                    {
                        islands++;
                        Mark(grid, x, y, grid.Cells[x, y], mark);
                    }
            return islands;
        }

        private List<Transition> GetAllTransition(Grid grid)
        {
            List<Transition> allMoves = new List<Transition>();

            Grid nextLeft = new Grid(grid);
            bool movedLeft = nextLeft.MoveLeft();
            if (movedLeft)
                allMoves.Add(new Transition
                    {
                        Grid = nextLeft,
                        Direction = Directions.Left
                    });

            Grid nextRight = new Grid(grid);
            bool movedRight = nextRight.MoveRight();
            if (movedRight)
                allMoves.Add(new Transition
                    {
                        Grid = nextRight,
                        Direction = Directions.Right
                    });

            Grid nextUp = new Grid(grid);
            bool movedUp = nextUp.MoveUp();
            if (movedUp)
                allMoves.Add(new Transition
                    {
                        Grid = nextUp,
                        Direction = Directions.Up
                    });

            Grid nextDown = new Grid(grid);
            bool movedDown = nextDown.MoveDown();
            if (movedDown)
                allMoves.Add(new Transition
                    {
                        Grid = nextDown,
                        Direction = Directions.Down
                    });

            return allMoves;
        }

        private IEnumerable<Tuple<int, int>> GetFree(Grid grid)
        {
            List<Tuple<int, int>> free = new List<Tuple<int, int>>();
            for (int y = 0; y < grid.Size; y++)
                for (int x = 0; x < grid.Size; x++)
                    if (grid.Cells[x, y] == 0)
                        free.Add(new Tuple<int, int>(x, y));

            return free;
        }

        private IEnumerable<Grid> GetAllRandom(Grid grid)
        {
            List<Grid> res = new List<Grid>();
            IEnumerable<Tuple<int, int>> free = GetFree(grid);

            foreach (Tuple<int, int> x in free)
            {
                Grid next2 = new Grid(grid);
                next2.Cells[x.Item1, x.Item2] = 2;
                res.Add(next2);

                Grid next4 = new Grid(grid);
                next4.Cells[x.Item1, x.Item2] = 4;
                res.Add(next4);
            }

            return res;
        }
    }
}