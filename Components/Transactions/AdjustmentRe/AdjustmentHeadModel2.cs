// using System.Data;
// using VisionSoft;

// public class AdjustmentHeadModel2
// {
//     ClsDatabase db = new ClsDatabase();
    
//     // Form field variables (strings for binding)
//     public string adjustmentNoStr = "";
//     public string adjustmentDate = "";
//     public string netAmountStr = "0.00";
//     // In AdjustmentHeadModel.cs
//     private string _narration = "";
//     public string narration 
//     { 
//         get => _narration;
//         set 
//         {
//             _narration = value;
//             // You can add validation here if needed
//             OnNarrationChanged?.Invoke(value);
//         }
//     }

// public Action<string>? OnNarrationChanged { get; set; }
//     public string cancelledStr = "0";
    
//     // Actual values for database operations
//     public long adjustmentNo = 0;
//     public float netAmount = 0.0f;
//     public int cancelled = 0;

//     public AdjustmentHeadModel2()
//     {
//         // Initialize with default values
//         netAmountStr = "0.00";
//         cancelledStr = "0";
//         narration = "-";
//     }

//     public void GenerateAdjustmentNo()
//     {
//         try
//         {
//             adjustmentNoStr = db.GenerateNextNo("AdjustmentHead", "AdjustmentNo");
//             if (long.TryParse(adjustmentNoStr, out long parsedNo))
//                 adjustmentNo = parsedNo;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error generating adjustment number: {ex.Message}");
//             adjustmentNoStr = "1";
//             adjustmentNo = 1;
//         }
//     }

//     public bool ValidateForm()
//     {
//         bool isValid = true;

//         if (string.IsNullOrWhiteSpace(adjustmentNoStr))
//             isValid = false;

//         if (string.IsNullOrWhiteSpace(narration))
//             isValid = false;

//         if (string.IsNullOrWhiteSpace(netAmountStr) || !float.TryParse(netAmountStr, out _))
//             isValid = false;

//         if (string.IsNullOrWhiteSpace(cancelledStr) || !int.TryParse(cancelledStr, out _))
//             isValid = false;

//         // Check varchar length constraints
//         if (!string.IsNullOrWhiteSpace(narration) && narration.Length > 200)
//             isValid = false;

//         return isValid;
//     }

//     public async Task SaveAdjustmentHead()
//     {
//         // Parse string values to actual types
//         if (long.TryParse(adjustmentNoStr, out long parsedAdjustmentNo))
//             adjustmentNo = parsedAdjustmentNo;

//         if (float.TryParse(netAmountStr, out float parsedNetAmount))
//             netAmount = parsedNetAmount;

//         if (int.TryParse(cancelledStr, out int parsedCancelled))
//             cancelled = parsedCancelled;

//         // Handle nullable datetime fields
//         string adjustmentDateValue = string.IsNullOrEmpty(adjustmentDate) ? "NULL" : $"'{adjustmentDate}'";

//         // Prepare columns and values for AdjustmentHead
//         string columns = "AdjustmentNo, AdjustmentDate, NetAmount, Narration, Cancelled";
//         string values = $"{adjustmentNo}, {adjustmentDateValue}, {netAmount}, '{narration}', {cancelled}";

//         // Save AdjustmentHead record only
//         bool saved = db.SaveRecord("AdjustmentHead", columns, values);
//         if (!saved)
//         {
//             throw new Exception("Failed to save Adjustment Head record");
//         }

//         Console.WriteLine($"Saved AdjustmentHead: {adjustmentNo}");
//     }

//     public async Task UpdateAdjustmentHead()
//     {
//         try
//         {
//             // Parse string values to actual types
//             if (long.TryParse(adjustmentNoStr, out long parsedAdjustmentNo))
//                 adjustmentNo = parsedAdjustmentNo;

//             if (float.TryParse(netAmountStr, out float parsedNetAmount))
//                 netAmount = parsedNetAmount;

//             if (int.TryParse(cancelledStr, out int parsedCancelled))
//                 cancelled = parsedCancelled;

//             // Handle nullable datetime fields
//             string adjustmentDateValue = string.IsNullOrEmpty(adjustmentDate) ? "NULL" : $"'{adjustmentDate}'";

//             // Build the UPDATE query for AdjustmentHead only
//             string query = $@"
//                 UPDATE AdjustmentHead SET 
//                     AdjustmentDate = {adjustmentDateValue},
//                     NetAmount = {netAmount},
//                     Narration = '{narration}',
//                     Cancelled = {cancelled}
//                 WHERE AdjustmentNo = {adjustmentNo}";

//             // Execute the query
//             bool saved = db.ExecuteQuery(query);
//             if (!saved)
//             {
//                 throw new Exception("Failed to update Adjustment Head record");
//             }

//             Console.WriteLine($"Updated AdjustmentHead: {adjustmentNo}");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error updating Adjustment Head: {ex.Message}");
//             throw;
//         }
//     }

//     public void Set(DataRow row)
//     {
//         try
//         {
//             // Populate form fields from the DataRow
//             adjustmentNoStr = row["AdjustmentNo"]?.ToString() ?? "";
//             adjustmentDate = row["AdjustmentDate"]?.ToString() ?? "";
//             netAmountStr = row["NetAmount"]?.ToString() ?? "0.00";
//             narration = row["Narration"]?.ToString() ?? "";
//             cancelledStr = row["Cancelled"]?.ToString() ?? "0";

//             // Parse actual values
//             if (long.TryParse(adjustmentNoStr, out long parsedNo))
//                 adjustmentNo = parsedNo;

//             if (float.TryParse(netAmountStr, out float parsedAmount))
//                 netAmount = parsedAmount;

//             if (int.TryParse(cancelledStr, out int parsedCancelled))
//                 cancelled = parsedCancelled;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error setting form data: {ex.Message}");
//             throw;
//         }
//     }

//     public void Clear()
//     {
//         adjustmentNoStr = "";
//         adjustmentDate = "";
//         netAmountStr = "0.00";
//         narration = "-";
//         cancelledStr = "0";

//         // Reset actual values
//         adjustmentNo = 0;
//         netAmount = 0.0f;
//         cancelled = 0;
//     }

//     public void UpdateNetAmount(decimal totalAmount)
//     {
//         netAmount = (float)totalAmount;
//         netAmountStr = totalAmount.ToString("F2");
//     }
// }