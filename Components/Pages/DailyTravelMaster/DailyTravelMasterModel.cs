using System.Data;
using VisionSoft;

class DailyTravelMasterModel
{
    // Form field variables (strings for binding)
    ClsDatabase db = new ClsDatabase();
    public string entryNoStr = "";
    public string entryDate = DateTime.Now.ToString("yyyy-MM-dd");
    public string engineerName = "";
    public string startKmStr = "0.0";
    public string endKmStr = "0.0";
    public string travelingRateStr = "0.0";
    public string route = "";
    
    // Calculated fields (read-only)
    public string travelDistanceStr = "0.0";
    public string totalAmountStr = "0.0";

    // Actual values for database operations
    public int entryNo = 0;
    public int engineerCode = 0;
    public double startKm = 0.0;
    public double endKm = 0.0;
    public double travelingRate = 0.0;
    public double travelDistance = 0.0;
    public double totalAmount = 0.0;

    public DailyTravelMasterModel()
    {
        // Initialize with current date
        entryDate = DateTime.Now.ToString("yyyy-MM-dd");
    }

    public void GenerateEntryNumber()
    {
        try
        {
            entryNo = Convert.ToInt32(db.GenerateNextNo("DailyTravelMaster", "EntryNo"));
            entryNoStr = entryNo.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating entry number: {ex.Message}");
            entryNo = 1;
            entryNoStr = "1";
        }
    }

    public void CalculateTravelDetails()
    {
        try
        {
            // Parse string values to doubles
            if (double.TryParse(startKmStr, out double parsedStartKm) && 
                double.TryParse(endKmStr, out double parsedEndKm) &&
                double.TryParse(travelingRateStr, out double parsedTravelingRate))
            {
                startKm = parsedStartKm;
                endKm = parsedEndKm;
                travelingRate = parsedTravelingRate;

                // Calculate travel distance (End Km - Start Km)
                if (endKm >= startKm)
                {
                    travelDistance = endKm - startKm;
                }
                else
                {
                    travelDistance = 0.0; // Invalid: end km should be greater than start km
                }

                // Calculate total amount (Distance * Rate)
                totalAmount = travelDistance * travelingRate;

                // Update string values for display
                travelDistanceStr = travelDistance.ToString("F2");
                totalAmountStr = totalAmount.ToString("F2");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calculating travel details: {ex.Message}");
            travelDistanceStr = "0.0";
            totalAmountStr = "0.0";
        }
    }

    public bool ValidateForm()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(entryNoStr) || !int.TryParse(entryNoStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(engineerName))
            isValid = false;

        if (string.IsNullOrWhiteSpace(startKmStr) || !double.TryParse(startKmStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(endKmStr) || !double.TryParse(endKmStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(travelingRateStr) || !double.TryParse(travelingRateStr, out _))
            isValid = false;

        if (string.IsNullOrWhiteSpace(route))
            isValid = false;

        // Additional validation: End Km should be greater than or equal to Start Km
        if (double.TryParse(startKmStr, out double startVal) && 
            double.TryParse(endKmStr, out double endVal))
        {
            if (endVal < startVal)
                isValid = false;
        }

        // EntryDate is optional (nullable), so no validation required

        return isValid;
    }

    public async Task SaveTravel()
    {
        // Parse and calculate values
        CalculateTravelDetails();
        
        if (int.TryParse(entryNoStr, out int parsedEntryNo))
            entryNo = parsedEntryNo;

        // Prepare date value for database
        string entryDateValue = string.IsNullOrEmpty(entryDate) ? "NULL" : $"'{entryDate}'";

        // Prepare columns and values
        string columns = "EntryNo, EntryDate, EngineerCode, StartKm, EndKm, TravelingRate, Route";
        string values = $"{entryNo}, {entryDateValue}, {engineerCode}, {startKm}, {endKm}, {travelingRate}, '{route}'";

        // Save Daily Travel record
        bool saved = db.SaveRecord("DailyTravelMaster", columns, values);
        if (!saved)
        {
            throw new Exception("Failed to save Daily Travel record");
        }
    }

    public async Task Update()
    {
        try
        {
            // Parse and calculate values
            CalculateTravelDetails();
            
            if (int.TryParse(entryNoStr, out int parsedEntryNo))
                entryNo = parsedEntryNo;

            // Prepare date value for database
            string entryDateValue = string.IsNullOrEmpty(entryDate) ? "NULL" : $"'{entryDate}'";

            // Build the UPDATE query
            string query = $@"
                UPDATE DailyTravelMaster SET 
                    EntryDate = {entryDateValue}, 
                    EngineerCode = {engineerCode},
                    StartKm = {startKm},
                    EndKm = {endKm},
                    TravelingRate = {travelingRate},
                    Route = '{route}'
                WHERE EntryNo = {entryNo}";

            // Execute the query
            bool saved = db.ExecuteQuery(query);
            if (!saved)
            {
                throw new Exception("Failed to update Daily Travel record");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Daily Travel record: {ex.Message}");
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
            startKmStr = row["StartKm"]?.ToString() ?? "0.0";
            endKmStr = row["EndKm"]?.ToString() ?? "0.0";
            travelingRateStr = row["TravelingRate"]?.ToString() ?? "0.0";
            route = row["Route"]?.ToString() ?? "";

            // Populate engineer reference field
            if (row["EngineerCode"] != DBNull.Value)
            {
                engineerCode = Convert.ToInt32(row["EngineerCode"]);
                engineerName = db.GetScalar("EngineerMaster", "EngineerName", $"EngineerCode = {engineerCode}");
            }

            // Calculate travel details
            CalculateTravelDetails();
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
        startKmStr = "0.0";
        endKmStr = "0.0";
        travelingRateStr = "0.0";
        route = "";
        travelDistanceStr = "0.0";
        totalAmountStr = "0.0";

        // Reset actual values
        entryNo = 0;
        engineerCode = 0;
        startKm = 0.0;
        endKm = 0.0;
        travelingRate = 0.0;
        travelDistance = 0.0;
        totalAmount = 0.0;
    }

    public void InitializeTestData()
    {
        // Form field variables with test data
        engineerName = "John Smith";
        entryDate = DateTime.Now.ToString("yyyy-MM-dd");
        startKmStr = "100.5";
        endKmStr = "250.8";
        travelingRateStr = "12.50";
        route = "Mumbai to Pune via Express Highway";
        
        // Actual values for database operations
        engineerCode = 101;
        
        // Calculate travel details
        CalculateTravelDetails();
    }
}