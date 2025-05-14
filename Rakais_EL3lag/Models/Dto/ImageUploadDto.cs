using System.ComponentModel.DataAnnotations;

namespace Rakais_EL3lag.Models.Dto
{
    public class ImageUploadDto
    {
        [Required]
        public string SectionName { get; set; } = null!;
       

        [Required]
        public IFormFile ImageFile { get; set; } = null!;

        public bool Active { get; set; } = true;
    }
}
