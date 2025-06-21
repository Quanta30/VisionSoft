using System.Data;
using VisionSoft;

public class AdjustmentDetailModel
{
    ClsDatabase db = new ClsDatabase();
    
    // Form field variables (strings for binding)
    public string adjustmentTypeStr = "";
    public string srNoStr = "0";
    public string quantityStr = "0.00";
    public string rateStr = "0.00";
    public string stockIDStr = "0";
    
    // Additional fields for display (not in database)
    public string productName = "";
    public string currentQuantity = "0.00";
    
    // Actual values for database operations
    public long adjustmentNo = 0;
    public int srNo = 0;
    public float quantity = 0.0f;
    public float rate = 0.0f;
    public long stockID = 0;

    public AdjustmentDetailModel()
    {
        // Initialize with default values
        adjustmentTypeStr = "-";
        srNoStr = "0";
        quantityStr = "0.00";
        rateStr = "0.00";
        stockIDStr = "0";
    }

    public bool ValidateDetail()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(adjustmentTypeStr))
            isValid = false;

        if (string.IsNullOrWhiteSpace(srNoStr) || !int.TryParse(srNoStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(quantityStr) || !float.TryParse(quantityStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(rateStr) || !float.TryParse(rateStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(stockIDStr) || !long.TryParse(stockIDStr, out _))
            isValid = false;

        // Check varchar length constraints
        if (!string.IsNullOrWhiteSpace(adjustmentTypeStr) && adjustmentTypeStr.Length > 50)
            isValid = false;

        return isValid;
    }

    public async Task SaveDetail()
    {
        // Parse string values to actual types
        if (int.TryParse(srNoStr, out int parsedSrNo))
            srNo = parsedSrNo;

        if (float.TryParse(quantityStr, out float parsedQuantity))
            quantity = parsedQuantity;

        if (float.TryParse(rateStr, out float parsedRate))
            rate = parsedRate;

        if (long.TryParse(stockIDStr, out long parsedStockID))
            stockID = parsedStockID;

        // Prepare columns and values for AdjustmentDetails
        string columns = "AdjustmentNo, AdjustmentType, SrNo, Quantity, Rate, StockID";
        string values = $"{adjustmentNo}, '{adjustmentTypeStr}', {srNo}, {quantity}, {rate}, {stockID}";

        // Save AdjustmentDetail record
        bool saved = db.SaveRecord("AdjustmentDetails", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Adjustment Detail record");
        }

        // Update PhysicalStock quantity based on adjustment type
        await UpdatePhysicalStockQuantity();
    }

    private async Task UpdatePhysicalStockQuantity()
    {
        try
        {
            // Get current quantity from PhysicalStock
            string selectQuery = $"SELECT Quantity FROM PhysicalStock WHERE StockID = {stockID}";
            DataTable dt = db.GetDataTable(selectQuery);
            
            if (dt.Rows.Count == 0)
            {
                throw new Exception($"PhysicalStock record not found for StockID: {stockID}");
            }

            float currentStockQuantity = Convert.ToSingle(dt.Rows[0]["Quantity"]);
            float newQuantity = currentStockQuantity;

            // Determine new quantity based on adjustment type
            switch (adjustmentTypeStr.ToUpper())
            {
                case "ADD":
                    // Add quantity to stock
                    newQuantity = currentStockQuantity + quantity;
                    break;
                case "LESS":
                    // Subtract quantity from stock
                    newQuantity = currentStockQuantity - quantity;
                    break;
                    
                default:
                    // For other adjustment types, subtract by default (most common scenario)
                    newQuantity = currentStockQuantity - quantity;
                    break;
            }

            // Ensure quantity doesn't go below zero (optional - you might want to allow negative stock)
            if (newQuantity < 0)
            {
                Console.WriteLine($"Warning: Stock quantity for StockID {stockID} will become negative: {newQuantity}");
                // Uncomment the next line if you want to prevent negative stock
                // throw new Exception($"Insufficient stock. Current: {currentStockQuantity}, Adjustment: {quantity}");
            }

            // Update PhysicalStock table
            string updateQuery = $@"
                UPDATE PhysicalStock 
                SET Quantity = {newQuantity} 
                WHERE StockID = {stockID}";

            bool updated = db.ExecuteQuery(updateQuery);
            if (!updated)
            {
                throw new Exception($"Failed to update PhysicalStock quantity for StockID: {stockID}");
            }

            Console.WriteLine($"Updated PhysicalStock StockID {stockID}: {currentStockQuantity} -> {newQuantity} (Adjustment: {adjustmentTypeStr} {quantity})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating PhysicalStock quantity: {ex.Message}");
            throw;
        }
    }

    public void Set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            adjustmentNo = row["AdjustmentNo"] != DBNull.Value ? Convert.ToInt64(row["AdjustmentNo"]) : 0;
            adjustmentTypeStr = row["AdjustmentType"]?.ToString() ?? "";
            srNoStr = row["SrNo"]?.ToString() ?? "0";
            quantityStr = row["Quantity"]?.ToString() ?? "0.00";
            rateStr = row["Rate"]?.ToString() ?? "0.00";
            stockIDStr = row["StockID"]?.ToString() ?? "0";

            // Parse actual values
            if (int.TryParse(srNoStr, out int parsedSrNo))
                srNo = parsedSrNo;

            if (float.TryParse(quantityStr, out float parsedQuantity))
                quantity = parsedQuantity;

            if (float.TryParse(rateStr, out float parsedRate))
                rate = parsedRate;

            if (long.TryParse(stockIDStr, out long parsedStockID))
                stockID = parsedStockID;

            // Load product information if StockID is available
            LoadProductInfo();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting detail data: {ex.Message}");
            throw;
        }
    }

    private void LoadProductInfo()
    {
        try
        {
            if (stockID > 0)
            {
                // Join PhysicalStock with ProductMaster to get product information
                string query = $@"
                    SELECT p.ProductName, ps.Quantity 
                    FROM PhysicalStock ps 
                    INNER JOIN ProductMaster p ON ps.ProductCode = p.ProductCode 
                    WHERE ps.StockID = {stockID}";
                
                DataTable dt = db.GetDataTable(query);
                if (dt.Rows.Count > 0)
                {
                    productName = dt.Rows[0]["ProductName"]?.ToString() ?? "";
                    currentQuantity = dt.Rows[0]["Quantity"]?.ToString() ?? "0.00";
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading product info: {ex.Message}");
        }
    }

    public void Clear()
    {
        adjustmentTypeStr = "-";
        srNoStr = "0";
        quantityStr = "0.00";
        rateStr = "0.00";
        stockIDStr = "0";
        productName = "";
        currentQuantity = "0.00";

        // Reset actual values
        adjustmentNo = 0;
        srNo = 0;
        quantity = 0.0f;
        rate = 0.0f;
        stockID = 0;
    }

    public float GetAmount()
    {
        return quantity * rate;
    }
}