using System;

namespace InventBox.Test.Utils;

public class FileUtils
{
    public static void CleanUpFile(string path)
    {
        File.Delete(path);
    }
}
