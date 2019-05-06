/*
 * makemap
 * (c) mjt, 2019
 * Released under MIT-license.
 */
using System;
using System.Collections.Generic;

namespace MakeMap
{
    public class Model
    {
        static int globalIndexCount = 0, globalUVCount = 0, globalFaceCount = 0;
        static int objCounter = 0;
        static List<Model> models = new List<Model>();
        List<string> lines = new List<string>();
        char ch;
        string name = "";

        List<string> Load(string filename, float x, float y, float z,
                                int addIndex, int addUVIndex, int addFaceNum)
        {
            lines.Clear();
            int maxI = 0, maxUV = 0;

            string[] strs = System.IO.File.ReadAllLines(filename);
            foreach (string line in strs)
            {
                if (line.StartsWith("o ")) // objektin nimi
                {
                    string[] oname = line.Split(' ');
                    lines.Add("o " + oname[1] + objCounter++ + "\n");
                }
                else
                if (line.StartsWith("f "))
                {
                    string[] face = line.Split(' ');
                    string outline = "f ";

                    foreach (string fs in face)
                    {
                        string[] fi = fs.Split('/');
                        if (fi[0] == "f")
                            continue;

                        int a = int.Parse(fi[0]);
                        int b = int.Parse(fi[1]);
                        int c = int.Parse(fi[2]);

                        if (a > maxI) maxI = a;
                        if (b > maxUV) maxUV = b;

                        a += addIndex;
                        b += addUVIndex;
                        c += addFaceNum;

                        outline += " " + a + "/" + b + "/" + c;
                    }
                    lines.Add(outline + "\n");
                    globalFaceCount++;
                }
                else
                if (line.StartsWith("v "))
                {
                    string[] v = line.Split(' ');
                    float xx = float.Parse(v[1]) + x * 2; // TODO miks *2?
                    float yy = float.Parse(v[2]) + y * 2;
                    float zz = float.Parse(v[3]) + z * 2;
                    lines.Add("v " + xx + " " + yy + " " + zz + "\n");
                }
                else
                if (line.StartsWith("vt "))
                {
                    string[] uv = line.Split(' ');
                    float u = float.Parse(uv[1]) + x;
                    float v = float.Parse(uv[2]) + y;
                    lines.Add("vt " + u + " " + v + "\n");
                }
            }
            lines.Add("\n");

            globalIndexCount += maxI;
            globalUVCount += maxUV;
            return lines;
        }

        public static void CreateObj(string mapname)
        {
            List<string> mapstr = new List<string>();
            bool loadModels = true;

            string[] strs = System.IO.File.ReadAllLines(mapname);
            foreach (string line in strs)
            {
                // skip comments
                if (line.StartsWith("//"))
                    continue;

                if (line.Length == 0) // tyhjä rivi erottaa .obj tiedot ja kartan
                    loadModels = false;

                if (loadModels) // setup character with models
                {
                    string[] n = line.Split(' ');
                    Model m = new Model();
                    if (line[0] == ' ')
                        m.ch = ' ';
                    else m.ch = n[0][0];

                    m.name = n[n.Length - 1];
                    models.Add(m);
                }
                else
                {
                    mapstr.Add(line);
                }
            }

            Console.WriteLine("Creating map...");
            string outstr = "";
            int y = 0;
            foreach (string str in mapstr) // y
            {
                for (int x = 0; x < str.Length; x++)
                {
                    char ch = str[x];

                    // find model which ch is
                    foreach (Model m in models)
                    {
                        if (ch == m.ch)
                        {
                            List<string> data = m.Load(m.name, x, 0, y,
                                        globalIndexCount, globalUVCount, globalFaceCount);

                            foreach (string ss in data)
                                outstr += ss;

                            break;
                        }
                    }
                }
                y++;
            }

            outstr = "# obj created with makemap\n" + outstr;
            System.IO.File.WriteAllText(mapname + ".obj", outstr);
        }
    }
}
