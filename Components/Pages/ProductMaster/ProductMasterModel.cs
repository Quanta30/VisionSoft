using System.Data;
using VisionSoft;

class ProductMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string productCodeStr = "";
    public string productName = "";
    public string companyName = "";
    public string gstCategoryName = "";
    public string hsnCodeName = "";
    public string description = "";
    public string warrantyStr = "0";
    public string onlineCallsStr = "0";
    public string onsiteCallsStr = "0";
    public string trainingCallsStr = "0";
    public string maximumBookingStockStr = "0";

    // Boolean field
    public bool maintainStock = true; // Default to true as per schema

    // Actual values for database operations
    public int productCode = 0;
    public int companyCode = 0;
    public int gstCategoryCode = 0;
    public string hsnCode = "";
    public int warranty = 0;
    public int onlineCalls = 0;
    public int onsiteCalls = 0;
    public int trainingCalls = 0;
    public int maximumBookingStock = 0;

    public ProductMasterModel()
    {
        // Initialize with default values
        warrantyStr = "0";
        onlineCallsStr = "0";
        onsiteCallsStr = "0";
        trainingCallsStr = "0";
        maximumBookingStockStr = "0";
        maintainStock = true; // Default to true ('1' in schema)
    }

    public void GenerateProductCode()
    {
        try
        {
            productCode = Convert.ToInt32(db.GenerateNextNo("ProductMaster", "ProductCode"));
            productCodeStr = productCode.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating product code: {ex.Message}");
            productCode = 0;
            productCodeStr = "0";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(productCodeStr) || !int.TryParse(productCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(productName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(companyName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(gstCategoryName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(hsnCodeName))
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

        if (string.IsNullOrWhiteSpace(maximumBookingStockStr) || !int.TryParse(maximumBookingStockStr, out _))
            isValid = false;

        // MaintainStock is a boolean field, always valid (checkbox state)

        return isValid;
    }

    public async Task SaveProduct()
    {
        // Parse string values to actual types
        if (int.TryParse(productCodeStr, out int parsedProductCode))
            productCode = parsedProductCode;

        if (int.TryParse(warrantyStr, out int parsedWarranty))
            warranty = parsedWarranty;

        if (int.TryParse(onlineCallsStr, out int parsedOnlineCalls))
            onlineCalls = parsedOnlineCalls;

        if (int.TryParse(onsiteCallsStr, out int parsedOnsiteCalls))
            onsiteCalls = parsedOnsiteCalls;

        if (int.TryParse(trainingCallsStr, out int parsedTrainingCalls))
            trainingCalls = parsedTrainingCalls;

        if (int.TryParse(maximumBookingStockStr, out int parsedMaximumBookingStock))
            maximumBookingStock = parsedMaximumBookingStock;

        // Prepare columns and values
        string columns = @"ProductCode, ProductName, CompanyCode, GSTCategoryCode, HSNCode, Description, 
                          Warranty, MaintainStock, OnlineCalls, OnsiteCalls, TrainingCalls, MaximumBookingStock";

        string values = $@"{productCode}, '{productName}', {companyCode}, {gstCategoryCode}, '{hsnCode}', '{description}', 
                          {warranty}, {(maintainStock ? 1 : 0)}, {onlineCalls}, {onsiteCalls}, {trainingCalls}, {maximumBookingStock}";

        // Save Product record
        bool saved = db.SaveRecord("ProductMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Product record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(productCodeStr, out int parsedProductCode))
                productCode = parsedProductCode;

            if (int.TryParse(warrantyStr, out int parsedWarranty))
                warranty = parsedWarranty;

            if (int.TryParse(onlineCallsStr, out int parsedOnlineCalls))
                onlineCalls = parsedOnlineCalls;

            if (int.TryParse(onsiteCallsStr, out int parsedOnsiteCalls))
                onsiteCalls = parsedOnsiteCalls;

            if (int.TryParse(trainingCallsStr, out int parsedTrainingCalls))
                trainingCalls = parsedTrainingCalls;

            if (int.TryParse(maximumBookingStockStr, out int parsedMaximumBookingStock))
                maximumBookingStock = parsedMaximumBookingStock;

            // Build the UPDATE query
            string query = $@"
                UPDATE ProductMaster SET 
                    ProductName = '{productName}',
                    CompanyCode = {companyCode},
                    GSTCategoryCode = {gstCategoryCode},
                    HSNCode = '{hsnCode}',
                    Description = '{description}',
                    Warranty = {warranty},
                    MaintainStock = {(maintainStock ? 1 : 0)},
                    OnlineCalls = {onlineCalls},
                    OnsiteCalls = {onsiteCalls},
                    TrainingCalls = {trainingCalls},
                    MaximumBookingStock = {maximumBookingStock}
                WHERE ProductCode = {productCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Product record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Product record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            productCodeStr = row["ProductCode"]?.ToString() ?? "";
            productName = row["ProductName"]?.ToString() ?? "";
            description = row["Description"]?.ToString() ?? "";
            hsnCode = row["HSNCode"]?.ToString() ?? "";
            warrantyStr = row["Warranty"]?.ToString() ?? "0";
            onlineCallsStr = row["OnlineCalls"]?.ToString() ?? "0";
            onsiteCallsStr = row["OnsiteCalls"]?.ToString() ?? "0";
            trainingCallsStr = row["TrainingCalls"]?.ToString() ?? "0";
            maximumBookingStockStr = row["MaximumBookingStock"]?.ToString() ?? "0";

            // Handle boolean field
            if (row["MaintainStock"] != DBNull.Value)
                maintainStock = Convert.ToBoolean(row["MaintainStock"]);

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

            // For HSN Code, display the HSNCode value
            hsnCodeName = hsnCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        productCodeStr = "";
        productName = "";
        companyName = "";
        gstCategoryName = "";
        hsnCodeName = "";
        description = "";
        warrantyStr = "0";
        onlineCallsStr = "0";
        onsiteCallsStr = "0";
        trainingCallsStr = "0";
        maximumBookingStockStr = "0";
        maintainStock = true; // Reset to default

        // Reset actual values
        productCode = 0;
        companyCode = 0;
        gstCategoryCode = 0;
        hsnCode = "";
        warranty = 0;
        onlineCalls = 0;
        onsiteCalls = 0;
        trainingCalls = 0;
        maximumBookingStock = 0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        productName = "Professional Security Camera System";
        companyName = "VisionSoft Technologies";
        gstCategoryName = "SGST + CGST";
        hsnCodeName = "8525";
        description = "High-definition security camera with night vision";
        warrantyStr = "24";
        maintainStock = true;
        onlineCallsStr = "5";
        onsiteCallsStr = "3";
        trainingCallsStr = "2";
        maximumBookingStockStr = "50";
        
        // Actual values for database operations
        hsnCode = "8525";
        warranty = 24;
        onlineCalls = 5;
        onsiteCalls = 3;
        trainingCalls = 2;
        maximumBookingStock = 50;
        // productCode will be auto-generated
    }
}