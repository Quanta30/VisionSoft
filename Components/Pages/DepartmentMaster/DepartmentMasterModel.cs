using System.Data;
using VisionSoft;

class DepartmentMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string departmentCodeStr = "";
    public string departmentName = "";

    // Actual values for database operations
    public int departmentCode = 0;

    public DepartmentMasterModel()
    {
        // Constructor - no special initialization needed
    }

    public void GenerateDepartmentCode()
    {
        try
        {
            departmentCode = Convert.ToInt32(db.GenerateNextNo("DepartmentMaster", "DepartmentCode"));
            departmentCodeStr = departmentCode.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating department code: {ex.Message}");
            departmentCode = 1;
            departmentCodeStr = "1";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(departmentCodeStr) || !int.TryParse(departmentCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(departmentName))
            isValid = false;

        return isValid;
    }

    public async Task SaveDepartment()
    {
        // Parse string values to actual types
        if (int.TryParse(departmentCodeStr, out int parsedDepartmentCode))
            departmentCode = parsedDepartmentCode;

        // Prepare columns and values
        string columns = "DepartmentCode, DepartmentName";
        string values = $"{departmentCode}, '{departmentName}'";

        // Save Department record
        bool saved = db.SaveRecord("DepartmentMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Department record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(departmentCodeStr, out int parsedDepartmentCode))
                departmentCode = parsedDepartmentCode;

            // Build the UPDATE query
            string query = $@"
                UPDATE DepartmentMaster SET 
                    DepartmentName = '{departmentName}'
                WHERE DepartmentCode = {departmentCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Department record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Department record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            departmentCodeStr = row["DepartmentCode"]?.ToString() ?? "";
            departmentName = row["DepartmentName"]?.ToString() ?? "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        departmentCodeStr = "";
        departmentName = "";

        // Reset actual values
        departmentCode = 0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        departmentName = "Information Technology";
        
        // Actual values for database operations
        // departmentCode will be auto-generated
    }
}