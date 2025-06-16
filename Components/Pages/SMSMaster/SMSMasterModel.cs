using System.Data;
using VisionSoft;

class SMSMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string entryNoStr = "";
    public string mobileNo = "";
    public string message = "";
    public string partyName = "";
    
    // Boolean field
    public bool sent = false;

    public SMSMasterModel()
    {
        // Constructor - no special initialization needed
    }

    public void GenerateEntryNumber()
    {
        try
        {
            // For varchar EntryNo, we format it appropriately
            int nextNo = Convert.ToInt32(db.GenerateNextNo("SMSMaster", "EntryNo"));
            entryNoStr = $"SMS{nextNo:D6}"; // Format as SMS000001, SMS000002, etc.
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating entry number: {ex.Message}");
            entryNoStr = "SMS000001";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(entryNoStr))
            isValid = false;

        if (string.IsNullOrWhiteSpace(mobileNo))
            isValid = false;

        if (string.IsNullOrWhiteSpace(message))
            isValid = false;

        // Check varchar length constraints
        if (!string.IsNullOrWhiteSpace(entryNoStr) && entryNoStr.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(mobileNo) && mobileNo.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(message) && message.Length > 500)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(partyName) && partyName.Length > 150)
            isValid = false;

        // Basic mobile number validation (should contain only digits, +, -, spaces)
        if (!string.IsNullOrWhiteSpace(mobileNo))
        {
            string cleanMobile = mobileNo.Replace(" ", "").Replace("-", "").Replace("+", "");
            if (!cleanMobile.All(char.IsDigit))
                isValid = false;
        }

        // PartyName is nullable/optional
        // Sent is a boolean field, always valid (checkbox state)

        return isValid;
    }

    public async Task SaveSMS()
    {
        // Handle nullable partyName field
        string partyNameValue = string.IsNullOrEmpty(partyName) ? "NULL" : $"'{partyName}'";

        // Prepare columns and values
        string columns = "EntryNo, MobileNo, Message, partyName, Sent";
        string values = $"'{entryNoStr}', '{mobileNo}', '{message}', {partyNameValue}, {(sent ? 1 : 0)}";

        // Save SMS record
        bool saved = db.SaveRecord("SMSMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save SMS record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Handle nullable partyName field
            string partyNameValue = string.IsNullOrEmpty(partyName) ? "NULL" : $"'{partyName}'";

            // Build the UPDATE query
            string query = $@"
                UPDATE SMSMaster SET 
                    MobileNo = '{mobileNo}',
                    Message = '{message}',
                    partyName = {partyNameValue},
                    Sent = {(sent ? 1 : 0)}
                WHERE EntryNo = '{entryNoStr}'";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update SMS record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating SMS record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            entryNoStr = row["EntryNo"]?.ToString() ?? "";
            mobileNo = row["MobileNo"]?.ToString() ?? "";
            message = row["Message"]?.ToString() ?? "";
            partyName = row["partyName"]?.ToString() ?? "";

            // Handle boolean field
            if (row["Sent"] != DBNull.Value)
                sent = Convert.ToBoolean(row["Sent"]);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        entryNoStr = "";
        mobileNo = "";
        message = "";
        partyName = "";
        sent = false;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        mobileNo = "+91-9876543210";
        message = "Thank you for your recent purchase! Your order has been confirmed and will be delivered within 3-5 business days.";
        partyName = "John Doe";
        sent = false;
    }
}