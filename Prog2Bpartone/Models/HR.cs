namespace Prog2Bpartone.Models
{
    public class HR
    {
        public int Id { get; set; }

        public string UserName { get; set; }
        public string FullName { get; set; }

        public string? Email { get; set; }

        public int? ContactNumber { get; set; }

        public int? hourlyRate { get; set; }

        public UserStatus Status { get; set; }
        public List<UploadedDocument> Documents { get; set; }
    }
}
