using dotNetRpg.Dtos.Character;
using dotNetRpg.Dtos.Weapon;

namespace dotNetRpg.Services.WeaponService
{
    public interface IWeaponService
    {
        Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);
    }
}
