using System;
using System.Globalization;
using CsvHelper;
using InventBox.Core.Interfaces;
using InventBox.Core.Models;

namespace InventBox.Core.DataTransfer;

public class CategoryDataTransfer : IDataTransfer<List<Category>>
{
    public void Export(List<Category> categories, string path)
    {
        using (var writer = new StreamWriter(path))
        using (var csvParser = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csvParser.WriteHeader<Category>();
            csvParser.NextRecord();
            foreach (var category in categories)
            {
                csvParser.WriteRecord(category);
                csvParser.NextRecord();
            }
        }
    }

    public List<Category> Import(string path)
    {
        List<Category> categories = new List<Category>();
        using (var reader = new StreamReader(path))
        using (var csvParser = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            while (csvParser.Read())
            {
                var records = csvParser.GetRecord<Category>();
                categories.Add(records);
            }
        }
        return categories;
    }
}
