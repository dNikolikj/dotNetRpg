namespace dotNetRpg.Dtos.Fight
{
    public class AttackResultDto
    {
        public string Attacker { get; set; } = string.Empty;
        public string Opponent { get; set; } = string.Empty;

        public int AttackerHeatPoints { get; set; }
        public int OpponentHeatPoints { get; set; }
        public int Damage { get; set; }
    }
}
