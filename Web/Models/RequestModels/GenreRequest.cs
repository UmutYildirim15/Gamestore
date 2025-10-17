namespace Web.Models.RequestModels 
{ 
    public class GenreRequest
    {
        public required string Name { get; set; }
        public string? ParentGenreName { get; set; }
        public Guid? ParentGenreId { get; set; }
    }
}