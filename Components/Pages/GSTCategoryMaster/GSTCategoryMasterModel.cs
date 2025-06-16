using System.Data;
using VisionSoft;

class GSTCategoryMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string gstCategoryCodeStr = "";
    public string gstCategoryName = "";
    public string gstPercentageStr = "0";

    // Actual values for database operations
    public int gstCategoryCode = 0;
    public double gstPercentage = 0.0;

    public GSTCategoryMasterModel()
    {
        // Initialize with default percentage
        gstPercentageStr = "0";
    }

    public void GenerateGSTCategoryCode()
    {
        try
        {
            gstCategoryCode = Convert.ToInt32(db.GenerateNextNo("GstCategoryMaster", "GSTCategoryCode"));
            gstCategoryCodeStr = gstCategoryCode.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating GST category code: {ex.Message}");
            gstCategoryCode = 1;
            gstCategoryCodeStr = "1";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(gstCategoryCodeStr) || !int.TryParse(gstCategoryCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(gstCategoryName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(gstPercentageStr) || !double.TryParse(gstPercentageStr, out _))
            isValid = false;

        return isValid;
    }

    public async Task SaveCategory()
    {
        // Parse string values to actual types
        if (int.TryParse(gstCategoryCodeStr, out int parsedGSTCategoryCode))
            gstCategoryCode = parsedGSTCategoryCode;

        if (double.TryParse(gstPercentageStr, out double parsedGSTPercentage))
            gstPercentage = parsedGSTPercentage;

        // Prepare columns and values
        string columns = "GSTCategoryCode, GSTCategoryName, GSTPercentage";
        string values = $"{gstCategoryCode}, '{gstCategoryName}', {gstPercentage}";

        // Save GST Category record
        bool saved = db.SaveRecord("GstCategoryMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save GST Category record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(gstCategoryCodeStr, out int parsedGSTCategoryCode))
                gstCategoryCode = parsedGSTCategoryCode;

            if (double.TryParse(gstPercentageStr, out double parsedGSTPercentage))
                gstPercentage = parsedGSTPercentage;

            // Build the UPDATE query
            string query = $@"
                UPDATE GstCategoryMaster SET 
                    GSTCategoryName = '{gstCategoryName}',
                    GSTPercentage = {gstPercentage}
                WHERE GSTCategoryCode = {gstCategoryCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update GST Category record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating GST Category record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            gstCategoryCodeStr = row["GSTCategoryCode"]?.ToString() ?? "";
            gstCategoryName = row["GSTCategoryName"]?.ToString() ?? "";
            gstPercentageStr = row["GSTPercentage"]?.ToString() ?? "0";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        gstCategoryCodeStr = "";
        gstCategoryName = "";
        gstPercentageStr = "0";

        // Reset actual values
        gstCategoryCode = 0;
        gstPercentage = 0.0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        gstCategoryName = "SGST + CGST";
        gstPercentageStr = "18.00";
        
        // Actual values for database operations
        gstPercentage = 18.00;
        // gstCategoryCode will be auto-generated
    }
}