using System.Data;
using VisionSoft;

class LedgerMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string ledgerCodeStr = "";
    public string ledgerName = "";
    public string groupName = "";
    public string debitOpeningBalanceStr = "0.00";
    public string creditOpeningBalanceStr = "0.00";
    public string closingBalanceStr = "0.00";

    // Actual values for database operations
    public long ledgerCode = 0;
    public int groupCode = 0;
    public double debitOpeningBalance = 0.0;
    public double creditOpeningBalance = 0.0;
    public double closingBalance = 0.0;

    public LedgerMasterModel()
    {
        // Initialize with default balance values
        debitOpeningBalanceStr = "0.00";
        creditOpeningBalanceStr = "0.00";
        closingBalanceStr = "0.00";
    }

    public void GenerateLedgerCode()
    {
        try
        {
            ledgerCode = Convert.ToInt64(db.GenerateNextNo("LedgerMaster", "LedgerCode"));
            ledgerCodeStr = ledgerCode.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating ledger code: {ex.Message}");
            ledgerCode = 1;
            ledgerCodeStr = "1";
        }
    }

    public void CalculateClosingBalance()
    {
        try
        {
            // Parse string values to doubles
            if (double.TryParse(debitOpeningBalanceStr, out double parsedDebit) && 
                double.TryParse(creditOpeningBalanceStr, out double parsedCredit))
            {
                debitOpeningBalance = parsedDebit;
                creditOpeningBalance = parsedCredit;

                // Calculate closing balance (Debit - Credit)
                // Positive = Debit balance, Negative = Credit balance
                closingBalance = debitOpeningBalance - creditOpeningBalance;

                // Update string value for display
                closingBalanceStr = closingBalance.ToString("F2");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calculating closing balance: {ex.Message}");
            closingBalanceStr = "0.00";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(ledgerCodeStr) || !long.TryParse(ledgerCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(ledgerName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(groupName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(debitOpeningBalanceStr) || !double.TryParse(debitOpeningBalanceStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(creditOpeningBalanceStr) || !double.TryParse(creditOpeningBalanceStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(closingBalanceStr) || !double.TryParse(closingBalanceStr, out _))
            isValid = false;

        return isValid;
    }

    public async Task SaveLedger()
    {
        // Parse and calculate values
        CalculateClosingBalance();
        
        if (long.TryParse(ledgerCodeStr, out long parsedLedgerCode))
            ledgerCode = parsedLedgerCode;

        if (double.TryParse(closingBalanceStr, out double parsedClosingBalance))
            closingBalance = parsedClosingBalance;

        // Prepare columns and values
        string columns = "LedgerCode, LedgerName, GroupCode, DebitOpeningBalance, CreditOpeningBalance, ClosingBalance";
        string values = $"{ledgerCode}, '{ledgerName}', {groupCode}, {debitOpeningBalance}, {creditOpeningBalance}, {closingBalance}";

        // Save Ledger record
        bool saved = db.SaveRecord("LedgerMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Ledger record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse and calculate values
            CalculateClosingBalance();
            
            if (long.TryParse(ledgerCodeStr, out long parsedLedgerCode))
                ledgerCode = parsedLedgerCode;

            if (double.TryParse(closingBalanceStr, out double parsedClosingBalance))
                closingBalance = parsedClosingBalance;

            // Build the UPDATE query
            string query = $@"
                UPDATE LedgerMaster SET 
                    LedgerName = '{ledgerName}',
                    GroupCode = {groupCode},
                    DebitOpeningBalance = {debitOpeningBalance},
                    CreditOpeningBalance = {creditOpeningBalance},
                    ClosingBalance = {closingBalance}
                WHERE LedgerCode = {ledgerCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Ledger record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Ledger record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            ledgerCodeStr = row["LedgerCode"]?.ToString() ?? "";
            ledgerName = row["LedgerName"]?.ToString() ?? "";
            debitOpeningBalanceStr = row["DebitOpeningBalance"]?.ToString() ?? "0.00";
            creditOpeningBalanceStr = row["CreditOpeningBalance"]?.ToString() ?? "0.00";
            closingBalanceStr = row["ClosingBalance"]?.ToString() ?? "0.00";

            // Populate group reference field
            if (row["GroupCode"] != DBNull.Value)
            {
                groupCode = Convert.ToInt32(row["GroupCode"]);
                groupName = db.GetScalar("GroupMaster", "GroupName", $"GroupCode = {groupCode}");
            }

            // Calculate closing balance to ensure consistency
            CalculateClosingBalance();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        ledgerCodeStr = "";
        ledgerName = "";
        groupName = "";
        debitOpeningBalanceStr = "0.00";
        creditOpeningBalanceStr = "0.00";
        closingBalanceStr = "0.00";

        // Reset actual values
        ledgerCode = 0;
        groupCode = 0;
        debitOpeningBalance = 0.0;
        creditOpeningBalance = 0.0;
        closingBalance = 0.0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        ledgerName = "Cash in Hand";
        groupName = "Current Assets";
        debitOpeningBalanceStr = "50000.00";
        creditOpeningBalanceStr = "0.00";
        
        // Actual values for database operations
        groupCode = 1; // Assuming Current Assets has GroupCode 1
        debitOpeningBalance = 50000.00;
        creditOpeningBalance = 0.00;
        
        // Calculate closing balance
        CalculateClosingBalance();
    }
}