using System.ComponentModel;
using System.Runtime.CompilerServices;
using InventBox.Core.Models;

namespace InventBox.Desktop.ModelViews;

public class LocationsModelView : Locations, INotifyPropertyChanged
{
    private int id;
    private string floor;
    private string room;
    private string container;
    private int x;
    private int y;

    public override int Id { get { return id; } set
        {
            if (id != value)
            {
                id = value;
                OnPropertyChanged();
            }
        } 
    }
    public override string Floor { get { return floor; } set
        {
            if (floor != value)
            {
                floor = value;
                OnPropertyChanged();
            }
        } 
    }
    public override string Room { get { return room; } set
        {
            if (room != value)
            {
                room = value;
                OnPropertyChanged();
            }
        }
    }
    public override string Container {get {return container; } set
        {
            if (container != value)
            {
                container = value;
                OnPropertyChanged();
            }
        }
    }
    public override int X {get { return x; } set
        {
            if (x != value)
            {
                x = value;
                OnPropertyChanged();
            }
        }
    }
    public override int Y {get { return y; } set
        {
            if (y != value)
            {
                y = value;
                OnPropertyChanged();
            }
        }
    }
    void OnPropertyChanged([CallerMemberName] string memberName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
    }
    public event PropertyChangedEventHandler PropertyChanged;
}
