using Web.Models.RequestModels;

namespace Web.Models.CreateModels;

public class GameCreate
{
    public required GameRequest Game { get; set; }
    public required Guid[] Genres { get; set; }
    public required Guid[] Platforms { get; set; }
    public required Guid Publisher { get; set; }
    
}