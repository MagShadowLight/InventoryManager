using InventBox.Core.Interfaces;
using Microsoft.VisualBasic.FileIO;

namespace InventBox.Core;

public class CSVDataTransfer : IDataTransfer<string[]>
{
    public List<string> textList = new List<string>();
    public void Export(string[] text, string path)
    {
        using (var writer = new StreamWriter(path))
        {
            writer.WriteLine(string.Join(",", text));

        }
    }

    public string[] Import(string path)
    {
        using (TextFieldParser parser = new TextFieldParser(path))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields()!;
                foreach (string field in fields)
                {
                    textList.Add(field);
                }
            }
        }
        return null;
    }
}
