using CsvHelper.Configuration;
using InventBox.Core.Models;

namespace InventBox.Core.Map;

/// <summary>
/// Represents the class map for items.
/// </summary>
public class ItemMap : ClassMap<Items>
{
    public ItemMap()
    {

        Map(m => m.Id).Index(0).NameIndex(0);
        Map(m => m.Name).Index(1).NameIndex(0);
        Map(m => m.Description).Index(2).NameIndex(0);
        Map(m => m.Quantity).Index(3);
        Map(m => m.SerialNumber).Index(4);
        Map(m => m.ModelNumber).Index(5);
        Map(m => m.Manufacturer).Index(6);
        Map(m => m.Notes).Index(7);
        Map(m => m.CreatedAt).Index(8);
        Map(m => m.UpdatedAt).Index(9);
        Map(m => m.Conditions).Index(10);
        Map(m => m.Category.Id).Index(11).NameIndex(1);
        Map(m => m.Category.Name).Index(12).NameIndex(1);
        Map(m => m.Category.Description).Index(13).NameIndex(1);
        Map(m => m.Locations.Id).Index(14).NameIndex(2);
        Map(m => m.Locations.Floor).Index(15);
        Map(m => m.Locations.Room).Index(16);
        Map(m => m.Locations.Container).Index(17);
        Map(m => m.Locations.X).Index(18);
        Map(m => m.Locations.Y).Index(19);
        Map(m => m.Warrantly.Id).Index(20).NameIndex(3);
        Map(m => m.Warrantly.StartDate).Index(21).NameIndex(0);
        Map(m => m.Warrantly.EndDate).Index(22).NameIndex(0);
        Map(m => m.Warrantly.Status).Index(23);
        Map(m => m.Warrantly.Provider).Index(24).NameIndex(0);
        Map(m => m.Warrantly.ContactNumber).Index(25).NameIndex(0);
        Map(m => m.Insurance.Id).Index(26).NameIndex(4);
        Map(m => m.Insurance.StartDate).Index(27).NameIndex(1);
        Map(m => m.Insurance.EndDate).Index(28).NameIndex(1);
        Map(m => m.Insurance.Provider).Index(29).NameIndex(1);
        Map(m => m.Insurance.ContactNumber).Index(30).NameIndex(1);
        Map(m => m.Insurance.Insured).Index(31);
    }
}
