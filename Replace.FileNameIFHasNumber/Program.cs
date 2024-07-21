using System.Text.RegularExpressions;

string directoryPath = @"D:\Education\[01] Road Map\00.Channels\Coding from A to Z\Angular 17 For Beginners In Arabic";
string searchPattern = @"Learn Angular 17 in Arabic _\d+";
string replacePattern = "Learn Angular 17 in Arabic #";

try
{
    DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
    FileInfo[] files = dirInfo.GetFiles();

    foreach (FileInfo file in files)
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.Name);
        string fileExtension = file.Extension;

        if (Regex.IsMatch(fileNameWithoutExtension, searchPattern))
        {
            string newFileName = Regex.Replace(fileNameWithoutExtension, searchPattern, m =>
            {
                string number = Regex.Match(m.Value, @"\d+").Value;
                return replacePattern + number;
            }) + fileExtension;

            string newFilePath = Path.Combine(directoryPath, newFileName);

            file.MoveTo(newFilePath);

            Console.WriteLine($"Renamed: {file.Name} -> {newFileName}");
        }
    }

    Console.WriteLine("All matching files have been renamed.");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}