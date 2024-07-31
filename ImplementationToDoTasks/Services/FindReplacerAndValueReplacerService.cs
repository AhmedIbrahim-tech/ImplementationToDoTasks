using ImplementationToDoTasks.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ImplementationToDoTasks.Services;

public interface IFindReplacerAndValueReplacerService
{
    string ConnectionString { get; }
    List<TableValues> FetchValues(string columnName, string searchValue);
    List<string> FetchValuesFromTable(string tableName, string columnName, string value);
    List<string> GenerateUpdateQueries(string columnName, string searchValue, string newValue);
    void ReplaceValues(string columnName, string searchValue, string newValue);
    List<string> GetExecutedQueries();
    List<string> GetProcessedTables();

}


public class FindReplacerAndValueReplacerService : IFindReplacerAndValueReplacerService
{
    public string ConnectionString { get; private set; }
    private readonly List<string> _executedQueries;
    private readonly List<string> _processedTables;

    public FindReplacerAndValueReplacerService(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("DefaultLiveConnection");
        _executedQueries = new List<string>();
        _processedTables = new List<string>();
    }

    public List<TableValues> FetchValues(string columnName, string searchValue)
    {
        var results = new List<TableValues>();

        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            var tables = connection.GetSchema("Tables");

            foreach (DataRow row in tables.Rows)
            {
                var tableName = row["TABLE_NAME"].ToString();
                try
                {
                    if (ColumnExists(connection, tableName, columnName))
                    {
                        var values = FetchValuesFromTable(tableName, columnName, searchValue);
                        if (values.Count > 0)
                        {
                            results.Add(new TableValues { TableName = tableName, Values = values });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching values from table {tableName}: {ex.Message}");
                }
            }
        }

        return results;
    }

    public List<string> FetchValuesFromTable(string tableName, string columnName, string value)
    {
        var values = new List<string>();

        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            string query = $"SELECT {columnName} FROM {tableName} WHERE {columnName} LIKE '%' + @Value + '%'";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Value", value);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        values.Add(reader[columnName].ToString());
                    }
                }
            }
        }

        return values;
    }

    private bool ColumnExists(SqlConnection connection, string tableName, string columnName)
    {
        string query = @"SELECT COUNT(*)
                             FROM INFORMATION_SCHEMA.COLUMNS
                             WHERE TABLE_NAME = @TableName AND COLUMN_NAME = @ColumnName";

        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@TableName", tableName);
            command.Parameters.AddWithValue("@ColumnName", columnName);
            return (int)command.ExecuteScalar() > 0;
        }
    }

    public List<string> GenerateUpdateQueries(string columnName, string searchValue, string newValue)
    {
        var queries = new List<string>();

        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            var tables = connection.GetSchema("Tables");

            foreach (DataRow row in tables.Rows)
            {
                var tableName = row["TABLE_NAME"].ToString();
                if (ColumnExists(connection, tableName, columnName))
                {
                    string query = $"UPDATE {tableName} SET {columnName} = REPLACE({columnName}, @OldValue, @NewValue) WHERE {columnName} LIKE '%' + @OldValue + '%'";
                    queries.Add(query.Replace("@OldValue", $"'{searchValue}'").Replace("@NewValue", $"'{newValue}'"));
                }
            }
        }

        return queries;
    }

    public void ReplaceValues(string columnName, string searchValue, string newValue)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            var tables = connection.GetSchema("Tables");

            foreach (DataRow row in tables.Rows)
            {
                var tableName = row["TABLE_NAME"].ToString();
                try
                {
                    if (ColumnExists(connection, tableName, columnName))
                    {
                        bool tableUpdated = ReplaceValueInTable(connection, tableName, columnName, searchValue, newValue);
                        if (tableUpdated)
                        {
                            _processedTables.Add(tableName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating table {tableName}: {ex.Message}");
                }
            }
        }
    }

    private bool ReplaceValueInTable(SqlConnection connection, string tableName, string columnName, string oldValue, string newValue)
    {
        string query = $"UPDATE {tableName} SET {columnName} = REPLACE({columnName}, @OldValue, @NewValue) WHERE {columnName} LIKE '%' + @OldValue + '%'";
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@OldValue", oldValue);
            command.Parameters.AddWithValue("@NewValue", newValue);
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                _executedQueries.Add(query.Replace("@OldValue", $"'{oldValue}'").Replace("@NewValue", $"'{newValue}'"));
                return true;
            }
        }

        return false;
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