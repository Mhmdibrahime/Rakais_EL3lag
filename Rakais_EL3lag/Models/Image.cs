using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rakais_EL3lag.Models
{
    public class Image
    {
        public int Id { get; set; }
        public int SectionId { get; set; }

        [MaxLength(500)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string FileName { get; set; } = null!; 
        public bool Active { get; set; } = true;

        public Section? Section { get; set; } = null!;

        [NotMapped]
        public string ImageUrl => $"/images/{FileName}"; 
    }


}
