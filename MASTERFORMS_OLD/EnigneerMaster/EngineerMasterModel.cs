using System.Data;
using VisionSoft;

class EngineerMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string engineerCodeStr = "";
    public string engineerName = "";
    public string address1 = "";
    public string city = "";
    public string mobileNo = "";
    public string emailID = "";
    public string password = "123"; // Default value as per schema
    public string fcmToken = "";

    // Boolean fields
    public bool sendSMS = false;
    public bool isActive = false;
    public bool loginflag = false;

    // Actual values for database operations
    public int engineerCode = 0;

    public EngineerMasterModel()
    {
        // Initialize with default password
        password = "123";
    }

    public void GenerateEngineerCode()
    {
        try
        {
            engineerCode = Convert.ToInt32(db.GenerateNextNo("EngineerMaster", "EngineerCode"));
            engineerCodeStr = engineerCode.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating engineer code: {ex.Message}");
            engineerCode = 1;
            engineerCodeStr = "1";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(engineerCodeStr) || !int.TryParse(engineerCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(engineerName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(address1))
            isValid = false;

        if (string.IsNullOrWhiteSpace(city))
            isValid = false;

        if (string.IsNullOrWhiteSpace(mobileNo))
            isValid = false;

        if (string.IsNullOrWhiteSpace(emailID))
            isValid = false;

        if (string.IsNullOrWhiteSpace(password))
            isValid = false;

        // FCMToken is optional (nullable), so no validation required
        // Boolean fields are always valid (checkbox state)

        return isValid;
    }

    public async Task SaveEngineer()
    {
        // Parse string values to actual types
        if (int.TryParse(engineerCodeStr, out int parsedEngineerCode))
            engineerCode = parsedEngineerCode;

        // Handle nullable FCMToken
        string fcmTokenValue = string.IsNullOrEmpty(fcmToken) ? "NULL" : $"'{fcmToken}'";

        // Prepare columns and values
        string columns = @"EngineerCode, EngineerName, Address1, City, MobileNo, EmailID, 
                          SendSMS, Password, FCMToken, IsActive, Loginflag";

        string values = $@"{engineerCode}, '{engineerName}', '{address1}', '{city}', '{mobileNo}', '{emailID}', 
                          {(sendSMS ? 1 : 0)}, '{password}', {fcmTokenValue}, {(isActive ? 1 : 0)}, {(loginflag ? 1 : 0)}";

        // Save Engineer record
        bool saved = db.SaveRecord("EngineerMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Engineer record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(engineerCodeStr, out int parsedEngineerCode))
                engineerCode = parsedEngineerCode;

            // Handle nullable FCMToken
            string fcmTokenValue = string.IsNullOrEmpty(fcmToken) ? "NULL" : $"'{fcmToken}'";

            // Build the UPDATE query
            string query = $@"
                UPDATE EngineerMaster SET 
                    EngineerName = '{engineerName}',
                    Address1 = '{address1}',
                    City = '{city}',
                    MobileNo = '{mobileNo}',
                    EmailID = '{emailID}',
                    SendSMS = {(sendSMS ? 1 : 0)},
                    Password = '{password}',
                    FCMToken = {fcmTokenValue},
                    IsActive = {(isActive ? 1 : 0)},
                    Loginflag = {(loginflag ? 1 : 0)}
                WHERE EngineerCode = {engineerCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Engineer record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Engineer record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            engineerCodeStr = row["EngineerCode"]?.ToString() ?? "";
            engineerName = row["EngineerName"]?.ToString() ?? "";
            address1 = row["Address1"]?.ToString() ?? "";
            city = row["City"]?.ToString() ?? "";
            mobileNo = row["MobileNo"]?.ToString() ?? "";
            emailID = row["EmailID"]?.ToString() ?? "";
            password = row["Password"]?.ToString() ?? "123";
            fcmToken = row["FCMToken"]?.ToString() ?? "";

            // Handle boolean fields
            if (row["SendSMS"] != DBNull.Value)
                sendSMS = Convert.ToBoolean(row["SendSMS"]);
            if (row["IsActive"] != DBNull.Value)
                isActive = Convert.ToBoolean(row["IsActive"]);
            if (row["Loginflag"] != DBNull.Value)
                loginflag = Convert.ToBoolean(row["Loginflag"]);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        engineerCodeStr = "";
        engineerName = "";
        address1 = "";
        city = "";
        mobileNo = "";
        emailID = "";
        password = "123"; // Reset to default
        fcmToken = "";

        // Reset boolean fields
        sendSMS = false;
        isActive = false;
        loginflag = false;

        // Reset actual values
        engineerCode = 0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        engineerName = "John Smith";
        address1 = "123 Engineering Street";
        city = "Mumbai";
        mobileNo = "9876543210";
        emailID = "john.smith@visionsoft.com";
        password = "123";
        fcmToken = "sample_fcm_token_123";
        
        // Boolean test values
        sendSMS = true;
        isActive = true;
        loginflag = false;
        
        // Actual values for database operations
        // engineerCode will be auto-generated
    }
}