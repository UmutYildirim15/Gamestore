namespace Data.Entities
{
    public class Genre
    {
        public virtual Guid Id { get; set; }
        public virtual required string Name { get; set; }
        public virtual Guid? ParentGenreId { get; set; }
        public virtual Genre? ParentGenre { get; set; }
        public virtual List<Genre>? SubGenres { get; set; }
        public virtual string? Description { get; set; }
    }
}
