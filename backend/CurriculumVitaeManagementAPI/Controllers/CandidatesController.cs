using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CurriculumVitaeManagementAPI.AppDbContext;
using CurriculumVitaeManagementAPI.Models;
using CurriculumVitaeManagementAPI.Services;
using CurriculumVitaeManagementAPI.Interfaces;

namespace CurriculumVitaeManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController(ICandidateService candidateService, ISessionValidationService validationService) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCsrfToken()
        {
            var token = validationService.GetCsrfToken();
            return Ok(token);
        }

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Candidate>> PostCandidate([FromForm] Candidate candidate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await candidateService.AddCandidateAsync(candidate);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return CreatedAtAction("PostCandidate", new { Candidate = candidate });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Candidate>> GetCandidate(int id)
        {
            try
            {
                return await candidateService.GetCandidateAsync(id);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }        

        [HttpPut("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PutCandidate(int id, [FromForm] Candidate candidate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await candidateService.EditCandidateAsync(id, candidate);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCandidate(int id)
        {
            try
            {
                await candidateService.RemoveCandidate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
