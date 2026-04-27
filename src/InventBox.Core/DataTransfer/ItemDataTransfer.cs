using System.Globalization;
using CsvHelper;
using InventBox.Core.Interfaces;
using InventBox.Core.Models;

namespace InventBox.Core.DataTransfer;

public class ItemDataTransfer : IDataTransfer<List<Items>>
{
    public void Export(List<Items> items, string path)
    {
        using (var writer = new StreamWriter(path))
        using (var csvParser = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csvParser.WriteHeader<Items>();
            csvParser.NextRecord();
            foreach(var item in items)
            {
                csvParser.WriteRecord(item);
                csvParser.NextRecord();
            }
        }
    }

    public List<Items> Import(string path)
    {
        List<Items> items = new List<Items>();
        using (var reader = new StreamReader(path))
        using (var csvParser = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            while (csvParser.Read()) {
                var records = csvParser.GetRecord<Items>();
                items.Add(records);
            }
        }
        return items;
    }
}
