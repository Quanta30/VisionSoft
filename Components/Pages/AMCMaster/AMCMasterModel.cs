using System.Data;
using VisionSoft;

class AMCMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string entryNo = "";
    public string entryDate = DateTime.Now.ToString("yyyy-MM-dd");
    public string partyCodeStr = "";
    public string city = "";
    public string productName = "";
    public string installationDate = "";
    public string amcFromDate = "";
    public string amcToDate = "";
    public string amcAmountStr = "";
    public string onlineCallStr = "";
    public string onsiteCallStr = "";
    public string noOfNodeStr = "";

    // Actual values for database operations
    public int partyCode = 0;
    public int productCode = 0;
    public decimal amcAmount = 0;
    public int onlineCall = 0;
    public int onsiteCall = 0;
    public int noOfNode = 0;


    public AMCMasterModel()
    {
        GenerateEntryNumber();
    }


    public bool ValidateForm()
    {

        bool isValid = true;

        if (string.IsNullOrWhiteSpace(partyCodeStr) || !int.TryParse(partyCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(city))
            isValid = false;

        if (string.IsNullOrWhiteSpace(productName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(amcAmountStr) || !decimal.TryParse(amcAmountStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(onlineCallStr) || !int.TryParse(onlineCallStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(onsiteCallStr) || !int.TryParse(onsiteCallStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(noOfNodeStr) || !int.TryParse(noOfNodeStr, out _))
            isValid = false;

        return isValid;
    }




    public async Task SaveAMC()
    {
        // Prepare date values for database
        string entryDateValue = entryDate;
        string installationDateValue = installationDate;
        string amcFromDateValue = amcFromDate;
        string amcToDateValue = amcToDate;
        

        // Prepare columns and values
        string columns = "EntryNo, EntryDate, PartyCode, City, ProductCode, InstallationDate, AMCFromDate, AMCToDate, AMCAmount, OnlineCall, OnsiteCall, NoOfNode";

        string values = $"{entryNo}, {entryDateValue}, {partyCodeStr}, '{city}', {productCode}, {installationDateValue}, " +
                       $"{amcFromDateValue}, {amcToDateValue}, {amcAmountStr}, {onlineCallStr}, {onsiteCallStr}, {noOfNodeStr}";

        // Save AMC record
        bool saved = db.SaveRecord("AMCMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save AMC record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Prepare date values for database
            string entryDateValue = string.IsNullOrEmpty(entryDate) ? "NULL" : $"'{entryDate}'";
            string installationDateValue = string.IsNullOrEmpty(installationDate) ? "NULL" : $"'{installationDate}'";
            string amcFromDateValue = string.IsNullOrEmpty(amcFromDate) ? "NULL" : $"'{amcFromDate}'";
            string amcToDateValue = string.IsNullOrEmpty(amcToDate) ? "NULL" : $"'{amcToDate}'";

            // Build the UPDATE query
            string query = $@"
                UPDATE AMCMaster SET 
                    EntryDate = {entryDateValue}, 
                    PartyCode = {partyCodeStr}, 
                    City = '{city}', 
                    ProductCode = {productCode}, 
                    InstallationDate = {installationDateValue}, 
                    AMCFromDate = {amcFromDateValue}, 
                    AMCToDate = {amcToDateValue}, 
                    AMCAmount = {amcAmountStr}, 
                    OnlineCall = {onlineCallStr}, 
                    OnsiteCall = {onsiteCallStr}, 
                    NoOfNode = {noOfNodeStr}
                WHERE EntryNo = '{entryNo}'";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update AMC record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating AMC record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            entryNo = row["EntryNo"]?.ToString() ?? "";
            entryDate = row["EntryDate"] != DBNull.Value ? Convert.ToDateTime(row["EntryDate"]).ToString("yyyy-MM-dd") : "";
            partyCodeStr = row["PartyCode"]?.ToString() ?? "";
            city = row["City"]?.ToString() ?? "";
            
            // Populate ProductCode and ProductName
            if (row["ProductCode"] != DBNull.Value)
            {
                productCode = Convert.ToInt32(row["ProductCode"]);
                productName = db.GetScalar("ProductMaster", "ProductName", $"ProductCode = {productCode}");
            }
            
            installationDate = row["InstallationDate"] != DBNull.Value ? Convert.ToDateTime(row["InstallationDate"]).ToString("yyyy-MM-dd") : "";
            amcFromDate = row["AMCFromDate"] != DBNull.Value ? Convert.ToDateTime(row["AMCFromDate"]).ToString("yyyy-MM-dd") : "";
            amcToDate = row["AMCToDate"] != DBNull.Value ? Convert.ToDateTime(row["AMCToDate"]).ToString("yyyy-MM-dd") : "";
            amcAmountStr = row["AMCAmount"]?.ToString() ?? "";
            onlineCallStr = row["OnlineCall"]?.ToString() ?? "";
            onsiteCallStr = row["OnsiteCall"]?.ToString() ?? "";
            noOfNodeStr = row["NoOfNode"]?.ToString() ?? "";

            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        partyCodeStr = "";
        city = "";
        productName = "";
        installationDate = "";
        amcFromDate = "";
        amcToDate = "";
        amcAmountStr = "";
        onlineCallStr = "";
        onsiteCallStr = "";
        noOfNodeStr = "";
        entryDate = DateTime.Now.ToString("yyyy-MM-dd");

        // Reset actual values
        partyCode = 0;
        productCode = 0;
        amcAmount = 0;
        onlineCall = 0;
        onsiteCall = 0;
        noOfNode = 0;
    }






    private void InitializeTestData()
    {
        // Form field variables with test data
        entryDate = DateTime.Now.ToString("yyyy-MM-dd");
        partyCodeStr = "12345";
        city = "Mumbai";
        productName = "Vision Security System Pro";
        installationDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd"); // 30 days ago
        amcFromDate = DateTime.Now.ToString("yyyy-MM-dd");
        amcToDate = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd"); // 1 year from now
        amcAmountStr = "15000.50";
        onlineCallStr = "24";
        onsiteCallStr = "12";
        noOfNodeStr = "8";

        // Actual values for database operations
        partyCode = 12345;
        productCode = 101; // You might need to adjust this based on your ProductMaster table
        amcAmount = 15000.50m;
        onlineCall = 24;
        onsiteCall = 12;
        noOfNode = 8;
    }
    
    public void GenerateEntryNumber()
    {
        try
        {
            string nextNo = db.GenerateNextNo("AMCMaster", "EntryNo");
            entryNo = nextNo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating entry number: {ex.Message}");
        }
    }
}