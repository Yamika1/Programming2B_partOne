using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prog2Bpartone.Models
{
    [Table("ClaimReview")]
    public class ClaimReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReviewerId { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        [Required]
        public ClaimStatus Decision { get; set; }

        [Column(TypeName = "VARCHAR(MAX)")]
        public string Comments { get; set; } = string.Empty;

        [NotMapped]
        public string? ReviewerName { get; set; }

        [NotMapped]
        public string? ReviewerRole { get; set; }

        public int ClaimId { get; set; }
    }
}