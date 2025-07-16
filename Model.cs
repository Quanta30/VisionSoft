using System.Data;
using System.Text;
// Assuming VisionSoft contains your database and transaction classes.
using VisionSoft; 

/// <summary>
/// A generic model class for handling database operations (CRUD) for a specific table.
/// It uses a dictionary to store column data and provides methods for saving, updating,
/// deleting, and populating records.
/// </summary>
public class Model
{
    // =================================================================================
    // PRIVATE FIELDS
    // =================================================================================

    private readonly ClsDatabase db = new ClsDatabase();
    private readonly string _tableName;
    private readonly string _primaryKeyColumn;
    private string _primaryKeyPrefix;

    // =================================================================================
    // PUBLIC PROPERTIES
    // =================================================================================

    /// <summary>
    /// The primary dictionary holding the current record's data, with column names as keys.
    /// This dictionary is used for data binding in the UI.
    /// </summary>
    public Dictionary<string, string> dict { get; private set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// A secondary dictionary to hold the original state of the primary key when a record is loaded for editing.
    /// This is crucial for the WHERE clause in UPDATE statements to ensure the correct record is updated.
    /// </summary>
    public Dictionary<string, string> dict2 { get; private set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    // =================================================================================
    // CONSTRUCTORS
    // =================================================================================

    /// <summary>
    /// Initializes a new model for a table with a numeric primary key.
    /// </summary>
    /// <param name="tableName">The name of the database table.</param>
    /// <param name="primaryKeyColumn">The name of the primary key column.</param>
    public Model(string tableName, string primaryKeyColumn)
    {
        _tableName = tableName;
        _primaryKeyColumn = primaryKeyColumn;
        _primaryKeyPrefix = "";
        InitiateKeys();
        if (!string.IsNullOrEmpty(_primaryKeyColumn)) SetPrimaryKey();
    }

    /// <summary>
    /// Initializes a new model for a table with a prefixed primary key (e.g., "INV-001").
    /// </summary>
    /// <param name="tableName">The name of the database table.</param>
    /// <param name="primaryKeyColumn">The name of the primary key column.</param>
    /// <param name="primaryKeyPrefix">The string prefix for the primary key.</param>
    public Model(string tableName, string primaryKeyColumn, string primaryKeyPrefix)
    {
        _tableName = tableName;
        _primaryKeyColumn = primaryKeyColumn;
        _primaryKeyPrefix = primaryKeyPrefix;
        InitiateKeys();
        if (!string.IsNullOrEmpty(_primaryKeyColumn)) SetPrimaryKey(primaryKeyPrefix);
    }

    // =================================================================================
    // PUBLIC CRUD METHODS
    // =================================================================================

    #region Public CRUD Methods

    /// <summary>
    /// Saves the current record in the dictionary as a new row in the database.
    /// </summary>
    /// <returns>True if the save operation was successful, otherwise false.</returns>
    public bool Save()
    {
        // SECURITY NOTE: This assumes `db.SaveRecord` is implemented using parameterized queries
        // to prevent SQL injection. The implementation should not directly concatenate values.
        return db.SaveRecord(_tableName, GetColumnString(), GetValueString());
    }

    /// <summary>
    /// Adds a save query for the current record to a database transaction.
    /// </summary>
    /// <param name="transaction">The transaction object to add the query to.</param>
    public void Save(Transaction transaction)
    {
        // SECURITY NOTE: The `transaction` class should also use parameterized queries.
        string query = $"INSERT INTO {_tableName} ({GetColumnString()}) VALUES ({GetValueString()});";
        transaction.AddQuery(query);
    }

    /// <summary>
    /// Updates an existing record in the database based on the primary key stored in `dict2`.
    /// </summary>
    /// <returns>True if the update was successful, otherwise false.</returns>
    public bool Update()
    {
        if (!dict2.ContainsKey(_primaryKeyColumn) || string.IsNullOrEmpty(dict2[_primaryKeyColumn]))
        {
            throw new InvalidOperationException("Cannot update record. Original primary key not found.");
        }
        
        string setClause = GetSetClause();
        string query = $"UPDATE {_tableName} SET {setClause} WHERE {_primaryKeyColumn} = '{dict2[_primaryKeyColumn]}'";
        
        // SECURITY NOTE: This assumes `db.ExecuteQuery` is safe. Ideally, it should take parameters.
        return db.ExecuteQuery(query);
    }

    /// <summary>
    /// Adds an update query for the current record to a database transaction.
    /// </summary>
    /// <param name="transaction">The transaction object to add the query to.</param>
    public void Update(Transaction transaction)
    {
        if (!dict2.ContainsKey(_primaryKeyColumn) || string.IsNullOrEmpty(dict2[_primaryKeyColumn]))
        {
            throw new InvalidOperationException("Cannot add update query to transaction. Original primary key not found.");
        }

        string setClause = GetSetClause();
        string query = $"UPDATE {_tableName} SET {setClause} WHERE {_primaryKeyColumn} = '{dict2[_primaryKeyColumn]}'";
        
        transaction.AddQuery(query);
    }

    /// <summary>
    /// Deletes a record from the database by its primary key.
    /// </summary>
    /// <param name="key">The primary key value of the record to delete.</param>
    /// <returns>True if the delete was successful, otherwise false.</returns>
    public bool Delete(int key)
    {
        // SECURITY NOTE: This is vulnerable to SQL injection if `key` were a string.
        // Always prefer parameterized queries.
        string query = $"DELETE FROM {_tableName} WHERE {_primaryKeyColumn} = {key}";
        return db.ExecuteQuery(query);
    }

    #endregion

    // =================================================================================
    // DATA POPULATION & INITIALIZATION
    // =================================================================================

    #region Data Population

    /// <summary>
    /// Initializes the dictionary with keys corresponding to the table's columns and empty string values.
    /// </summary>
    public void InitiateKeys()
    {
        try
        {
            string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{_tableName}'";
            DataTable dt = db.GetDataTable(query);
            dict.Clear();
            foreach (DataRow row in dt.Rows)
            {
                string columnName = row["COLUMN_NAME"].ToString();
                if (!string.IsNullOrEmpty(columnName))
                {
                    dict[columnName] = "";
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed.
            Console.WriteLine($"Error initiating keys for table '{_tableName}': {ex.Message}");
            throw; // Re-throw to make the caller aware of the failure.
        }
    }

    /// <summary>
    /// Populates the model's dictionary with data from a DataRow.
    /// Also populates `dict2` to store the original state for updates.
    /// </summary>
    /// <param name="row">The DataRow containing the record's data.</param>
    public void Populate(DataRow row)
    {
        if (row == null) throw new ArgumentNullException(nameof(row));

        // Create a fresh copy for dict2 to store the original state.
        dict2 = new Dictionary<string, string>(dict, StringComparer.OrdinalIgnoreCase);

        foreach (DataColumn column in row.Table.Columns)
        {
            string colName = column.ColumnName;
            if (dict.ContainsKey(colName))
            {
                object value = row[colName];
                string stringValue;

                if (value == DBNull.Value || value == null)
                {
                    stringValue = "";
                }
                else if (column.DataType == typeof(DateTime))
                {
                    stringValue = ((DateTime)value).ToString("yyyy-MM-dd");
                }
                else
                {
                    stringValue = value.ToString() ?? "";
                }

                dict[colName] = stringValue;
                dict2[colName] = stringValue; // Also update dict2 with the loaded values.
            }
        }
    }

    /// <summary>
    /// Fetches a record by its primary key and populates the model.
    /// </summary>
    /// <param name="key">The primary key of the record to fetch.</param>
    public void PopulateViaKey(object key)
    {
        try
        {
            // Using a parameterized query approach is safer.
            DataTable dataTable = db.GetDataTable($"SELECT * FROM {_tableName} WHERE {_primaryKeyColumn} = '{key}'");
            if (dataTable.Rows.Count > 0)
            {
                Populate(dataTable.Rows[0]);
            }
            else
            {
                // Handle case where no record is found.
                Clear();
                throw new KeyNotFoundException($"Record with key '{key}' not found in table '{_tableName}'.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error populating via key: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Clears all values in the dictionary and resets the primary key.
    /// </summary>
    public void Clear()
    {
        var keys = dict.Keys.ToList();
        foreach (var key in keys)
        {
            dict[key] = "";
        }
        if (!string.IsNullOrEmpty(_primaryKeyColumn))
        {
            SetPrimaryKey(_primaryKeyPrefix);
        }
    }

    #endregion

    // =================================================================================
    // PRIMARY KEY MANAGEMENT
    // =================================================================================

    #region Primary Key
    
    /// <summary>
    /// Generates and sets the next numeric primary key.
    /// </summary>
    public void SetPrimaryKey()
    {
        dict[_primaryKeyColumn] = db.GenerateNextNo(_tableName, $"CAST({_primaryKeyColumn} AS INT)", $"{_primaryKeyColumn} LIKE '[0-9]%'");
    }

    /// <summary>
    /// Generates and sets the next primary key for a key with a string prefix.
    /// </summary>
    /// <param name="prefix">The prefix of the primary key.</param>
    public void SetPrimaryKey(string prefix)
    {
        _primaryKeyPrefix = prefix;
        if (string.IsNullOrEmpty(prefix))
        {
            SetPrimaryKey();
            return;
        }

        try
        {
            string countQuery = $"SELECT COUNT(*) FROM {_tableName} WHERE {_primaryKeyColumn} LIKE '{prefix}%'";
            bool recordExists = Convert.ToInt32(db.GetScalar(countQuery)) > 0;

            if (!recordExists)
            {
                dict[_primaryKeyColumn] = prefix + "1";
            }
            else
            {
                string maxValQuery = $"SELECT MAX(CAST(SUBSTRING({_primaryKeyColumn}, {prefix.Length + 1}, LEN({_primaryKeyColumn})) AS INT)) FROM {_tableName} WHERE {_primaryKeyColumn} LIKE '{prefix}%'";
                int maxVal = Convert.ToInt32(db.GetScalar(maxValQuery));
                dict[_primaryKeyColumn] = prefix + (maxVal + 1).ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting prefixed primary key: {ex.Message}");
            dict[_primaryKeyColumn] = ""; // Set to empty on error.
        }
    }

    /// <summary>
    /// Public method to change the prefix and regenerate the primary key.
    /// </summary>
    /// <param name="prefix">The new prefix to use.</param>
    public void SetPrefix(string prefix)
    {
        SetPrimaryKey(prefix);
    }
    
    #endregion

    // =================================================================================
    // PRIVATE HELPER FUNCTIONS
    // =================================================================================

    #region Private Helpers

    /// <summary>
    /// Gets a comma-separated string of column names from the dictionary.
    /// </summary>
    private string GetColumnString()
    {
        return string.Join(",", dict.Keys);
    }

    /// <summary>
    /// Gets a comma-separated, single-quoted string of values from the dictionary.
    /// </summary>
    private string GetValueString()
    {
        // Escaping single quotes to provide basic protection against SQL injection.
        // A fully parameterized approach is strongly recommended.
        return string.Join(",", dict.Values.Select(v => $"'{v?.Replace("'", "''")}'"));
    }

    /// <summary>
    /// Builds the SET clause for an UPDATE statement (e.g., "Col1='Val1',Col2='Val2'").
    /// </summary>
    private string GetSetClause()
    {
        return string.Join(",", dict.Select(kvp => $"{kvp.Key}='{kvp.Value?.Replace("'", "''")}'"));
    }

    #endregion
}
