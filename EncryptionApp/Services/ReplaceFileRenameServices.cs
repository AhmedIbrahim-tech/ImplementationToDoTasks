using ImplementationToDoTasks.Models;
using System.Text.RegularExpressions;

namespace ImplementationToDoTasks.Services;

public interface IReplaceFileRenameServices
{
    List<string> RenameFiles(ReplaceFileRenameModel model);
}


public class ReplaceFileRenameServices : IReplaceFileRenameServices
{

    public List<string> RenameFiles(ReplaceFileRenameModel model)
    {
        List<string> results = new List<string>();

        if (model.ContainsNumber)
        {
            ExecuteSecondCode(model, results);
        }
        else
        {
            ExecuteFirstCode(model, results);
        }

        return results;
    }

    private void ExecuteFirstCode(ReplaceFileRenameModel model, List<string> results)
    {
        try
        {
            DirectoryInfo dirInfo = new DirectoryInfo(model.DirectoryPath);
            FileInfo[] files = dirInfo.GetFiles("*" + model.SearchPattern + "*");

            foreach (FileInfo file in files)
            {
                string newFileName = file.Name.Replace(model.SearchPattern, model.ReplacePattern);
                string newFilePath = Path.Combine(model.DirectoryPath, newFileName);

                file.MoveTo(newFilePath);
                results.Add($"Renamed To : {newFileName}");
            }

            results.Add("All matching files have been renamed.");
        }
        catch (Exception ex)
        {
            results.Add($"An error occurred: {ex.Message}");
        }
    }

    private void ExecuteSecondCode(ReplaceFileRenameModel model, List<string> results)
    {
        try
        {
            DirectoryInfo dirInfo = new DirectoryInfo(model.DirectoryPath);
            FileInfo[] files = dirInfo.GetFiles();

            foreach (FileInfo file in files)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.Name);
                string fileExtension = file.Extension;

                if (Regex.IsMatch(fileNameWithoutExtension, model.SearchPattern + @"\d+"))
                {
                    string newFileName = Regex.Replace(fileNameWithoutExtension, model.SearchPattern + @"\d+", m =>
                    {
                        string number = Regex.Match(m.Value, @"\d+").Value;
                        return model.ReplacePattern + number;
                    }) + fileExtension;

                    string newFilePath = Path.Combine(model.DirectoryPath, newFileName);

                    file.MoveTo(newFilePath);

                    results.Add($"Renamed: {file.Name} -> {newFileName}");
                }
            }

            results.Add("All matching files have been renamed.");
        }
        catch (Exception ex)
        {
            results.Add($"An error occurred: {ex.Message}");
        }
    }





}
