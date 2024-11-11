using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumVitaeManagementAPI.Models
{
    public class Degree
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }
        
        [Required]
        public string Name { get; set; }

        public DateTime CreationTime { get; } = DateTime.Now;
    }
}
