using System;
using System.Collections.Generic;
using InventBox.Core.Models;

namespace InventBox.Desktop.ModelView;

public class ModelsList
{
    public static List<Items> items = new List<Items>();
    public static List<Category> categories = new List<Category>()
    {
        new Category
        {
            Id = 1,
            Name = "Test 1",
            Description = "This is test"
        }, new Category
        {
            Id = 2,
            Name = "Test 2",
            Description = "This is test 2"
        }, new Category
        {
            Id = 3,
            Name = "Test 3",
            Description = "This is test 3"
        }
    };
    public static List<Locations> locations = new List<Locations>()
    {
        new Locations
        {
            Id = 1,
            Floor = "1",
            Room = "Bedroom",
            Container = "Dresser",
            X = 10,
            Y = 1
        },
        new Locations
        {
            Id = 2,
            Floor = "1",
            Room = "Bathroom",
            Container = "Cabinet",
            X = 1,
            Y = 1
        }, 
        new Locations
        {
            Id = 3,
            Floor = "2",
            Room = "Bedroom",
            Container = "Bedside",
            X = 20,
            Y = 20
        }
    };
}
