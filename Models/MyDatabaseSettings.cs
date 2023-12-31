﻿namespace MongoDB_Code.Models
{
    public class MyDatabaseSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string CollectionName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}