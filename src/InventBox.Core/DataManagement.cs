using System.Globalization;
using CsvHelper;
using InventBox.Core.Interfaces;

namespace InventBox.Core;

public class DataManagement<T> : IDataManagement<List<T>>
{
    private FileLogger _logger = new FileLogger();
    private string _loggerPath = string.Empty;
    public DataManagement(string path)
    {
        _loggerPath = path;
    }
    public void Save(List<T> values, string path)
    {
        try {
            _logger.Logs("Writing the list of item in csv file.", _loggerPath);
            if (values.Count == 0)
                throw new Exception("data is empty");
            using (var writer = new StreamWriter(path))
            using (var csvParser = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csvParser.WriteHeader<T>();
                csvParser.NextRecord();
                foreach(var value in values)
                {
                    csvParser.WriteRecord(value);
                    csvParser.NextRecord();
                }
            }
            _logger.Logs("List of items have been stored", _loggerPath);
        } catch (Exception ex)
        {
            _logger.Error(ex.Message, _loggerPath);
        }
    }

    public List<T> Load(string path)
    {
        try {
            _logger.Logs("Attempting to read the data from the csv file.", _loggerPath);
            List<T> values = new List<T>();
            using (var reader = new StreamReader(path))
            using (var csvParser = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                while (csvParser.Read()) {
                    var records = csvParser.GetRecord<T>();
                    values.Add(records);
                }
            }
            _logger.Logs("Item data have been imported.", _loggerPath);
            return values;
        } catch (Exception ex)
        {
            _logger.Error(ex.Message, _loggerPath);
            return null;
        }
    }
}
