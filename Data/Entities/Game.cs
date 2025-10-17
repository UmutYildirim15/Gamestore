using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Game
    {
        public virtual required Guid Id { get; set; }
        public virtual required string Name { get; set; }
        public virtual required string Key { get; set; }
        public virtual string? Description { get; set; }
        public virtual required double Price { get; set; }
        public virtual required int UnitInStock { get; set; }
        public virtual required int Discount { get; set; }
        public virtual required Guid PublisherId { get; set; }

        public virtual List<GameGenre>? Genres { get; set; }
        public virtual List<GamePlatform>? Platforms { get; set; }
        
        public virtual int? ViewCount { get; set; }
        public virtual int? CommentCount { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
    }
}
