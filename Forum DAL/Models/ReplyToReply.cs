using Authorization_DAL.Entities;
using AutoMapper.Configuration.Annotations;

namespace Forum_DAL.Models
{
    public class ReplyToReply
    {
        public int Id { get; set; }
        public int ReplyId { get; set; }
        public int PostedBy { get; set; }
        public string Content { get; set; }
        public DateTime WhenReplied { get; set; }

        [Ignore]
        public User PostedUser { get; set; }
    }
}
