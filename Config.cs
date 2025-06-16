namespace VisionSoft
{
    public class Config
    {
        public static string ConnectionString { get; set; } = "Server=PARAG;Database=VisionSoft;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;";
        public static string UserTable = "Users";
        public static string CustomerTable = "Customers";
        public static bool IsLoggedIn { get; set; } = false;

    }
}
