using System.Data;
using VisionSoft;

class RouteMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string routeCodeStr = "";
    public string routeName = "";

    // Actual values for database operations
    public int routeCode = 0;

    public RouteMasterModel()
    {
        // Constructor - no special initialization needed
    }

    public void GenerateRouteCode()
    {
        try
        {
            routeCode = Convert.ToInt32(db.GenerateNextNo("RouteMaster", "RouteCode"));
            routeCodeStr = routeCode.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating route code: {ex.Message}");
            routeCode = 0;
            routeCodeStr = "0";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(routeCodeStr) || !int.TryParse(routeCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(routeName))
            isValid = false;

        // Check route name length (max 500 characters)
        if (!string.IsNullOrWhiteSpace(routeName) && routeName.Length > 500)
            isValid = false;

        return isValid;
    }

    public async Task SaveRoute()
    {
        // Parse string values to actual types
        if (int.TryParse(routeCodeStr, out int parsedRouteCode))
            routeCode = parsedRouteCode;

        // Prepare columns and values
        string columns = "RouteCode, RouteName";
        string values = $"{routeCode}, '{routeName}'";

        // Save Route record
        bool saved = db.SaveRecord("RouteMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Route record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(routeCodeStr, out int parsedRouteCode))
                routeCode = parsedRouteCode;

            // Build the UPDATE query
            string query = $@"
                UPDATE RouteMaster SET 
                    RouteName = '{routeName}'
                WHERE RouteCode = {routeCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Route record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Route record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            routeCodeStr = row["RouteCode"]?.ToString() ?? "";
            routeName = row["RouteName"]?.ToString() ?? "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        routeCodeStr = "";
        routeName = "";

        // Reset actual values
        routeCode = 0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        routeName = "Mumbai - Pune Express Route";
        
        // Actual values for database operations
        // routeCode will be auto-generated
    }
}