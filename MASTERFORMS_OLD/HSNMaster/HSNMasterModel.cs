using System.Data;
using VisionSoft;

class HsnMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string entryNoStr = "";
    public string hsnCode = "";
    public string description = "";
    public string uqc = "";
    public string uqcCodeStr = "0";

    // Actual values for database operations
    public int entryNo = 0;
    public int uqcCode = 0;

    public HsnMasterModel()
    {
        // Initialize with default UQC code
        uqcCodeStr = "0";
    }

    public void GenerateEntryNumber()
    {
        try
        {
            entryNo = Convert.ToInt32(db.GenerateNextNo("HsnMaster", "EntryNo"));
            entryNoStr = entryNo.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating entry number: {ex.Message}");
            entryNo = 1;
            entryNoStr = "1";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(entryNoStr) || !int.TryParse(entryNoStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(hsnCode))
            isValid = false;

        if (string.IsNullOrWhiteSpace(description))
            isValid = false;

        if (string.IsNullOrWhiteSpace(uqc))
            isValid = false;

        if (string.IsNullOrWhiteSpace(uqcCodeStr) || !int.TryParse(uqcCodeStr, out _))
            isValid = false;

        return isValid;
    }

    public async Task SaveHsn()
    {
        // Parse string values to actual types
        if (int.TryParse(entryNoStr, out int parsedEntryNo))
            entryNo = parsedEntryNo;

        if (int.TryParse(uqcCodeStr, out int parsedUqcCode))
            uqcCode = parsedUqcCode;

        // Prepare columns and values
        string columns = "EntryNo, HSNCode, Description, UQC, UQCCode";
        string values = $"{entryNo}, '{hsnCode}', '{description}', '{uqc}', {uqcCode}";

        // Save HSN record
        bool saved = db.SaveRecord("HsnMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save HSN record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(entryNoStr, out int parsedEntryNo))
                entryNo = parsedEntryNo;

            if (int.TryParse(uqcCodeStr, out int parsedUqcCode))
                uqcCode = parsedUqcCode;

            // Build the UPDATE query
            string query = $@"
                UPDATE HsnMaster SET 
                    HSNCode = '{hsnCode}',
                    Description = '{description}',
                    UQC = '{uqc}',
                    UQCCode = {uqcCode}
                WHERE EntryNo = {entryNo}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update HSN record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating HSN record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            entryNoStr = row["EntryNo"]?.ToString() ?? "";
            hsnCode = row["HSNCode"]?.ToString() ?? "";
            description = row["Description"]?.ToString() ?? "";
            uqc = row["UQC"]?.ToString() ?? "";
            uqcCodeStr = row["UQCCode"]?.ToString() ?? "0";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        entryNoStr = "";
        hsnCode = "";
        description = "";
        uqc = "";
        uqcCodeStr = "0";

        // Reset actual values
        entryNo = 0;
        uqcCode = 0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        hsnCode = "8517";
        description = "Telephone sets, mobile phones and other communication equipment";
        uqc = "NOS";
        uqcCodeStr = "01";
        
        // Actual values for database operations
        uqcCode = 1;
        // entryNo will be auto-generated
    }
}