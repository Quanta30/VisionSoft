using System.Data;
using VisionSoft;

class ReminderMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string entryNoStr = "";
    public string entryDate = "";
    public string remindWhat = "";
    public string remindTime = "";
    public string agent = "";
    public string snoozeStr = "0";
    public string repeatStr = "0";

    // Boolean fields for control
    public bool reminderOn = false;
    public bool reminderOff = false;

    // Days of week
    public bool dayMon = false;
    public bool dayTue = false;
    public bool dayWed = false;
    public bool dayThu = false;
    public bool dayFri = false;
    public bool daySat = false;
    public bool daySun = false;

    // Dictionary for date properties (1-31)
    public Dictionary<int, bool> DateProperties { get; set; } = new Dictionary<int, bool>();

    // Months
    public bool monthJan = false, monthFeb = false, monthMar = false, monthApr = false;
    public bool monthMay = false, monthJun = false, monthJul = false, monthAug = false;
    public bool monthSep = false, monthOct = false, monthNov = false, monthDec = false;

    // Years (simplified - only showing recent years)
    public bool year2020 = false;

    // Actual values for database operations
    public int entryNo = 0;
    public int snooze = 0;
    public int repeat = 0;

    public ReminderMasterModel()
    {
        // Initialize with default values
        snoozeStr = "0";
        repeatStr = "0";
        
        // Initialize date dictionary (1-31)
        for (int i = 1; i <= 31; i++)
        {
            DateProperties[i] = false;
        }
    }

    public void GenerateEntryNumber()
    {
        try
        {
            entryNo = Convert.ToInt32(db.GenerateNextNo("ReminderMaster", "EntryNo"));
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

        if (string.IsNullOrWhiteSpace(remindWhat))
            isValid = false;

        if (string.IsNullOrWhiteSpace(agent))
            isValid = false;

        if (string.IsNullOrWhiteSpace(snoozeStr) || !int.TryParse(snoozeStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(repeatStr) || !int.TryParse(repeatStr, out _))
            isValid = false;

        // EntryDate and RemindTime are optional (nullable)
        // All boolean fields are always valid (checkbox state)

        return isValid;
    }

    public async Task SaveReminder()
    {
        // Parse string values to actual types
        if (int.TryParse(entryNoStr, out int parsedEntryNo))
            entryNo = parsedEntryNo;

        if (int.TryParse(snoozeStr, out int parsedSnooze))
            snooze = parsedSnooze;

        if (int.TryParse(repeatStr, out int parsedRepeat))
            repeat = parsedRepeat;

        // Handle nullable datetime fields
        string entryDateValue = string.IsNullOrEmpty(entryDate) ? "NULL" : $"'{entryDate}'";
        string remindTimeValue = string.IsNullOrEmpty(remindTime) ? "NULL" : $"'{remindTime}'";

        // Build date values string from dictionary
        string dateValues = string.Join(", ", DateProperties.Select(kvp => kvp.Value ? 1 : 0));

        // Prepare columns and values
        string columns = @"EntryNo, EntryDate, RemindWhat, ReminderOn, ReminderOff, 
                          DayMon, DayTue, DayWed, DayThu, DayFri, DaySat, DaySun,
                          Date1, Date2, Date3, Date4, Date5, Date6, Date7, Date8, Date9, Date10,
                          Date11, Date12, Date13, Date14, Date15, Date16, Date17, Date18, Date19, Date20,
                          Date21, Date22, Date23, Date24, Date25, Date26, Date27, Date28, Date29, Date30, Date31,
                          MonthJan, MonthFeb, MonthMar, MonthApr, MonthMay, MonthJun,
                          MonthJul, MonthAug, MonthSep, MonthOct, MonthNov, MonthDec,
                          Year2020, RemindTime, Snooze, Repeat, Agent";

        string values = $@"{entryNo}, {entryDateValue}, '{remindWhat}', {(reminderOn ? 1 : 0)}, {(reminderOff ? 1 : 0)},
                          {(dayMon ? 1 : 0)}, {(dayTue ? 1 : 0)}, {(dayWed ? 1 : 0)}, {(dayThu ? 1 : 0)}, {(dayFri ? 1 : 0)}, {(daySat ? 1 : 0)}, {(daySun ? 1 : 0)},
                          {dateValues},
                          {(monthJan ? 1 : 0)}, {(monthFeb ? 1 : 0)}, {(monthMar ? 1 : 0)}, {(monthApr ? 1 : 0)}, {(monthMay ? 1 : 0)}, {(monthJun ? 1 : 0)},
                          {(monthJul ? 1 : 0)}, {(monthAug ? 1 : 0)}, {(monthSep ? 1 : 0)}, {(monthOct ? 1 : 0)}, {(monthNov ? 1 : 0)}, {(monthDec ? 1 : 0)},
                          {(year2020 ? 1 : 0)}, {remindTimeValue}, {snooze}, {repeat}, '{agent}'";

        // Save Reminder record
        bool saved = db.SaveRecord("ReminderMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Reminder record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse string values to actual types
            if (int.TryParse(entryNoStr, out int parsedEntryNo))
                entryNo = parsedEntryNo;

            if (int.TryParse(snoozeStr, out int parsedSnooze))
                snooze = parsedSnooze;

            if (int.TryParse(repeatStr, out int parsedRepeat))
                repeat = parsedRepeat;

            // Handle nullable datetime fields
            string entryDateValue = string.IsNullOrEmpty(entryDate) ? "NULL" : $"'{entryDate}'";
            string remindTimeValue = string.IsNullOrEmpty(remindTime) ? "NULL" : $"'{remindTime}'";

            // Build UPDATE query with dictionary values
            string query = $@"
                UPDATE ReminderMaster SET 
                    EntryDate = {entryDateValue},
                    RemindWhat = '{remindWhat}',
                    ReminderOn = {(reminderOn ? 1 : 0)},
                    ReminderOff = {(reminderOff ? 1 : 0)},
                    DayMon = {(dayMon ? 1 : 0)}, DayTue = {(dayTue ? 1 : 0)}, DayWed = {(dayWed ? 1 : 0)}, 
                    DayThu = {(dayThu ? 1 : 0)}, DayFri = {(dayFri ? 1 : 0)}, DaySat = {(daySat ? 1 : 0)}, DaySun = {(daySun ? 1 : 0)},
                    Date1 = {(DateProperties[1] ? 1 : 0)}, Date2 = {(DateProperties[2] ? 1 : 0)}, Date3 = {(DateProperties[3] ? 1 : 0)}, 
                    Date4 = {(DateProperties[4] ? 1 : 0)}, Date5 = {(DateProperties[5] ? 1 : 0)}, Date6 = {(DateProperties[6] ? 1 : 0)}, 
                    Date7 = {(DateProperties[7] ? 1 : 0)}, Date8 = {(DateProperties[8] ? 1 : 0)}, Date9 = {(DateProperties[9] ? 1 : 0)}, 
                    Date10 = {(DateProperties[10] ? 1 : 0)}, Date11 = {(DateProperties[11] ? 1 : 0)}, Date12 = {(DateProperties[12] ? 1 : 0)}, 
                    Date13 = {(DateProperties[13] ? 1 : 0)}, Date14 = {(DateProperties[14] ? 1 : 0)}, Date15 = {(DateProperties[15] ? 1 : 0)}, 
                    Date16 = {(DateProperties[16] ? 1 : 0)}, Date17 = {(DateProperties[17] ? 1 : 0)}, Date18 = {(DateProperties[18] ? 1 : 0)}, 
                    Date19 = {(DateProperties[19] ? 1 : 0)}, Date20 = {(DateProperties[20] ? 1 : 0)}, Date21 = {(DateProperties[21] ? 1 : 0)}, 
                    Date22 = {(DateProperties[22] ? 1 : 0)}, Date23 = {(DateProperties[23] ? 1 : 0)}, Date24 = {(DateProperties[24] ? 1 : 0)}, 
                    Date25 = {(DateProperties[25] ? 1 : 0)}, Date26 = {(DateProperties[26] ? 1 : 0)}, Date27 = {(DateProperties[27] ? 1 : 0)}, 
                    Date28 = {(DateProperties[28] ? 1 : 0)}, Date29 = {(DateProperties[29] ? 1 : 0)}, Date30 = {(DateProperties[30] ? 1 : 0)}, 
                    Date31 = {(DateProperties[31] ? 1 : 0)},
                    MonthJan = {(monthJan ? 1 : 0)}, MonthFeb = {(monthFeb ? 1 : 0)}, MonthMar = {(monthMar ? 1 : 0)}, 
                    MonthApr = {(monthApr ? 1 : 0)}, MonthMay = {(monthMay ? 1 : 0)}, MonthJun = {(monthJun ? 1 : 0)},
                    MonthJul = {(monthJul ? 1 : 0)}, MonthAug = {(monthAug ? 1 : 0)}, MonthSep = {(monthSep ? 1 : 0)}, 
                    MonthOct = {(monthOct ? 1 : 0)}, MonthNov = {(monthNov ? 1 : 0)}, MonthDec = {(monthDec ? 1 : 0)},
                    Year2020 = {(year2020 ? 1 : 0)},
                    RemindTime = {remindTimeValue},
                    Snooze = {snooze},
                    Repeat = {repeat},
                    Agent = '{agent}'
                WHERE EntryNo = {entryNo}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Reminder record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Reminder record: {ex.Message}");
            throw;
        }
    }

    public void set(DataRow row)
    {
        try
        {
            // Populate basic fields
            entryNoStr = row["EntryNo"]?.ToString() ?? "";
            entryDate = row["EntryDate"]?.ToString() ?? "";
            remindWhat = row["RemindWhat"]?.ToString() ?? "";
            remindTime = row["RemindTime"]?.ToString() ?? "";
            agent = row["Agent"]?.ToString() ?? "";
            snoozeStr = row["Snooze"]?.ToString() ?? "0";
            repeatStr = row["Repeat"]?.ToString() ?? "0";

            // Populate boolean fields
            if (row["ReminderOn"] != DBNull.Value)
                reminderOn = Convert.ToBoolean(row["ReminderOn"]);
            if (row["ReminderOff"] != DBNull.Value)
                reminderOff = Convert.ToBoolean(row["ReminderOff"]);

            // Populate days
            if (row["DayMon"] != DBNull.Value) dayMon = Convert.ToBoolean(row["DayMon"]);
            if (row["DayTue"] != DBNull.Value) dayTue = Convert.ToBoolean(row["DayTue"]);
            if (row["DayWed"] != DBNull.Value) dayWed = Convert.ToBoolean(row["DayWed"]);
            if (row["DayThu"] != DBNull.Value) dayThu = Convert.ToBoolean(row["DayThu"]);
            if (row["DayFri"] != DBNull.Value) dayFri = Convert.ToBoolean(row["DayFri"]);
            if (row["DaySat"] != DBNull.Value) daySat = Convert.ToBoolean(row["DaySat"]);
            if (row["DaySun"] != DBNull.Value) daySun = Convert.ToBoolean(row["DaySun"]);

            // Populate date dictionary
            for (int i = 1; i <= 31; i++)
            {
                string columnName = $"Date{i}";
                if (row[columnName] != DBNull.Value)
                    DateProperties[i] = Convert.ToBoolean(row[columnName]);
            }

            // Populate months
            if (row["MonthJan"] != DBNull.Value) monthJan = Convert.ToBoolean(row["MonthJan"]);
            if (row["MonthFeb"] != DBNull.Value) monthFeb = Convert.ToBoolean(row["MonthFeb"]);
            if (row["MonthMar"] != DBNull.Value) monthMar = Convert.ToBoolean(row["MonthMar"]);
            if (row["MonthApr"] != DBNull.Value) monthApr = Convert.ToBoolean(row["MonthApr"]);
            if (row["MonthMay"] != DBNull.Value) monthMay = Convert.ToBoolean(row["MonthMay"]);
            if (row["MonthJun"] != DBNull.Value) monthJun = Convert.ToBoolean(row["MonthJun"]);
            if (row["MonthJul"] != DBNull.Value) monthJul = Convert.ToBoolean(row["MonthJul"]);
            if (row["MonthAug"] != DBNull.Value) monthAug = Convert.ToBoolean(row["MonthAug"]);
            if (row["MonthSep"] != DBNull.Value) monthSep = Convert.ToBoolean(row["MonthSep"]);
            if (row["MonthOct"] != DBNull.Value) monthOct = Convert.ToBoolean(row["MonthOct"]);
            if (row["MonthNov"] != DBNull.Value) monthNov = Convert.ToBoolean(row["MonthNov"]);
            if (row["MonthDec"] != DBNull.Value) monthDec = Convert.ToBoolean(row["MonthDec"]);
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
        entryDate = "";
        remindWhat = "";
        remindTime = "";
        agent = "";
        snoozeStr = "0";
        repeatStr = "0";

        // Reset all boolean fields
        reminderOn = reminderOff = false;
        dayMon = dayTue = dayWed = dayThu = dayFri = daySat = daySun = false;
        
        // Reset date dictionary
        for (int i = 1; i <= 31; i++)
        {
            DateProperties[i] = false;
        }
        
        monthJan = monthFeb = monthMar = monthApr = monthMay = monthJun = false;
        monthJul = monthAug = monthSep = monthOct = monthNov = monthDec = false;

        // Reset actual values
        entryNo = snooze = repeat = 0;
    }

    public void InitializeTestData()
    {
        remindWhat = "Weekly team meeting reminder";
        agent = "System Admin";
        reminderOn = true;
        dayMon = true;
        dayWed = true;
        dayFri = true;
        monthJan = monthFeb = monthMar = true;
        snoozeStr = "5";
        repeatStr = "3";
        
        // Set some test dates
        DateProperties[1] = true;
        DateProperties[15] = true;
        DateProperties[30] = true;
    }
}