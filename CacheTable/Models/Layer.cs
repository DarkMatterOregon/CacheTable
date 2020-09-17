namespace CacheTable.Models
{
    public class Layer: IAirtable
    {
        public string Id { get; set; }
        public string Name  { get; set; }
        public string Lat  { get; set; }
        public string Log  { get; set; }
        public string Address  { get; set; }
        public string Directions  { get; set; }
        public string Phone  { get; set; }
        public string Hours  { get; set; }

    }

}
