﻿using System;
using System.Collections.Generic;
using System.IO;

namespace RenderWareFile.Sections
{
    public class TextureNative_0015 : RWSection
    {
        public TextureNativeStruct_0001 textureNativeStruct;
        public Extension_0003 textureNativeExtension;
        
        public TextureNative_0015 Read(BinaryReader binaryReader)
        {
            sectionIdentifier = Section.TextureNative;
            sectionSize = binaryReader.ReadInt32();
            renderWareVersion = binaryReader.ReadInt32();

            long startSectionPosition = binaryReader.BaseStream.Position;

            Section textureNativeStructSection = (Section)binaryReader.ReadInt32();
            if (textureNativeStructSection != Section.Struct) throw new Exception(binaryReader.BaseStream.Position.ToString());
            textureNativeStruct = new TextureNativeStruct_0001().Read(binaryReader);
            
            Section textureNativeExtensionSection = (Section)binaryReader.ReadInt32();
            if (textureNativeExtensionSection == Section.Extension)
                textureNativeExtension = new Extension_0003().Read(binaryReader);

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            return this;
        }

        public override void SetListBytes(int fileVersion, ref List<byte> listBytes)
        {
            sectionIdentifier = Section.TextureNative;

            listBytes.AddRange(textureNativeStruct.GetBytes(fileVersion));
            listBytes.AddRange(textureNativeExtension.GetBytes(fileVersion));
        }
    }
}