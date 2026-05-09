using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using InventBox.Core.Models;

namespace InventBox.Desktop.ModelView;

public class ItemModelView : Items, INotifyPropertyChanged
{
    private int id;
    private string name;
    private string description;
    private int quantity;
    private string serialNumber;
    private string modelNumber;
    private string manufacturer;
    private bool insured;
    private string notes;
    private Conditions conditions;

    public override int Id { get { return id; } set
        {
            if (id != value)
            {
                id = value;
                OnPropertyChanged();
            }
        } }
    public override string Name { get { return name;}
        set
        {
           if (name != value)
            {
                name = value;
                OnPropertyChanged();
            } 
        } }
    public override string Description { get { return description;}
        set
        {
           if (description != value)
            {
                description = value;
                OnPropertyChanged();
            } 
        }  }
    public override int Quantity { get { return quantity;}
        set
        {
           if (quantity != value)
            {
                quantity = value;
                OnPropertyChanged();
            } 
        }  }
    public override string SerialNumber { get { return serialNumber;}
        set
        {
           if (serialNumber != value)
            {
                serialNumber = value;
                OnPropertyChanged();
            } 
        }  }
    public override string ModelNumber { get { return modelNumber;}
        set
        {
           if (modelNumber != value)
            {
                modelNumber = value;
                OnPropertyChanged();
            } 
        }  }
    public override string Manufacturer { get { return manufacturer;}
        set
        {
           if (manufacturer != value)
            {
                manufacturer = value;
                OnPropertyChanged();
            } 
        }  }
    public override bool Insured { get { return insured;}
        set
        {
           if (insured != value)
            {
                insured = value;
                OnPropertyChanged();
            } 
        }  }
    public override string Notes { get { return notes;}
        set
        {
           if (notes != value)
            {
                notes = value;
                OnPropertyChanged();
            } 
        }  }
    public override DateTime CreatedAt { get; set; } = DateTime.Now;
    public override DateTime UpdatedAt { get; set; } = DateTime.Now;
    public override Conditions Conditions { get { return conditions;}
        set
        {
            if (conditions != value)
            {
                conditions = value;
                OnPropertyChanged();
            }
        }  }
    void OnPropertyChanged([CallerMemberName] string memberName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}