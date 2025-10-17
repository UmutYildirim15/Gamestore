using Data.Repositories.Abstract;

namespace Business.Facades;

public class GameRepositoryFacade(
    IGameRepository gameRepository,
    IPlatformRepository platformRepository,
    IGenreRepository genreRepository,
    IGamePlatformRepository gamePlatformRepository,
    IGameGenreRepository gameGenreRepository,
    IPublisherRepository publisherRepository,
    IOrderRepository orderRepository)
{
    public IGameRepository GameRepository { get; } = gameRepository;
    public IPlatformRepository PlatformRepository { get; } = platformRepository;
    public IGenreRepository GenreRepository { get; } = genreRepository;
    public IGamePlatformRepository GamePlatformRepository { get; } = gamePlatformRepository;
    public IGameGenreRepository GameGenreRepository { get; } = gameGenreRepository;
    public IPublisherRepository PublisherRepository { get; } = publisherRepository;
    public IOrderRepository OrderRepository { get; } = orderRepository;
}