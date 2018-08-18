using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Movies.Models;
using Movies.Repository;

namespace Movies.Services
{
    public class ActorsService
    {
        private readonly Repository<Actor> _repository;
        private readonly DataContext _dataContext;

        public ActorsService(Repository<Actor> repository, DataContext dataContext)
        {
            _repository = repository;
            _dataContext = dataContext;
        }

        public async Task<Actor> AddActor(string firstName, string lastName, DateTime birthDate)
        {
            var actor = new Actor()
            {
                FirstName = firstName,
                LastName = lastName,
                BirthDate = birthDate
            };
            await _repository.AddAsync(actor);
            await _dataContext.SaveChangesAsync();

            return actor;
        }

        public async Task<bool> DeleteActor(int actorId)
        {
            Actor actor = await _repository.GetAsync(actorId);

            if (actor == null)
                return false;

            _dataContext.Remove(actor);
            await _dataContext.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Actor>> GetActors()
        {
            return await _repository.GetAll().Include(x => x.MovieRoles).ToListAsync();
        }

    }
}
