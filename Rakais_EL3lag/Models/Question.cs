using System.ComponentModel.DataAnnotations;

namespace Rakais_EL3lag.Models
{
    public class Question
    {
        public int Id { get; set; }

        [MaxLength(700)]
        public string QuestionText { get; set; } = null!;

        
        public string AnswerText { get; set; } = null!;
        [MaxLength(500)]
      
        public bool Active { get; set; } = true;
        public int SectionId { get; set; }
        public Section? Section { get; set; }
    }
}
