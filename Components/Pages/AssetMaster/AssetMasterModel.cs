using System.Data;
using VisionSoft;

class AssetMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string assetCodeStr = "";
    public string assetName = "";
    public string assetNumber = "";
    public string departmentName = "";
    public string assetTypeName = "";

    // Actual values for database operations
    public int assetCode = 0;
    public int departmentCode = 0;
    public int assetTypeCode = 0;

    public AssetMasterModel()
    {
        // No auto-generation needed for AssetMaster
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(assetCodeStr) || !int.TryParse(assetCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(assetName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(assetNumber))
            isValid = false;

        if (string.IsNullOrWhiteSpace(departmentName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(assetTypeName))
            isValid = false;

        return isValid;
    }

    public async Task SaveAsset()
    {
        // Parse string values to actual types
        if (int.TryParse(assetCodeStr, out int parsedAssetCode))
            assetCode = parsedAssetCode;

        // Prepare columns and values
        string columns = "AssetCode, AssetName, AssetNumber, DepartmentCode, AssetTypeCode";
        string values = $"{assetCode}, N'{assetName}', N'{assetNumber}', {departmentCode}, {assetTypeCode}";

        // Save Asset record
        bool saved = db.SaveRecord("AssetMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Asset record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(assetCodeStr, out int parsedAssetCode))
                assetCode = parsedAssetCode;

            // Build the UPDATE query
            string query = $@"
                UPDATE AssetMaster SET 
                    AssetName = N'{assetName}', 
                    AssetNumber = N'{assetNumber}',
                    DepartmentCode = {departmentCode},
                    AssetTypeCode = {assetTypeCode}
                WHERE AssetCode = {assetCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Asset record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Asset record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            assetCodeStr = row["AssetCode"]?.ToString() ?? "";
            assetName = row["AssetName"]?.ToString() ?? "";
            assetNumber = row["AssetNumber"]?.ToString() ?? "";
            
            // Populate DepartmentCode and DepartmentName
            if (row["DepartmentCode"] != DBNull.Value)
            {
                departmentCode = Convert.ToInt32(row["DepartmentCode"]);
                departmentName = db.GetScalar("DepartmentMaster", "DepartmentName", $"DepartmentCode = {departmentCode}");
            }

            // Populate AssetTypeCode and AssetTypeName
            if (row["AssetTypeCode"] != DBNull.Value)
            {
                assetTypeCode = Convert.ToInt32(row["AssetTypeCode"]);
                assetTypeName = db.GetScalar("AssetTypeMaster", "AssetTypeName", $"AssetTypeCode = {assetTypeCode}");
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
        assetCodeStr = "";
        assetName = "";
        assetNumber = "";
        departmentName = "";
        assetTypeName = "";

        // Reset actual values
        assetCode = 0;
        departmentCode = 0;
        assetTypeCode = 0;
    }

    private void InitializeTestData()
    {
        // Form field variables with test data
        assetCodeStr = "1001";
        assetName = "Dell Laptop";
        assetNumber = "DL001";
        departmentName = "IT Department";
        assetTypeName = "Computer";
        
        // Actual values for database operations
        assetCode = 1001;
        departmentCode = 101;
        assetTypeCode = 201;
    }
}