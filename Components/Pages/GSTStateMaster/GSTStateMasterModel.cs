    using System.Data;
using VisionSoft;

class GSTStateMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string stateCodeStr = "";
    public string stateName = "";

    // Actual values for database operations
    public int stateCode = 0;

    public GSTStateMasterModel()
    {
        // Constructor - no special initialization needed
    }

    public void GenerateStateCode()
    {
        try
        {
            stateCode = Convert.ToInt32(db.GenerateNextNo("GSTStateMaster", "StateCode"));
            stateCodeStr = stateCode.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating state code: {ex.Message}");
            stateCode = 0;
            stateCodeStr = "0";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(stateCodeStr) || !int.TryParse(stateCodeStr, out _))
            isValid = false;

        // StateName is optional (nullable), so no validation required

        return isValid;
    }

    public async Task SaveState()
    {
        // Parse string values to actual types
        if (int.TryParse(stateCodeStr, out int parsedStateCode))
            stateCode = parsedStateCode;

        // Handle nullable StateName
        string stateNameValue = string.IsNullOrEmpty(stateName) ? "NULL" : $"'{stateName}'";

        // Prepare columns and values
        string columns = "StateCode, StateName";
        string values = $"{stateCode}, {stateNameValue}";

        // Save GST State record
        bool saved = db.SaveRecord("GSTStateMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save GST State record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(stateCodeStr, out int parsedStateCode))
                stateCode = parsedStateCode;

            // Handle nullable StateName
            string stateNameValue = string.IsNullOrEmpty(stateName) ? "NULL" : $"'{stateName}'";

            // Build the UPDATE query
            string query = $@"
                UPDATE GSTStateMaster SET 
                    StateName = {stateNameValue}
                WHERE StateCode = {stateCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update GST State record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating GST State record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            stateCodeStr = row["StateCode"]?.ToString() ?? "";
            stateName = row["StateName"]?.ToString() ?? "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        stateCodeStr = "";
        stateName = "";

        // Reset actual values
        stateCode = 0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        stateName = "Maharashtra";
        
        // Actual values for database operations
        // stateCode will be auto-generated
    }
}