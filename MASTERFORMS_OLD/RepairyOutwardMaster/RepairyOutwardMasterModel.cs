using System.Data;
using VisionSoft;

class RepairyOutwardMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string entryNoStr = "";
    public string entryDate = "";
    public string tokenIdStr = "0";
    public string supplierName = "";
    public string quantityStr = "0.00";
    public string remark = "";
    public string cancelledStr = "0";
    public string narration = "0"; // Note: Schema shows varchar but default is ((0))
    
    // Boolean field
    public bool received = false;

    // Actual values for database operations
    public long tokenId = 0;
    public int supplierCode = 0;
    public float quantity = 0.0f;
    public int cancelled = 0;

    public RepairyOutwardMasterModel()
    {
        // Initialize with default values
        tokenIdStr = "0";
        quantityStr = "0.00";
        cancelledStr = "0";
        narration = "0"; // Default as per schema
    }

    public void GenerateEntryNumber()
    {
        try
        {
            // For varchar EntryNo, we might need to format it appropriately
            int nextNo = Convert.ToInt32(db.GenerateNextNo("RepairyOutwardMaster", "EntryNo"));
            entryNoStr = $"{nextNo}"; // Format as RO000001, RO000002, etc.
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating entry number: {ex.Message}");
            entryNoStr = "RO000001";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(entryNoStr))
            isValid = false;

        if (string.IsNullOrWhiteSpace(tokenIdStr) || !long.TryParse(tokenIdStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(supplierName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(quantityStr) || !float.TryParse(quantityStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(remark))
            isValid = false;

        if (string.IsNullOrWhiteSpace(cancelledStr) || !int.TryParse(cancelledStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(narration))
            isValid = false;

        // EntryDate is nullable (optional)
        // Received is a boolean field, always valid (checkbox state)

        return isValid;
    }

    public async Task SaveRepairyOutward()
    {
        // Parse string values to actual types
        if (long.TryParse(tokenIdStr, out long parsedTokenId))
            tokenId = parsedTokenId;

        if (float.TryParse(quantityStr, out float parsedQuantity))
            quantity = parsedQuantity;

        if (int.TryParse(cancelledStr, out int parsedCancelled))
            cancelled = parsedCancelled;

        // Handle nullable datetime field
        string entryDateValue = string.IsNullOrEmpty(entryDate) ? "NULL" : $"'{entryDate}'";

        // Prepare columns and values
        string columns = "EntryNo, EntryDate, TokenId, SupplierCode, Quantity, Remark, Cancelled, Received, Narration";
        string values = $"'{entryNoStr}', {entryDateValue}, {tokenId}, {supplierCode}, {quantity}, '{remark}', {cancelled}, {(received ? 1 : 0)}, '{narration}'";

        // Save RepairyOutward record
        bool saved = db.SaveRecord("RepairyOutwardMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Repairy Outward record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (long.TryParse(tokenIdStr, out long parsedTokenId))
                tokenId = parsedTokenId;

            if (float.TryParse(quantityStr, out float parsedQuantity))
                quantity = parsedQuantity;

            if (int.TryParse(cancelledStr, out int parsedCancelled))
                cancelled = parsedCancelled;

            // Handle nullable datetime field
            string entryDateValue = string.IsNullOrEmpty(entryDate) ? "NULL" : $"'{entryDate}'";

            // Build the UPDATE query
            string query = $@"
                UPDATE RepairyOutwardMaster SET 
                    EntryDate = {entryDateValue},
                    TokenId = {tokenId},
                    SupplierCode = {supplierCode},
                    Quantity = {quantity},
                    Remark = '{remark}',
                    Cancelled = {cancelled},
                    Received = {(received ? 1 : 0)},
                    Narration = '{narration}'
                WHERE EntryNo = '{entryNoStr}'";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Repairy Outward record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Repairy Outward record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            entryNoStr = row["EntryNo"]?.ToString() ?? "";
            entryDate = row["EntryDate"]?.ToString() ?? "";
            tokenIdStr = row["TokenId"]?.ToString() ?? "0";
            quantityStr = row["Quantity"]?.ToString() ?? "0.00";
            remark = row["Remark"]?.ToString() ?? "";
            cancelledStr = row["Cancelled"]?.ToString() ?? "0";
            narration = row["Narration"]?.ToString() ?? "0";

            // Handle boolean field
            if (row["Received"] != DBNull.Value)
                received = Convert.ToBoolean(row["Received"]);

            // Populate supplier reference field
            if (row["SupplierCode"] != DBNull.Value)
            {
                supplierCode = Convert.ToInt32(row["SupplierCode"]);
                supplierName = db.GetScalar("SupplierMaster", "SupplierName", $"SupplierCode = {supplierCode}");
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
        entryNoStr = "";
        entryDate = "";
        tokenIdStr = "0";
        supplierName = "";
        quantityStr = "0.00";
        remark = "";
        cancelledStr = "0";
        narration = "0";
        received = false;

        // Reset actual values
        tokenId = 0;
        supplierCode = 0;
        quantity = 0.0f;
        cancelled = 0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        tokenIdStr = "12345";
        supplierName = "TechRepair Solutions";
        quantityStr = "5.00";
        remark = "Defective motherboards sent for repair";
        cancelledStr = "0";
        narration = "Warranty repair items";
        received = false;
        
        // Actual values for database operations
        tokenId = 12345;
        quantity = 5.0f;
        cancelled = 0;
        // supplierCode will be fetched from supplier name
    }
}