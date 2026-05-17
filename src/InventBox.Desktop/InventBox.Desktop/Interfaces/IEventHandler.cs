using System;

namespace InventBox.Desktop.Interfaces;

public interface IEventHandler
{
    public void OnCreate();
    public void OnSave();
    public void OnLoad();
    public void OnEdit();
    public void OnDelete();
    public void RefreshData();
    public void ClearFilter();
}
