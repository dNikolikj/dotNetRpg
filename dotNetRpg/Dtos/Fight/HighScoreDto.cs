namespace dotNetRpg.Dtos.Fight
{
    public class HighScoreDto
    {

        public int CharacterId { get; set; }
        public string Name { get; set; } = string.Empty;

        public int Fights { get; set; }
        public int Victories { get; set; }
        public int Defeats { get; set; }
    }
}
