using System.Data;
using VisionSoft;

class ServiceMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string serviceCodeStr = "";
    public string serviceName = "";
    public string rateStr = "0.00";

    // Actual values for database operations
    public int serviceCode = 0;
    public float rate = 0.0f;

    public ServiceMasterModel()
    {
        // Initialize with default values
        rateStr = "0.00";
    }

    public void GenerateServiceCode()
    {
        try
        {
            serviceCode = Convert.ToInt32(db.GenerateNextNo("ServiceMaster", "ServiceCode"));
            serviceCodeStr = serviceCode.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating service code: {ex.Message}");
            serviceCode = 1;
            serviceCodeStr = "1";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(serviceCodeStr) || !int.TryParse(serviceCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(serviceName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(rateStr) || !float.TryParse(rateStr, out _))
            isValid = false;

        // Check nvarchar length constraint (100 characters max)
        if (!string.IsNullOrWhiteSpace(serviceName) && serviceName.Length > 100)
            isValid = false;

        // Check for negative rate
        if (float.TryParse(rateStr, out float parsedRate) && parsedRate < 0)
            isValid = false;

        return isValid;
    }

    public async Task SaveService()
    {
        // Parse string values to actual types
        if (int.TryParse(serviceCodeStr, out int parsedServiceCode))
            serviceCode = parsedServiceCode;

        if (float.TryParse(rateStr, out float parsedRate))
            rate = parsedRate;

        // Prepare columns and values
        string columns = "ServiceCode, ServiceName, Rate";
        string values = $"{serviceCode}, N'{serviceName}', {rate}";

        // Save Service record
        bool saved = db.SaveRecord("ServiceMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Service record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(serviceCodeStr, out int parsedServiceCode))
                serviceCode = parsedServiceCode;

            if (float.TryParse(rateStr, out float parsedRate))
                rate = parsedRate;

            // Build the UPDATE query
            string query = $@"
                UPDATE ServiceMaster SET 
                    ServiceName = N'{serviceName}',
                    Rate = {rate}
                WHERE ServiceCode = {serviceCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Service record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Service record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            serviceCodeStr = row["ServiceCode"]?.ToString() ?? "";
            serviceName = row["ServiceName"]?.ToString() ?? "";
            rateStr = row["Rate"]?.ToString() ?? "0.00";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        serviceCodeStr = "";
        serviceName = "";
        rateStr = "0.00";

        // Reset actual values
        serviceCode = 0;
        rate = 0.0f;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        serviceName = "Software Installation and Configuration";
        rateStr = "2500.00";
        
        // Actual values for database operations
        rate = 2500.0f;
        // serviceCode will be auto-generated
    }
}