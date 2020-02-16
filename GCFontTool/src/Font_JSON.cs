using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace BFNDump
{
    public partial class Font
    {
        private void LoadJSON(string FileName)
        {
            string json = File.ReadAllText(FileName);
            JsonConvert.PopulateObject(json, this);

            string dir_name = Path.Combine(Path.GetDirectoryName(FileName), "sheets");

            for (int i = 0; i < Sheets.Count; i++)
            {
                Sheets[i].LoadImages(i, dir_name);
            }
        }

        private void SaveJSON(string FileName)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(FileName, json);

            string sheet_path = Path.Combine(Path.GetDirectoryName(FileName), "sheets");

            for (int i = 0; i < Sheets.Count; i++)
            {
                Sheets[i].SaveImage(i, sheet_path);
            }
        }
    }
}
