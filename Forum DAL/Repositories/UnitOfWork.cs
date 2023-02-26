using Forum_DAL.Repositories.Contracts;
using System.Data;

namespace Forum_DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IDbTransaction transaction;

        public UnitOfWork(IPostRepository postRepository, IReplyRepository replyRepository,
            IReplyToReplyRepository replyToRepliesRepository, IGameRepository gameRepository,
            IDbTransaction transaction)
        {
            PostRepository = postRepository;
            ReplyRepository = replyRepository;
            ReplyToReplyRepository = replyToRepliesRepository;
            GameRepository = gameRepository;

            this.transaction = transaction;
        }

        public IPostRepository PostRepository { get; }
        public IReplyRepository ReplyRepository { get; }
        public IReplyToReplyRepository ReplyToReplyRepository { get; }
        public IGameRepository GameRepository { get; }

        public void Commit()
        {
            try
            {
                transaction.Commit();
            }
            catch(Exception ex)
            {
                transaction.Rollback();

                Console.WriteLine(ex.Message);
            }
        }

        public void Dispose()
        {
            transaction.Connection?.Close();
            transaction.Connection?.Dispose();
            transaction.Dispose();
        }
    }
}
