using CurriculumVitaeManagementAPI.Models;

namespace CurriculumVitaeManagementAPI.Interfaces
{
    public interface IDegreeService
    {
        Task<Degree> AddDegreeAsync(Degree degree);
        Task<List<Degree>> AddDegreesAsync(List<Degree> degrees);
        Task EditDegreeAsync(int id, Degree degree);
        Task<Degree> GetDegreeAsync(int id);
        Task<Degree> GetDegreeFromLinkedCandidatesAsync(string degreeName);
        Task<List<string>> RemoveDegree(int id);
        Task<List<string>> RemoveNonLinkedDegrees();
        Task RemoveDegreeOfRemovedCandidate(IEnumerable<Degree> RemovedCandidateDegree);
    }
}