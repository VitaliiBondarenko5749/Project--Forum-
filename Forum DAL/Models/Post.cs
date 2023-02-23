using Catalog_of_Games_DAL.Entities;
using Authorization_DAL.Entities;
using AutoMapper.Configuration.Annotations;

namespace Forum_DAL.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }

        [Ignore]
        public List<Game> Games { get; set; } = new List<Game>();

        [Ignore]
        public User User { get; set; }

        [Ignore]
        public List<Reply> Replies { get; set; } = new List<Reply>();
    }
}
