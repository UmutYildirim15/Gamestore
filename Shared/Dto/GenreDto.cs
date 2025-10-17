namespace Shared.Dto
{
    public class GenreResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Guid? ParentGenreId { get; set; }
    }
}
