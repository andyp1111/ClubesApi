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
    public class SociosController : ControllerBase
    {
        private readonly SocioRepository _socioRepository;

        public SociosController(SocioRepository socioRepository)
        {
            _socioRepository = socioRepository;
        }
        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] SocioCreateDto socioDto)
        {
            if (_socioRepository.DniExists(socioDto.Dni))
            {
                return BadRequest($"Ya existe un socio con DNI {socioDto.Dni}.");
            }

            var newSocio = _socioRepository.Create(socioDto);

            if (newSocio == null)
            {
                return StatusCode(500, "Error interno al crear el socio. Verifique el ClubId.");
            }

            return CreatedAtAction(nameof(GetById), new { id = newSocio.SocioId }, newSocio);
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var socios = _socioRepository.GetAll();
            return Ok(socios);
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var socio = _socioRepository.GetById(id);

            if (socio == null)
            {
                return NotFound();
            }
            return Ok(socio);
        }
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Update(int id, [FromBody] Socio socio)
        {
            if (id != socio.SocioId)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del cuerpo.");
            }

            var success = _socioRepository.Update(socio);

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
            var success = _socioRepository.Delete(id);

            if (success)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
