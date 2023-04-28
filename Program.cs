using System;
using System.IO;
using System.Text;

namespace Pat_quick_editor
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Pat pat = new Pat();
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-load":
                        pat.loadFile(args[i + 1]);
                        i++;
                        break;
                    case "-uvRefactor":
                        //Console.WriteLine("Uv refactor");
                        int originalX = Int32.Parse(args[i + 1]) / 256;
                        int originalY = Int32.Parse(args[i + 2]) / 256;
                        int newX = Int32.Parse(args[i + 3]) / 256;
                        int newY = Int32.Parse(args[i + 4]) / 256;
                        i += 4;
                        for(int j = 0; j < pat.objects.Count; j++)
                        {
                            //if(j != 0)
                                //Console.WriteLine(pat.objects[pat.objectIds[j - 1]].uvX);
                            //Console.WriteLine(pat.objects[pat.objectIds[j]].uvX);
                            //Console.ReadLine();
                            int x = pat.objects[pat.objectIds[j]].uvX * originalX;
                            x /= newX;
                            pat.objects[pat.objectIds[j]].uvX = x;
                            //Console.WriteLine(pat.objects[pat.objectIds[j]].uvX);
                            //Console.ReadLine();
                            int y = pat.objects[pat.objectIds[j]].uvY * originalY;
                            y /= newY;
                            pat.objects[pat.objectIds[j]].uvY = y;
                            int w = pat.objects[pat.objectIds[j]].uvW * originalX;
                            w /= newX;
                            pat.objects[pat.objectIds[j]].uvW = w;
                            int h = pat.objects[pat.objectIds[j]].uvH * originalY;
                            h /= newY;
                            pat.objects[pat.objectIds[j]].uvH = h;
                        }
                        break;
                    case "-addUvOnTex":
                        //Console.WriteLine("Add uv on tex");
                        int tex = Int32.Parse(args[i + 1]);
                        int addX = Int32.Parse(args[i + 2]);
                        int addY = Int32.Parse(args[i + 3]);
                        i += 3;
                        for(int j = 0; j < pat.objects.Count; j++)
                        {
                            if (pat.objects[pat.objectIds[j]].textureIndex != tex)
                                continue;
                            pat.objects[pat.objectIds[j]].uvX += addX;
                            pat.objects[pat.objectIds[j]].uvY += addY;
                        }
                        break;
                    case "-changeTexId":
                        int tex1 = Int32.Parse(args[i + 1]);
                        int tex2 = Int32.Parse(args[i + 2]);
                        i += 2;
                        for (int j = 0; j < pat.objects.Count; j++)
                        {
                            if (pat.objects[pat.objectIds[j]].textureIndex != tex1)
                                continue;
                            pat.objects[pat.objectIds[j]].textureIndex = tex2;
                        }
                        break;
                    case "-save":
                        //Console.WriteLine("save");
                        pat.save(args[i + 1]);
                        i++;
                        break;
                }
            }
        }


    }
}