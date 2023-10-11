namespace MongoDB_Code.Models
{
    public class BackofficeModel
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string CollectionName { get; set; } = string.Empty;
        public string EncryptionKey { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
