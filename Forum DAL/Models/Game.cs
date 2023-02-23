using AutoMapper.Configuration.Annotations;

namespace Catalog_of_Games_DAL.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public string GmName { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Publisher { get; set; }
        public float Rating { get; set; }
        public string GmDescription { get; set; }
        public string MainImg { get; set; }
        public string? Img1 { get; set; }
        public string? Img2 { get; set; }
        public string? Img3 { get; set; } 

        [Ignore]
        Publisher Publisher1 { get; set; }
    }
}
