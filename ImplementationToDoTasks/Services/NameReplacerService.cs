using ImplementationToDoTasks.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ImplementationToDoTasks.Services;
public interface INameReplacerService
{
    void ReplaceNames(ReplaceNameModel model);
    List<string> GetExecutedQueries();
    List<string> GetProcessedTables();

}
public class NameReplacerService : INameReplacerService
{
    private readonly string _connectionString;
    private readonly List<string> _executedQueries;
    private readonly List<string> _processedTables;

    public NameReplacerService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultLiveConnection");
        _executedQueries = new List<string>();
        _processedTables = new List<string>();
    }

    public void ReplaceNames(ReplaceNameModel model)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var tables = connection.GetSchema("Tables");

            foreach (DataRow row in tables.Rows)
            {
                var tableName = row["TABLE_NAME"].ToString();
                try
                {
                    bool tableUpdated = ReplaceNameInTable(connection, tableName, model.OldName, model.NewName);
                    if (tableUpdated)
                    {
                        _processedTables.Add(tableName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating table {tableName}: {ex.Message}");
                }
            }
        }
    }

    private bool ReplaceNameInTable(SqlConnection connection, string tableName, string oldName, string newName)
    {
        var columns = GetTextColumns(connection, tableName);
        bool tableUpdated = false;

        foreach (var column in columns)
        {
            string query = $"UPDATE {tableName} SET {column} = REPLACE({column}, @OldName, @NewName) WHERE {column} LIKE '%' + @OldName + '%'";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@OldName", oldName);
                command.Parameters.AddWithValue("@NewName", newName);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    _executedQueries.Add(query.Replace("@OldName", $"'{oldName}'").Replace("@NewName", $"'{newName}'"));
                    tableUpdated = true;
                }
            }
        }

        return tableUpdated;
    }

    private List<string> GetTextColumns(SqlConnection connection, string tableName)
    {
        var textColumns = new List<string>();
        string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName AND DATA_TYPE IN ('nchar', 'nvarchar', 'varchar', 'char', 'text', 'ntext')";
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@TableName", tableName);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    textColumns.Add(reader["COLUMN_NAME"].ToString());
                }
            }
        }
        return textColumns;
    }

    public List<string> GetExecutedQueries()
    {
        return _executedQueries;
    }

    public List<string> GetProcessedTables()
    {
        return _processedTables;
    }




}



