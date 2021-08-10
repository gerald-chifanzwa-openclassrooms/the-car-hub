namespace CarHub.Data
{
    public class VehicleImage
    {
        public int Id { get; set; }
        public Vehicle Vehicle { get; set; }
        public int VehicleId { get; set; }
        public byte [] ImageData { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
    }
}
