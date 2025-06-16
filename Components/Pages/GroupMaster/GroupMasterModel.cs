using System.Data;
using VisionSoft;

class GroupMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string groupCodeStr = "";
    public string groupName = "";
    public string alias = "";
    public string underGroupCodeStr = "0";
    public string natureOfGroup = "";

    // Boolean field
    public bool affectProfitLoss = false;

    // Actual values for database operations
    public int groupCode = 0;
    public int underGroupCode = 0;

    public GroupMasterModel()
    {
        // Initialize with default under group code
        underGroupCodeStr = "0";
    }

    public void GenerateGroupCode()
    {
        try
        {
            groupCode = Convert.ToInt32(db.GenerateNextNo("GroupMaster", "GroupCode"));
            groupCodeStr = groupCode.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating group code: {ex.Message}");
            groupCode = 1;
            groupCodeStr = "1";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(groupCodeStr) || !int.TryParse(groupCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(groupName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(alias))
            isValid = false;

        if (string.IsNullOrWhiteSpace(underGroupCodeStr) || !int.TryParse(underGroupCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(natureOfGroup))
            isValid = false;

        // AffectProfitLoss is a boolean field, always valid (checkbox state)

        return isValid;
    }

    public async Task SaveGroup()
    {
        // Parse string values to actual types
        if (int.TryParse(groupCodeStr, out int parsedGroupCode))
            groupCode = parsedGroupCode;

        if (int.TryParse(underGroupCodeStr, out int parsedUnderGroupCode))
            underGroupCode = parsedUnderGroupCode;

        // Prepare columns and values
        string columns = "GroupCode, GroupName, Alias, UnderGroupCode, NatureOfGroup, AffectProfitLoss";
        string values = $"{groupCode}, N'{groupName}', N'{alias}', {underGroupCode}, N'{natureOfGroup}', {(affectProfitLoss ? 1 : 0)}";

        // Save Group record
        bool saved = db.SaveRecord("GroupMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Group record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(groupCodeStr, out int parsedGroupCode))
                groupCode = parsedGroupCode;

            if (int.TryParse(underGroupCodeStr, out int parsedUnderGroupCode))
                underGroupCode = parsedUnderGroupCode;

            // Build the UPDATE query
            string query = $@"
                UPDATE GroupMaster SET 
                    GroupName = N'{groupName}',
                    Alias = N'{alias}',
                    UnderGroupCode = {underGroupCode},
                    NatureOfGroup = N'{natureOfGroup}',
                    AffectProfitLoss = {(affectProfitLoss ? 1 : 0)}
                WHERE GroupCode = {groupCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Group record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Group record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            groupCodeStr = row["GroupCode"]?.ToString() ?? "";
            groupName = row["GroupName"]?.ToString() ?? "";
            alias = row["Alias"]?.ToString() ?? "";
            underGroupCodeStr = row["UnderGroupCode"]?.ToString() ?? "0";
            natureOfGroup = row["NatureOfGroup"]?.ToString() ?? "";

            // Handle boolean field
            if (row["AffectProfitLoss"] != DBNull.Value)
                affectProfitLoss = Convert.ToBoolean(row["AffectProfitLoss"]);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        groupCodeStr = "";
        groupName = "";
        alias = "";
        underGroupCodeStr = "0"; // Reset to default
        natureOfGroup = "";

        // Reset boolean field
        affectProfitLoss = false;

        // Reset actual values
        groupCode = 0;
        underGroupCode = 0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        groupName = "Current Assets";
        alias = "CA";
        underGroupCodeStr = "0";
        natureOfGroup = "Assets";
        affectProfitLoss = true;
        
        // Actual values for database operations
        underGroupCode = 0;
        // groupCode will be auto-generated
    }
}