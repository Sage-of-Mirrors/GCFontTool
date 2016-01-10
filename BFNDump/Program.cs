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
            using (FileStream stream = new FileStream(@"C:\Program Files (x86)\SZS Tools\TestFont\TestFont.fnt", FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(font));
                font fnt = (font)serializer.Deserialize(stream);

                fontPages pages = (fontPages)fnt.Items[2];
                fontChars chars = (fontChars)fnt.Items[3];

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

                List<byte> finalCompiledSheet = new List<byte>();

                foreach (Bitmap bmp in alignedGlyphSheets)
                {
                    finalCompiledSheet.AddRange(BinaryTextureImage.EncodeData(bmp, (uint)bmp.Width, (uint)bmp.Height, BinaryTextureImage.TextureFormats.I4));
                }

                byte[] finalSheetArray = BinaryTextureImage.DecodeData(new EndianBinaryReader(finalCompiledSheet.ToArray(), Endian.Big), 160, 160 * 4, BinaryTextureImage.TextureFormats.I4);

                BinaryTextureImage tex = new BinaryTextureImage();

                tex.SaveImageToDisk(@"C:\Program Files (x86)\SZS Tools\TestFont\finalSheetTest.png", finalSheetArray, 160, 160 * 4);
            }
        }
    }
}
