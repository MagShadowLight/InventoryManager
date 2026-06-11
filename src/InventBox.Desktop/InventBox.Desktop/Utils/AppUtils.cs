using System;
using System.IO;
using Eto.Forms;
using InventBox.Core;

namespace InventBox.Desktop.Utils;
/// <summary>
/// Represents the utilities for application.
/// </summary>
/// <typeparam name="T"></typeparam>
public class AppUtils<T>
{
    private GridView? _grid;
    private JsonParser<T> jsonParser;
    private FileLogger _logger;
    private string _path;
    public AppUtils(FileLogger logger, string path, GridView grid = null, JsonParser<T> parser = null)
    {
        _logger = logger;
        _path = path;
        _grid = grid;
        jsonParser = parser;
    }
    /// <summary>
    /// Copy the category into the clipboard.
    /// </summary>
    public void OnCopy()
    {
        _logger.Logs("Copying data to clipboard.", _path);
        T SelectedValues = (T)_grid.SelectedItem;
        var jsonItem = jsonParser.ParseJson(SelectedValues);
        Clipboard.Instance.Clear();		
        Clipboard.Instance.Text = jsonItem;
        _logger.Logs("Data copied successfully", _path);
    }
    /// <summary>
    /// Create a button for the panel.
    /// </summary>
    /// <param name="text">The text for the button.</param>
    /// <param name="width">The width for the button.</param>
    /// <param name="height">The height for the button.</param>
    /// <param name="eventHandler">The handler for clicking the button.</param>
    /// <returns>The button to be placed.</returns>
    public Button AddButton(string text, int width, int height, Action eventHandler)
    {
        var command = new Command();
        command.Executed += (sender, eventArgs) => eventHandler();
        return new Button {Text = text, Width = width, Height = height, Command = command, Cursor = Cursors.Pointer};
    }
    /// <summary>
    /// Create the directory if the directory does not exists.
    /// </summary>
    /// <param name="dir">The path for the directory.</param>
    public void CreateDirectory(string dir)
    {
        if (!Directory.Exists(dir)) {
            _logger.Logs("Creating a directory.", _path);
            Directory.CreateDirectory(dir);
        }
    }
    /// <summary>
    /// Create the button menu item for context menu.
    /// </summary>
    /// <param name="text">The text for the menu item.</param>
    /// <param name="clickHandler">The handler for clicking into the menu.</param>
    /// <param name="keys">The key shortcuts for the event.</param>
    /// <returns>The button menu item for context menu.</returns>
    public ButtonMenuItem CreateMenuItem(string text, Action clickHandler, Keys keys = Keys.None)
    {			
        var menuItem = new ButtonMenuItem{Text = text, Shortcut = keys};
        menuItem.Click += (sender, eventArgs) => clickHandler();
        return menuItem;
    }
    /// <summary>
    /// Create the grid column for the grid.
    /// </summary>
    /// <param name="header">The header for the column.</param>
    /// <param name="data">The data for the column.</param>
    /// <returns>The column for the grid.</returns>
    public GridColumn GetColumn(string header, Func<T, string> data)
    {
        return new GridColumn
        {
            HeaderText = header,
            Editable = false,
            DataCell = GetData(data)
        };
    }
    /// <summary>
    /// Create the text box cell for data.
    /// </summary>
    /// <param name="data">the data of the category.</param>
    /// <returns>The text box cell that return the data.</returns>
    public TextBoxCell GetData(Func<T, string> data)
    {
        return new TextBoxCell
        {
            Binding = Binding.Delegate<T, string>(data, null)
        };
    }
}
