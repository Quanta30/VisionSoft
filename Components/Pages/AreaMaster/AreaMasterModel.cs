using System.Data;
using VisionSoft;

class AreaMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string areaCodeStr = "";
    public string areaName = "";
    public string routeName = "";

    // Actual values for database operations
    public int areaCode = 0;
    public int routeCode = 0;

    public AreaMasterModel()
    {
        // No auto-generation needed for AreaMaster
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(areaCodeStr) || !int.TryParse(areaCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(areaName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(routeName))
            isValid = false;

        return isValid;
    }

    public async Task SaveArea()
    {
        // Parse string values to actual types
        if (int.TryParse(areaCodeStr, out int parsedAreaCode))
            areaCode = parsedAreaCode;

        // Prepare columns and values
        string columns = "AreaCode, AreaName, RouteCode";
        string values = $"{areaCode}, '{areaName}', {routeCode}";

        // Save Area record
        bool saved = db.SaveRecord("AreaMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Area record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(areaCodeStr, out int parsedAreaCode))
                areaCode = parsedAreaCode;

            // Build the UPDATE query
            string query = $@"
                UPDATE AreaMaster SET 
                    AreaName = '{areaName}', 
                    RouteCode = {routeCode}
                WHERE AreaCode = {areaCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Area record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Area record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            areaCodeStr = row["AreaCode"]?.ToString() ?? "";
            areaName = row["AreaName"]?.ToString() ?? "";
            
            // Populate RouteCode and RouteName
            if (row["RouteCode"] != DBNull.Value)
            {
                routeCode = Convert.ToInt32(row["RouteCode"]);
                routeName = db.GetScalar("RouteMaster", "RouteName", $"RouteCode = {routeCode}");
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
        areaCodeStr = "";
        areaName = "";
        routeName = "";

        // Reset actual values
        areaCode = 0;
        routeCode = 0;
    }

    private void InitializeTestData()
    {
        // Form field variables with test data
        areaCodeStr = "101";
        areaName = "Andheri West";
        routeName = "Western Line Route";
        
        // Actual values for database operations
        areaCode = 101;
        routeCode = 201; // You might need to adjust this based on your RouteMaster table
    }
}