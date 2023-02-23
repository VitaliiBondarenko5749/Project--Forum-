namespace Forum_DAL.Repositories.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IPostRepository PostRepository { get; }
        IReplyRepository ReplyRepository { get; }
        IReplyToReplyRepository ReplyToReplyRepository { get; }
        IGameRepository GameRepository { get; }

        void Commit();
        new void Dispose();
    }
}
