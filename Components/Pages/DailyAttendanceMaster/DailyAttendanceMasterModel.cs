using System.Data;
using VisionSoft;

class DailyAttendanceMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string entryNoStr = "";
    public string entryDate = DateTime.Now.ToString("yyyy-MM-dd");
    public string engineerName = "";
    public string inTime = "";
    public string outTime = "";

    // Actual values for database operations
    public int entryNo = 0;
    public int engineerCode = 0;

    public DailyAttendanceMasterModel()
    {
        // Initialize with current date
        entryDate = DateTime.Now.ToString("yyyy-MM-dd");
    }

    public void GenerateEntryNumber()
    {
        try
        {
            entryNo = Convert.ToInt32(db.GenerateNextNo("DailyAttendanceMaster", "EntryNo"));
            entryNoStr = entryNo.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating entry number: {ex.Message}");
            entryNo = 1;
            entryNoStr = "1";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(entryNoStr) || !int.TryParse(entryNoStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(engineerName))
            isValid = false;

        // EntryDate, InTime, and OutTime are optional (nullable), so no validation required

        return isValid;
    }

    public async Task SaveAttendance()
    {
        // Parse string values to actual types
        if (int.TryParse(entryNoStr, out int parsedEntryNo))
            entryNo = parsedEntryNo;

        // Prepare date/time values for database
        string entryDateValue = string.IsNullOrEmpty(entryDate) ? "NULL" : $"'{entryDate}'";
        
        string inTimeValue = "NULL";
        if (!string.IsNullOrEmpty(inTime))
        {
            // Combine date and time for datetime field
            string inDateTime = string.IsNullOrEmpty(entryDate) ? 
                $"{DateTime.Now:yyyy-MM-dd} {inTime}" : 
                $"{entryDate} {inTime}";
            inTimeValue = $"'{inDateTime}'";
        }
        
        string outTimeValue = "NULL";
        if (!string.IsNullOrEmpty(outTime))
        {
            // Combine date and time for datetime field
            string outDateTime = string.IsNullOrEmpty(entryDate) ? 
                $"{DateTime.Now:yyyy-MM-dd} {outTime}" : 
                $"{entryDate} {outTime}";
            outTimeValue = $"'{outDateTime}'";
        }

        // Prepare columns and values
        string columns = "EntryNo, EntryDate, EngineerCode, InTime, OutTime";
        string values = $"{entryNo}, {entryDateValue}, {engineerCode}, {inTimeValue}, {outTimeValue}";

        // Save Daily Attendance record
        bool saved = db.SaveRecord("DailyAttendanceMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Daily Attendance record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(entryNoStr, out int parsedEntryNo))
                entryNo = parsedEntryNo;

            // Prepare date/time values for database
            string entryDateValue = string.IsNullOrEmpty(entryDate) ? "NULL" : $"'{entryDate}'";
            
            string inTimeValue = "NULL";
            if (!string.IsNullOrEmpty(inTime))
            {
                string inDateTime = string.IsNullOrEmpty(entryDate) ? 
                    $"{DateTime.Now:yyyy-MM-dd} {inTime}" : 
                    $"{entryDate} {inTime}";
                inTimeValue = $"'{inDateTime}'";
            }
            
            string outTimeValue = "NULL";
            if (!string.IsNullOrEmpty(outTime))
            {
                string outDateTime = string.IsNullOrEmpty(entryDate) ? 
                    $"{DateTime.Now:yyyy-MM-dd} {outTime}" : 
                    $"{entryDate} {outTime}";
                outTimeValue = $"'{outDateTime}'";
            }

            // Build the UPDATE query
            string query = $@"
                UPDATE DailyAttendanceMaster SET 
                    EntryDate = {entryDateValue}, 
                    EngineerCode = {engineerCode},
                    InTime = {inTimeValue},
                    OutTime = {outTimeValue}
                WHERE EntryNo = {entryNo}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Daily Attendance record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Daily Attendance record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate form fields from the DataRow
            entryNoStr = row["EntryNo"]?.ToString() ?? "";
            entryDate = row["EntryDate"] != DBNull.Value ? Convert.ToDateTime(row["EntryDate"]).ToString("yyyy-MM-dd") : "";
            
            // Handle InTime and OutTime - extract time part from datetime
            if (row["InTime"] != DBNull.Value)
            {
                DateTime inDateTime = Convert.ToDateTime(row["InTime"]);
                inTime = inDateTime.ToString("HH:mm");
            }
            else
            {
                inTime = "";
            }
            
            if (row["OutTime"] != DBNull.Value)
            {
                DateTime outDateTime = Convert.ToDateTime(row["OutTime"]);
                outTime = outDateTime.ToString("HH:mm");
            }
            else
            {
                outTime = "";
            }

            // Populate engineer reference field
            if (row["EngineerCode"] != DBNull.Value)
            {
                engineerCode = Convert.ToInt32(row["EngineerCode"]);
                engineerName = db.GetScalar("EngineerMaster", "EngineerName", $"EngineerCode = {engineerCode}");
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
        entryNoStr = "";
        entryDate = DateTime.Now.ToString("yyyy-MM-dd");
        engineerName = "";
        inTime = "";
        outTime = "";

        // Reset actual values
        entryNo = 0;
        engineerCode = 0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        engineerName = "John Smith";
        entryDate = DateTime.Now.ToString("yyyy-MM-dd");
        inTime = "09:00";
        outTime = "18:00";
        
        // Actual values for database operations
        engineerCode = 101;
    }
}