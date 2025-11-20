using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prog2Bpartone.Models
{

    [Table("Claims")]
    public class Claims
    {
        [Key]
        public int ClaimId { get; set; }
        [Required]
        [MaxLength(255)]
        public string? ClaimName { get; set; }
        [Required]
        [MaxLength(255)]
        public string? ClaimType { get; set; }


        public int HoursWorked { get; set; }


        public decimal HourlyRate { get; set; }


        public decimal TotalAmount { get; set; }
        public string? ClaimMonth { get; set; }
        [Required]
        public ClaimStatus Status { get; set; }

        public DateTime SubmittedDate { get; set; }

        public DateTime? ReviewedDate { get; set; }
        public virtual List<UploadedDocument> Documents { get; set; } = new List<UploadedDocument>();
        public virtual List<ClaimReview> claimReview { get; set; } = new List<ClaimReview>();

        [NotMapped]
        public string SubmittedBy { get; set; } = string.Empty;

        [NotMapped]
        public string ReviewedBy { get; set; } = string.Empty;
    }


}
