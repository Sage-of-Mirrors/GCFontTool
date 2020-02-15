using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;

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

                Mapping = (GlyphMapping)reader.ReadInt16();
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
                        LoadGlyphs(reader);
                        break;
                    // WID1
                    case 0x57494431:
                        LoadWidths(reader);
                        break;
                }
            }
        }
        private void LoadGlyphs(EndianBinaryReader reader)
        {

        }

        private void LoadWidths(EndianBinaryReader reader)
        {

        }

        private void SaveBFN()
        {

        }
    }
}
