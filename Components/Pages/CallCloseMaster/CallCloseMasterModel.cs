using System.Data;
using VisionSoft;

class CallCloseMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string entryNo = "";
    public string entryDate = DateTime.Now.ToString("yyyy-MM-dd");
    public string modeOfPay = "";
    public string tokenID = "";
    public string completionDate = "";
    public string completionTime = "";
    public string amountStr = "";
    public string discountStr = "";
    public string netAmountStr = "";
    public string narration = "";
    public bool considerInAccounts = false;
    public string cancelledStr = "0";
    public string closedBy = "";

    // Actual values for database operations
    public double amount = 0;
    public double discount = 0;
    public double netAmount = 0;
    public int cancelled = 0;

    public CallCloseMasterModel()
    {
        GenerateEntryNumber();
    }

    public void GenerateEntryNumber()
    {
        try
        {
            string prefix = "A10";
            
            
            // Get next sequence number for today
            string query = $"SELECT COUNT(*) FROM CallCloseMaster WHERE EntryNo LIKE '{prefix}%'";
            DataTable dt = db.GetDataTable(query);
            
            int sequenceNumber = 1;
            if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
            {
                sequenceNumber = Convert.ToInt32(dt.Rows[0][0]) + 1;
            }
            
            entryNo = $"{prefix}{sequenceNumber:D3}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating entry number: {ex.Message}");
            entryNo = $"CC{DateTime.Now:yyyyMMdd}001";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(entryNo))
            isValid = false;

        if (string.IsNullOrWhiteSpace(modeOfPay))
            isValid = false;

        if (string.IsNullOrWhiteSpace(tokenID))
            isValid = false;

        if (string.IsNullOrWhiteSpace(amountStr) || !double.TryParse(amountStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(discountStr) || !double.TryParse(discountStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(narration))
            isValid = false;

        if (string.IsNullOrWhiteSpace(cancelledStr) || !int.TryParse(cancelledStr, out _))
            isValid = false;

        return isValid;
    }

    public async Task SaveCallClose()
    {
        // Parse string values to actual types
        double.TryParse(amountStr, out amount);
        double.TryParse(discountStr, out discount);
        double.TryParse(netAmountStr, out netAmount);
        int.TryParse(cancelledStr, out cancelled);

        // Prepare date values for database
        string entryDateValue = string.IsNullOrEmpty(entryDate) ? "NULL" : $"'{entryDate}'";
        string completionDateValue = string.IsNullOrEmpty(completionDate) ? "NULL" : $"'{completionDate}'";
        string completionTimeValue = string.IsNullOrEmpty(completionTime) ? "NULL" : $"'{completionTime}'";
        string closedByValue = string.IsNullOrEmpty(closedBy) ? "NULL" : $"'{closedBy}'";

        // Prepare columns and values
        string columns = "EntryNo, EntryDate, ModeOfPay, TokenID, CompletionDate, CompletionTime, Amount, Discount, NetAmount, Narration, ConsiderInAccounts, Cancelled, ClosedBy";
        string values = $"'{entryNo}', {entryDateValue}, '{modeOfPay}', '{tokenID}', {completionDateValue}, {completionTimeValue}, " +
                       $"{amount}, {discount}, {netAmount}, '{narration}', {(considerInAccounts ? 1 : 0)}, {cancelled}, {closedByValue}";

        // Save Call Close record
        bool saved = db.SaveRecord("CallCloseMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Call Close record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            double.TryParse(amountStr, out amount);
            double.TryParse(discountStr, out discount);
            double.TryParse(netAmountStr, out netAmount);
            int.TryParse(cancelledStr, out cancelled);

            // Prepare date values for database
            string entryDateValue = string.IsNullOrEmpty(entryDate) ? "NULL" : $"'{entryDate}'";
            string completionDateValue = string.IsNullOrEmpty(completionDate) ? "NULL" : $"'{completionDate}'";
            string completionTimeValue = string.IsNullOrEmpty(completionTime) ? "NULL" : $"'{completionTime}'";
            string closedByValue = string.IsNullOrEmpty(closedBy) ? "NULL" : $"'{closedBy}'";

            // Build the UPDATE query
            string query = $@"
                UPDATE CallCloseMaster SET 
                    EntryDate = {entryDateValue}, 
                    ModeOfPay = '{modeOfPay}', 
                    TokenID = '{tokenID}',
                    CompletionDate = {completionDateValue},
                    CompletionTime = {completionTimeValue},
                    Amount = {amount},
                    Discount = {discount},
                    NetAmount = {netAmount},
                    Narration = '{narration}',
                    ConsiderInAccounts = {(considerInAccounts ? 1 : 0)},
                    Cancelled = {cancelled},
                    ClosedBy = {closedByValue}
                WHERE EntryNo = '{entryNo}'";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Call Close record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Call Close record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            entryNo = row["EntryNo"]?.ToString() ?? "";
            entryDate = row["EntryDate"] != DBNull.Value ? Convert.ToDateTime(row["EntryDate"]).ToString("yyyy-MM-dd") : "";
            modeOfPay = row["ModeOfPay"]?.ToString() ?? "";
            tokenID = row["TokenID"]?.ToString() ?? "";
            completionDate = row["CompletionDate"] != DBNull.Value ? Convert.ToDateTime(row["CompletionDate"]).ToString("yyyy-MM-dd") : "";
            completionTime = row["CompletionTime"] != DBNull.Value ? Convert.ToDateTime(row["CompletionTime"]).ToString("HH:mm") : "";
            amountStr = row["Amount"]?.ToString() ?? "";
            discountStr = row["Discount"]?.ToString() ?? "";
            netAmountStr = row["NetAmount"]?.ToString() ?? "";
            narration = row["Narration"]?.ToString() ?? "";
            considerInAccounts = row["ConsiderInAccounts"] != DBNull.Value && Convert.ToBoolean(row["ConsiderInAccounts"]);
            cancelledStr = row["Cancelled"]?.ToString() ?? "";
            closedBy = row["ClosedBy"]?.ToString() ?? "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        entryNo = "";
        entryDate = DateTime.Now.ToString("yyyy-MM-dd");
        modeOfPay = "";
        tokenID = "";
        completionDate = "";
        completionTime = "";
        amountStr = "";
        discountStr = "";
        netAmountStr = "";
        narration = "";
        considerInAccounts = false;
        cancelledStr = "0";
        closedBy = "";

        // Reset actual values
        amount = 0;
        discount = 0;
        netAmount = 0;
        cancelled = 0;
    }

    public void CalculateNetAmount()
    {
        try
        {
            // Parse string values to doubles
            if (double.TryParse(amountStr, out double parsedAmount) &&
                double.TryParse(discountStr, out double parsedDiscount))
            {
                amount = parsedAmount;
                discount = parsedDiscount;

                // Calculate net amount (Amount - Discount)
                netAmount = amount - discount;

                // Ensure net amount doesn't go negative
                if (netAmount < 0)
                    netAmount = 0;

                // Update string value for display
                netAmountStr = netAmount.ToString("F2");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calculating net amount: {ex.Message}");
            netAmountStr = "0.00";
        }
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        modeOfPay = "Cash";
        tokenID = "TK001";
        completionDate = DateTime.Now.ToString("yyyy-MM-dd");
        completionTime = DateTime.Now.ToString("HH:mm");
        amountStr = "1000.00";
        discountStr = "100.00";
        netAmountStr = "900.00";
        narration = "Service call completed";
        considerInAccounts = true;
        cancelledStr = "0";
        closedBy = "Admin";

        // Actual values for database operations
        amount = 1000.00;
        discount = 100.00;
        netAmount = 900.00;
        cancelled = 0;
    }
}