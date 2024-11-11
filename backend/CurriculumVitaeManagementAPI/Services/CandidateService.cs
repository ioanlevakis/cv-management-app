using CurriculumVitaeManagementAPI.AppDbContext;
using CurriculumVitaeManagementAPI.Interfaces;
using CurriculumVitaeManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CurriculumVitaeManagementAPI.Services
{
    public class CandidateService(DatabaseContext context, IDegreeService degreeService) : ICandidateService
    {

        private async Task<byte[]> SetCandidateCVFile(IFormFile file)
        {
            if (file != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            else
                return null;
        }

        
        public async Task<Candidate> AddCandidateAsync(Candidate candidate)
        {
            if (candidate.CVFile != null)
                candidate.CVBlob = await SetCandidateCVFile(candidate.CVFile);

            try
            {
                await context.Candidates.AddAsync(candidate);

                var candidateDegree = candidate.Degree;

                if(candidateDegree != null)
                {
                    await degreeService.AddDegreesAsync(candidateDegree);
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error has occured while trying to add new Candidate ({ex.Message})");
            }

            return candidate;
        }

        public async Task<Candidate> GetCandidateAsync(int id)
        {
            var candidate = await context.Candidates.Include(c => c.Degree).FirstOrDefaultAsync(c => c.Id == id); ;

            if (candidate == null)
            {
                throw new Exception($"The requested Candidate was not found (id: {id})");
            }

            return candidate;
        }

        public async Task EditCandidateAsync(int id, Candidate editedCandidate)
        {
            {
                var existingCandidate = await GetCandidateAsync(id);

                if (existingCandidate == null)
                {
                    throw new Exception($"There is no Candidate with id: {id}");
                }

                existingCandidate.FirstName = editedCandidate.FirstName;
                existingCandidate.LastName = editedCandidate.LastName;
                existingCandidate.Email = editedCandidate.Email;
                existingCandidate.Mobile = editedCandidate.Mobile;
                existingCandidate.CVBlob = await SetCandidateCVFile(editedCandidate.CVFile); ;

                if (editedCandidate.Degree == null)
                {
                    if (existingCandidate.Degree != null)
                    {
                        context.Degrees.RemoveRange(existingCandidate.Degree);
                        existingCandidate.Degree = null;
                    }
                }
                else
                {
                    if (existingCandidate.Degree == null)
                    {
                        existingCandidate.Degree = editedCandidate.Degree;
                    }
                    else
                    {
                        existingCandidate.Degree = editedCandidate.Degree;
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveCandidate(int id)
        {
            try
            {
                var candidate = await GetCandidateAsync(id);

                var candidateDegree = candidate.Degree;

                context.Candidates.Remove(candidate);
                await context.SaveChangesAsync();

                if (candidateDegree != null)
                {
                    await degreeService.RemoveDegreeOfRemovedCandidate(candidateDegree);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }
    }
}
