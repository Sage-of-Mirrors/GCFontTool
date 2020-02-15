using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameFormatReader.Common;
using System.IO;

namespace BFNDump
{
    public class Sheet
    {
        public List<byte[]> Images { get; set; }

        public int SheetWidth { get; set; }
        public int SheetHeight { get; set; }

        public int CellWidth { get; set; }
        public int CellHeight { get; set; }

        public int RowCount { get; set; }
        public int ColumnCount { get; set; }

        public BinaryTextureImage.TextureFormats TextureFormat { get; set; }

        public Sheet(EndianBinaryReader reader)
        {
            Images = new List<byte[]>();

            reader.Skip(4);

            int first_code = reader.ReadInt16();
            int last_code = reader.ReadInt16();

            CellWidth = reader.ReadInt16();
            CellHeight = reader.ReadInt16();

            int total_sheet_size = reader.ReadInt32();

            TextureFormat = (BinaryTextureImage.TextureFormats)reader.ReadInt16();

            RowCount = reader.ReadInt16();
            ColumnCount = reader.ReadInt16();

            SheetWidth = reader.ReadInt16();
            SheetHeight = reader.ReadInt16();

            reader.SkipInt16();

            int num_sheets = (last_code - first_code) / (RowCount * ColumnCount) + 1;

            for (int i = 0; i < num_sheets; i++)
            {
                Images.Add(BinaryTextureImage.DecodeData(reader, (uint)SheetWidth, (uint)SheetHeight, TextureFormat));

                BinaryTextureImage aa = new BinaryTextureImage();
                aa.SaveImageToDisk("C:\\Games\\test_" + i + ".png", Images[i], SheetWidth, SheetHeight);
            }
        }
    }
}
