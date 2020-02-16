using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing.Imaging;
using System.Drawing;
using GameFormatReader.Common;

namespace BFNDump
{
    class Program
    {
        static void Main(string[] args)
        {
            Font f = new Font();
            f.Load(@"D:\SZS Tools\FontStuff\font.json");
            f.Save(@"D:\SZS Tools\FontStuff\font_winnebago.bfn");

            /*
            font fnt;

            fontPages pages;
            fontChars chars;

            byte[] finalSheetArray;

            List<byte> finalCompiledSheet = new List<byte>();

            using (FileStream stream = new FileStream(@"C:\Program Files (x86)\SZS Tools\TestFont\TestFont.fnt", FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(font));
                fnt = (font)serializer.Deserialize(stream);

                pages = (fontPages)fnt.Items[2];
                chars = (fontChars)fnt.Items[3];

                List<Bitmap> alignedGlyphSheets = new List<Bitmap>();

                int charCount = 0;

                while (charCount != chars.count)
                {
                    Bitmap alignedGlyphSheet = new Bitmap(160, 160);
                    int baseXPixel = 0;
                    int baseYPixel = 0;
                    int numCharsBlockY = 0;
                    int numCharsBlockX = 0;

                    for (int alignedY = 0; alignedY < alignedGlyphSheet.Height; alignedY++)
                    {
                        for (int alignedX = 0; alignedX < alignedGlyphSheet.Width; alignedX++)
                        {
                            alignedGlyphSheet.SetPixel(alignedX, alignedY, Color.Black);
                        }
                    }

                    for (int glyphBlockY = 0; glyphBlockY < 5; glyphBlockY++)
                    {
                        if (charCount >= chars.count)
                            break;

                        baseXPixel = 0;

                        for (int glyphBlockX = 0; glyphBlockX < 5; glyphBlockX++)
                        {
                            using (FileStream glyphSheet = new FileStream(@"C:\Program Files (x86)\SZS Tools\TestFont\" + pages.page[chars.@char[charCount].page].file, FileMode.Open))
                            {

                                using (Bitmap sourceGlyphSheet = new Bitmap(glyphSheet))
                                {
                                    for (int y = 0; y < chars.@char[charCount].height; y++)
                                    {
                                        for (int x = 0; x < chars.@char[charCount].width; x++)
                                        {
                                            int charOffsetX = 32 / 2 - chars.@char[charCount].width / 2;
                                            int charOffsetY = 32 / 2 - chars.@char[charCount].height / 2;

                                            alignedGlyphSheet.SetPixel(baseXPixel + charOffsetX + x, baseYPixel + charOffsetY + y,
                                                sourceGlyphSheet.GetPixel(chars.@char[charCount].x + x,
                                                chars.@char[charCount].y + y));
                                        }
                                    }

                                    charCount++;

                                    if (charCount >= chars.count)
                                        break;
                                }
                            }

                            numCharsBlockX++;

                            baseXPixel += 32;
                        }

                        numCharsBlockY++;

                        baseYPixel += 32;
                    }

                    alignedGlyphSheets.Add(alignedGlyphSheet);

                    alignedGlyphSheet.Save(@"C:\Program Files (x86)\SZS Tools\TestFont\alignedSheet_at_" + charCount + ".bmp");
                }

                foreach (Bitmap bmp in alignedGlyphSheets)
                {
                    finalCompiledSheet.AddRange(BinaryTextureImage.EncodeData(bmp, (uint)bmp.Width, (uint)bmp.Height, BinaryTextureImage.TextureFormats.I4));
                }

                finalSheetArray = BinaryTextureImage.DecodeData(new EndianBinaryReader(finalCompiledSheet.ToArray(), Endian.Big), 160, 160 * 4, BinaryTextureImage.TextureFormats.I4);

                BinaryTextureImage tex = new BinaryTextureImage();

                tex.SaveImageToDisk(@"C:\Program Files (x86)\SZS Tools\TestFont\finalSheetTest.png", finalSheetArray, 160, 160 * 4);
            }

            Bitmap pngOrig = new Bitmap(@"C:\Program Files (x86)\SZS Tools\fontres.arc_dir\fontres\rock_24_20_4i_usa_noheader.bmp");

            byte[] origIdea = BinaryTextureImage.EncodeData(pngOrig, 128, 1152, BinaryTextureImage.TextureFormats.I4);

            using (EndianBinaryWriter writ = new EndianBinaryWriter(new FileStream(@"C:\Program Files (x86)\SZS Tools\TestFont\testOutput.bin", FileMode.Create), Endian.Big))
            {
                writ.Write(origIdea);
            }

            using(EndianBinaryWriter writer = new EndianBinaryWriter(new FileStream(@"C:\Program Files (x86)\SZS Tools\TestFont\test.bfn",FileMode.Create), Endian.Big))
            {
                WriteBFN(writer, chars, finalCompiledSheet.ToArray());
            }*/
        }

        private static void WriteBFN(EndianBinaryWriter stream, fontChars characters, byte[] encodedImageData)
        {
            stream.Write("FONTbfn1".ToCharArray());

            stream.Write((int)0);
            
            stream.Write((int)4);

            for (int i = 0; i < 4; i++)
            {
                stream.Write((int)0);
            }

            stream.Write("INF1".ToCharArray());

            stream.Write((int)0x20);

            stream.Write((int)0x12);

            stream.Write((short)6);

            stream.Write((short)0x15);

            stream.Write((short)0x18);

            stream.Write((short)0);

            for (int j = 0; j < 3; j++)
            {
                stream.Write((int)0);
            }

            stream.Write("GLY1".ToCharArray());

            stream.Write((int)(encodedImageData.Count() + 0x20));

            stream.Write((int)characters.count);

            stream.Write((short)32);

            stream.Write((short)32);

            stream.Write((int)0x2000);

            stream.Write((short)0);

            stream.Write((short)5);

            stream.Write((short)20);

            stream.Write((short)160);

            stream.Write((short)0x280);

            stream.Write((short)0);

            stream.Write(encodedImageData);

            stream.Write("MAP1".ToCharArray());

            stream.Write((int)0x20);

            stream.Write((short)0);

            stream.Write((short)characters.@char[0].id);

            stream.Write((short)characters.@char[characters.count - 1].id);

            stream.Write((short)0);

            for (int k = 0; k < 4; k++)
            {
                stream.Write((int)0);
            }

            stream.Write("WID1".ToCharArray());

            stream.Write((int)(0x20 + (characters.count * 2)));

            stream.Write((int)characters.count);

            foreach (fontCharsChar chara in characters.@char)
            {
                if (chara.xoffset < 0)
                {
                    stream.Write((byte)-chara.xoffset);

                    stream.Write((byte)chara.xadvance);
                }
                
                else
                {
                    stream.Write((byte)chara.xoffset);

                    stream.Write((byte)chara.xadvance);
                }
            }

            stream.BaseStream.Position = 8;

            stream.Write((int)stream.BaseStream.Length);
        }
    }
}
