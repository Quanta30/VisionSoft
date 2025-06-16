using System.Data;
using VisionSoft;

class CustomerMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string customerCodeStr = "";
    public string ledgerName = "";
    public string customerName = "";
    public string customerCity = "";
    public string customerDistrict = "";
    public string customerPhoneNo = "";
    public string customerMobileNo = "";
    public string customerEmail = "";
    public string customerGSTNo = "";
    public string customerPanNo = "";
    public string customerStateCodeStr = "";
    public string customerBillMode = "";
    public string customerCreditDaysStr = "";
    public string personIncharge = "";
    public string engineerName = "";
    public string customertDistrict = "";
    public string customerGrade = "";
    public string customerAddress = "";
    public string routeName = "";
    public string areaName = "";
    public string password = "123"; // Default value as per schema
    public string fcmToken = "";

    // Boolean fields
    public bool dontShowInDebtorsList = false;
    public bool discontinueParty = false;
    public bool loginflag = false;

    // Actual values for database operations
    public int customerCode = 0;
    public int ledgerCode = 0;
    public int customerStateCode = 0;
    public int customerCreditDays = 0;
    public int engineerCode = 0;
    public int routeCode = 0;
    public int areaCode = 0;

    public CustomerMasterModel()
    {
        // Initialize with default password
        password = "123";
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(customerCodeStr) || !int.TryParse(customerCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(ledgerName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customerName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customerCity))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customerDistrict))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customerPhoneNo))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customerMobileNo))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customerEmail))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customerGSTNo))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customerPanNo))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customerStateCodeStr) || !int.TryParse(customerStateCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customerBillMode))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customerCreditDaysStr) || !int.TryParse(customerCreditDaysStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(personIncharge))
            isValid = false;

        if (string.IsNullOrWhiteSpace(engineerName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customertDistrict))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customerGrade))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customerAddress))
            isValid = false;

        if (string.IsNullOrWhiteSpace(routeName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(areaName))
            isValid = false;

        // Password and FCMToken are optional (nullable), so no validation required

        return isValid;
    }

    public async Task SaveCustomer()
    {
        // Parse string values to actual types
        int.TryParse(customerCodeStr, out customerCode);
        int.TryParse(customerStateCodeStr, out customerStateCode);
        int.TryParse(customerCreditDaysStr, out customerCreditDays);

        // Handle nullable fields
        string passwordValue = string.IsNullOrEmpty(password) ? "NULL" : $"'{password}'";
        string fcmTokenValue = string.IsNullOrEmpty(fcmToken) ? "NULL" : $"'{fcmToken}'";

        // Prepare columns and values
        string columns = @"CustomerCode, LedgerCode, CustomerName, CustomerCity, CustomerDistrict, 
                          CustomerPhoneNo, CustomerMobileNo, CustomerEmail, CustomerGSTNo, CustomerPanNo, 
                          CustomerStateCode, CustomerBillMode, CustomerCreditDays, PersonIncharge, EngineerCode, 
                          CustomertDistrict, DontShowInDebtorsList, DiscontinueParty, CustomerGrade, 
                          CustomerAddress, RouteCode, AreaCode, Password, FCMToken, Loginflag";

        string values = $@"{customerCode}, {ledgerCode}, '{customerName}', '{customerCity}', '{customerDistrict}', 
                          '{customerPhoneNo}', '{customerMobileNo}', '{customerEmail}', '{customerGSTNo}', '{customerPanNo}', 
                          {customerStateCode}, '{customerBillMode}', {customerCreditDays}, '{personIncharge}', {engineerCode}, 
                          '{customertDistrict}', {(dontShowInDebtorsList ? 1 : 0)}, {(discontinueParty ? 1 : 0)}, '{customerGrade}', 
                          '{customerAddress}', {routeCode}, {areaCode}, {passwordValue}, {fcmTokenValue}, {(loginflag ? 1 : 0)}";

        // Save Customer record
        bool saved = db.SaveRecord("CustomerMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Customer record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            int.TryParse(customerCodeStr, out customerCode);
            int.TryParse(customerStateCodeStr, out customerStateCode);
            int.TryParse(customerCreditDaysStr, out customerCreditDays);

            // Handle nullable fields
            string passwordValue = string.IsNullOrEmpty(password) ? "NULL" : $"'{password}'";
            string fcmTokenValue = string.IsNullOrEmpty(fcmToken) ? "NULL" : $"'{fcmToken}'";

            // Build the UPDATE query
            string query = $@"
                UPDATE CustomerMaster SET 
                    LedgerCode = {ledgerCode},
                    CustomerName = '{customerName}',
                    CustomerCity = '{customerCity}',
                    CustomerDistrict = '{customerDistrict}',
                    CustomerPhoneNo = '{customerPhoneNo}',
                    CustomerMobileNo = '{customerMobileNo}',
                    CustomerEmail = '{customerEmail}',
                    CustomerGSTNo = '{customerGSTNo}',
                    CustomerPanNo = '{customerPanNo}',
                    CustomerStateCode = {customerStateCode},
                    CustomerBillMode = '{customerBillMode}',
                    CustomerCreditDays = {customerCreditDays},
                    PersonIncharge = '{personIncharge}',
                    EngineerCode = {engineerCode},
                    CustomertDistrict = '{customertDistrict}',
                    DontShowInDebtorsList = {(dontShowInDebtorsList ? 1 : 0)},
                    DiscontinueParty = {(discontinueParty ? 1 : 0)},
                    CustomerGrade = '{customerGrade}',
                    CustomerAddress = '{customerAddress}',
                    RouteCode = {routeCode},
                    AreaCode = {areaCode},
                    Password = {passwordValue},
                    FCMToken = {fcmTokenValue},
                    Loginflag = {(loginflag ? 1 : 0)}
                WHERE CustomerCode = {customerCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Customer record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Customer record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            customerCodeStr = row["CustomerCode"]?.ToString() ?? "";
            customerName = row["CustomerName"]?.ToString() ?? "";
            customerCity = row["CustomerCity"]?.ToString() ?? "";
            customerDistrict = row["CustomerDistrict"]?.ToString() ?? "";
            customerPhoneNo = row["CustomerPhoneNo"]?.ToString() ?? "";
            customerMobileNo = row["CustomerMobileNo"]?.ToString() ?? "";
            customerEmail = row["CustomerEmail"]?.ToString() ?? "";
            customerGSTNo = row["CustomerGSTNo"]?.ToString() ?? "";
            customerPanNo = row["CustomerPanNo"]?.ToString() ?? "";
            customerStateCodeStr = row["CustomerStateCode"]?.ToString() ?? "";
            customerBillMode = row["CustomerBillMode"]?.ToString() ?? "";
            customerCreditDaysStr = row["CustomerCreditDays"]?.ToString() ?? "";
            personIncharge = row["PersonIncharge"]?.ToString() ?? "";
            customertDistrict = row["CustomertDistrict"]?.ToString() ?? "";
            customerGrade = row["CustomerGrade"]?.ToString() ?? "";
            customerAddress = row["CustomerAddress"]?.ToString() ?? "";
            password = row["Password"]?.ToString() ?? "123";
            fcmToken = row["FCMToken"]?.ToString() ?? "";

            // Handle boolean fields
            if (row["DontShowInDebtorsList"] != DBNull.Value)
                dontShowInDebtorsList = Convert.ToBoolean(row["DontShowInDebtorsList"]);
            if (row["DiscontinueParty"] != DBNull.Value)
                discontinueParty = Convert.ToBoolean(row["DiscontinueParty"]);
            if (row["Loginflag"] != DBNull.Value)
                loginflag = Convert.ToBoolean(row["Loginflag"]);

            // Populate reference fields
            if (row["LedgerCode"] != DBNull.Value)
            {
                ledgerCode = Convert.ToInt32(row["LedgerCode"]);
                ledgerName = db.GetScalar("LedgerMaster", "LedgerName", $"LedgerCode = {ledgerCode}");
            }

            if (row["EngineerCode"] != DBNull.Value)
            {
                engineerCode = Convert.ToInt32(row["EngineerCode"]);
                engineerName = db.GetScalar("EngineerMaster", "EngineerName", $"EngineerCode = {engineerCode}");
            }

            if (row["RouteCode"] != DBNull.Value)
            {
                routeCode = Convert.ToInt32(row["RouteCode"]);
                routeName = db.GetScalar("RouteMaster", "RouteName", $"RouteCode = {routeCode}");
            }

            if (row["AreaCode"] != DBNull.Value)
            {
                areaCode = Convert.ToInt32(row["AreaCode"]);
                areaName = db.GetScalar("AreaMaster", "AreaName", $"AreaCode = {areaCode}");
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
        customerCodeStr = "";
        ledgerName = "";
        customerName = "";
        customerCity = "";
        customerDistrict = "";
        customerPhoneNo = "";
        customerMobileNo = "";
        customerEmail = "";
        customerGSTNo = "";
        customerPanNo = "";
        customerStateCodeStr = "";
        customerBillMode = "";
        customerCreditDaysStr = "";
        personIncharge = "";
        engineerName = "";
        customertDistrict = "";
        customerGrade = "";
        customerAddress = "";
        routeName = "";
        areaName = "";
        password = "123"; // Reset to default
        fcmToken = "";

        // Reset boolean fields
        dontShowInDebtorsList = false;
        discontinueParty = false;
        loginflag = false;

        // Reset actual values
        customerCode = 0;
        ledgerCode = 0;
        customerStateCode = 0;
        customerCreditDays = 0;
        engineerCode = 0;
        routeCode = 0;
        areaCode = 0;
    }

    public void InitializeTestData()
    {
        // Test data
        customerCodeStr = "1001";
        ledgerName = "Customer Ledger";
        customerName = "ABC Technologies Pvt Ltd";
        customerCity = "Mumbai";
        customerDistrict = "Mumbai Central";
        customerPhoneNo = "022-12345678";
        customerMobileNo = "9876543210";
        customerEmail = "contact@abctech.com";
        customerGSTNo = "27ABCDE1234F1Z5";
        customerPanNo = "ABCDE1234F";
        customerStateCodeStr = "27";
        customerBillMode = "Monthly";
        customerCreditDaysStr = "30";
        personIncharge = "John Doe";
        engineerName = "Senior Engineer";
        customertDistrict = "Mumbai West";
        customerGrade = "A";
        customerAddress = "123, Business District, Mumbai";
        routeName = "Western Route";
        areaName = "Andheri West";
        password = "123";
        
        // Actual values
        customerCode = 1001;
        ledgerCode = 501;
        customerStateCode = 27;
        customerCreditDays = 30;
        engineerCode = 101;
        routeCode = 201;
        areaCode = 301;
    }
}