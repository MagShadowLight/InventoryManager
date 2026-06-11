using System.Globalization;
using CsvHelper;
using InventBox.Core.Interfaces;
using InventBox.Core.Map;

namespace InventBox.Core;

/// <summary>
/// Provides the way to managing data.
/// </summary>
/// <typeparam name="T">The general variable of the data.</typeparam>
public class DataManagement<T> : IDataManagement<List<T>>
{
    private FileLogger _logger = new FileLogger();
    private string _loggerPath = string.Empty;
    /// <summary>
    /// Initialize a new instance of the <see cref="DataManagement"/>
    /// </summary>
    /// <param name="path">The path for the logger.</param>
    public DataManagement(string path)
    {
        _loggerPath = path;
    }

    /// <summary>
    /// Save the data into the file.
    /// </summary>
    /// <param name="values">The value that will be saved.</param>
    /// <param name="path">The path that will be place for data.</param>
    public void Save(List<T> values, string path)
    {
        try {
            _logger.Logs("Writing the Data in csv file.", _loggerPath);
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
            _logger.Logs("Data have been stored", _loggerPath);
        } catch (Exception ex)
        {
            _logger.Error(ex.Message, _loggerPath);
        }
    }

    /// <summary>
    /// Load the data from the file.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="isItem"></param>
    /// <returns></returns>
    public List<T> Load(string path, bool isItem = false)
    {
        List<T> values = new List<T>();
        try {
            _logger.Logs("Attempting to read the data from the csv file.", _loggerPath);
            using (var reader = new StreamReader(path))
            using (var csvParser = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                if (isItem)
                    csvParser.Context.RegisterClassMap<ItemMap>();
                while (csvParser.Read()) {
                    var records = csvParser.GetRecord<T>();
                    values.Add(records);
                }
            }
            _logger.Logs("Data have been imported successfully.", _loggerPath);
            return values;
        } catch (Exception ex)
        {
            _logger.Error($"Loading data failed. Message: {ex.Message}", _loggerPath);
            return values;
        }
    }
}
