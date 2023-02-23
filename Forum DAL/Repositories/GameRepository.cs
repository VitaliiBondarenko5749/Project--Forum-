using Catalog_of_Games_DAL.Entities;
using Dapper;
using Forum_DAL.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Forum_DAL.Repositories
{
    public class GameRepository : GenericRepository<Game>, IGameRepository
    {
        public GameRepository(SqlConnection sqlConnection, IDbTransaction dbTransaction)
            : base(sqlConnection, dbTransaction, "gamecatalog.Games") { }

        public async Task<IEnumerable<Game>> GetGamesForPostAsync(int gameId)
        {

            return null;
        }
    }
}
