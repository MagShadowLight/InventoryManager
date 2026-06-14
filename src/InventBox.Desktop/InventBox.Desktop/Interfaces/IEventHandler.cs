using System;

namespace InventBox.Desktop.Interfaces;
/// <summary>
/// Represents the interface for event handler.
/// </summary>
public interface IEventHandler
{
    public void OnCreate();
    public void OnSave();
    public void OnLoad();
    public void OnEdit();
    public void OnDelete();
    public void RefreshData();
    public void Search();
}
