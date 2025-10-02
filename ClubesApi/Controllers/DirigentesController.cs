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
    public class DirigentesController : ControllerBase
    {
        private readonly DirigenteRepository _dirigenteRepository;

        public DirigentesController(DirigenteRepository dirigenteRepository)
        {
            _dirigenteRepository = dirigenteRepository;
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] DirigenteCreateDto dirigenteDto)
        {
            if (_dirigenteRepository.DniExists(dirigenteDto.Dni))
            {
                return BadRequest($"Ya existe un dirigente con DNI {dirigenteDto.Dni}.");
            }

            var newDirigente = _dirigenteRepository.Create(dirigenteDto);

            if (newDirigente == null)
            {
                return StatusCode(500, "Error al crear el dirigente. Verifique el ClubId o la conexión a la base de datos.");
            }

            return CreatedAtAction(nameof(GetById), new { id = newDirigente.DirigenteId }, newDirigente);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var dirigentes = _dirigenteRepository.GetAll();
            return Ok(dirigentes);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var dirigente = _dirigenteRepository.GetById(id);

            if (dirigente == null)
            {
                return NotFound();
            }
            return Ok(dirigente);
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Update(int id, [FromBody] Dirigente dirigente)
        {
            if (id != dirigente.DirigenteId)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del cuerpo.");
            }


            var success = _dirigenteRepository.Update(dirigente);

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
            var success = _dirigenteRepository.Delete(id);

            if (success)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
