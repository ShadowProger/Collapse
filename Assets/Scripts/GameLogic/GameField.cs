using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Manybits
{
    public class GameField
    {
        public const int MIN_MATCH = 3;
        public int width;
        public int height;
        public int[,] cells;
        public int[,] back;



        public GameField() { }

        public GameField(int width, int height)
        {
            this.width = width;
            this.height = height;
            cells = new int[height, width];
            back = new int[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    cells[i, j] = 0;
                    back[i, j] = 0;
                }
        }



        public Vector2[] GetPath(Vector2 start, Vector2 dest)
        {
            if (start.x == dest.x && start.y == dest.y)
                return null;

            int[,] matrix = new int[height, width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (cells[i, j] == 0)
                        matrix[i, j] = 0;
                    else
                        matrix[i, j] = -1;
            matrix[(int)start.y, (int)start.x] = 0;

            if (matrix[(int)dest.y, (int)dest.x] != 0)
                return null;

            // Строим матрицу
            List<Vector2> curCells = new List<Vector2>();
            List<Vector2> nextCells = new List<Vector2>();
            curCells.Add(dest);
            int waveNumber = 1;
            while (curCells.Count > 0)
            {
                nextCells.Clear();
                for (int i = 0; i < curCells.Count; i++)
                {
                    int x = (int)curCells[i].x;
                    int y = (int)curCells[i].y;
                    matrix[y, x] = waveNumber;
                    Vector2 v = new Vector2(x - 1, y);
                    if (IsOnField(v) && matrix[(int)v.y, (int)v.x] == 0)
                        nextCells.Add(v);
                    v = new Vector2(x + 1, y);
                    if (IsOnField(v) && matrix[(int)v.y, (int)v.x] == 0)
                        nextCells.Add(v);
                    v = new Vector2(x, y - 1);
                    if (IsOnField(v) && matrix[(int)v.y, (int)v.x] == 0)
                        nextCells.Add(v);
                    v = new Vector2(x, y + 1);
                    if (IsOnField(v) && matrix[(int)v.y, (int)v.x] == 0)
                        nextCells.Add(v);
                }
                curCells.Clear();
                curCells.AddRange(nextCells);
                waveNumber++;
                if (matrix[(int)start.y, (int)start.x] != 0)
                    break;
            }

            if (matrix[(int)start.y, (int)start.x] == 0)
                return null;

            // Строим маршрут
            int min;
            List<Vector2> path = new List<Vector2>();
            path.Add(start);
            int stepX = (int)start.x;
            int stepY = (int)start.y;
            int minX;
            int minY;
            while (stepX != dest.x || stepY != dest.y)
            {
                min = matrix[stepY, stepX];
                minX = stepX;
                minY = stepY;
                for (int i = stepY - 1; i <= stepY + 1; i++)
                    for (int j = stepX - 1; j <= stepX + 1; j++)
                    {
                        if (IsOnField(j, i))
                        {
                            if (matrix[i, j] > 0 && matrix[i, j] < min)
                            {
                                int dX = j - stepX;
                                int dY = i - stepY;
                                if (Mathf.Abs(dX) == 1 && Mathf.Abs(dY) == 1)
                                {
                                    if (matrix[stepY, stepX + dX] != -1 && matrix[stepY + dY, stepX] != -1)
                                    {
                                        min = matrix[i, j];
                                        minX = j;
                                        minY = i;
                                    }
                                }
                                else
                                {
                                    min = matrix[i, j];
                                    minX = j;
                                    minY = i;
                                }
                            }
                        }
                    }
                stepX = minX;
                stepY = minY;
                path.Add(new Vector2(stepX, stepY));
            }

            // Добавление промежуточных точек
            for (int i = 0; i < path.Count - 1; i++)
            {
                int aX = (int)path[i].x;
                int aY = (int)path[i].y;
                int bX = (int)path[i + 1].x;
                int bY = (int)path[i + 1].y;
                int dX = bX - aX;
                int dY = bY - aY;
                if (Mathf.Abs(dX) == 1 && Mathf.Abs(dY) == 1)
                {
                    if (matrix[aY, aX + dX] != -1)
                    {
                        path.Insert(i + 1, new Vector2(aX + dX, aY));
                        continue;
                    }
                    if (matrix[aY + dY, aX] != -1)
                        path.Insert(i + 1, new Vector2(aX + dX, aY));
                }
            }

            path.RemoveAt(0);

            return path.ToArray();
        }



        public void StartInfinityGame(int width, int height)
        {
            this.width = width;
            this.height = height;
            cells = new int[height, width];
            back = new int[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    cells[i, j] = 0;
                    back[i, j] = 0;
                }
        }



        public void StartLevel(Level level)
        {
            this.width = level.width;
            this.height = level.height;
            cells = new int[height, width];
            back = new int[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    cells[i, j] = 0;
                    back[i, j] = level.backCells[i, j];
                }
        }



        public Vector2[] Generate(int count, bool withoutMatches)
        {
            Vector2[] points = new Vector2[count];

            if (withoutMatches)
            {
                for (int i = 0; i < count; i++)
                {
                    int x, y, value = 0;
                    bool f;
                    do
                    {
                        f = false;
                        x = Random.Range(0, width);
                        y = Random.Range(0, height);
                        if (cells[y, x] == 0)
                        {
                            value = GetValue();
                            if (!CheckMatch(x, y, value))
                            {
                                f = true;
                                cells[y, x] = value;
                            }
                        }
                    } while (!f);

                    points[i].Set(x, y);
                }
            }
            else
            {
                List<Vector2> emptyCells = new List<Vector2>();
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        if (cells[i, j] == 0)
                            emptyCells.Add(new Vector2(j, i));

                //shuffle
                for (int i = emptyCells.Count - 1; i >= 0; i--)
                {
                    int ii = Random.Range(0, i);
                    Vector2 tmp = emptyCells[i];
                    emptyCells[i] = emptyCells[ii];
                    emptyCells[ii] = tmp;
                }

                //truncate
                if (emptyCells.Count > count)
                {
                    emptyCells.RemoveRange(count, emptyCells.Count - count);
                }

                points = emptyCells.ToArray();

                for (int i = 0; i < points.Length; i++)
                    cells[(int)points[i].y, (int)points[i].x] = GetValue();
            }

            return points;
        }



        private bool CheckMatch(int x, int y, int value)
        {
            if (IsOnField(x, y))
            {
                Vector2 left = new Vector2(x - 1, y);
                Vector2 right = new Vector2(x + 1, y);
                Vector2 up = new Vector2(x, y - 1);
                Vector2 down = new Vector2(x, y + 1);

                int count = 0;
                if (IsOnField(left) && cells[(int)left.y, (int)left.x] == value)
                    count++;
                if (IsOnField(right) && cells[(int)right.y, (int)right.x] == value)
                    count++;
                if (IsOnField(up) && cells[(int)up.y, (int)up.x] == value)
                    count++;
                if (IsOnField(down) && cells[(int)down.y, (int)down.x] == value)
                    count++;

                if (count >= 2)
                    return true;

                if (IsOnField(left) && cells[(int)left.y, (int)left.x] == value)
                {
                    if (IsOnField((int)left.x - 1, (int)left.y) && cells[(int)left.y, (int)left.x - 1] == value ||
                        IsOnField((int)left.x, (int)left.y - 1) && cells[(int)left.y - 1, (int)left.x] == value ||
                        IsOnField((int)left.x, (int)left.y + 1) && cells[(int)left.y + 1, (int)left.x] == value)
                    {
                        return true;
                    }
                }

                if (IsOnField(right) && cells[(int)right.y, (int)right.x] == value)
                {
                    if (IsOnField((int)right.x + 1, (int)right.y) && cells[(int)right.y, (int)right.x + 1] == value ||
                        IsOnField((int)right.x, (int)right.y - 1) && cells[(int)right.y - 1, (int)right.x] == value ||
                        IsOnField((int)right.x, (int)right.y + 1) && cells[(int)right.y + 1, (int)right.x] == value)
                    {
                        return true;
                    }
                }

                if (IsOnField(up) && cells[(int)up.y, (int)up.x] == value)
                {
                    if (IsOnField((int)up.x, (int)up.y - 1) && cells[(int)up.y - 1, (int)up.x] == value ||
                        IsOnField((int)up.x - 1, (int)up.y) && cells[(int)up.y, (int)up.x - 1] == value ||
                        IsOnField((int)up.x + 1, (int)up.y) && cells[(int)up.y, (int)up.x + 1] == value)
                    {
                        return true;
                    }
                }

                if (IsOnField(down) && cells[(int)down.y, (int)down.x] == value)
                {
                    if (IsOnField((int)down.x, (int)down.y + 1) && cells[(int)down.y + 1, (int)down.x] == value ||
                        IsOnField((int)down.x - 1, (int)down.y) && cells[(int)down.y, (int)down.x - 1] == value ||
                        IsOnField((int)down.x + 1, (int)down.y) && cells[(int)down.y, (int)down.x + 1] == value)
                    {
                        return true;
                    }
                }
            }
            return false;
        }



        private int GetValue()
        {
            int r = Random.Range(0, 100);
            if (r >= 0 && r < 60)
                return 1;
            if (r >= 60 && r < 87)
                return 2;
            else
                return 3;
        }



        public Vector2[] GetAvailableCells(Vector2 point)
        {
            if (!IsOnField(point))
                return null;
            List<Vector2> points = new List<Vector2>();
            Check(new Vector2(point.x - 1, point.y), 0, points);
            Check(new Vector2(point.x + 1, point.y), 0, points);
            Check(new Vector2(point.x, point.y - 1), 0, points);
            Check(new Vector2(point.x, point.y + 1), 0, points);
            return points.ToArray();
        }



        public Match[] GetMatches(Vector2[] points)
        {
            for (int i = 0; i < points.Length; i++)
                if (!IsOnField(points[i]))
                    return null;

            List<Match> matches = new List<Match>();

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 p1 = points[i];
                bool isAlreadyUsed = false;
                foreach (Match match in matches)
                {
                    for (int j = 0; j < match.points.Length; j++)
                    {
                        Vector2 p2 = match.points[j];
                        if (p1 == p2)
                        {
                            isAlreadyUsed = true;
                            break;
                        }
                    }
                    if (isAlreadyUsed)
                        break;
                }
                if (!isAlreadyUsed)
                {
                    Match match = GetMatch(p1);
                    if (match != null)
                        matches.Add(match);
                }
            }

            if (matches.Count > 0)
                return matches.ToArray();
            else
                return null;
        }



        public Match GetMatch(Vector2 point)
        {
            if (!IsOnField(point))
                return null;

            List<Vector2> points = new List<Vector2>();
            int value = cells[(int)point.y, (int)point.x];
            Check(point, value, points);
            if (points.Count >= MIN_MATCH)
            {
                Match match = new Match();
                match.value = value;
                match.mainPoint = point;
                match.count = points.Count;
                int i;
                for (i = 0; i < points.Count; i++)
                    if (points[i].x == point.x && points[i].y == point.y)
                        break;
                if (i < points.Count)
                    points.RemoveAt(i);
                match.points = points.ToArray();
                return match;
            }
            else
                return null;
        }



        private void Check(Vector2 point, int value, List<Vector2> points)
        {
            if (!IsOnField(point))
                return;
            if (cells[((int)point.y), ((int)point.x)] != value)
                return;
            if (points.Contains(point))
                return;
            else
                points.Add(point);
            Check(new Vector2(point.x - 1, point.y), value, points);
            Check(new Vector2(point.x + 1, point.y), value, points);
            Check(new Vector2(point.x, point.y - 1), value, points);
            Check(new Vector2(point.x, point.y + 1), value, points);
        }



        public bool IsOnField(Vector2 point)
        {
            return point.x >= 0 && point.y >= 0 && point.x < width && point.y < height;
        }



        public bool IsOnField(int x, int y)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }



        public int GetEmptyCellCount()
        {
            int count = 0;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (cells[i, j] == 0)
                        count++;
            return count;
        }



        public bool IsEmptyCell(Vector2 cell)
        {
            return cells[((int)cell.y), ((int)cell.x)] == 0;
        }



        public bool IsEmptyCell(int x, int y)
        {
            return cells[y, x] == 0;
        }
    }
}