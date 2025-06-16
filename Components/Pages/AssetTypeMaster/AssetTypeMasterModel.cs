using System.Data;
using VisionSoft;

class AssetTypeMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string assetTypeCodeStr = "";
    public string assetTypeName = "";

    // Actual values for database operations
    public int assetTypeCode = 0;

    public AssetTypeMasterModel()
    {
        // No auto-generation needed for AssetTypeMaster
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(assetTypeCodeStr) || !int.TryParse(assetTypeCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(assetTypeName))
            isValid = false;

        return isValid;
    }

    public async Task SaveAssetType()
    {
        // Parse string values to actual types
        if (int.TryParse(assetTypeCodeStr, out int parsedAssetTypeCode))
            assetTypeCode = parsedAssetTypeCode;

        // Prepare columns and values
        string columns = "AssetTypeCode, AssetTypeName";
        string values = $"{assetTypeCode}, '{assetTypeName}'";

        // Save Asset Type record
        bool saved = db.SaveRecord("AssetTypeMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Asset Type record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(assetTypeCodeStr, out int parsedAssetTypeCode))
                assetTypeCode = parsedAssetTypeCode;

            // Build the UPDATE query
            string query = $@"
                UPDATE AssetTypeMaster SET 
                    AssetTypeName = '{assetTypeName}'
                WHERE AssetTypeCode = {assetTypeCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Asset Type record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Asset Type record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            assetTypeCodeStr = row["AssetTypeCode"]?.ToString() ?? "";
            assetTypeName = row["AssetTypeName"]?.ToString() ?? "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        assetTypeCodeStr = "";
        assetTypeName = "";

        // Reset actual values
        assetTypeCode = 0;
    }

    private void InitializeTestData()
    {
        // Form field variables with test data
        assetTypeCodeStr = "1";
        assetTypeName = "Computer";
        
        // Actual values for database operations
        assetTypeCode = 1;
    }
}