# HomeInventory

## Overview

HomeInventory is an inventory manager application where user can manage their home inventory. This application allow them to create, manage, delete, and search the items, category, and locations the items is at. 

## Features

- Create, manage, and delete the items, category, and location of items.
- Search for items name, category, floor, and room.
- Scan the bar code to filter by item name.
- Save and load the data with a .csv file.
- copy the data to the clipboard.
- Create and remove the warrantly and insurance inside the item.
- Local and offline-only
- Supported on Windows, MacOS, and Linux.

## Prerequisites

- .NET SDK 10.0 or later
- Windows:
    - Visual Studio 2026 or later
- MacOS or Linux:
    - VS Code.
    - C# Dev Kit Extension for VS Code.
    - Terminal emulator

## How to build and run project

### Windows:

    1. Clone this repository
    2. Open the InventBox.sln in visual studio
    3. Change startup to InventBox.Desktop.Wpf.csproj
    4. run the project

### Mac:

#### VS Code:

    1. Clone this repository.
    2. Open VS code on src folder.
    3. Select Run and Debug on the left side.
    4. Press Run and Debug button.
    5. Select C# from the list
    6. Select C#: InventBox.Desktop.Mac

#### Terminal:

    1. Open terminal
    2. Navigate to 'src/InventBox.Desktop/Inventbox.Desktop.Mac' directory
    3. run the following:
        - dotnet run

### Linux:

#### VS Code:

    1. Clone this repository.
    2. Open VS code on src directory.
    3. Select Run and Debug on the left side.
    4. Press Run and Debug button.
    5. Select C# from the list
    6. Select C#: InventBox.Desktop.Gtk

#### Terminal:

    1. Open terminal
    2. Navigate to 'src/InventBox.Desktop/Inventbox.Desktop.Gtk' directory
    3. run the following:
        - dotnet run

## How to test the project

### VS Code:

    1. Open VS Code in src directory.
    2. Select the testing section.
    3. Click the stacked arrow that shows Run Tests.

### Terminal:

    1. Open terminal
    2. Navigate to 'src/InventBox.Test'
    3. run the following:
        - dotnet test 

## Issues

Open an issue in github if you find any bugs or have any suggestions about this app like feature request or improvements.