string directoryPath = @"D:\Education\[01] Road Map\07.Client-Side framework\Mastering Angular 16 Build Powerful Web Applications";
string searchPattern = "#.mp4";
string replacePattern = ".mp4";

try
{
    DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
    FileInfo[] files = dirInfo.GetFiles("*" + searchPattern + "*");

    foreach (FileInfo file in files)
    {
        string newFileName = file.Name.Replace(searchPattern, replacePattern);
        string newFilePath = Path.Combine(directoryPath, newFileName);

        file.MoveTo(newFilePath);

        Console.WriteLine($"Renamed: {file.Name} -> {newFileName}");
    }

    Console.WriteLine("All matching files have been renamed.");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}


Console.ReadKey();