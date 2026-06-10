using System;
using System.Collections.Generic;
using InventBox.Core.Models;

namespace InventBox.Desktop.ModelView;
/// <summary>
/// Represents the dataset for the application.
/// </summary>
public class ModelsList
{
    public static List<Category> categories = new List<Category>();
    // public static List<Category> categories = new List<Category>()
    // {
    //     new Category{Id = 1, Name = "Pets", Description = "This is pet"},
    //     new Category{Id = 2, Name = "Arts", Description = "This is art"},
    //     new Category{Id = 3, Name = "Tech", Description = "This is tech"},
    // };
    public static List<Locations> locations = new List<Locations>();
    // public static List<Locations> locations = new List<Locations>()
    // {
    //     new Locations{Id = 1, Floor = "1st floor", Room = "bedroom", Container = "dresser", X = 10, Y = 10},
    //     new Locations{Id = 2, Floor = "2nd floor", Room = "antic", Container = "box", X = 10, Y = 10},
    //     new Locations{Id = 3, Floor = "1st floor", Room = "bathroom", Container = "closet", X = 10, Y = 10},
    // };
    public static List<Items> items = new List<Items>();
    // public static List<Items> items = new List<Items>()
    // {
    //     new Items{Id = 1, Name = "Test 1", Description = "This is test", Quantity = 10, SerialNumber = "jncnwbhwcnjniqc", ModelNumber = "jcwncuwuibcbi", Manufacturer = "Manufacturer 1", Notes = "This is test note", Conditions = Conditions.Good, Category = categories[1], Locations = locations[2], Warrantly = new Warrantly{Id = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), Status = Status.Covered, ContactNumber = "111-222-3333", Provider = "Provider 1"}, Insurance = new Insurance{Id = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), Insured = Status.Covered, ContactNumber = "111-222-3333", Provider = "Provider 1"}},
    //     new Items{Id = 2, Name = "Item 1", Description = "This is test 2", Quantity = 5, SerialNumber = "amkxmkcmakkm", ModelNumber = " ccqncnqwinicniqw", Manufacturer = "Manufacturer 2", Notes = "This is test note 2", Conditions = Conditions.Lost, Category = categories[0], Locations = locations[0], Warrantly = new Warrantly(), Insurance = new Insurance{Id = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), Insured = Status.Covered, ContactNumber = "111-222-3333", Provider = "Provider 1"}},
    //     new Items{Id = 3, Name = "Tech 1", Description = "This is test 3", Quantity = 20, SerialNumber = "xkanxqnjcnxj", ModelNumber = "qkcnncqwnjcqnj", Manufacturer = "Manufacturer 1", Notes = "This is test note 3", Conditions = Conditions.Excellent, Category = categories[2], Locations = locations[1], Warrantly = new Warrantly{Id = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), Status = Status.Covered, ContactNumber = "111-222-3333", Provider = "Provider 1"}, Insurance = new Insurance()}
    // };
}
