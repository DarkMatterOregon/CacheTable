using Newtonsoft.Json;

namespace CacheTable.Models
{
    public class App: IAirtable
    {
        public string Id { get; set; }
        public string Name  { get; set; }
        public string BaseId  { get; set; }
        public string AppKey  { get; set; }
    }

    public class Table: IAirtable
    {
        public string Id  { get; set; }
        public string Name  { get; set; }
        [JsonProperty("Table")]
        public string TableName  { get; set; }
        public string View  { get; set; }
    }
}
