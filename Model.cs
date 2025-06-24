using System.Data;
using Microsoft.Extensions.Primitives;
using VisionSoft;
using System.Text;

public class Model
{
    ClsDatabase db = new ClsDatabase();

    string TableName = "";
    string PrimaryKeyColumn = "";
    string PrimaryKeyPrefix = "";
    public Dictionary<string, string> dict = new Dictionary<string, string>();


    public Model(String TableNamePara, String PrimaryKeyPara)
    {
        TableName = TableNamePara;
        PrimaryKeyColumn = PrimaryKeyPara;
        InitiateKeys();
        if(PrimaryKeyColumn != "")SetPrimaryKey();
    }
    public Model(String TableNamePara, String PrimaryKeyPara, String PrimaryKeyPrefixPara)
    {
        TableName = TableNamePara;
        PrimaryKeyColumn = PrimaryKeyPara;
        PrimaryKeyPrefix = PrimaryKeyPrefixPara;
        InitiateKeys();
        if(PrimaryKeyColumn != "")SetPrimaryKey(PrimaryKeyPrefix);
    }


    //Save
    public bool Save()
    {
        return db.SaveRecord(TableName, GetColumnString(), GetValueString());
    }


    //Update
    public bool Update()
    {
        StringBuilder setQuantity = new StringBuilder();
        bool start = true;
        foreach (var kvp in dict)
        {

            if (kvp.Key != PrimaryKeyColumn)
            {
                if (start)
                {
                    setQuantity.Append($"{kvp.Key}='{kvp.Value}'");
                    start = false;
                }
                else setQuantity.Append($",{kvp.Key}='{kvp.Value}'");
            }
        }
        string Query = $@"
                UPDATE {TableName}
                SET {setQuantity} 
                WHERE {PrimaryKeyColumn}='{dict[PrimaryKeyColumn]}'";

        return db.ExecuteQuery(Query);
    }


    //Delete
    public bool Delete(int code)
    {
        String Query = $@"DELETE FROM {TableName} 
                        WHERE {dict[PrimaryKeyColumn]}={code}";
        return db.ExecuteQuery(Query);
    }


    //Initiate The Dictionary With Keys and Empty Values
    public void InitiateKeys()
    {
        //Console.WriteLine("Here2");
        String Query = $@"SELECT COLUMN_NAME,DATA_TYPE 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = '{TableName}'";

        DataTable dt = db.GetDataTable(Query);
        foreach (DataRow row in dt.Rows)
        {
            //Console.WriteLine(row["COLUMN_NAME"]);
            dict[row["COLUMN_NAME"].ToString()] = "";
            string columnName = row["COLUMN_NAME"].ToString();
            string dataType = row["DATA_TYPE"].ToString().ToLower();

            // INITIALIZE WITH APPROPRIATE VALUE FOR TESTING PURPOSE : TO BE REMOVED
            //TO BE REMOVED : DATA_TYPE FROM THE QUERY, string dataType, SwitchCase
            switch (dataType)
            {
                case "int":
                case "bigint":
                case "smallint":
                case "tinyint":
                    dict[columnName] = "1";
                    break;
                case "float":
                case "real":
                case "decimal":
                case "numeric":
                case "money":
                case "smallmoney":
                    dict[columnName] = "1.1";
                    break;
                case "bit":
                    dict[columnName] = "false";
                    break;
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "date":
                case "time":
                    dict[columnName] = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                default: // varchar, nvarchar, char, nchar, text, ntext, etc.
                    dict[columnName] = "DemoValue";
                    break;
            }
        }
    }

    //Populate Dictionary Using DataRow
    public void Populate(DataRow row)
    {
        foreach (DataColumn column in row.Table.Columns)
        {
            if (dict.ContainsKey(column.ColumnName))
            {
                dict[column.ColumnName] = row[column.ColumnName]?.ToString() ?? "";
            }
            else throw new Exception($"Column '{column.ColumnName}' does not exist. Populate Error");        
        }
    }


    //Populate Dictionary Using Primary Key
    public void PopulateViaKey(int key)
    {
        DataTable dataTable = db.GetDataTable($"Select * From {TableName} where {PrimaryKeyColumn}={key}");
        DataRow row = dataTable.Rows[0];
        Populate(row);
    }



    //AutoGenerate The Primary Key Code with Max
    public void SetPrimaryKey()
    {
        dict[PrimaryKeyColumn] = db.GenerateNextNo(TableName, PrimaryKeyColumn);
    }

    //AutoGenerate The Primary Key Code Having Prefix
    public void SetPrimaryKey(string prefix){
        string query = $@"
            SELECT MAX(CAST(SUBSTRING({PrimaryKeyColumn}, {PrimaryKeyPrefix.Length + 1}, LEN({PrimaryKeyColumn})) AS INT))
            FROM {TableName}
            WHERE {PrimaryKeyColumn} LIKE '{PrimaryKeyPrefix}%'";

        string maxVal = db.GetScalar(query);
        int nextNumber = int.Parse(maxVal) + 1;
        dict[PrimaryKeyColumn] = prefix + nextNumber.ToString();
    }


    //Clear
    public void Clear()
    {
        foreach (var key in dict.Keys.ToList())
        {
            dict[key] = "";
        }
        if(PrimaryKeyColumn != ""){
            if(PrimaryKeyPrefix == "")SetPrimaryKey(); // Reset primary key
            else SetPrimaryKey(PrimaryKeyPrefix);
        }
    }


    //Helper Functions
    public string GetColumnString()
    {
        StringBuilder column = new StringBuilder();
        bool start = true;
        foreach (var kvp in dict)
        {
            if (start)
            {
                start = false;
                column.Append(kvp.Key);
            }
            else column.Append($",{kvp.Key}");
        }
        return column.ToString();
    }

    public string GetValueString()
    {
        StringBuilder column = new StringBuilder();
        bool start = true;
        foreach (var kvp in dict)
        {
            if (start)
            {
                start = false;
                column.Append($"'{kvp.Value}'");
            }
            else column.Append($",'{kvp.Value}'");
        }
        return column.ToString();
    }


}