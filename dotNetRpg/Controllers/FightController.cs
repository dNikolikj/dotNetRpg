﻿using dotNetRpg.Dtos.Fight;
using dotNetRpg.Services.FightService;
using Microsoft.AspNetCore.Mvc;

namespace dotNetRpg.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FightController : ControllerBase
    {
        private readonly IFightService _fightService;

        public FightController(IFightService fightService)
        {
            _fightService = fightService;
        }

        [HttpPost("weapon")]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> WeaponAttack(WeaponAttackDto request)
        {
            return Ok(await _fightService.WeaponAttack(request));
        }
        [HttpPost("skill")]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> SkillAttack(SkillAttackDto request)
        {
            return Ok(await _fightService.SkillAttack(request));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<FightResultDto>>> Fight(FightRequestDto request)
        {
            return Ok(await _fightService.Fight(request));
        }
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<HighScoreDto>>>> GetHighScore()
        {
            return Ok(await _fightService.GetHighScore());
        }
    }
}
