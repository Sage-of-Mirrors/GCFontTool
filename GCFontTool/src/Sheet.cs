using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameFormatReader.Common;
using System.IO;
using Newtonsoft.Json;

namespace BFNDump
{
    public class Sheet
    {
        [JsonIgnore]
        public List<byte[]> Images { get; set; }

        public int FirstCode { get; set; }
        public int LastCode { get; set; }

        public int SheetWidth { get; set; }
        public int SheetHeight { get; set; }

        public int CellWidth { get; set; }
        public int CellHeight { get; set; }

        public int RowCount { get; set; }
        public int ColumnCount { get; set; }

        public BinaryTextureImage.TextureFormats TextureFormat { get; set; }

        public Sheet()
        {
            Images = new List<byte[]>();
        }

        public Sheet(EndianBinaryReader reader)
        {
            Images = new List<byte[]>();

            reader.Skip(4);

            FirstCode = reader.ReadInt16();
            LastCode = reader.ReadInt16();

            CellWidth = reader.ReadInt16();
            CellHeight = reader.ReadInt16();

            int total_sheet_size = reader.ReadInt32();

            TextureFormat = (BinaryTextureImage.TextureFormats)reader.ReadInt16();

            RowCount = reader.ReadInt16();
            ColumnCount = reader.ReadInt16();

            SheetWidth = reader.ReadInt16();
            SheetHeight = reader.ReadInt16();

            reader.SkipInt16();

            int num_sheets = (LastCode - FirstCode) / (RowCount * ColumnCount) + 1;

            for (int i = 0; i < num_sheets; i++)
            {
                Images.Add(BinaryTextureImage.DecodeData(reader, (uint)SheetWidth, (uint)SheetHeight, TextureFormat));
            }
        }

        public void LoadImages(int index, string directory)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(directory, $"sheet_{ index }_*.png");

            for (int i = 0; i < files.Count(); i++)
            {
                string img_name = files.ElementAt(i);
                Bitmap img = new Bitmap(img_name);

                Images.Add(BinaryTextureImage.EncodeData(img, (uint)SheetWidth, (uint)SheetHeight, TextureFormat));
            }
        }

        public void SaveImage(int index, string directory)
        {
            BinaryTextureImage aa = new BinaryTextureImage();
            
            for (int i = 0; i < Images.Count; i++)
            {
                aa.SaveImageToDisk(Path.Combine(directory, $"sheet_{ index }_{ i }.png"), Images[i], SheetWidth, SheetHeight);
            }
        }

        public bool IsCodeInSheet(int codepoint)
        {
            if (codepoint >= FirstCode && codepoint <= LastCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
