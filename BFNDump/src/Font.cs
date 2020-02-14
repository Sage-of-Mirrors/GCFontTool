using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BFNDump
{
    public enum CodepointEncoding
    {
        Byte,
        Short,
        Mixed
    }

    public enum GlyphMapping
    {
        Linear,
        KanjiLinear,
        Table,
        Map
    }

    public partial class Font
    {
        public CodepointEncoding Encoding { get; set; }
        public GlyphMapping Mapping { get; set; }

        public int Ascent { get; set; }
        public int Descent { get; set; }

        public int CharacterWidth { get; set; }
        public int Leading { get; set; }

        public int ReplacementCode { get; set; }

        public List<Sheet> Sheets { get; set; }
        public List<Glyph> Glyphs { get; set; }

        public void Load(string FileName)
        {
            string extension = Path.GetExtension(FileName);

            switch(extension)
            {
                case ".bfn":
                    break;
                case ".json":
                    break;
                case ".fnt":
                    break;
            }

            string name_csv_path = Path.Combine(Path.GetDirectoryName(FileName), "glyph_names.csv");

            if (File.Exists(name_csv_path))
            {
                LoadNames(name_csv_path);
            }
        }

        private void LoadNames(string FileName)
        {

        }

        public void Save(string FileName)
        {
            string extension = Path.GetExtension(FileName);

            switch (extension)
            {
                case ".bfn":
                    break;
                case ".json":
                    break;
                case ".fnt":
                    break;
            }
        }
    }
}
