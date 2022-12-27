namespace ChatGPTLineBot.Models
{
    public class Result
    {
        public string Id { get; set; }
        public string @Object { get; set; }
        public int Created { get; set; }
        public string Model { get; set; }
        public List<Choice> Choices { get; set; }
        public Usage Usage { get; set; }
    }
}
