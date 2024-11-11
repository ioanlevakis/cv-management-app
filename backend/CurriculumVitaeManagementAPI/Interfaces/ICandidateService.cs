using CurriculumVitaeManagementAPI.Models;

namespace CurriculumVitaeManagementAPI.Interfaces
{
    public interface ICandidateService
    {
        Task<Candidate> AddCandidateAsync(Candidate candidate);
        Task EditCandidateAsync(int id, Candidate candidate);
        Task<Candidate> GetCandidateAsync(int id);
        Task RemoveCandidate(int id);
    }
}