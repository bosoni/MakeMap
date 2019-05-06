/*
 * makemap
 * (c) mjt, 2019
 * Released under MIT-license.
 * 
 * Program creates random maze or loads bitmap files which contains map.
 * Exports maze to [filename].txt or out_[bitmapfilename].txt
 * 
 * Check exported .txt files, set modelfiles right at the beginning of the file.
 * 
 * With these .txt mapfiles one can convert maps to .obj file.
 * 
 */
using System;
using System.Drawing;

namespace MakeMap
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("makemap (c) mjt, 2019");
            if (args.Length == 0)
            {
                Console.WriteLine("Parameters:");
                Console.WriteLine("  -maze      [output maze .txt file]");
                Console.WriteLine("  -bitmap    [bitmap files]");
                Console.WriteLine("  -createmap [mapfile.txt]");
                return;
            }

            if (args.Length < 2)
            {
                Console.WriteLine("check parameters");
                return;
            }

            if (args[0].Contains("-maze"))
            {
                /* NOTE!
                 * you can change parameters below: size, room sizes etc               
                 */

                char[,] maze = Maze.MakeMaze(20, 10, false, false);
                Maze.MakeRooms(5, maze, 2, 2, 4, 4, '-');
                string strmap = "";
                strmap += "#   wall.obj\n";
                strmap += "    floor.obj\n";
                strmap += "-   floor2.obj\n";
                strmap += "\n" + Maze.MazeToString(maze);
                System.IO.File.WriteAllText(args[1], strmap);
                return;
            }

            if (args[0].Contains("-createmap"))
            {
                Model.CreateObj(args[1]);
                return;
            }

            foreach (string arg in args)
            {
                string mapstr = "";
                long[] colors = new long[500]; // different colors
                int[] ch = new int[500]; // different characters
                ch[0] = ' ';
                int differentColors = 1;

                string s = arg.ToLower();
                if (s.Contains(".bmp") || s.Contains(".png") || s.Contains(".jpg"))
                {
                    Bitmap bmp;

                    bmp = new Bitmap(arg);
                    int w = bmp.Width;
                    int h = bmp.Height;
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            // convert colors to characters
                            Color color = bmp.GetPixel(x, y);
                            long c = (color.B << 16) + (color.G << 8) + color.R;

                            int cc = 0;
                            bool found = false;
                            // check color
                            for (cc = 0; cc < differentColors; cc++)
                            {
                                if (colors[cc] == c)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (found)
                            {
                                mapstr += (char)ch[cc];
                            }
                            else
                            {
                                ch[differentColors] = 'a' + (differentColors - 1);
                                colors[differentColors] = c;

                                mapstr += (char)ch[differentColors];
                                differentColors++;
                            }


                        }
                        mapstr += "\n";

                    }
                    string objs = "";
                    for (int q = 0; q < differentColors; q++)
                        objs += (char)ch[q] + "    model.obj\n";

                    mapstr = "\n" + objs + mapstr;
                    System.IO.File.WriteAllText("out_" + arg + ".txt", mapstr);
                }
            }
        }
    }
}
