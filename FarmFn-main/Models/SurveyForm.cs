namespace Farm.Models
{
    public class SurveyForm
    {
        public int Id { get; set; }
        public int CageId { get; set; }
        public int ChickenCount { get; set; }
        public DateTime SurveyDate { get; set; }
        public Cage Cage { get; set; }
    }
}