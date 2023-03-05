using Forum_DAL.Repositories.Contracts;
using System.Data;

namespace Forum_DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IDbTransaction transaction;

        public UnitOfWork(IPostRepository postRepository, IReplyRepository replyRepository,
            IReplyToReplyRepository replyToRepliesRepository, IGameRepository gameRepository,
            IPostReplyRepository postReplyRepository, IPostGameRepository postGameRepository,
            ILikedReplyRepository likedReplyRepository, IReplyToReply_ReplyRepository replyToReply_ReplyRepository,
            ILikedReplyToReplyRepository likedReplyToReplyRepository, IDbTransaction transaction)
        {
            PostRepository = postRepository;
            ReplyRepository = replyRepository;
            ReplyToReplyRepository = replyToRepliesRepository;
            GameRepository = gameRepository;
            PostReplyRepository = postReplyRepository;
            PostGameRepository = postGameRepository;
            LikedReplyRepository = likedReplyRepository;
            ReplyToReply_ReplyRepository = replyToReply_ReplyRepository;
            LikedReplyToReplyRepository = likedReplyToReplyRepository;

            this.transaction = transaction;
        }

        public IPostRepository PostRepository { get; }
        public IReplyRepository ReplyRepository { get; }
        public IReplyToReplyRepository ReplyToReplyRepository { get; }
        public IGameRepository GameRepository { get; }
        public IPostReplyRepository PostReplyRepository { get; }
        public IPostGameRepository PostGameRepository { get; }
        public ILikedReplyRepository LikedReplyRepository { get; }
        public IReplyToReply_ReplyRepository ReplyToReply_ReplyRepository { get; }
        public ILikedReplyToReplyRepository LikedReplyToReplyRepository { get; }

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
