using System.Data;
using VisionSoft;

class TokenMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string tokenIDStr = "";
    public string tokenDate = "";
    public string customerName = "";
    public string engineerName = "";
    public string productName = "";
    public string productNameManual = "";
    public string remark = "";
    public string scheduleDate = "";
    public string scheduleTime = "";
    public string salesBillNo = "";
    public string salesBillDate = "";
    public string cancelledStr = "0";
    public string customerWarrantyEntryNoStr = "0";
    public string priority = "";
    public string callChargesStr = "0.00";
    public string raisedBy = "";
    public string suggestion = "";

    // Boolean fields
    public bool inHouseSold = false;
    public bool online = false;
    public bool underWarranty = false;
    public bool callClosed = false;
    public bool underAMC = false;

    // Actual values for database operations
    public int customerCode = 0;
    public int engineerCode = 0;
    public int productCode = 0;
    public int cancelled = 0;
    public long customerWarrantyEntryNo = 0;
    public float callCharges = 0.0f;

    public TokenMasterModel()
    {
        // Initialize with default values
        cancelledStr = "0";
        customerWarrantyEntryNoStr = "0";
        callChargesStr = "0.00";
    }

    public void GenerateTokenID()
    {
        try
        {
            // For varchar TokenID, we format it appropriately
            tokenIDStr = db.GenerateNextNo("TokenMaster", "TokenID");
            Console.WriteLine(tokenIDStr);
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("Here");
            Console.WriteLine($"Error generating token ID: {ex.Message}");
            tokenIDStr = "1";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(tokenIDStr))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customerName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(engineerName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(productName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(productNameManual))
            isValid = false;

        if (string.IsNullOrWhiteSpace(remark))
            isValid = false;

        if (string.IsNullOrWhiteSpace(salesBillNo))
            isValid = false;

        if (string.IsNullOrWhiteSpace(priority))
            isValid = false;

        if (string.IsNullOrWhiteSpace(callChargesStr) || !float.TryParse(callChargesStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(cancelledStr) || !int.TryParse(cancelledStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(customerWarrantyEntryNoStr) || !long.TryParse(customerWarrantyEntryNoStr, out _))
            isValid = false;

        // Check varchar length constraints
        if (!string.IsNullOrWhiteSpace(tokenIDStr) && tokenIDStr.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(productNameManual) && productNameManual.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(remark) && remark.Length > 500)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(salesBillNo) && salesBillNo.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(priority) && priority.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(raisedBy) && raisedBy.Length > 50)
            isValid = false;

        // Optional fields (nullable) don't need validation for empty values
        // TokenDate, ScheduleDate, ScheduleTime, SalesBillDate, RaisedBy, Suggestion

        return isValid;
    }

    public async Task SaveToken()
    {
        // Parse string values to actual types
        if (int.TryParse(cancelledStr, out int parsedCancelled))
            cancelled = parsedCancelled;

        if (long.TryParse(customerWarrantyEntryNoStr, out long parsedWarrantyEntryNo))
            customerWarrantyEntryNo = parsedWarrantyEntryNo;

        if (float.TryParse(callChargesStr, out float parsedCallCharges))
            callCharges = parsedCallCharges;

        // Handle nullable datetime fields
        string tokenDateValue = string.IsNullOrEmpty(tokenDate) ? "NULL" : $"'{tokenDate}'";
        string scheduleDateValue = string.IsNullOrEmpty(scheduleDate) ? "NULL" : $"'{scheduleDate}'";
        string scheduleTimeValue = string.IsNullOrEmpty(scheduleTime) ? "NULL" : $"'{scheduleTime}'";
        string salesBillDateValue = string.IsNullOrEmpty(salesBillDate) ? "NULL" : $"'{salesBillDate}'";
        string raisedByValue = string.IsNullOrEmpty(raisedBy) ? "NULL" : $"'{raisedBy}'";
        string suggestionValue = string.IsNullOrEmpty(suggestion) ? "NULL" : $"N'{suggestion}'";

        // Prepare columns and values
        string columns = @"TokenID, TokenDate, CustomerCode, EngineerCode, InHouseSold, ProductName, Remark, 
                          ScheduleDate, ScheduleTime, Online, UnderWarranty, SalesBillNo, SalesBillDate, 
                          CallClosed, Cancelled, CustomerWarrantyEntryNo, Priority, CallCharges, UnderAMC, 
                          ProductCode, RaisedBy, Suggestion";

        string values = $@"'{tokenIDStr}', {tokenDateValue}, {customerCode}, {engineerCode}, {(inHouseSold ? 1 : 0)}, 
                          '{productNameManual}', '{remark}', {scheduleDateValue}, {scheduleTimeValue}, 
                          {(online ? 1 : 0)}, {(underWarranty ? 1 : 0)}, '{salesBillNo}', {salesBillDateValue}, 
                          {(callClosed ? 1 : 0)}, {cancelled}, {customerWarrantyEntryNo}, '{priority}', 
                          {callCharges}, {(underAMC ? 1 : 0)}, {productCode}, {raisedByValue}, {suggestionValue}";

        // Save Token record
        bool saved = db.SaveRecord("TokenMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Token record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(cancelledStr, out int parsedCancelled))
                cancelled = parsedCancelled;

            if (long.TryParse(customerWarrantyEntryNoStr, out long parsedWarrantyEntryNo))
                customerWarrantyEntryNo = parsedWarrantyEntryNo;

            if (float.TryParse(callChargesStr, out float parsedCallCharges))
                callCharges = parsedCallCharges;

            // Handle nullable datetime fields
            string tokenDateValue = string.IsNullOrEmpty(tokenDate) ? "NULL" : $"'{tokenDate}'";
            string scheduleDateValue = string.IsNullOrEmpty(scheduleDate) ? "NULL" : $"'{scheduleDate}'";
            string scheduleTimeValue = string.IsNullOrEmpty(scheduleTime) ? "NULL" : $"'{scheduleTime}'";
            string salesBillDateValue = string.IsNullOrEmpty(salesBillDate) ? "NULL" : $"'{salesBillDate}'";
            string raisedByValue = string.IsNullOrEmpty(raisedBy) ? "NULL" : $"'{raisedBy}'";
            string suggestionValue = string.IsNullOrEmpty(suggestion) ? "NULL" : $"N'{suggestion}'";

            // Build the UPDATE query
            string query = $@"
                UPDATE TokenMaster SET 
                    TokenDate = {tokenDateValue},
                    CustomerCode = {customerCode},
                    EngineerCode = {engineerCode},
                    InHouseSold = {(inHouseSold ? 1 : 0)},
                    ProductName = '{productNameManual}',
                    Remark = '{remark}',
                    ScheduleDate = {scheduleDateValue},
                    ScheduleTime = {scheduleTimeValue},
                    Online = {(online ? 1 : 0)},
                    UnderWarranty = {(underWarranty ? 1 : 0)},
                    SalesBillNo = '{salesBillNo}',
                    SalesBillDate = {salesBillDateValue},
                    CallClosed = {(callClosed ? 1 : 0)},
                    Cancelled = {cancelled},
                    CustomerWarrantyEntryNo = {customerWarrantyEntryNo},
                    Priority = '{priority}',
                    CallCharges = {callCharges},
                    UnderAMC = {(underAMC ? 1 : 0)},
                    ProductCode = {productCode},
                    RaisedBy = {raisedByValue},
                    Suggestion = {suggestionValue}
                WHERE TokenID = '{tokenIDStr}'";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Token record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Token record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            tokenIDStr = row["TokenID"]?.ToString() ?? "";
            tokenDate = row["TokenDate"]?.ToString() ?? "";
            productNameManual = row["ProductName"]?.ToString() ?? "";
            remark = row["Remark"]?.ToString() ?? "";
            scheduleDate = row["ScheduleDate"]?.ToString() ?? "";
            scheduleTime = row["ScheduleTime"]?.ToString() ?? "";
            salesBillNo = row["SalesBillNo"]?.ToString() ?? "";
            salesBillDate = row["SalesBillDate"]?.ToString() ?? "";
            cancelledStr = row["Cancelled"]?.ToString() ?? "0";
            customerWarrantyEntryNoStr = row["CustomerWarrantyEntryNo"]?.ToString() ?? "0";
            priority = row["Priority"]?.ToString() ?? "";
            callChargesStr = row["CallCharges"]?.ToString() ?? "0.00";
            raisedBy = row["RaisedBy"]?.ToString() ?? "";
            suggestion = row["Suggestion"]?.ToString() ?? "";

            // Handle boolean fields
            if (row["InHouseSold"] != DBNull.Value)
                inHouseSold = Convert.ToBoolean(row["InHouseSold"]);

            if (row["Online"] != DBNull.Value)
                online = Convert.ToBoolean(row["Online"]);

            if (row["UnderWarranty"] != DBNull.Value)
                underWarranty = Convert.ToBoolean(row["UnderWarranty"]);

            if (row["CallClosed"] != DBNull.Value)
                callClosed = Convert.ToBoolean(row["CallClosed"]);

            if (row["UnderAMC"] != DBNull.Value)
                underAMC = Convert.ToBoolean(row["UnderAMC"]);

            // Populate reference fields
            if (row["CustomerCode"] != DBNull.Value)
            {
                customerCode = Convert.ToInt32(row["CustomerCode"]);
                customerName = db.GetScalar("CustomerMaster", "CustomerName", $"CustomerCode = {customerCode}");
            }

            if (row["EngineerCode"] != DBNull.Value)
            {
                engineerCode = Convert.ToInt32(row["EngineerCode"]);
                engineerName = db.GetScalar("EngineerMaster", "EngineerName", $"EngineerCode = {engineerCode}");
            }

            if (row["ProductCode"] != DBNull.Value)
            {
                productCode = Convert.ToInt32(row["ProductCode"]);
                productName = db.GetScalar("ProductMaster", "ProductName", $"ProductCode = {productCode}");
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
        tokenIDStr = "";
        tokenDate = "";
        customerName = "";
        engineerName = "";
        productName = "";
        productNameManual = "";
        remark = "";
        scheduleDate = "";
        scheduleTime = "";
        salesBillNo = "";
        salesBillDate = "";
        cancelledStr = "0";
        customerWarrantyEntryNoStr = "0";
        priority = "";
        callChargesStr = "0.00";
        raisedBy = "";
        suggestion = "";

        // Reset boolean fields
        inHouseSold = false;
        online = false;
        underWarranty = false;
        callClosed = false;
        underAMC = false;

        // Reset actual values
        customerCode = 0;
        engineerCode = 0;
        productCode = 0;
        cancelled = 0;
        customerWarrantyEntryNo = 0;
        callCharges = 0.0f;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        customerName = "ABC Technologies Pvt Ltd";
        engineerName = "Rajesh Kumar";
        productName = "ERP Software";
        productNameManual = "ERP Software Enterprise Edition";
        remark = "Customer reporting slow performance in inventory module. Need immediate attention.";
        priority = "High";
        salesBillNo = "SB2024001234";
        callChargesStr = "1500.00";
        cancelledStr = "0";
        customerWarrantyEntryNoStr = "123456";
        raisedBy = "Customer Support";
        suggestion = "Recommended to upgrade database server RAM and optimize queries for better performance. Schedule onsite visit for hardware assessment.";
        
        // Boolean values
        inHouseSold = true;
        online = false;
        underWarranty = true;
        callClosed = false;
        underAMC = true;
        
        // Actual values for database operations
        cancelled = 0;
        customerWarrantyEntryNo = 123456;
        callCharges = 1500.0f;
    }
}