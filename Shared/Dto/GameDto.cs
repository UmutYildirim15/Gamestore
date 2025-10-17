namespace Business.Services.Dto
{
    public class GameResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Key { get; set; }
        public string? Description { get; set; }
        public required double Price { get; set; }
        public required int UnitInStock { get; set; }
        public required int Discount { get; set; }
    }
    
    public class GameCreateDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Key { get; set; }
        public double Price { get; set; }
        public int UnitInStock { get; set; }
        public int Discount { get; set; }
        public IEnumerable<Guid>? GenreIds { get; set; }
        public IEnumerable<Guid>? PlatformIds { get; set; }
        public Guid PublisherId { get; set; }
    }

    public class GameUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double Price { get; set; }
        public int UnitInStock { get; set; }
        public int Discount { get; set; }
        public IEnumerable<Guid> GenreIds { get; set; } = new List<Guid>();
        public IEnumerable<Guid> PlatformIds { get; set; } = new List<Guid>();
        public Guid PublisherId { get; set; }
    }

}