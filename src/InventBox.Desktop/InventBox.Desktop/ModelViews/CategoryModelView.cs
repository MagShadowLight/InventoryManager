using System.ComponentModel;
using System.Runtime.CompilerServices;
using InventBox.Core.Models;

namespace InventBox.Desktop.ModelViews;

public class CategoryModelView : Category, INotifyPropertyChanged
{
    private int id;
    private string name;
    private string description;
    public override int Id {get { return id; } set
        {
            if (id != value)
            {
                id = value;
                OnPropertyChanged();
            }
        }
    }    
    public override string Name {get { return name; } set
        {
            if (name != value)
            {
                name = value;
                OnPropertyChanged();
            }
        }
    }
    public override string Description {get { return description; } set
        {
            if (description != value)
            {
                description = value;
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
