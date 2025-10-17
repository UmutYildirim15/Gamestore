namespace Business.Services.Resources;

public static class ErrorMessages
{
    public const string NoDescription = "No description available";
    public const string NoName = "No name";
    public const string NoKey = "No key";
    public const string GameNotFoundById = "Game with ID {0} not found.";
    public const string PlatformNotFoundById = "Platform with the specified ID was not found.";
    public const string GameNotFoundByKey = "Game with key {0} not found.";
    public const string GenreNotFound = "Genre was not found.";
    public const string PlatformNotFound = "Platform was not found.";
    public const string NullGenre = "Genre name cannot be null or empty.";
    public const string NullPlatform = "Platform type cannot be null or empty.";
    public const string CompanyNameRequired = "Company name is required.";
    public const string CompanyUnique = "Company name must be unique.";
    public const string PublisherNotFound = "Publisher not found.";
    public const string NullOrWhitespacePublisher = "There are null or empty parameters.";
    public const string GameOutOfStock = "There are no games in stock.";
    public const string UnsuccessfulPayment = "Unexpected payment error.";
    public const string UserNotFound = "User not found.";
    public const string RoleNotFound = "Role not found.";
    public const string CartNotFound = "Cart is empty.";
    public const string OrderNotFound = "Order not found.";
    public const string OrderDetailNotFound = "Order details are empty.";
}
