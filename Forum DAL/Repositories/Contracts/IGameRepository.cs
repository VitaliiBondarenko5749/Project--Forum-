using Catalog_of_Games_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum_DAL.Repositories.Contracts
{
    public interface IGameRepository : IGenericRepository<Game> 
    {
        Task<IEnumerable<Game>> GetGamesForPostAsync(int gameId);
    }
}
