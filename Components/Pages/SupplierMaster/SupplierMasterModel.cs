using System.Data;
using VisionSoft;

class SupplierMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string supplierCodeStr = "";
    public string ledgerName = "";
    public string supplierName = "";
    public string supplierCity = "";
    public string supplierPhoneNo = "";
    public string supplierMobileNo = "";
    public string supplierEmail = "";
    public string supplierGSTNo = "";
    public string supplierPanNo = "";
    public string billMode = "";
    public string supplierSeriesName = "A";
    public string supplierStateCodeStr = "0";
    public string creditDaysStr = "0";

    // Actual values for database operations
    public int supplierCode = 0;
    public int ledgerCode = 0;
    public int supplierStateCode = 0;
    public int creditDays = 0;

    public SupplierMasterModel()
    {
        // Initialize with default values
        supplierSeriesName = "A"; // Default as per schema
        supplierStateCodeStr = "0";
        creditDaysStr = "0";
    }

    public void GenerateSupplierCode()
    {
        try
        {
            supplierCode = Convert.ToInt32(db.GenerateNextNo("SupplierMaster", "SupplierCode"));
            supplierCodeStr = supplierCode.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating supplier code: {ex.Message}");
            supplierCode = 1;
            supplierCodeStr = "1";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(supplierCodeStr) || !int.TryParse(supplierCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(supplierName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(supplierCity))
            isValid = false;

        if (string.IsNullOrWhiteSpace(ledgerName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(supplierPhoneNo))
            isValid = false;

        if (string.IsNullOrWhiteSpace(supplierMobileNo))
            isValid = false;

        if (string.IsNullOrWhiteSpace(supplierEmail))
            isValid = false;

        if (string.IsNullOrWhiteSpace(supplierGSTNo))
            isValid = false;

        if (string.IsNullOrWhiteSpace(supplierPanNo))
            isValid = false;

        if (string.IsNullOrWhiteSpace(billMode))
            isValid = false;

        if (string.IsNullOrWhiteSpace(supplierSeriesName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(supplierStateCodeStr) || !int.TryParse(supplierStateCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(creditDaysStr) || !int.TryParse(creditDaysStr, out _))
            isValid = false;

        // Check varchar length constraints (all varchar fields have 50 char limit)
        if (!string.IsNullOrWhiteSpace(supplierName) && supplierName.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(supplierCity) && supplierCity.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(supplierPhoneNo) && supplierPhoneNo.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(supplierMobileNo) && supplierMobileNo.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(supplierEmail) && supplierEmail.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(supplierGSTNo) && supplierGSTNo.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(supplierPanNo) && supplierPanNo.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(billMode) && billMode.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(supplierSeriesName) && supplierSeriesName.Length > 50)
            isValid = false;

        // Basic email validation
        if (!string.IsNullOrWhiteSpace(supplierEmail) && !supplierEmail.Contains("@"))
            isValid = false;

        // GST number format validation (basic check for 15 characters)
        if (!string.IsNullOrWhiteSpace(supplierGSTNo) && supplierGSTNo.Length != 15)
            isValid = false;

        // PAN number format validation (basic check for 10 characters)
        if (!string.IsNullOrWhiteSpace(supplierPanNo) && supplierPanNo.Length != 10)
            isValid = false;

        return isValid;
    }

    public async Task SaveSupplier()
    {
        // Parse string values to actual types
        if (int.TryParse(supplierCodeStr, out int parsedSupplierCode))
            supplierCode = parsedSupplierCode;

        if (int.TryParse(supplierStateCodeStr, out int parsedStateCode))
            supplierStateCode = parsedStateCode;

        if (int.TryParse(creditDaysStr, out int parsedCreditDays))
            creditDays = parsedCreditDays;

        // Prepare columns and values
        string columns = @"SupplierCode, LedgerCode, SupplierName, SupplierCity, SupplierPhoneNo, SupplierMobileNo, 
                          SupplierEmail, SupplierGSTNo, SupplierPanNo, BillMode, SupplierSeriesName, 
                          SupplierStateCode, CreditDays";

        string values = $@"{supplierCode}, {ledgerCode}, '{supplierName}', '{supplierCity}', '{supplierPhoneNo}', 
                          '{supplierMobileNo}', '{supplierEmail}', '{supplierGSTNo}', '{supplierPanNo}', 
                          '{billMode}', '{supplierSeriesName}', {supplierStateCode}, {creditDays}";

        // Save Supplier record
        bool saved = db.SaveRecord("SupplierMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Supplier record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(supplierCodeStr, out int parsedSupplierCode))
                supplierCode = parsedSupplierCode;

            if (int.TryParse(supplierStateCodeStr, out int parsedStateCode))
                supplierStateCode = parsedStateCode;

            if (int.TryParse(creditDaysStr, out int parsedCreditDays))
                creditDays = parsedCreditDays;

            // Build the UPDATE query
            string query = $@"
                UPDATE SupplierMaster SET 
                    LedgerCode = {ledgerCode},
                    SupplierName = '{supplierName}',
                    SupplierCity = '{supplierCity}',
                    SupplierPhoneNo = '{supplierPhoneNo}',
                    SupplierMobileNo = '{supplierMobileNo}',
                    SupplierEmail = '{supplierEmail}',
                    SupplierGSTNo = '{supplierGSTNo}',
                    SupplierPanNo = '{supplierPanNo}',
                    BillMode = '{billMode}',
                    SupplierSeriesName = '{supplierSeriesName}',
                    SupplierStateCode = {supplierStateCode},
                    CreditDays = {creditDays}
                WHERE SupplierCode = {supplierCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Supplier record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Supplier record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            supplierCodeStr = row["SupplierCode"]?.ToString() ?? "";
            supplierName = row["SupplierName"]?.ToString() ?? "";
            supplierCity = row["SupplierCity"]?.ToString() ?? "";
            supplierPhoneNo = row["SupplierPhoneNo"]?.ToString() ?? "";
            supplierMobileNo = row["SupplierMobileNo"]?.ToString() ?? "";
            supplierEmail = row["SupplierEmail"]?.ToString() ?? "";
            supplierGSTNo = row["SupplierGSTNo"]?.ToString() ?? "";
            supplierPanNo = row["SupplierPanNo"]?.ToString() ?? "";
            billMode = row["BillMode"]?.ToString() ?? "";
            supplierSeriesName = row["SupplierSeriesName"]?.ToString() ?? "A";
            supplierStateCodeStr = row["SupplierStateCode"]?.ToString() ?? "0";
            creditDaysStr = row["CreditDays"]?.ToString() ?? "0";

            // Populate ledger reference field
            if (row["LedgerCode"] != DBNull.Value)
            {
                ledgerCode = Convert.ToInt32(row["LedgerCode"]);
                ledgerName = db.GetScalar("LedgerMaster", "LedgerName", $"LedgerCode = {ledgerCode}");
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
        supplierCodeStr = "";
        ledgerName = "";
        supplierName = "";
        supplierCity = "";
        supplierPhoneNo = "";
        supplierMobileNo = "";
        supplierEmail = "";
        supplierGSTNo = "";
        supplierPanNo = "";
        billMode = "";
        supplierSeriesName = "A"; // Reset to default
        supplierStateCodeStr = "0";
        creditDaysStr = "0";

        // Reset actual values
        supplierCode = 0;
        ledgerCode = 0;
        supplierStateCode = 0;
        creditDays = 0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        ledgerName = "Accounts Payable";
        supplierName = "TechHub Solutions Pvt Ltd";
        supplierCity = "Mumbai";
        supplierPhoneNo = "022-12345678";
        supplierMobileNo = "+91-9876543210";
        supplierEmail = "info@techhub.com";
        supplierGSTNo = "27ABCDE1234F1Z5";
        supplierPanNo = "ABCDE1234F";
        billMode = "Credit";
        supplierSeriesName = "A";
        supplierStateCodeStr = "27";
        creditDaysStr = "30";
        
        // Actual values for database operations
        supplierStateCode = 27;
        creditDays = 30;
        // supplierCode and ledgerCode will be handled separately
    }
}