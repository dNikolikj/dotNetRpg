using dotNetRpg.Dtos.Skill;
using dotNetRpg.Dtos.Weapon;

namespace dotNetRpg.Dtos.Character
{
    public class GetCharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Frodo";
        public int HeatPoints { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defense { get; set; } = 10;
        public int Inteligence { get; set; } = 10;

        public RpgClass Class { get; set; } = RpgClass.Knight;

        public GetWeaponDto Weapon { get; set; }
        public List<GetSkillDto> Skills { get; set; }
        public int Fights { get; set; }
        public int Victories { get; set; }
        public int Defeats { get; set; }
    }
}
