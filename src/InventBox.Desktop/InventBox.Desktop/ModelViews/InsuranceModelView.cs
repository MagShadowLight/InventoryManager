using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using InventBox.Core.Models;

namespace InventBox.Desktop.ModelViews;

public class InsuranceModelView : Insurance, INotifyPropertyChanged
{
    private int id;
    private DateTime startDate;
    private DateTime endDate;
    private string provider;
    private string contactNumber;
    private Status status;
    public override int Id { get { return id; } set
        {
            if (id != value)
            {
                id = value;
                OnPropertyChanged();
            }
        }
    }
    public override DateTime StartDate { get { return startDate; } set
        {
            if (startDate != value)
            {
                startDate = value;
                OnPropertyChanged();
            }
        }
    }
    public override DateTime EndDate { get { return endDate; } set
        {
            if (endDate != value)
            {
                endDate = value;
                OnPropertyChanged();
            }
        }
    }
    public override string Provider { get {return provider; } set
        {
            if (provider != value)
            {
                provider = value;
                OnPropertyChanged();
            }
        }
    }
    public override string ContactNumber { get { return contactNumber; } set
        {
            if (contactNumber != value)
            {
                contactNumber = value;
                OnPropertyChanged();
            }
        }
    }
    public override Status Insured { get { return status; } set
        {
            if (status != value)
            {
                status = value;
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
