using System.Data;
using VisionSoft;

class SecondHandProductMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string secondHandProductCodeStr = "";
    public string secondHandProductName = "";
    public string companyName = "";
    public string gstCategoryName = "";
    public string hsnCode = "";
    public string description = "";
    public string warrantyStr = "0";
    public string onlineCallsStr = "0";
    public string onsiteCallsStr = "0";
    public string trainingCallsStr = "0";

    // Actual values for database operations
    public int secondHandProductCode = 0;
    public int companyCode = 0;
    public int gstCategoryCode = 0;
    public int warranty = 0;
    public int onlineCalls = 0;
    public int onsiteCalls = 0;
    public int trainingCalls = 0;

    public SecondHandProductMasterModel()
    {
        // Initialize with default values
        warrantyStr = "0";
        onlineCallsStr = "0";
        onsiteCallsStr = "0";
        trainingCallsStr = "0";
    }

    public void GenerateProductCode()
    {
        try
        {
            secondHandProductCode = Convert.ToInt32(db.GenerateNextNo("SecondHandProductMaster", "SecondHandProductCode"));
            secondHandProductCodeStr = secondHandProductCode.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating second hand product code: {ex.Message}");
            secondHandProductCode = 0;
            secondHandProductCodeStr = "0";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(secondHandProductCodeStr) || !int.TryParse(secondHandProductCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(secondHandProductName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(companyName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(gstCategoryName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(hsnCode))
            isValid = false;

        if (string.IsNullOrWhiteSpace(description))
            isValid = false;

        if (string.IsNullOrWhiteSpace(warrantyStr) || !int.TryParse(warrantyStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(onlineCallsStr) || !int.TryParse(onlineCallsStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(onsiteCallsStr) || !int.TryParse(onsiteCallsStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(trainingCallsStr) || !int.TryParse(trainingCallsStr, out _))
            isValid = false;

        return isValid;
    }

    public async Task SaveSecondHandProduct()
    {
        // Parse string values to actual types
        if (int.TryParse(secondHandProductCodeStr, out int parsedProductCode))
            secondHandProductCode = parsedProductCode;

        if (int.TryParse(warrantyStr, out int parsedWarranty))
            warranty = parsedWarranty;

        if (int.TryParse(onlineCallsStr, out int parsedOnlineCalls))
            onlineCalls = parsedOnlineCalls;

        if (int.TryParse(onsiteCallsStr, out int parsedOnsiteCalls))
            onsiteCalls = parsedOnsiteCalls;

        if (int.TryParse(trainingCallsStr, out int parsedTrainingCalls))
            trainingCalls = parsedTrainingCalls;

        // Prepare columns and values
        string columns = @"SecondHandProductCode, SecondHandProductName, CompanyCode, GSTCategoryCode, HSNCode, 
                          Description, Warranty, OnlineCalls, OnsiteCalls, TrainingCalls";

        string values = $@"{secondHandProductCode}, '{secondHandProductName}', {companyCode}, {gstCategoryCode}, 
                          '{hsnCode}', '{description}', {warranty}, {onlineCalls}, {onsiteCalls}, {trainingCalls}";

        // Save SecondHandProduct record
        bool saved = db.SaveRecord("SecondHandProductMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Second Hand Product record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(secondHandProductCodeStr, out int parsedProductCode))
                secondHandProductCode = parsedProductCode;

            if (int.TryParse(warrantyStr, out int parsedWarranty))
                warranty = parsedWarranty;

            if (int.TryParse(onlineCallsStr, out int parsedOnlineCalls))
                onlineCalls = parsedOnlineCalls;

            if (int.TryParse(onsiteCallsStr, out int parsedOnsiteCalls))
                onsiteCalls = parsedOnsiteCalls;

            if (int.TryParse(trainingCallsStr, out int parsedTrainingCalls))
                trainingCalls = parsedTrainingCalls;

            // Build the UPDATE query
            string query = $@"
                UPDATE SecondHandProductMaster SET 
                    SecondHandProductName = '{secondHandProductName}',
                    CompanyCode = {companyCode},
                    GSTCategoryCode = {gstCategoryCode},
                    HSNCode = '{hsnCode}',
                    Description = '{description}',
                    Warranty = {warranty},
                    OnlineCalls = {onlineCalls},
                    OnsiteCalls = {onsiteCalls},
                    TrainingCalls = {trainingCalls}
                WHERE SecondHandProductCode = {secondHandProductCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Second Hand Product record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Second Hand Product record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            secondHandProductCodeStr = row["SecondHandProductCode"]?.ToString() ?? "";
            secondHandProductName = row["SecondHandProductName"]?.ToString() ?? "";
            hsnCode = row["HSNCode"]?.ToString() ?? "";
            description = row["Description"]?.ToString() ?? "";
            warrantyStr = row["Warranty"]?.ToString() ?? "0";
            onlineCallsStr = row["OnlineCalls"]?.ToString() ?? "0";
            onsiteCallsStr = row["OnsiteCalls"]?.ToString() ?? "0";
            trainingCallsStr = row["TrainingCalls"]?.ToString() ?? "0";

            // Populate reference fields
            if (row["CompanyCode"] != DBNull.Value)
            {
                companyCode = Convert.ToInt32(row["CompanyCode"]);
                companyName = db.GetScalar("CompanyMaster", "CompanyName", $"CompanyCode = {companyCode}");
            }

            if (row["GSTCategoryCode"] != DBNull.Value)
            {
                gstCategoryCode = Convert.ToInt32(row["GSTCategoryCode"]);
                gstCategoryName = db.GetScalar("GstCategoryMaster", "GSTCategoryName", $"GSTCategoryCode = {gstCategoryCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        secondHandProductCodeStr = "";
        secondHandProductName = "";
        companyName = "";
        gstCategoryName = "";
        hsnCode = "";
        description = "";
        warrantyStr = "0";
        onlineCallsStr = "0";
        onsiteCallsStr = "0";
        trainingCallsStr = "0";

        // Reset actual values
        secondHandProductCode = 0;
        companyCode = 0;
        gstCategoryCode = 0;
        warranty = 0;
        onlineCalls = 0;
        onsiteCalls = 0;
        trainingCalls = 0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        secondHandProductName = "Refurbished Laptop Dell Latitude E7470";
        companyName = "VisionSoft Technologies";
        gstCategoryName = "SGST + CGST";
        hsnCode = "8471";
        description = "Used laptop in good condition, tested and certified";
        warrantyStr = "6";
        onlineCallsStr = "2";
        onsiteCallsStr = "1";
        trainingCallsStr = "1";
        
        // Actual values for database operations
        warranty = 6;
        onlineCalls = 2;
        onsiteCalls = 1;
        trainingCalls = 1;
        // secondHandProductCode will be auto-generated
    }
}