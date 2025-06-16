using System.Data;
using VisionSoft;

class UQCMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string uqcCodeStr = "";
    public string uqcName = "";

    // Actual values for database operations
    public int uqcCode = 0;

    public UQCMasterModel()
    {
        // Constructor - no special initialization needed
    }

    public void GenerateUQCCode()
    {
        try
        {
            uqcCode = Convert.ToInt32(db.GenerateNextNo("UQCMaster", "UQCCode"));
            uqcCodeStr = uqcCode.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating UQC code: {ex.Message}");
            uqcCode = 1;
            uqcCodeStr = "1";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(uqcCodeStr) || !int.TryParse(uqcCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(uqcName))
            isValid = false;

        // Check varchar length constraint (100 characters max)
        if (!string.IsNullOrWhiteSpace(uqcName) && uqcName.Length > 100)
            isValid = false;

        return isValid;
    }

    public async Task SaveUQC()
    {
        // Parse string values to actual types
        if (int.TryParse(uqcCodeStr, out int parsedUQCCode))
            uqcCode = parsedUQCCode;

        // Prepare columns and values
        string columns = "UQCCode, UQCName";
        string values = $"{uqcCode}, '{uqcName}'";

        // Save UQC record
        bool saved = db.SaveRecord("UQCMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save UQC record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(uqcCodeStr, out int parsedUQCCode))
                uqcCode = parsedUQCCode;

            // Build the UPDATE query
            string query = $@"
                UPDATE UQCMaster SET 
                    UQCName = '{uqcName}'
                WHERE UQCCode = {uqcCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update UQC record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating UQC record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            uqcCodeStr = row["UQCCode"]?.ToString() ?? "";
            uqcName = row["UQCName"]?.ToString() ?? "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        uqcCodeStr = "";
        uqcName = "";

        // Reset actual values
        uqcCode = 0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        uqcName = "PCS - Pieces";
        
        // Actual values for database operations
        // uqcCode will be auto-generated
    }
}