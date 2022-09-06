using AutoMapper;
using dotNetRpg.Dtos.Character;
using dotNetRpg.Dtos.Fight;
using dotNetRpg.Dtos.Skill;
using dotNetRpg.Dtos.Weapon;

namespace dotNetRpg
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();
            CreateMap<UpdateCharacterDto, Character>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<Skill, GetSkillDto>();
            CreateMap<Character, HighScoreDto>();

        }
    }
}
