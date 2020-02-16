using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;
using System.IO;

namespace BFNDump
{
    public partial class Font
    {
        private void LoadBFN(EndianBinaryReader reader)
        {
            // Get the block count
            reader.Skip(12);
            int block_count = reader.ReadInt32();
            reader.Skip(16);

            // There can be multiple INF1 blocks in a file, but only the last one is used.
            do
            {
                block_count--;
                reader.Skip(8);

                Encoding = (CodepointEncoding)reader.ReadInt16();
                Ascent = reader.ReadInt16();
                Descent = reader.ReadInt16();
                CharacterWidth = reader.ReadInt16();
                Leading = reader.ReadInt16();
                ReplacementCode = reader.ReadInt16();

                reader.Skip(12);

            } while (reader.PeekReadInt32() == 0x494E4631); // INF1

            for (int i = 0; i < block_count; i++)
            {
                int fourcc = reader.ReadInt32();

                switch (fourcc)
                {
                    // GLY1
                    case 0x474C5931:
                        Sheets.Add(new Sheet(reader));
                        break;
                    // MAP1
                    case 0x4D415031:
                        GlyphBlocks.Add(new GlyphBlock(reader));
                        break;
                    // WID1
                    case 0x57494431:
                        LoadWidths(reader);
                        break;
                }
            }
        }

        private void LoadWidths(EndianBinaryReader reader)
        {
            reader.SkipInt32();

            int first_code = reader.ReadInt16();
            int last_code = reader.ReadInt16();

            byte[] width_data = new byte[(last_code - first_code) * 2];
            for (int i = 0; i < (last_code - first_code) * 2; i += 2)
            {
                width_data[i] = reader.ReadByte();
                width_data[i + 1] = reader.ReadByte();
            }

            foreach (GlyphBlock b in GlyphBlocks)
            {
                if (b.IsCodeInBlock(first_code))
                {
                    foreach (Glyph g in b.Glyphs)
                    {
                        g.Kerning = width_data[g.CodePoint * 2];
                        g.Width = width_data[(g.CodePoint * 2) + 1];
                    }
                }
            }
        }

        private void SaveBFN(string FileName)
        {
            using(FileStream strm = new FileStream(FileName, FileMode.Create, FileAccess.Write))
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(strm, Endian.Big);

                WriteHeader(writer);
                WriteINF1(writer);

                foreach (Sheet s in Sheets)
                {
                    WriteGLY1(writer, s);
                }

                foreach (GlyphBlock b in GlyphBlocks)
                {
                    WriteMAP1(writer, b);
                }

                WriteWID1(writer);

                writer.BaseStream.Seek(8, SeekOrigin.Begin);
                writer.Write((int)writer.BaseStream.Length);
            }
        }

        private void WriteHeader(EndianBinaryWriter writer)
        {
            int block_count = 2 + GlyphBlocks.Count + Sheets.Count;

            writer.Write("FONTbfn1".ToCharArray());
            writer.Write(0);
            writer.Write(block_count);
            writer.Write((long)0);
            writer.Write((long)0);
        }

        private void WriteINF1(EndianBinaryWriter writer)
        {
            writer.Write("INF1".ToCharArray());
            writer.Write(32);
            writer.Write((short)Encoding);
            writer.Write((short)Ascent);
            writer.Write((short)Descent);
            writer.Write((short)CharacterWidth);
            writer.Write((short)Leading);
            writer.Write((short)ReplacementCode);
            writer.Write(0);
            writer.Write((long)0);
        }

        private void WriteGLY1(EndianBinaryWriter writer, Sheet sheet)
        {
            long start_offset = writer.BaseStream.Position;

            writer.Write("GLY1".ToCharArray());
            writer.Write(0);

            writer.Write((short)sheet.FirstCode);
            writer.Write((short)sheet.LastCode);
            writer.Write((short)sheet.CellWidth);
            writer.Write((short)sheet.CellHeight);
            writer.Write(sheet.Images[0].Length);
            writer.Write((short)sheet.TextureFormat);
            writer.Write((short)sheet.RowCount);
            writer.Write((short)sheet.ColumnCount);
            writer.Write((short)sheet.SheetWidth);
            writer.Write((short)sheet.SheetHeight);
            writer.Write((short)0);

            foreach (byte[] b in sheet.Images)
            {
                writer.Write(b);
            }

            writer.BaseStream.Seek(start_offset + 4, SeekOrigin.Begin);
            writer.Write((int)(writer.BaseStream.Length - start_offset));
            writer.BaseStream.Seek(0, SeekOrigin.End);
        }

        private void WriteMAP1(EndianBinaryWriter writer, GlyphBlock block)
        {
            long start_offset = writer.BaseStream.Position;

            writer.Write("MAP1".ToCharArray());
            writer.Write(0);
            writer.Write((short)block.Mapping);
            writer.Write((short)block.FirstCharacter);
            writer.Write((short)block.LastCharacter);

            switch (block.Mapping)
            {
                case GlyphMapping.Linear:
                    writer.Write((short)0);
                    writer.Write((long)0);
                    writer.Write((long)0);
                    break;
            }

            writer.BaseStream.Seek(start_offset + 4, SeekOrigin.Begin);
            writer.Write((int)(writer.BaseStream.Length - start_offset));
            writer.BaseStream.Seek(0, SeekOrigin.End);
        }

        private void WriteWID1(EndianBinaryWriter writer)
        {
            long start_offset = writer.BaseStream.Position;

            writer.Write("WID1".ToCharArray());
            writer.Write(0);
            writer.Write((short)GlyphBlocks[0].FirstCode);
            writer.Write((short)GlyphBlocks[GlyphBlocks.Count - 1].LastCode);

            foreach (GlyphBlock b in GlyphBlocks)
            {
                foreach (Glyph g in b.Glyphs)
                {
                    writer.Write((byte)g.Kerning);
                    writer.Write((byte)g.Width);
                }
            }

            writer.BaseStream.Seek(start_offset + 4, SeekOrigin.Begin);
            writer.Write((int)(writer.BaseStream.Length - start_offset));
            writer.BaseStream.Seek(0, SeekOrigin.End);
        }
    }
}
