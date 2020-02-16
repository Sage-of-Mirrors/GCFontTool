using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFormatReader.Common;

namespace BFNDump
{
    public class GlyphBlock
    {
        public int FirstCharacter { get; set; }
        public int LastCharacter { get; set; }

        public int FirstCode { get; set; }
        public int LastCode { get; set; }

        public GlyphMapping Mapping { get; set; }

        public List<Glyph> Glyphs { get; set; }

        public GlyphBlock()
        {
            Glyphs = new List<Glyph>();
        }

        public GlyphBlock(EndianBinaryReader reader)
        {
            Glyphs = new List<Glyph>();

            reader.SkipInt32();

            Mapping = (GlyphMapping)reader.ReadInt16();
            FirstCharacter = reader.ReadInt16();
            LastCharacter = reader.ReadInt16();

            int entry_count = reader.ReadInt16();

            reader.Skip(16);

            switch (Mapping)
            {
                case GlyphMapping.Linear:
                    MapLinear();
                    break;
                case GlyphMapping.KanjiLinear:
                    int base_code = 796;
                    if (entry_count == 1)
                    {
                        base_code = reader.ReadInt16();
                    }

                    MapKanjiLinear(base_code);
                    break;
                case GlyphMapping.Table:
                    MapTable(entry_count);
                    break;
                case GlyphMapping.Map:
                    MapMap();
                    break;
            }
        }

        private void MapLinear()
        {
            FirstCode = 0;
            LastCode = (LastCharacter - FirstCharacter);

            for (int i = FirstCharacter; i <= LastCharacter; i++)
            {
                Glyphs.Add(new Glyph(i, i - FirstCharacter));
            }
        }

        private void MapKanjiLinear(int base_code)
        {
            for (int i = FirstCharacter; i <= LastCharacter; i++)
            {
                int lead_byte = ((i >> 8) & 255);
                int trail_byte = ((i) & 255);

                int index = (trail_byte - 64);

                if (index >= 64)
                {
                    index--;
                }

                int final_code = base_code + index + ((lead_byte - 136) * 188 - 94);
                Glyphs.Add(new Glyph(i, final_code));
            }
        }

        private void MapTable(int entry_count)
        {

        }
        
        private void MapMap()
        {

        }

        public void ReadGlyphData(EndianBinaryReader reader)
        {
            foreach (Glyph g in Glyphs)
            {
                g.Kerning = reader.ReadByte();
                g.Width = reader.ReadByte();
            }
        }

        public bool IsCodeInBlock(int codepoint)
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
