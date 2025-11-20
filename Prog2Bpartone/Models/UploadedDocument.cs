using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prog2Bpartone.Models
{
    [Table("UploadedDocuments")]
    public class UploadedDocument
    {
        [Key]
        public int Id { get; set; }



        [Required]
        [MaxLength(225)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        public long FileSize { get; set; } = 1;

        public DateTime UploadedDate { get; set; } = DateTime.Now;

        [NotMapped]
        public bool IsEncrypted { get; set; } = true;

        public int ClaimId { get; set; }
    }
}
