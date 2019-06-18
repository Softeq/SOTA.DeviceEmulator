using Newtonsoft.Json;

class StaticWebsiteMetadata
{
    [JsonProperty("artifacts_storage_account_name")]
    public string BlobStorageAccountName { get; set; }
}