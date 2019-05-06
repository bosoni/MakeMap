// orig maze code: http://jonathanzong.com/blog/2012/11/06/maze-generation-with-prims-algorithm
using System;
using System.Collections.Generic;

namespace MakeMap
{
    public static class Maze
    {
        static int mapWidth = 10, mapHeight = 10;
        static Random random = new Random();

        /*
         * create maze.
         * r, c: dimensions of generated maze
         */
        public static char[,] MakeMaze(int r, int c, bool setStart, bool setEnd)
        {
            mapWidth = r;
            mapHeight = c;

            // vähennetään 2 koska metodin lopussa lisätään finalMazeen 2 (reunat)
            r -= 2;
            c -= 2;

            char[,] maz = new char[r, c];
            for (int x = 0; x < c; x++)
                for (int y = 0; y < r; y++)
                    maz[y, x] = (char)'#';

            // select random point and open as start node
            Point st = new Point((int)((float)random.NextDouble() * r), (int)((float)random.NextDouble() * c), null);
            if (setStart) maz[st.r, st.c] = 'S';

            // iterate through direct neighbors of node
            List<Point> frontier = new List<Point>();
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0 || x != 0 && y != 0)
                        continue;
                    try
                    {
                        if (maz[st.r + x, st.c + y] == ' ')
                            continue;
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                    // add eligible points to frontier
                    frontier.Add(new Point(st.r + x, st.c + y, st));
                }

            Point last = null;
            while (frontier.Count > 0)
            {
                // pick current node at random
                int rnd = (int)((float)random.NextDouble() * frontier.Count);
                Point cu = frontier[rnd];
                frontier.RemoveAt(rnd);

                Point op = cu.opposite();

                try
                {
                    // if both node and its opposite are walls
                    if (maz[cu.r, cu.c] == '#')
                    {
                        if (maz[op.r, op.c] == '#')
                        {

                            // open path between the nodes
                            maz[cu.r, cu.c] = ' ';
                            maz[op.r, op.c] = ' ';

                            // store last node in order to mark it later
                            last = op;

                            // iterate through direct neighbors of node, same as earlier
                            for (int x = -1; x <= 1; x++)
                                for (int y = -1; y <= 1; y++)
                                {
                                    if (x == 0 && y == 0 || x != 0 && y != 0)
                                        continue;
                                    try
                                    {
                                        if (maz[op.r + x, op.c + y] == ' ')
                                            continue;
                                    }
                                    catch (Exception e)
                                    {
                                        continue;
                                    }
                                    frontier.Add(new Point(op.r + x, op.c + y, op));
                                }
                        }
                    }
                }
                catch (Exception e)
                {
                }

                // if algorithm has resolved, mark end node
                if (frontier.Count == 0)
                    if (setEnd)
                        maz[last.r, last.c] = 'E';
                //else
                //  maz[last.r, last.c] = ' ';
            }

            // tehdään mazen reunat
            char[,] finalMaz = new char[r + 2, c + 2];
            for (int i = 0; i < r + 2; i++)
                for (int j = 0; j < c + 2; j++)
                {
                    if (i == 0 || j == 0 || i == r + 1 || j == c + 1)
                        finalMaz[i, j] = '#';
                    else
                        finalMaz[i, j] = maz[i - 1, j - 1];
                }

            return finalMaz;
        }

        /**
         * luo huoneita. huoneiden koko randomilla [minSX, maxSX], [minSY, maxSY]
         * material on joku merkki (esim ' ' on floor)
         */
        public static void MakeRooms(int roomCount, char[,] maze, int minSX, int minSY, int maxSX,
                              int maxSY, char material)
        {
            int x, y, sx, sy;
            for (int c = 0; c < roomCount; c++)
            {
                // looppaa kunnes huone on kartalla (ei mene reunoista yli)
                while (true)
                {
                    x = random.Next(mapWidth) + 1;
                    y = random.Next(mapHeight) + 1; // random pos
                    sx = random.Next(maxSX) + minSX;
                    sy = random.Next(maxSY) + minSY; // random size
                    if (x + sx < mapWidth - 2 && y + sy < mapHeight - 2)
                        break;
                }

                for (int xx = 0; xx < sx; xx++)
                {
                    for (int yy = 0; yy < sy; yy++)
                    {
                        maze[x + xx, y + yy] = material;
                    }
                }
            }
        }

        public static string MazeToString(char[,] maze)
        {
            string s = "";
            for (int j = 0; j < mapHeight; j++)
            {
                for (int i = 0; i < mapWidth; i++)
                    s += (char)maze[i, j];
                s += "\n";
            }
            return s;
        }
    }

    class Point
    {
        public int r, c;
        Point parent;

        public Point(int x, int y, Point p)
        {
            r = x;
            c = y;
            parent = p;
        }

        // compute opposite node given that it is in the other direction from the parent
        public Point opposite()
        {
            int w = 0;
            if (r != parent.r)
            {
                if (r < parent.r) w = -1;
                if (r > parent.r) w = 1;
                return new Point(this.r + w, this.c, this);
            }
            if (c != parent.c)
            {
                if (c < parent.c) w = -1;
                if (c > parent.c) w = 1;
                return new Point(this.r, this.c + w, this);
            }
            return null;
        }
    }
}
