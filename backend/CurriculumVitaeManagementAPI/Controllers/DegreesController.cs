using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CurriculumVitaeManagementAPI.AppDbContext;
using CurriculumVitaeManagementAPI.Models;
using CurriculumVitaeManagementAPI.Interfaces;
using CurriculumVitaeManagementAPI.Services;

namespace CurriculumVitaeManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DegreesController(IDegreeService degreeService, ISessionValidationService validationService) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCsrfToken()
        {
            var token = validationService.GetCsrfToken();
            return Ok(token);
        }

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Degree>> PostDegree([FromBody] List<Degree> degrees)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await degreeService.AddDegreesAsync(degrees);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return CreatedAtAction("PostDegree", new { Degrees = degrees });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Degree>> PostDegreee([FromBody] Degree degree)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await degreeService.AddDegreeAsync(degree);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return CreatedAtAction("PostDegree", new { Degrees = degree });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Degree>> GetDegree(int id)
        {
            try
            {
                return await degreeService.GetDegreeAsync(id);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("choose")]
        public async Task<ActionResult<Degree>> GetDegreeFromLinkedCandidates([FromQuery] string degree)
        {
            try
            {
                return await degreeService.GetDegreeFromLinkedCandidatesAsync(degree);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PutDegree(int id, [FromBody] Degree degree)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await degreeService.EditDegreeAsync(id, degree);
                return NoContent();
            }
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDegree(int id)
        {
            try
            {
                await degreeService.RemoveDegree(id);
                return NoContent();
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }  
        
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNonLinkedDegrees()
        {
            try
            {
                var messages = await degreeService.RemoveNonLinkedDegrees();

                if (messages.Any())
                {
                    return Ok(new { Warnings = messages });
                }

                return NoContent();
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }        
    }
}
