using Microsoft.AspNetCore.Mvc;
using ClubesApi.DTOs;
using ClubesApi.Repositories;
using ClubesApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace ClubesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class ClubesController : ControllerBase
    {
        private readonly ClubRepository _clubRepository;

        public ClubesController(ClubRepository clubRepository)
        {
            _clubRepository = clubRepository;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var clubes = _clubRepository.GetAll();
            return Ok(clubes);
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var club = _clubRepository.GetById(id);

            if (club == null)
            {
                return NotFound();
            }
            return Ok(club);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] ClubCreateDto clubDto)
        {
            var newClub = _clubRepository.Create(clubDto);

            if (newClub == null)
            {
                return StatusCode(500, "Error interno al crear el club.");
            }

            return CreatedAtAction(nameof(GetById), new { id = newClub.ClubId }, newClub);
        }
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Update(int id, [FromBody] Club club)
        {
            if (id != club.ClubId)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del cuerpo.");
            }

            var success = _clubRepository.Update(club);

            if (success)
            {
                return NoContent();
            }

            return NotFound();
        }
        [HttpDelete("{id}")]
        [Authorize] 
        public IActionResult Delete(int id)
        {
            var success = _clubRepository.Deactivate(id);

            if (success)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}