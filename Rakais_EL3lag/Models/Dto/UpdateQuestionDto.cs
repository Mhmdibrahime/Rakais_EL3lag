using System.ComponentModel.DataAnnotations;

namespace Rakais_EL3lag.Models.Dto
{
    public class UpdateQuestionDto
    {
        [MaxLength(700)]
        public string QuestionText { get; set; } = null!;


        public string AnswerText { get; set; } = null!;
    }
}
