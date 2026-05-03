using System;

namespace InventBox.Test.Utils;

public class FileUtils
{
    public static void CleanUpFile(string[] paths)
    {
        foreach (var path in paths) {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
