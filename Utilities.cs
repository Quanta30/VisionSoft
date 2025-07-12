using VisionSoft;

public class Utilities
{
    public ClsDatabase db = new ClsDatabase();
    //Stock Adjustment Utilities
    public void AdjustStock(String StockId, String AdjustmentType, String value)
    {
        bool flag = false;
        try
        {
            float quantity = float.Parse(value);

            string operation = (AdjustmentType == "ADD") ? "+" : "-";

            string updateQuery = $@"
                    UPDATE PhysicalStock 
                    SET Quantity = Quantity {operation} {quantity}
                    WHERE StockID = {StockId}";

            db.ExecuteQuery(updateQuery);
            Console.WriteLine($"Updated PhysicalStock for StockID: {StockId}, Quantity: {operation}{quantity}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating physical stock: {ex.Message}");
        }
    }
    public void AdjustStock(String StockId, String AdjustmentType, String value, Transaction transaction)
    {
  
            float quantity = float.Parse(value);

            string operation = (AdjustmentType == "ADD") ? "+" : "-";

            string updateQuery = $@"
                    UPDATE PhysicalStock 
                    SET Quantity = Quantity {operation} {quantity}
                    WHERE StockID = {StockId}";

            transaction.AddQuery(updateQuery);
        
    }

    public string StockIdToName(string id)
    {
        string code = db.GetScalar($"select ProductCode from PhysicalStock where StockID=3");
        string name = db.GetScalar($"select ProductName from ProductMaster where ProductCode={code}");
        Console.WriteLine($"{code} {name}");
        return name;
    }
    public string StockNameToId(string sname){
        string pcode = db.GetScalar($"select ProductCode from ProductMaster where ProductName='{sname}'");
        string stockId = db.GetScalar($"select StockId from PhysicalStock where ProductCode = '{pcode}'");
        return stockId;
    }
}