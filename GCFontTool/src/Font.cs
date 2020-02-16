using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GameFormatReader.Common;
using Newtonsoft.Json;

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

        public int Ascent { get; set; }
        public int Descent { get; set; }

        public int CharacterWidth { get; set; }
        public int Leading { get; set; }

        public int ReplacementCode { get; set; }

        public List<Sheet> Sheets { get; set; }
        public List<GlyphBlock> GlyphBlocks { get; set; }

        public Font()
        {
            Sheets = new List<Sheet>();
            GlyphBlocks = new List<GlyphBlock>();
        }

        public void Load(string FileName)
        {
            string extension = Path.GetExtension(FileName);

            switch(extension)
            {
                case ".bfn":
                    using (FileStream strm = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                    {
                        EndianBinaryReader reader = new EndianBinaryReader(strm, Endian.Big);
                        LoadBFN(reader);
                    }
                    break;
                case ".json":
                    LoadJSON(FileName);
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
                    SaveBFN(FileName);
                    break;
                case ".json":
                    SaveJSON(FileName);
                    break;
                case ".fnt":
                    break;
            }
        }
    }
}
