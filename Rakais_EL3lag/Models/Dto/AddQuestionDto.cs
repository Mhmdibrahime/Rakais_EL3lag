namespace Rakais_EL3lag.Models.Dto
{
    public class AddQuestionDto
    {
        public string QuestionText { get; set; } = null!;
        public string AnswerText { get; set; } = null!;
        public string SectionName { get; set; }
    }
}
