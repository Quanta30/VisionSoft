using System.Data;
using VisionSoft;

public class AdjustmentHeadModel
{
    ClsDatabase db = new ClsDatabase();
    
    // Form field variables (strings for binding)
    public string adjustmentNoStr = "";
    public string adjustmentDate = "";
    public string netAmountStr = "0.00";
    public string narration = "";
    public string cancelledStr = "0";
    
    // Actual values for database operations
    public long adjustmentNo = 0;
    public float netAmount = 0.0f;
    public int cancelled = 0;
    
    public List<AdjustmentDetailModel> adjustmentDetails = new List<AdjustmentDetailModel>();

    public AdjustmentHeadModel()
    {
        // Initialize with default values
        netAmountStr = "0.00";
        cancelledStr = "0";
        narration = "-";
    }

    public void GenerateAdjustmentNo()
    {
        try
        {
            adjustmentNoStr = db.GenerateNextNo("AdjustmentHead", "AdjustmentNo");
            if (long.TryParse(adjustmentNoStr, out long parsedNo))
                adjustmentNo = parsedNo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating adjustment number: {ex.Message}");
            adjustmentNoStr = "1";
            adjustmentNo = 1;
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(adjustmentNoStr))
            isValid = false;

        if (string.IsNullOrWhiteSpace(narration))
            isValid = false;

        if (string.IsNullOrWhiteSpace(netAmountStr) || !float.TryParse(netAmountStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(cancelledStr) || !int.TryParse(cancelledStr, out _))
            isValid = false;

        // Check varchar length constraints
        if (!string.IsNullOrWhiteSpace(narration) && narration.Length > 200)
            isValid = false;

        // Check if at least one detail exists
        if (!adjustmentDetails.Any())
            isValid = false;

        return isValid;
    }

    public async Task SaveAdjustment()
    {
        // Parse string values to actual types
        if (long.TryParse(adjustmentNoStr, out long parsedAdjustmentNo))
            adjustmentNo = parsedAdjustmentNo;

        if (float.TryParse(netAmountStr, out float parsedNetAmount))
            netAmount = parsedNetAmount;

        if (int.TryParse(cancelledStr, out int parsedCancelled))
            cancelled = parsedCancelled;

        // Handle nullable datetime fields
        string adjustmentDateValue = string.IsNullOrEmpty(adjustmentDate) ? "NULL" : $"'{adjustmentDate}'";

        // Prepare columns and values for AdjustmentHead
        string columns = "AdjustmentNo, AdjustmentDate, NetAmount, Narration, Cancelled";
        string values = $"{adjustmentNo}, {adjustmentDateValue}, {netAmount}, '{narration}', {cancelled}";

        // Save AdjustmentHead record
        bool saved = db.SaveRecord("AdjustmentHead", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Adjustment Head record");
        }

        // Save AdjustmentDetails records
        foreach (var detail in adjustmentDetails)
        {
            detail.adjustmentNo = adjustmentNo;
            await detail.SaveDetail();
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (long.TryParse(adjustmentNoStr, out long parsedAdjustmentNo))
                adjustmentNo = parsedAdjustmentNo;

            if (float.TryParse(netAmountStr, out float parsedNetAmount))
                netAmount = parsedNetAmount;

            if (int.TryParse(cancelledStr, out int parsedCancelled))
                cancelled = parsedCancelled;

            // Handle nullable datetime fields
            string adjustmentDateValue = string.IsNullOrEmpty(adjustmentDate) ? "NULL" : $"'{adjustmentDate}'";

            // Build the UPDATE query for AdjustmentHead
            string query = $@"
                UPDATE AdjustmentHead SET 
                    AdjustmentDate = {adjustmentDateValue},
                    NetAmount = {netAmount},
                    Narration = '{narration}',
                    Cancelled = {cancelled}
                WHERE AdjustmentNo = {adjustmentNo}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Adjustment Head record");
            }

            // Delete existing details and re-insert
            string deleteQuery = $"DELETE FROM AdjustmentDetails WHERE AdjustmentNo = {adjustmentNo}";
            db.ExecuteQuery(deleteQuery);

            // Save updated details
            foreach (var detail in adjustmentDetails)
            {
                detail.adjustmentNo = adjustmentNo;
                await detail.SaveDetail();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Adjustment record: {ex.Message}");
            throw;
        }
    }

    public void Set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            adjustmentNoStr = row["AdjustmentNo"]?.ToString() ?? "";
            adjustmentDate = row["AdjustmentDate"]?.ToString() ?? "";
            netAmountStr = row["NetAmount"]?.ToString() ?? "0.00";
            narration = row["Narration"]?.ToString() ?? "";
            cancelledStr = row["Cancelled"]?.ToString() ?? "0";

            // Parse actual values
            if (long.TryParse(adjustmentNoStr, out long parsedNo))
                adjustmentNo = parsedNo;

            if (float.TryParse(netAmountStr, out float parsedAmount))
                netAmount = parsedAmount;

            if (int.TryParse(cancelledStr, out int parsedCancelled))
                cancelled = parsedCancelled;

            // Load adjustment details
            LoadAdjustmentDetails();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    private void LoadAdjustmentDetails()
    {
        try
        {
            string query = $"SELECT * FROM AdjustmentDetails WHERE AdjustmentNo = {adjustmentNo} ORDER BY SrNo";
            DataTable dt = db.GetDataTable(query);
            
            adjustmentDetails.Clear();
            foreach (DataRow row in dt.Rows)
            {
                var detail = new AdjustmentDetailModel();
                detail.Set(row);
                adjustmentDetails.Add(detail);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading adjustment details: {ex.Message}");
        }
    }

    public void Clear()
    {
        adjustmentNoStr = "";
        adjustmentDate = "";
        netAmountStr = "0.00";
        narration = "-";
        cancelledStr = "0";

        // Reset actual values
        adjustmentNo = 0;
        netAmount = 0.0f;
        cancelled = 0;

        // Clear details
        adjustmentDetails.Clear();
    }

    public void CalculateNetAmount()
    {
        netAmount = adjustmentDetails.Sum(d => d.quantity * d.rate);
        netAmountStr = netAmount.ToString("F2");
    }

    public void InitializeTestData()
    {
        narration = "Monthly stock adjustment for damaged goods";
        
        // Add some test details
        adjustmentDetails.Add(new AdjustmentDetailModel
        {
            adjustmentTypeStr = "Add",
            srNoStr = "1",
            quantityStr = "5.00",
            rateStr = "25.50",
            stockIDStr = "1001",
            productName = "Product A"
        });

        adjustmentDetails.Add(new AdjustmentDetailModel
        {
            adjustmentTypeStr = "Less",
            srNoStr = "2", 
            quantityStr = "3.00",
            rateStr = "15.75",
            stockIDStr = "1002",
            productName = "Product B"
        });

        CalculateNetAmount();
    }
}