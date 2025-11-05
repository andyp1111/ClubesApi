using Microsoft.AspNetCore.Mvc;
using TrabajoProyecto.Data;
using ClubesApi.Models;
using ClubesApi.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ClubesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClubesController : ControllerBase
    {
        private readonly Db _db;

        public ClubesController(Db db)
        {
            _db = db;
        }

        
        [HttpGet]
        public IActionResult GetAll()
        {
            var clubes = _db.GetAllClubes();
            return Ok(clubes);
        }

        
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var club = _db.GetClubById(id); 
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
            try
            {
                var newClub = _db.CreateClub(clubDto); 
                if (newClub == null)
                {
                    return StatusCode(500, "Error interno al crear el club.");
                }

                return CreatedAtAction(nameof(GetById), new { id = newClub.ClubId }, newClub);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                return BadRequest("Ya existe un club con ese nombre.");
            }
        }

        
        [HttpDelete("{id}")]
        [Authorize] 
        public IActionResult Delete(int id)
        {
            var success = _db.DeactivateClub(id);

            if (!success)
            {
                return NotFound($"No se encontró el Club con ID {id}.");
            }

            return NoContent();
        }

       
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Update(int id, [FromBody] Club club)
        {
            if (id != club.ClubId)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del cuerpo.");
            }

            var success = _db.UpdateClub(club);

            if (success)
            {
                return NoContent();
            }

            return NotFound("No se encontró el club para actualizar.");
        }
    }
}