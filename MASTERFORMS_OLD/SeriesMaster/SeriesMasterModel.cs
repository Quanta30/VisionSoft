using System.Data;
using VisionSoft;

class SeriesMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string seriesCodeStr = "";
    public string seriesName = "";
    public string voucherType = "";
    public string considerInAccounts = "YES";
    public string billAddressCodeStr = "0";
    
    // Boolean field
    public bool dontApplyGst = false;

    // Actual values for database operations
    public int seriesCode = 0;
    public int billAddressCode = 0;

    public SeriesMasterModel()
    {
        // Initialize with default values
        considerInAccounts = "YES"; // Default as per schema
        billAddressCodeStr = "0";
    }

    public void GenerateSeriesCode()
    {
        try
        {
            seriesCode = Convert.ToInt32(db.GenerateNextNo("SeriesMaster", "SeriesCode"));
            seriesCodeStr = seriesCode.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating series code: {ex.Message}");
            seriesCode = 1;
            seriesCodeStr = "1";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(seriesCodeStr) || !int.TryParse(seriesCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(seriesName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(voucherType))
            isValid = false;

        if (string.IsNullOrWhiteSpace(considerInAccounts))
            isValid = false;

        if (string.IsNullOrWhiteSpace(billAddressCodeStr) || !int.TryParse(billAddressCodeStr, out _))
            isValid = false;

        // Check nvarchar length constraints (50 characters max)
        if (!string.IsNullOrWhiteSpace(seriesName) && seriesName.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(voucherType) && voucherType.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(considerInAccounts) && considerInAccounts.Length > 50)
            isValid = false;

        // DontApplyGst is a boolean field, always valid (checkbox state)

        return isValid;
    }

    public async Task SaveSeries()
    {
        // Parse string values to actual types
        if (int.TryParse(seriesCodeStr, out int parsedSeriesCode))
            seriesCode = parsedSeriesCode;

        if (int.TryParse(billAddressCodeStr, out int parsedBillAddressCode))
            billAddressCode = parsedBillAddressCode;

        // Prepare columns and values
        string columns = "SeriesCode, SeriesName, VoucherType, ConsiderInAccounts, DontApplyGst, BillAddressCode";
        string values = $"{seriesCode}, N'{seriesName}', N'{voucherType}', N'{considerInAccounts}', {(dontApplyGst ? 1 : 0)}, {billAddressCode}";

        // Save Series record
        bool saved = db.SaveRecord("SeriesMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Series record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(seriesCodeStr, out int parsedSeriesCode))
                seriesCode = parsedSeriesCode;

            if (int.TryParse(billAddressCodeStr, out int parsedBillAddressCode))
                billAddressCode = parsedBillAddressCode;

            // Build the UPDATE query
            string query = $@"
                UPDATE SeriesMaster SET 
                    SeriesName = N'{seriesName}',
                    VoucherType = N'{voucherType}',
                    ConsiderInAccounts = N'{considerInAccounts}',
                    DontApplyGst = {(dontApplyGst ? 1 : 0)},
                    BillAddressCode = {billAddressCode}
                WHERE SeriesCode = {seriesCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Series record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Series record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            seriesCodeStr = row["SeriesCode"]?.ToString() ?? "";
            seriesName = row["SeriesName"]?.ToString() ?? "";
            voucherType = row["VoucherType"]?.ToString() ?? "";
            considerInAccounts = row["ConsiderInAccounts"]?.ToString() ?? "YES";
            billAddressCodeStr = row["BillAddressCode"]?.ToString() ?? "0";

            // Handle boolean field
            if (row["DontApplyGst"] != DBNull.Value)
                dontApplyGst = Convert.ToBoolean(row["DontApplyGst"]);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        seriesCodeStr = "";
        seriesName = "";
        voucherType = "";
        considerInAccounts = "YES"; // Reset to default
        billAddressCodeStr = "0";
        dontApplyGst = false;

        // Reset actual values
        seriesCode = 0;
        billAddressCode = 0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        seriesName = "SALES-2024";
        voucherType = "SALES";
        considerInAccounts = "YES";
        billAddressCodeStr = "1";
        dontApplyGst = false;
        
        // Actual values for database operations
        billAddressCode = 1;
        // seriesCode will be auto-generated
    }
}