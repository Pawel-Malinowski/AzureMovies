using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Movies.Models;

namespace Movies.Repository
{
    public interface IDataRepository
    {
        Task<Actor> AddActor(string firstName, string lastName, DateTime birthDate);

        Task<Actor> GetActor(int actorId);

        Task<bool> DeleteActor(int actorId);

        Task<IReadOnlyList<Actor>> GetActors();
        
        Task<IReadOnlyList<Movie>> GetActorFilmography(int actorId);

        Task<bool> Link(int actorId, int movieId);

        Task<Movie> GetMovie(int movieId);

        Task<bool> DeleteMovie(int movieId);

        Task<IReadOnlyList<Actor>> GetActorsFromMovie(int movieId);

        Task<IReadOnlyList<Movie>> GetMovies();

        Task<IReadOnlyList<Movie>> GetMovies(Expression<Func<Movie, bool>> predicate);
        Task<Movie> AddMovie(string title, string genre, int year, int[] actorIds);
    }
}