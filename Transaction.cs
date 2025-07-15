using System.Collections.Concurrent;
using System.Text;
using VisionSoft;

public class Transaction
{
    private List<string> queries = new List<string>();
    private ClsDatabase db = new ClsDatabase();

    public void AddQuery(string s)
    {
        if (!string.IsNullOrWhiteSpace(s))
            queries.Add(s);
    }

    public void ClearQuery()
    {
        queries.Clear();
    }

    public bool Execute()
    {
        string query = buildQuery();
        bool success = db.ExecuteQuery(query);
        return success;
    }
    private string buildQuery()
    {
        var sb = new StringBuilder();

        sb.AppendLine("BEGIN TRY");
        sb.AppendLine("BEGIN TRANSACTION;");

        foreach (string query in queries)
        {
            sb.AppendLine(query.Trim().TrimEnd(';') + ";");
        }

        sb.AppendLine("COMMIT TRANSACTION;");
        sb.AppendLine("END TRY");

        sb.AppendLine("BEGIN CATCH");
        sb.AppendLine("ROLLBACK TRANSACTION;");
        sb.AppendLine("END CATCH");

        return sb.ToString();
    }
}
