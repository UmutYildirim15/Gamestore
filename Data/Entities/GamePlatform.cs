namespace Data.Entities
{
    public class GamePlatform
    {
        public virtual required Guid GameId { get; set; }
        public virtual Game? Game { get; set; } 
        public virtual required Guid PlatformId { get; set; }
        public virtual Platform? Platform { get; set; }
    }
}
