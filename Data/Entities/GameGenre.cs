using System;

namespace Data.Entities
{
    public class GameGenre
    {
        public virtual required Guid GameId { get; set; }
        public virtual Game? Game { get; set; }
        public virtual required Guid GenreId { get; set; }
        public virtual Genre? Genre { get; set; }
    }
}
