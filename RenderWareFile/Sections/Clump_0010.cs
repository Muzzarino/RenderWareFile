﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RenderWareFile
{
    public class Clump_0010 : RWSection
    {
        public ClumpStruct_0001 clumpStruct;
        public RWSection frameList;
        public GeometryList_001A geometryList;
        public List<RWSection> atomicList;
        public Extension_0003 clumpExtension;

        public Clump_0010 Read(BinaryReader binaryReader)
        {
            sectionIdentifier = Section.Clump;
            sectionSize = binaryReader.ReadInt32();
            renderWareVersion = binaryReader.ReadInt32();

            long startSectionPosition = binaryReader.BaseStream.Position;

            Section clumpStructSection = (Section)binaryReader.ReadInt32();
            if (clumpStructSection != Section.Struct) throw new Exception(binaryReader.BaseStream.Position.ToString());
            clumpStruct = new ClumpStruct_0001().Read(binaryReader);

            Section frameListSection = (Section)binaryReader.ReadInt32();
            if (frameListSection != Section.FrameList) throw new Exception(binaryReader.BaseStream.Position.ToString());
            frameList = new GenericSection().Read(binaryReader, frameListSection);

            Section geometryListSection = (Section)binaryReader.ReadInt32();
            if (geometryListSection != Section.GeometryList) throw new Exception(binaryReader.BaseStream.Position.ToString());
            geometryList = new GeometryList_001A().Read(binaryReader);

            atomicList = new List<RWSection>();
            for (int i = 0; i < clumpStruct.atomicCount; i++)
            {
                Section atomicListSection = (Section)binaryReader.ReadInt32();
                if (atomicListSection != Section.Atomic) throw new Exception(binaryReader.BaseStream.Position.ToString());
                atomicList.Add(new GenericSection().Read(binaryReader, atomicListSection));
            }

            Section clumpExtensionSection = (Section)binaryReader.ReadInt32();
            if (clumpExtensionSection == Section.Extension)
                clumpExtension = new Extension_0003().Read(binaryReader);

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            return this;
        }

        public override void SetListBytes(int fileVersion, ref List<byte> listBytes)
        {
            sectionIdentifier = Section.Clump;

            listBytes.AddRange(clumpStruct.GetBytes(fileVersion));
            listBytes.AddRange(frameList.GetBytes(fileVersion));
            listBytes.AddRange(geometryList.GetBytes(fileVersion));
            for (int i = 0; i < atomicList.Count; i++)
                listBytes.AddRange(atomicList[i].GetBytes(fileVersion));
            listBytes.AddRange(clumpExtension.GetBytes(fileVersion));
        }
    }

    public class ClumpStruct_0001 : RWSection
    {
        public int atomicCount;
        public int lightCount;
        public int cameraCount;

        public ClumpStruct_0001 Read(BinaryReader binaryReader)
        {
            sectionIdentifier = Section.Struct;
            sectionSize = binaryReader.ReadInt32();
            renderWareVersion = binaryReader.ReadInt32();

            atomicCount = binaryReader.ReadInt32();
            lightCount = binaryReader.ReadInt32();
            cameraCount = binaryReader.ReadInt32();

            return this;
        }

        public override void SetListBytes(int fileVersion, ref List<byte> listBytes)
        {
            sectionIdentifier = Section.Struct;

            listBytes.AddRange(BitConverter.GetBytes(atomicCount));
            listBytes.AddRange(BitConverter.GetBytes(lightCount));
            listBytes.AddRange(BitConverter.GetBytes(cameraCount));
        }
    }
}
