﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RenderWareFile
{
    public enum Section : int
    {
        None = 0x0,
        Struct = 0x1,
        String = 0x2,
        Extension = 0x3,
        Texture = 0x6,
        Material = 0x7,
        MaterialList = 0x8,
        AtomicSection = 0x9,
        PlaneSection = 0xA,
        World = 0xB,
        FrameList = 0xE,
        Geometry = 0xF,
        Clump = 0x10,
        Atomic = 0x14,
        GeometryList = 0x1A,
        ChunkGroupStart = 0x29,
        ChunkGroupEnd = 0x2A,
        BinMeshPLG = 0x50E,
        NativeDataPLG = 0x510
    }

    public abstract class RWSection
    {
        public Section sectionIdentifier;
        public int sectionSize;
        public int renderWareVersion;

        public byte[] GetBytes(int fileVersion)
        {
            List<byte> listBytes = new List<byte>()
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0
            };

            SetListBytes(fileVersion, ref listBytes);

            sectionSize = listBytes.Count() - 0xC;
            renderWareVersion = fileVersion;

            listBytes[0] = BitConverter.GetBytes((int)sectionIdentifier)[0];
            listBytes[1] = BitConverter.GetBytes((int)sectionIdentifier)[1];
            listBytes[2] = BitConverter.GetBytes((int)sectionIdentifier)[2];
            listBytes[3] = BitConverter.GetBytes((int)sectionIdentifier)[3];
            listBytes[4] = BitConverter.GetBytes(sectionSize)[0];
            listBytes[5] = BitConverter.GetBytes(sectionSize)[1];
            listBytes[6] = BitConverter.GetBytes(sectionSize)[2];
            listBytes[7] = BitConverter.GetBytes(sectionSize)[3];
            listBytes[8] = BitConverter.GetBytes(renderWareVersion)[0];
            listBytes[9] = BitConverter.GetBytes(renderWareVersion)[1];
            listBytes[10] = BitConverter.GetBytes(renderWareVersion)[2];
            listBytes[11] = BitConverter.GetBytes(renderWareVersion)[3];

            return listBytes.ToArray();
        }

        public abstract void SetListBytes(int fileVersion, ref List<byte> listBytes);
    }

    public class GenericSection : RWSection
    {
        byte[] data;

        public GenericSection Read(BinaryReader binaryReader, Section section)
        {
            sectionIdentifier = section;
            sectionSize = binaryReader.ReadInt32();
            renderWareVersion = binaryReader.ReadInt32();

            data = binaryReader.ReadBytes(sectionSize);

            return this;
        }

        public override void SetListBytes(int fileVersion, ref List<byte> listBytes)
        {
            sectionIdentifier = Section.None;
            throw new NotImplementedException();
        }
    }

    public struct Vertex3
    {
        public float X;
        public float Y;
        public float Z;

        public Vertex3(float a, float b, float c)
        {
            X = a;
            Y = b;
            Z = c;
        }
    }

    public struct TextCoord
    {
        public float X;
        public float Y;

        public TextCoord(float a, float b)
        {
            X = a;
            Y = b;
        }
    }

    public struct Color
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color(byte[] v)
        {
            R = v[0];
            G = v[1];
            B = v[2];
            A = v[3];
        }

        public Color(int a)
        {
            this = new Color(BitConverter.GetBytes(a));
        }

        public static Color FromString(string s)
        {
            if (s.Length != 8) throw new InvalidDataException();

            Color c;
            c.R = Convert.ToByte(new string(new char[] { s[0], s[1] }), 16);
            c.G = Convert.ToByte(new string(new char[] { s[2], s[3] }), 16);
            c.B = Convert.ToByte(new string(new char[] { s[4], s[5] }), 16);
            c.A = Convert.ToByte(new string(new char[] { s[6], s[7] }), 16);            
            return c;
        }

        public static explicit operator int(Color v)
        {
            return BitConverter.ToInt32(new byte[] { v.R, v.G, v.B, v.A }, 0);
        }

        public override string ToString()
        {
            return String.Format("{0, 2:X2}{1, 2:X2}{2, 2:X2}{3, 2:X2}", R, G, B, A);
        }
    }

    public struct Triangle
    {
        public ushort materialIndex;
        public ushort vertex1;
        public ushort vertex2;
        public ushort vertex3;

        public Triangle(ushort m, ushort v1, ushort v2, ushort v3)
        {
            materialIndex = m;
            vertex1 = v1;
            vertex2 = v2;
            vertex3 = v3;
        }
    }

    public static class General
    {
        public static float Switch(float f)
        {
            byte[] a = BitConverter.GetBytes(f).Reverse().ToArray();
            return BitConverter.ToSingle(a, 0);
        }

        public static int Switch(int f)
        {
            byte[] a = BitConverter.GetBytes(f).Reverse().ToArray();
            return BitConverter.ToInt32(a, 0);
        }

        public static short Switch(short f)
        {
            byte[] a = BitConverter.GetBytes(f).Reverse().ToArray();
            return BitConverter.ToInt16(a, 0);
        }

        public static List<int> MaterialList;
    }
}
