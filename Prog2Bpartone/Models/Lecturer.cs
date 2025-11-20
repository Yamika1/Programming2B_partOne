using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prog2Bpartone.Models
{
    [Table("Lecturer")]
    public class Lecturer
    {
        [Key]
        public int LecturerId { get; set; }
        [Required]
        [MaxLength(255)]
        public string? LecturerName { get; set; }
        [Required]
        [MaxLength(255)]
        public string? Email { get; set; }
        [Required]
        public int ContactNumber { get; set; }

        [Required]
        public int HourlyRate { get; set; }
        public List<UploadedDocument> Documents { get; set; }



    }
}
