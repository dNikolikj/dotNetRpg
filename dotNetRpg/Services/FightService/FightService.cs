using AutoMapper;
using dotNetRpg.Data;
using dotNetRpg.Dtos.Fight;
using Microsoft.EntityFrameworkCore;

namespace dotNetRpg.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public FightService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            var response = new ServiceResponse<FightResultDto>()
            {
                Data = new FightResultDto()
            };


            try
            {
                var characters = await _context.Characters
                    .Include(w => w.Weapon)
                    .Include(s => s.Skills)
                    .Where(r => request.CharacacterIds.Contains(r.Id))
                    .ToListAsync();

                bool defeated = false;

                while (!defeated)
                {
                    foreach (Character attacker in characters)
                    {
                        var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        var opponent = opponents[new Random().Next(opponents.Count)];
                        //randomly choose one opponent ,with  new random().Next(opponents.Count), get random number to use 
                        // as an index from the opponent's list.

                        int damage = 0;
                        var attackUsed = string.Empty;

                        bool usedWeapon = new Random().Next(2) == 0;
                        if (usedWeapon)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent);

                        }
                        else
                        {
                            var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skill.Name;
                            damage = SkillAttackMethod(attacker, opponent, skill);

                        }

                        response.Data.Log
                            .Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage");

                        if (opponent.HeatPoints <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;

                            response.Data.Log.Add($"{opponent.Name} has been defeated!");
                            response.Data.Log.Add($"{attacker.Name} wins {attacker.HeatPoints} HP points left!");
                            break;
                        }
                    }
                }

                characters.ForEach(c =>
                {
                    c.Fights++;
                    c.HeatPoints = 100;
                });
                await _context.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;

            }
            return response;

        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();

            try
            {
                var attacker = await _context.Characters
                    .Include(s => s.Skills)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                var opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);
                if (skill == null)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesn't know that skill!";
                    return response;
                }

                int damage = SkillAttackMethod(attacker, opponent, skill);
                if (opponent.HeatPoints <= 0)
                {
                    response.Message = $"{opponent.Name} has been defeated!";
                }
                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto()
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHeatPoints = attacker.HeatPoints,
                    OpponentHeatPoints = opponent.HeatPoints,
                    Damage = damage
                };


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        private static int SkillAttackMethod(Character? attacker, Character? opponent, Skill? skill)
        {
            int damage = skill.Damage + (new Random().Next(attacker.Inteligence));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
            {
                opponent.HeatPoints -= damage;
            }

            return damage;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();

            try
            {
                var attacker = await _context.Characters
                      .Include(c => c.Weapon)
                      .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                var opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                int damage = DoWeaponAttack(attacker, opponent);
                if (opponent.HeatPoints <= 0)
                {
                    response.Message = $"{opponent.Name} has been defeated!";
                }

                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHeatPoints = attacker.HeatPoints,
                    OpponentHeatPoints = opponent.HeatPoints,
                    Damage = damage,
                };


            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                return response;

            }

            return response;
        }

        private static int DoWeaponAttack(Character? attacker, Character? opponent)
        {
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
            {
                opponent.HeatPoints -= damage;
            }

            return damage;
        }

        public async Task<ServiceResponse<List<HighScoreDto>>> GetHighScore()
        {
            

            var characters = await _context.Characters
                .Where(f => f.Fights > 0)
                .OrderByDescending(c => c.Victories)
                .ThenBy(c => c.Defeats)
                .ToListAsync();

            var response = new ServiceResponse<List<HighScoreDto>>()
            {
                Data = characters.Select(c=> _mapper.Map<HighScoreDto>(c)).ToList()
            };
            return response;

        }
    }
}
