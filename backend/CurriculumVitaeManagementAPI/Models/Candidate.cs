using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumVitaeManagementAPI.Models
{
    public class Candidate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]        
        [EmailAddress(ErrorMessage = "Please provide a valid Email Address")]
        public string? Email { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be 10 digits.")]
        public string? Mobile { get; set; }

        public List<Degree>? Degree { get; set; }

        public byte[]? CVBlob { get; set; }

        public DateTime CreationTime { get; } = DateTime.Now;

        [NotMapped] 
        public IFormFile? CVFile { get; set; }
    }
}
