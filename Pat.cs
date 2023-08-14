using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Pat_quick_editor
{
    #region structs
    public class PR
    {
        public int posX = 0;
        public int posY = 0;
        public bool additive = false;
        public byte flip = 0;
        public bool filter = false;
        public float scaleX = 1;
        public float scaleY = 1;
        public byte[] color = { 255, 255, 255, 255 };
        public byte[] colorOverlay = { 0, 0, 0, 0 };
        public int priority = 0;
        public int ppId = 0;
        public float rotX = 0;
        public float rotY = 0;
        public float rotZ = 0;
    }

    public class PP
    {
        public string name = "";
        public int centerX = 0;
        public int centerY = 0;
        public int uvX = 0;
        public int uvY = 0;
        public int uvW = 0;
        public int uvH = 0;
        public int sizeX = 0;
        public int sizeY = 0;
        public ushort ppte1 = 0;
        public ushort ppte2 = 0;
        public int paletteNum = 0;
        public int textureIndex = 0;
        public int shapeIndex = 0;
    }

    public class P_
    {
        public string name = "";
        public Dictionary<int, PR> layers = new Dictionary<int, PR>();
        public List<int> layerIds = new List<int>();
    }

    public class Shape
    {
        public int type;
        public int vertexCount;
        public int vertexCount2;
        public int length;
        public int length2;
        public int radius;
        public int dRadius;
        public int width;
        public int dz;
    }

    public class VE
    {
        public int count = 1;
        public int entrySize = 16;
        public List<Shape> shapes = new List<Shape>();
        public List<string> names = new List<string>();
    }
    #endregion

    internal class Pat
    {
        public Dictionary<int, P_> sprites = new Dictionary<int, P_>();
        public List<int> spriteIds = new List<int>();
        public Dictionary<int, PP> objects = new Dictionary<int, PP>();
        public List<int> objectIds = new List<int>();
        public VE ve = new VE();
        public List<byte> pgRawData = new List<byte>();

        public void loadFile(string path)
        {
            sprites.Clear();
            spriteIds.Clear();
            objects.Clear();
            objectIds.Clear();
            ve = new VE();
            pgRawData.Clear();
            BinaryReader file = new BinaryReader(File.Open(path, FileMode.Open));
            if(Encoding.ASCII.GetString(file.ReadBytes(12)) != "PAniDataFile")
            {
                Console.WriteLine("Not a valid pat file. Ending.");
                Environment.Exit(1);
            }
            file.ReadBytes(20);
            
            while(Encoding.ASCII.GetString(file.ReadBytes(4)) != "_END")
            {
                string arg = Encoding.ASCII.GetString(file.ReadBytes(4));
                //Console.WriteLine(arg);
                //Console.ReadLine();
                switch(arg)
                {
                    default:
                        Console.WriteLine("Unknown Argument at loadFile: " + arg);
                        break;
                    case "_STR":
                        break;
                    case "P_ST":
                        loadSprite(file);
                        break;
                    case "PPST":
                        loadPP(file);
                        break;
                    case "VEST":
                        loadVE(file);
                        break;
                    case "PGST":
                        //Console.WriteLine("PGST");
                        pgRawData.AddRange(file.ReadBytes(Convert.ToInt32(file.BaseStream.Length - file.BaseStream.Position - 4)));
                        break;
                }
            }    

        }

        void loadSprite(BinaryReader file)
        {
            P_ sprite = new P_();
            int id = file.ReadInt32();

            string arg = Encoding.ASCII.GetString(file.ReadBytes(4));
            while (arg != "P_ED")
            {
                switch(arg)
                {
                    default:
                        Console.WriteLine("Unknown Argument at loadSprite: " + arg + " at position: " + file.BaseStream.Position);
                        break;
                    case "PANA":
                        byte length = file.ReadByte();
                        sprite.name = Encoding.ASCII.GetString(file.ReadBytes(length));
                        break;
                    case "PRST":
                        loadPR(sprite, file);
                        break;
                }
                arg = Encoding.ASCII.GetString(file.ReadBytes(4));
            }
            //Console.WriteLine(arg);
            //Console.ReadLine();
            sprites.Add(id, sprite);
            spriteIds.Add(id);
            file.BaseStream.Position -= 4;
        }

        void loadPR(P_ sprite, BinaryReader file)
        {
            PR layer = new PR();
            int id = file.ReadInt32();

            string arg = Encoding.ASCII.GetString(file.ReadBytes(4));
            while (arg != "PRED")
            {
                //Console.WriteLine(arg);
                //Console.ReadLine();
                switch (arg)
                {
                    default:
                        Console.WriteLine("Unknown Argument at loadPR: " + arg + " at position: " + file.BaseStream.Position);
                        break;
                    case "PRXY":
                        layer.posX = file.ReadInt32();
                        layer.posY = file.ReadInt32();
                        break;
                    case "PRAL":
                        layer.additive = file.ReadBoolean();
                        break;
                    case "PRRV":
                        layer.flip = file.ReadByte();
                        break;
                    case "PRFL":
                        layer.filter = file.ReadBoolean();
                        break;
                    case "PRZM":
                        layer.scaleX = file.ReadSingle();
                        layer.scaleY = file.ReadSingle();
                        break;
                    case "PRSP":
                        layer.colorOverlay = new byte[]{ file.ReadByte(), file.ReadByte(), file.ReadByte(), file.ReadByte() };
                        break;
                    case "PRPR":
                        layer.priority = file.ReadInt32();
                        break;
                    case "PRID":
                        layer.ppId = file.ReadInt32();
                        break;
                    case "PRCL":
                        layer.color = new byte[] { file.ReadByte(), file.ReadByte(), file.ReadByte(), file.ReadByte() };
                        break;
                    case "PRA3":
                        file.ReadBytes(4);
                        layer.rotX = file.ReadSingle();
                        layer.rotY = file.ReadSingle();
                        layer.rotZ = file.ReadSingle();
                        break;
                }
                arg = Encoding.ASCII.GetString(file.ReadBytes(4));
            }

            sprite.layers.Add(id, layer);
            sprite.layerIds.Add(id);
        }

        void loadPP(BinaryReader file)
        {
            PP pp = new PP();
            int id = file.ReadInt32();

            string arg = Encoding.ASCII.GetString(file.ReadBytes(4));
            while (arg != "PPED")
            {
                switch(arg)
                {
                    default:
                        Console.WriteLine("Unknown Argument at loadPP: " + arg + " at position: " + file.BaseStream.Position);
                        break;
                    case "PPNA":
                        byte length = file.ReadByte();
                        pp.name = Encoding.ASCII.GetString(file.ReadBytes(length));
                        break;
                    case "PPCC":
                        pp.centerX = file.ReadInt32();
                        pp.centerY = file.ReadInt32();
                        break;
                    case "PPUV":
                        pp.uvX = file.ReadInt32();
                        pp.uvY = file.ReadInt32();
                        pp.uvW = file.ReadInt32();
                        pp.uvH = file.ReadInt32();
                        break;
                    case "PPSS":
                        pp.sizeX = file.ReadInt32();
                        pp.sizeY = file.ReadInt32();
                        break;
                    case "PPTE":
                        pp.ppte1 = file.ReadUInt16();
                        pp.ppte2 = file.ReadUInt16();
                        break;
                    case "PPPA":
                        pp.paletteNum = file.ReadInt32();
                        break;
                    case "PPTP":
                        pp.textureIndex = file.ReadInt32();
                        break;
                    case "PPPP":
                        pp.shapeIndex = file.ReadInt32();
                        break;
                }
                arg = Encoding.ASCII.GetString(file.ReadBytes(4));
            }

            objects.Add(id, pp);
            objectIds.Add(id);

            file.BaseStream.Position -= 4;
        }

        void loadVE(BinaryReader file)
        {
            ve.count = file.ReadInt32();
            ve.entrySize = file.ReadInt32();
            for(int i = 0; i < ve.count; i++)
            {
                //Console.WriteLine(file.BaseStream.Position);
                //Console.ReadLine();
                Shape sh = new Shape();
                sh.type = file.ReadInt32();
                switch (sh.type)
                {
                    //Plane
                    case 1:
                    case 2:
                        file.ReadBytes(4 * (ve.entrySize - 1));
                        break;
                    //Ring
                    case 3:
                    case 4:
                        file.ReadBytes(8);
                        sh.radius = file.ReadInt32();
                        sh.width = file.ReadInt32();
                        sh.vertexCount = file.ReadInt32();
                        sh.length = file.ReadInt32();
                        sh.dz = file.ReadInt32();
                        sh.dRadius = file.ReadInt32();
                        file.ReadBytes(4 * (ve.entrySize - 9));
                        break;
                    //Sphere
                    case 5:
                        file.ReadBytes(8);
                        sh.radius = file.ReadInt32();
                        sh.vertexCount = file.ReadInt32();
                        sh.vertexCount2 = file.ReadInt32();
                        sh.length = file.ReadInt32();
                        sh.length2 = file.ReadInt32();
                        file.ReadBytes(4 * (ve.entrySize - 8));
                        break;
                    //Cone
                    case 6:
                        file.ReadBytes(8);
                        sh.radius = file.ReadInt32();
                        sh.dz = file.ReadInt32();
                        sh.vertexCount = file.ReadInt32();
                        sh.vertexCount2 = file.ReadInt32();
                        sh.length = file.ReadInt32();
                        file.ReadBytes(4 * (ve.entrySize - 8));
                        break;
                    default:
                        Console.WriteLine("Unknown Shape type: " + sh.type + " at position: " + file.BaseStream.Position);
                        break;
                }

                ve.shapes.Add(sh);
            }

            file.ReadBytes(4);
            for(int i = 0; i < ve.count; i++)
                ve.names.Add(Encoding.ASCII.GetString(file.ReadBytes(32)));
        }

        public void save(string path)
        {
            List<byte> buffer = new List<byte>();
            buffer.AddRange(Encoding.ASCII.GetBytes("PAniDataFile\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0_STR"));

            for (int i = 0; i < sprites.Count; i++)
            {
                P_ sprite = sprites[spriteIds[i]];
                buffer.AddRange(Encoding.ASCII.GetBytes("P_ST"));
                buffer.AddRange(BitConverter.GetBytes(spriteIds[i]));
                buffer.AddRange(Encoding.ASCII.GetBytes("PANA"));
                buffer.Add(Convert.ToByte(sprite.name.Length));
                buffer.AddRange(Encoding.ASCII.GetBytes(sprite.name));
                for (int j = 0; j < sprite.layers.Count; j++)
                {
                    PR layer = sprite.layers[sprite.layerIds[j]];
                    buffer.AddRange(Encoding.ASCII.GetBytes("PRST"));
                    buffer.AddRange(BitConverter.GetBytes(sprite.layerIds[j]));

                    #region pr write
                    if (layer.posX != 0 || layer.posY != 0)
                    {
                        buffer.AddRange(Encoding.ASCII.GetBytes("PRXY"));
                        buffer.AddRange(BitConverter.GetBytes(layer.posX));
                        buffer.AddRange(BitConverter.GetBytes(layer.posY));
                    }
                    if (layer.additive)
                    {
                        buffer.AddRange(Encoding.ASCII.GetBytes("PRAL"));
                        buffer.Add(Convert.ToByte(layer.additive));
                    }
                    if (layer.flip != 0)
                    {
                        buffer.AddRange(Encoding.ASCII.GetBytes("PRRV"));
                        buffer.Add(layer.flip);
                    }
                    if (layer.filter)
                    {
                        buffer.AddRange(Encoding.ASCII.GetBytes("PRFL"));
                        buffer.Add(Convert.ToByte(layer.filter));
                    }
                    if (layer.scaleX != 1 || layer.scaleY != 1)
                    {
                        buffer.AddRange(Encoding.ASCII.GetBytes("PRZM"));
                        buffer.AddRange(BitConverter.GetBytes(layer.scaleX));
                        buffer.AddRange(BitConverter.GetBytes(layer.scaleY));
                    }
                    if (layer.rotX != 0 || layer.rotY != 0 || layer.rotZ != 0)
                    {
                        buffer.AddRange(Encoding.ASCII.GetBytes("PRA3"));
                        buffer.AddRange(BitConverter.GetBytes(0));
                        buffer.AddRange(BitConverter.GetBytes(layer.rotX));
                        buffer.AddRange(BitConverter.GetBytes(layer.rotY));
                        buffer.AddRange(BitConverter.GetBytes(layer.rotZ));
                    }
                    if (layer.color[0] != 255 || layer.color[1] != 255 || layer.color[2] != 255 || layer.color[3] != 255)
                    {
                        buffer.AddRange(Encoding.ASCII.GetBytes("PRCL"));
                        buffer.AddRange(layer.color);
                    }
                    if (layer.colorOverlay[0] != 0 || layer.colorOverlay[1] != 0 || layer.colorOverlay[2] != 0 || layer.colorOverlay[3] != 0)
                    {
                        buffer.AddRange(Encoding.ASCII.GetBytes("PRSP"));
                        buffer.AddRange(layer.colorOverlay);
                    }
                    if (layer.priority != 0)
                    {
                        buffer.AddRange(Encoding.ASCII.GetBytes("PRPR"));
                        buffer.AddRange(BitConverter.GetBytes(layer.priority));
                    }
                    buffer.AddRange(Encoding.ASCII.GetBytes("PRID"));
                    buffer.AddRange(BitConverter.GetBytes(layer.ppId));
                    #endregion

                    buffer.AddRange(Encoding.ASCII.GetBytes("PRED"));
                }
                buffer.AddRange(Encoding.ASCII.GetBytes("P_ED"));
            }
            for(int i = 0; i < objects.Count; i++)
            {
                PP pp = objects[objectIds[i]];
                buffer.AddRange(Encoding.ASCII.GetBytes("PPST"));
                buffer.AddRange(BitConverter.GetBytes(objectIds[i]));
                buffer.AddRange(Encoding.ASCII.GetBytes("PPNA"));
                buffer.Add(Convert.ToByte(pp.name.Length));
                buffer.AddRange(Encoding.ASCII.GetBytes(pp.name));

                #region pp write
                buffer.AddRange(Encoding.ASCII.GetBytes("PPCC"));
                buffer.AddRange(BitConverter.GetBytes(pp.centerX));
                buffer.AddRange(BitConverter.GetBytes(pp.centerY));

                buffer.AddRange(Encoding.ASCII.GetBytes("PPUV"));
                buffer.AddRange(BitConverter.GetBytes(pp.uvX));
                buffer.AddRange(BitConverter.GetBytes(pp.uvY));
                buffer.AddRange(BitConverter.GetBytes(pp.uvW));
                buffer.AddRange(BitConverter.GetBytes(pp.uvH));

                buffer.AddRange(Encoding.ASCII.GetBytes("PPSS"));
                buffer.AddRange(BitConverter.GetBytes(pp.sizeX));
                buffer.AddRange(BitConverter.GetBytes(pp.sizeY));

                buffer.AddRange(Encoding.ASCII.GetBytes("PPTE"));
                buffer.AddRange(BitConverter.GetBytes(pp.ppte1));
                buffer.AddRange(BitConverter.GetBytes(pp.ppte2));

                if (pp.textureIndex != 0)
                {
                    buffer.AddRange(Encoding.ASCII.GetBytes("PPTP"));
                    buffer.AddRange(BitConverter.GetBytes(pp.textureIndex));
                }
                if (pp.paletteNum != 0)
                {
                    buffer.AddRange(Encoding.ASCII.GetBytes("PPPA"));
                    buffer.AddRange(BitConverter.GetBytes(pp.paletteNum));
                }
                if(pp.shapeIndex != 0)
                {
                    buffer.AddRange(Encoding.ASCII.GetBytes("PPPP"));
                    buffer.AddRange(BitConverter.GetBytes(pp.shapeIndex));
                }

                #endregion

                buffer.AddRange(Encoding.ASCII.GetBytes("PPED"));
            }
            buffer.AddRange(Encoding.ASCII.GetBytes("VEST"));
            buffer.AddRange(BitConverter.GetBytes(ve.count));
            buffer.AddRange(BitConverter.GetBytes(ve.entrySize));
            for(int i = 0; i < ve.count; i++)
            {
                Shape sh = ve.shapes[i];
                buffer.AddRange(BitConverter.GetBytes(sh.type));
                buffer.AddRange(BitConverter.GetBytes(0l));

                #region shape write
                switch(sh.type)
                {
                    case 1:
                    case 2:
                        for (int j = 0; j < 13; j++)
                            buffer.AddRange(BitConverter.GetBytes(0));
                        break;
                    case 3:
                    case 4:
                        buffer.AddRange(BitConverter.GetBytes(sh.radius));
                        buffer.AddRange(BitConverter.GetBytes(sh.width));
                        buffer.AddRange(BitConverter.GetBytes(sh.vertexCount));
                        buffer.AddRange(BitConverter.GetBytes(sh.length));
                        buffer.AddRange(BitConverter.GetBytes(sh.dz));
                        buffer.AddRange(BitConverter.GetBytes(sh.dRadius));
                        for (int j = 0; j < 7; j++)
                            buffer.AddRange(BitConverter.GetBytes(0));
                        break;
                    case 5:
                        buffer.AddRange(BitConverter.GetBytes(sh.radius));
                        buffer.AddRange(BitConverter.GetBytes(sh.vertexCount));
                        buffer.AddRange(BitConverter.GetBytes(sh.vertexCount2));
                        buffer.AddRange(BitConverter.GetBytes(sh.length));
                        buffer.AddRange(BitConverter.GetBytes(sh.length2));
                        for (int j = 0; j < 8; j++)
                            buffer.AddRange(BitConverter.GetBytes(0));
                        break;
                    case 6:
                        buffer.AddRange(BitConverter.GetBytes(sh.radius));
                        buffer.AddRange(BitConverter.GetBytes(sh.dz));
                        buffer.AddRange(BitConverter.GetBytes(sh.vertexCount));
                        buffer.AddRange(BitConverter.GetBytes(sh.vertexCount2));
                        buffer.AddRange(BitConverter.GetBytes(sh.length));
                        for (int j = 0; j < 8; j++)
                            buffer.AddRange(BitConverter.GetBytes(0));
                        break;

                }
                #endregion
            }
            buffer.AddRange(Encoding.ASCII.GetBytes("VNST"));
            for (int i = 0; i < ve.count; i++)
            {
                buffer.AddRange(Encoding.ASCII.GetBytes(ve.names[i]));
                /*
                if (ve.names[i].Length < 32)
                    for (int j = ve.names[i].Length; j < 32; j++)
                        buffer.Add(0x00);
                */
            }
            buffer.AddRange(Encoding.ASCII.GetBytes("VEED"));
            buffer.AddRange(Encoding.ASCII.GetBytes("PGST"));
            buffer.AddRange(pgRawData.ToArray());
            buffer.AddRange(Encoding.ASCII.GetBytes("_END"));

            BinaryWriter file = new BinaryWriter(File.Create(path));
            file.Write(buffer.ToArray());
            file.Close();
        }
    }
}
