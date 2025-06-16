using System.Data;
using VisionSoft;

class CompanyMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string companyCodeStr = "";
    public string companyName = "";
    public bool companyPermanentLock = false;
    public string ownerMobileNo = "";

    // Actual values for database operations
    public int companyCode = 0;

    public CompanyMasterModel()
    {
        // No auto-generation needed for CompanyMaster
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(companyCodeStr) || !int.TryParse(companyCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(companyName))
            isValid = false;

        // Owner Mobile No is optional (nullable), so no validation required
        // Company Permanent Lock is a boolean, always has a value

        return isValid;
    }

    public async Task SaveCompany()
    {
        // Parse string values to actual types
        if (int.TryParse(companyCodeStr, out int parsedCompanyCode))
            companyCode = parsedCompanyCode;

        // Prepare columns and values
        string columns = "CompanyCode, CompanyName, CompanyPermanantLock, OwnerMobileNo";
        
        // Handle nullable OwnerMobileNo
        string ownerMobileValue = string.IsNullOrEmpty(ownerMobileNo) ? "NULL" : $"'{ownerMobileNo}'";
        
        string values = $"{companyCode}, '{companyName}', {(companyPermanentLock ? 1 : 0)}, {ownerMobileValue}";

        // Save Company record
        bool saved = db.SaveRecord("CompanyMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Company record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(companyCodeStr, out int parsedCompanyCode))
                companyCode = parsedCompanyCode;

            // Handle nullable OwnerMobileNo
            string ownerMobileValue = string.IsNullOrEmpty(ownerMobileNo) ? "NULL" : $"'{ownerMobileNo}'";

            // Build the UPDATE query
            string query = $@"
                UPDATE CompanyMaster SET 
                    CompanyName = '{companyName}', 
                    CompanyPermanantLock = {(companyPermanentLock ? 1 : 0)},
                    OwnerMobileNo = {ownerMobileValue}
                WHERE CompanyCode = {companyCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Company record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Company record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            companyCodeStr = row["CompanyCode"]?.ToString() ?? "";
            companyName = row["CompanyName"]?.ToString() ?? "";
            
            // Handle boolean field
            if (row["CompanyPermanantLock"] != DBNull.Value)
            {
                companyPermanentLock = Convert.ToBoolean(row["CompanyPermanantLock"]);
            }
            
            // Handle nullable field
            ownerMobileNo = row["OwnerMobileNo"]?.ToString() ?? "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        companyCodeStr = "";
        companyName = "";
        companyPermanentLock = false;
        ownerMobileNo = "";

        // Reset actual values
        companyCode = 0;
    }

    private void InitializeTestData()
    {
        // Form field variables with test data
        companyCodeStr = "1";
        companyName = "Vision Technologies Pvt Ltd";
        companyPermanentLock = false;
        ownerMobileNo = "+91-9876543210";
        
        // Actual values for database operations
        companyCode = 1;
    }
}