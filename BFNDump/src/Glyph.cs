using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BFNDump
{
    public class Glyph
    {
        public string Name { get; set; }

        public int CodePoint { get; set; }

        public int Kerning { get; set; }
        public int Width { get; set; }

        public Point Location { get; set; }
    }
}
