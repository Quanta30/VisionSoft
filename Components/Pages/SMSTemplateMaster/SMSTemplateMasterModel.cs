using System.Data;
using VisionSoft;

class SMSTemplateMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string smsTemplateCodeStr = "";
    public string smsTemplateName = "";
    public string smsTemplateMessage = "";

    // Actual values for database operations
    public int smsTemplateCode = 0;

    public SMSTemplateMasterModel()
    {
        // Constructor - no special initialization needed
    }

    public void GenerateTemplateCode()
    {
        try
        {
            smsTemplateCode = Convert.ToInt32(db.GenerateNextNo("SMSTemplateMaster", "SMSTemplateCode"));
            smsTemplateCodeStr = smsTemplateCode.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating SMS template code: {ex.Message}");
            smsTemplateCode = 1;
            smsTemplateCodeStr = "1";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(smsTemplateCodeStr) || !int.TryParse(smsTemplateCodeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(smsTemplateName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(smsTemplateMessage))
            isValid = false;

        // Check varchar length constraints
        if (!string.IsNullOrWhiteSpace(smsTemplateName) && smsTemplateName.Length > 50)
            isValid = false;

        if (!string.IsNullOrWhiteSpace(smsTemplateMessage) && smsTemplateMessage.Length > 500)
            isValid = false;

        return isValid;
    }

    public async Task SaveSMSTemplate()
    {
        // Parse string values to actual types
        if (int.TryParse(smsTemplateCodeStr, out int parsedTemplateCode))
            smsTemplateCode = parsedTemplateCode;

        // Prepare columns and values
        string columns = "SMSTemplateCode, SMSTemplateName, SMSTemplateMessage";
        string values = $"{smsTemplateCode}, '{smsTemplateName}', '{smsTemplateMessage}'";

        // Save SMS Template record
        bool saved = db.SaveRecord("SMSTemplateMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save SMS Template record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(smsTemplateCodeStr, out int parsedTemplateCode))
                smsTemplateCode = parsedTemplateCode;

            // Build the UPDATE query
            string query = $@"
                UPDATE SMSTemplateMaster SET 
                    SMSTemplateName = '{smsTemplateName}',
                    SMSTemplateMessage = '{smsTemplateMessage}'
                WHERE SMSTemplateCode = {smsTemplateCode}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update SMS Template record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating SMS Template record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            smsTemplateCodeStr = row["SMSTemplateCode"]?.ToString() ?? "";
            smsTemplateName = row["SMSTemplateName"]?.ToString() ?? "";
            smsTemplateMessage = row["SMSTemplateMessage"]?.ToString() ?? "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting form data: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        smsTemplateCodeStr = "";
        smsTemplateName = "";
        smsTemplateMessage = "";

        // Reset actual values
        smsTemplateCode = 0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        smsTemplateName = "Order Confirmation";
        smsTemplateMessage = "Dear {CustomerName}, your order {OrderNo} for {Amount} has been confirmed. Expected delivery: {Date}. Thank you for choosing {CompanyName}!";
        
        // Actual values for database operations
        // smsTemplateCode will be auto-generated
    }
}