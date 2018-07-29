using Movies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Movies.Repository
{
    public class DataRepository : IDataRepository
    {
        private readonly DataContext _dataContext;

        public DataRepository(DataContext dataContext)
        {
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
            await _dataContext.Actors.AddAsync(actor);
            await _dataContext.SaveChangesAsync();

            return actor;
        }

        public async Task<bool> DeleteActor(int actorId)
        {
            Actor actor = await GetActor(actorId);

            if (actor == null)
                return false;

            _dataContext.Actors.Remove(actor);
            await _dataContext.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Actor>> GetActors()
        {
            return await _dataContext.Actors.Include(x => x.MovieRoles).ToListAsync();
        }

        public Task<Actor> GetActor(int actorId)
        {
            return _dataContext.Actors.FindAsync(actorId);
        }

        public async Task<IReadOnlyList<Movie>> GetActorFilmography(int actorId)
        {
            return await _dataContext.Movies.Include(x => x.MovieRoles)
                                            .Where(x => x.MovieRoles.Any(r => r.ActorId == actorId))
                                            .ToListAsync();
        }

        public async Task<bool> Link(int actorId, int movieId)
        {
            bool actorExists = await _dataContext.Actors.AnyAsync(x => x.Id == actorId);

            if (!actorExists)
                return false;

            bool movieExists = await _dataContext.Movies.AnyAsync(x => x.Id == movieId);

            if (!movieExists)
                return false;

            bool alreadyLinked = await _dataContext.MovieRoles.AnyAsync(x => x.MovieId == movieId && x.ActorId == actorId);

            if (!alreadyLinked)
            {
                await _dataContext.MovieRoles.AddAsync(new MovieRole() { ActorId = actorId, MovieId = movieId });
                await _dataContext.SaveChangesAsync();
            }
            return true;
        }

        public Task<Movie> GetMovie(int movieId)
        {
            return _dataContext.Movies.FindAsync(movieId);
        }

        public async Task<bool> DeleteMovie(int movieId)
        {
            Movie movie = await GetMovie(movieId);

            if (movie == null)
                return false;

            _dataContext.Movies.Remove(movie);
            await _dataContext.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Actor>> GetActorsFromMovie(int movieId)
        {
            return await _dataContext.Actors.Include(x => x.MovieRoles)
                                            .Where(x => x.MovieRoles.Any(r => r.MovieId == movieId))
                                            .ToListAsync();
        }

        public async Task<IReadOnlyList<Movie>> GetMovies()
        {
            return await _dataContext.Movies.Include(x => x.MovieRoles).ToListAsync();
        }

        public async Task<IReadOnlyList<Movie>> GetMovies(Expression<Func<Movie, bool>> predicate)
        {
            return await _dataContext.Movies.Where(predicate).ToListAsync();
        }

        public async Task<Movie> AddMovie(string title, string genre, int year, int[] actorIds)
        {
            try
            {
                var newMovie = new Movie() { Title = title, Year = year, Genre = genre };

                await _dataContext.Movies.AddAsync(newMovie);

                foreach (var actorId in actorIds)
                {
                    await _dataContext.MovieRoles.AddAsync(new MovieRole() { Movie = newMovie, ActorId = actorId });
                }
                await _dataContext.SaveChangesAsync();

                return newMovie;

            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
