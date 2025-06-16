using System.Data;
using VisionSoft;

class ReferenceMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string referenceCodeStr = "";
    public string referenceName = "";
    public string address = "";
    public string city = "";
    public string phoneNo = "";
    public string mobileNo = "";

    // Actual values for database operations
    public long referenceCode = 0;

    public ReferenceMasterModel()
    {
        // Constructor - no special initialization needed
    }

    public void GenerateReferenceCode()
    {
        try
        {
            referenceCode = Convert.ToInt64(db.GenerateNextNo("ReferenceMaster", "ReferenceCode"));
            referenceCodeStr = referenceCode.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating reference code: {ex.Message}");
            referenceCode = 1;
            referenceCodeStr = "1";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(referenceCodeStr) || !long.TryParse(referenceCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(referenceName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(address))
            isValid = false;

        if (string.IsNullOrWhiteSpace(city))
            isValid = false;

        if (string.IsNullOrWhiteSpace(phoneNo))
            isValid = false;

        if (string.IsNullOrWhiteSpace(mobileNo))
            isValid = false;

        return isValid;
    }

    public async Task SaveReference()
    {
        // Parse string values to actual types
        if (long.TryParse(referenceCodeStr, out long parsedReferenceCode))
            referenceCode = parsedReferenceCode;

        // Prepare columns and values
        string columns = "ReferenceCode, ReferenceName, Address, City, PhoneNo, MobileNo";
        string values = $"{referenceCode}, '{referenceName}', '{address}', '{city}', '{phoneNo}', '{mobileNo}'";

        // Save Reference record
        bool saved = db.SaveRecord("ReferenceMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Reference record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (long.TryParse(referenceCodeStr, out long parsedReferenceCode))
                referenceCode = parsedReferenceCode;

            // Build the UPDATE query
            string query = $@"
                UPDATE ReferenceMaster SET 
                    ReferenceName = '{referenceName}',
                    Address = '{address}',
                    City = '{city}',
                    PhoneNo = '{phoneNo}',
                    MobileNo = '{mobileNo}'
                WHERE ReferenceCode = {referenceCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Reference record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Reference record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            referenceCodeStr = row["ReferenceCode"]?.ToString() ?? "";
            referenceName = row["ReferenceName"]?.ToString() ?? "";
            address = row["Address"]?.ToString() ?? "";
            city = row["City"]?.ToString() ?? "";
            phoneNo = row["PhoneNo"]?.ToString() ?? "";
            mobileNo = row["MobileNo"]?.ToString() ?? "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        referenceCodeStr = "";
        referenceName = "";
        address = "";
        city = "";
        phoneNo = "";
        mobileNo = "";

        // Reset actual values
        referenceCode = 0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        referenceName = "TechSolutions India Pvt Ltd";
        address = "456 Technology Park, Sector 21";
        city = "Pune";
        phoneNo = "020-12345678";
        mobileNo = "9876543210";
        
        // Actual values for database operations
        // referenceCode will be auto-generated
    }
}