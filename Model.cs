using System.Data;
using Microsoft.Extensions.Primitives;
using VisionSoft;
using System.Text;

public class Model
{
    ClsDatabase db = new ClsDatabase();

    string TableName = "";
    string PrimaryKeyColumn = "";
    public Dictionary<string, string> dict = new Dictionary<string, string>();


    public Model(String TableNamePara, String PrimaryKeyPara)
    {
        TableName = TableNamePara;
        PrimaryKeyColumn = PrimaryKeyPara;
        InitiateKeys();
        SetPrimarykey();
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
                SET Quantity = {setQuantity} 
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
        String Query = $@"SELECT COLUMN_NAME 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = '{TableName}'";

        DataTable dt = db.GetDataTable(Query);
        foreach (DataRow row in dt.Rows)
        {
            //Console.WriteLine(row["COLUMN_NAME"]);
            dict[row["COLUMN_NAME"].ToString()] = "";
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



    //AutoGenerate The Primary Key Code with Max
    public void SetPrimarykey()
    {
        dict[PrimaryKeyColumn] = db.GenerateNextNo(TableName, PrimaryKeyColumn);
    }


    //Clear
    public void Clear()
    {
        foreach (var key in dict.Keys.ToList())
        {
            dict[key] = "";
        }
        SetPrimarykey(); // Reset primary key
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