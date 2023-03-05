namespace Forum_DAL.Repositories.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IPostRepository PostRepository { get; }
        IReplyRepository ReplyRepository { get; }
        IReplyToReplyRepository ReplyToReplyRepository { get; }
        IGameRepository GameRepository { get; }
        IPostReplyRepository PostReplyRepository { get; }
        IPostGameRepository PostGameRepository { get; }
        ILikedReplyRepository LikedReplyRepository { get; }
        IReplyToReply_ReplyRepository ReplyToReply_ReplyRepository { get; }
        ILikedReplyToReplyRepository LikedReplyToReplyRepository { get; }

        void Commit();
        new void Dispose();
    }
}
