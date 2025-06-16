using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using VisionSoft;
public class TModel
{
    // Table name for database operations
    public string TableName { get; set; } = "";

    // Map of database column names to their string values
    public Dictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();
    ClsDatabase db = new ClsDatabase();
    // Constructor
    public TModel()
    {
        Fields = new Dictionary<string, string>();
    }

    public TModel(string tableName) : this()
    {
        TableName = tableName;
    }

    // Helper methods for easier field management
    public void SetField(string columnName, string value)
    {
        Fields[columnName] = value;
    }

    public void SetField(string columnName, object value)
    {
        Fields[columnName] = value?.ToString() ?? "";
    }

    public string GetField(string columnName)
    {
        return Fields.GetValueOrDefault(columnName, "");
    }

    public T GetField<T>(string columnName)
    {
        var value = GetField(columnName);
        if (string.IsNullOrEmpty(value))
            return default(T);

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default(T);
        }
    }

    public bool HasField(string columnName)
    {
        return Fields.ContainsKey(columnName);
    }

    public void RemoveField(string columnName)
    {
        Fields.Remove(columnName);
    }

    public void ClearFields()
    {
        Fields.Clear();
    }

    // Get all field names
    public IEnumerable<string> GetFieldNames()
    {
        return Fields.Keys;
    }

    // Get fields for database insert (excluding empty values if needed)
    public Dictionary<string, string> GetNonEmptyFields()
    {
        return Fields.Where(kvp => !string.IsNullOrEmpty(kvp.Value))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    // Virtual methods for derived classes
    public virtual void InitializeFields() { }
    public virtual void Validate() { }
    public virtual void OnBeforeSave() { }
    public virtual void OnAfterSave() { }

    // Generate SQL insert statement
    public string GenerateInsertSQL()
    {
        if (string.IsNullOrEmpty(TableName))
            throw new InvalidOperationException("TableName is required");

        var nonEmptyFields = GetNonEmptyFields();
        if (nonEmptyFields.Count == 0)
            throw new InvalidOperationException("No fields to insert");

        var columns = string.Join(", ", nonEmptyFields.Keys);
        var values = string.Join(", ", nonEmptyFields.Values.Select(v => $"'{v.Replace("'", "''")}'"));

        return $"INSERT INTO {TableName} ({columns}) VALUES ({values})";
    }

    // Generate SQL update statement (requires a primary key field)
    public string GenerateUpdateSQL(string primaryKeyColumn)
    {
        if (string.IsNullOrEmpty(TableName))
            throw new InvalidOperationException("TableName is required");

        if (!HasField(primaryKeyColumn))
            throw new InvalidOperationException($"Primary key field '{primaryKeyColumn}' not found");

        var nonEmptyFields = GetNonEmptyFields();
        var updateFields = nonEmptyFields.Where(kvp => kvp.Key != primaryKeyColumn);

        if (!updateFields.Any())
            throw new InvalidOperationException("No fields to update");

        var setPart = string.Join(", ", updateFields.Select(kvp => $"{kvp.Key} = '{kvp.Value.Replace("'", "''")}'"));
        var primaryKeyValue = GetField(primaryKeyColumn);

        return $"UPDATE {TableName} SET {setPart} WHERE {primaryKeyColumn} = '{primaryKeyValue.Replace("'", "''")}'";
    }

    // Debug/Display methods
    public override string ToString()
    {
        var fieldList = string.Join(", ", Fields.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        return $"TModel[Table: {TableName}, Fields: {fieldList}]";
    }

    public bool Save()
    {
        string query = GenerateInsertSQL();

        try
        {
            db.ExecuteQuery(query);
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }
}