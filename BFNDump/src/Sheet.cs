using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BFNDump
{
    public class Sheet
    {
        public Bitmap Image { get; set; }

        public int SheetWidth { get; set; }
        public int SheetHeight { get; set; }

        public int CellWidth { get; set; }
        public int CellHeight { get; set; }

        public int RowCount { get; set; }
        public int ColumnCount { get; set; }

        public int TextureFormat { get; set; }

    }
}
