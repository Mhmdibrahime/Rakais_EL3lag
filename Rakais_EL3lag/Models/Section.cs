using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace Rakais_EL3lag.Models
{
    public class Section
    {
        public int Id { get; set; }
        [MaxLength(500)]
        public string Name { get; set; } = null!;

        public ICollection<Image> Images { get; set; } = new List<Image>();
       
    }


}
