using AutoMapper;
using dotNetRpg.Data;
using dotNetRpg.Dtos.Character;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace dotNetRpg.Services.CharacterServiceFolder
{
    public class CharacterService : ICharacterService
    {

        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User
            .FindFirstValue(ClaimTypes.NameIdentifier));
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            Character character = _mapper.Map<Character>(newCharacter);
            character.User = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
            _dataContext.Characters.Add(character);
            await _dataContext.SaveChangesAsync();


            serviceResponse.Data = await _dataContext.Characters.
                Where(c => c.User.Id == GetUserId())
                .Select(c => _mapper.Map<GetCharacterDto>(c))
                .ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                Character dBCharacter = await _dataContext.Characters
                    .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
                if (dBCharacter != null)
                {
                    _dataContext.Characters.Remove(dBCharacter);
                    await _dataContext.SaveChangesAsync();
                    response.Data = await _dataContext.Characters
                        .Where(c => c.User.Id == GetUserId())
                        .Select(c => _mapper.Map<GetCharacterDto>(c))
                  .ToListAsync();
                }

                else
                {
                    response.Success = false;
                    response.Message = "Character not found!";
                }


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();

            var dbCharacters = await _dataContext.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .Where(c => c.User.Id == GetUserId()).ToListAsync();
            response.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            var dBCharacter = await _dataContext.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(u => u.Id == id && u.User.Id == GetUserId());
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dBCharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {
                var dBCharacter = await _dataContext.Characters
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                //_mapper.Map<Character>(updatedCharacter);
                //_mapper.Map(updateCharacter,character) the same as the line up
                if (dBCharacter.User.Id == GetUserId())
                {

                    dBCharacter.Name = updatedCharacter.Name;
                    dBCharacter.HeatPoints = updatedCharacter.HeatPoints;
                    dBCharacter.Strength = updatedCharacter.Strength;
                    dBCharacter.Defense = updatedCharacter.Defense;
                    dBCharacter.Inteligence = updatedCharacter.Inteligence;
                    dBCharacter.Class = updatedCharacter.Class;

                    await _dataContext.SaveChangesAsync();

                    response.Data = _mapper.Map<GetCharacterDto>(dBCharacter);
                }
                else
                {
                    response.Success = false;
                    response.Message = "Character not found!";

                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            var response = new ServiceResponse<GetCharacterDto>();

            try
            {
                Character character = await _dataContext.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId
                && c.User.Id == GetUserId());

                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found!";
                    return response;
                }
                var skill = await _dataContext.Skills.FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);

                if (skill == null)
                {
                    response.Success = false;
                    response.Message = "Skill not found!";
                    return response;
                }
                character.Skills.Add(skill);
                await _dataContext.SaveChangesAsync();
                response.Data = _mapper.Map<GetCharacterDto>(character);
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
