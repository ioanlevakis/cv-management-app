using CurriculumVitaeManagementAPI.AppDbContext;
using CurriculumVitaeManagementAPI.Interfaces;
using CurriculumVitaeManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace CurriculumVitaeManagementAPI.Services
{
    public class DegreeService(DatabaseContext context) : IDegreeService
    {
        private static readonly List<string> warningMessages = new ();

        public async Task<List<Degree>> AddDegreesAsync(List<Degree> degrees)
        {
            try
            {
                await context.Degrees.AddRangeAsync(degrees);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error has occured while trying to add new Degree ({ex.Message})");
            }

            return degrees;
        }


        public async Task<Degree> AddDegreeAsync(Degree degree)
        {
            try
            {
                await context.Degrees.AddAsync(degree);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error has occured while trying to add new Degree ({ex.Message})");
            }

            return degree;
        }

        public async Task<Degree> GetDegreeAsync(int id)
        {
            var degree = await context.Degrees.FindAsync(id);

            if (degree == null)
            {
                throw new Exception($"The requested Degree was not found (id: {id})");
            }

            return degree;
        }

        public async Task EditDegreeAsync(int id, Degree degree)
        {
            if (id != degree.Id)
            {
                throw new Exception("The ids provided do not match");
            }

            var degreeExists = context.Degrees.Any(d => d.Id == id);

            if (!degreeExists)
                throw new Exception($"There is no Degree with id: {id}");

            context.Entry(degree).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"There was an error while saving the changes to the Database ({ex.Message})");
            }
        }


        public async Task<List<string>> RemoveDegree(int id)
        {
            var isLinked = await IsDegreeLinkedToCandidate(id);                        

            if (isLinked)
            {
                warningMessages.Add($"Cannot delete a Degree linked to a Candidate (id: {id})");
                return warningMessages;
            }

            try
            {

                var degree = await GetDegreeAsync(id);
                context.Degrees.Remove(degree);
                await context.SaveChangesAsync();
                return warningMessages;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }        


        public async Task<List<string>> RemoveNonLinkedDegrees()
        {
            var allDegreeIds = await GetAllDegreeIdsAsync();

            List<string> messages = new List<string>();

            foreach (var degreeId in allDegreeIds)
            {
                messages = await RemoveDegree(degreeId);
            }

            return messages;
        }
        
        public async Task RemoveDegreeOfRemovedCandidate(IEnumerable<Degree> RemovedCandidateDegree)
        {
            foreach (var degree in RemovedCandidateDegree)
            {
                await RemoveDegree(degree.Id);
            }
        }

        public async Task<Degree> GetDegreeFromLinkedCandidatesAsync(string degreeName)
        {
            var candidates = await context.Candidates.Include(c => c.Degree).ToListAsync();

            foreach (var candidate in candidates)
            {
                if (candidate.Degree != null)
                {
                    foreach (var degree in candidate.Degree)
                    {
                        if (degree.Name == degreeName)
                        {
                            return degree;
                        }
                    }
                }
            }

            return null;
        }

        private async Task<List<int>> GetAllDegreeIdsAsync()
        {
            var degreeIds = await context.Degrees.Select(d => d.Id).ToListAsync();
            return degreeIds;
        }

        private async Task<bool> IsDegreeLinkedToCandidate(int id)
        {
            return await context.Candidates.AnyAsync(c => c.Degree != null && c.Degree.Any(d => d.Id == id));
        }
    }
}
