using Business.Services.Dto;
using Shared.Dto;
using Web.Models.CreateModels;
using Web.Models.RequestModels;
using PaymentCardModel = Web.Models.CreateModels.PaymentCardModel;

namespace Test.Helpers;

public static class WebTestHelper
{
    public static List<GameResponse> CreateGameResponseList(int count)
    {
        var games = new List<GameResponse>();
        for (int i = 0; i < count; i++)
        {
            games.Add(new GameResponse
            {
                Id = Guid.NewGuid(),
                Name = $"Game {i + 1}",
                Key = $"gamekey{i + 1}",
                Price = 0,
                UnitInStock = 0,
                Discount = 0
            });
        }
        return games;
    }

    public static GameResponse CreateSingleGameResponse()
    {
        return new GameResponse
        {
            Id = Guid.NewGuid(),
            Name = "Existing Game",
            Key = "gamekeyexisting",
            Price = 0,
            UnitInStock = 0,
            Discount = 0
        };
    }

    public static GameCreate CreateGameCreateRequest(Guid publisherId, string key = "gamekey", string name = "New Game",
        string description = "Description", double price = 100, int unitInStock = 10, int discount = 15)
    {
        return new GameCreate
        {
            Game = new GameRequest
            {
                Name = name,
                Description = description,
                Price = price,
                UnitInStock = unitInStock,
                Discount = discount,
                Key = key
            },
            Genres = new Guid[]
            {
            },
            Platforms = new Guid[]
            {
            },
            Publisher = publisherId
        };
    }
    
    public static List<GenreResponse> CreateGenreResponseList(int count)
    {
        var genres = new List<GenreResponse>();
        for (int i = 0; i < count; i++)
        {
            genres.Add(new GenreResponse
            {
                Id = Guid.NewGuid(),
                Name = $"Genre {i + 1}",
            });
        }
        return genres;
    }

    public static GenreResponse CreateSingleGenreResponse(Guid? id = null, string name = "Default Genre")
    {
        return new GenreResponse
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
        };
    }

    public static GenreCreate CreateGenreCreateRequest(string name = "New Genre")
    {
        return new GenreCreate
        {
            Genre = new GenreRequest
            {
                Name = name
            }
        };
    }

    public static GenreCreate CreateGenreUpdateRequest(string name = "Updated Genre")
    {
        return new GenreCreate
        {
            Genre = new GenreRequest
            {
                Name = "genreUpdate"
            }
        };
    }
    
    public static List<PlatformResponse> CreatePlatformResponseList(int count)
    {
        var platforms = new List<PlatformResponse>();
        for (int i = 0; i < count; i++)
        {
            platforms.Add(new PlatformResponse
            {
                Id = Guid.NewGuid(),
                Type = $"Platform {i + 1}"
            });
        }
        return platforms;
    }

    public static PlatformResponse CreateSinglePlatformResponse(Guid platformId)
    {
        return new PlatformResponse
        {
            Id = platformId,
            Type = "Existing Platform"
        };
    }


    public static PlatformCreate CreatePlatformCreateRequest(string type = "New Platform")
    {
        return new PlatformCreate
        {
            Platform = new PlatformRequest
            {
                Type = type
            }
        };
    }
    
    public static List<OrderDetailResponse> CreateOrderDetailList(int count)
    {
        var details = new List<OrderDetailResponse>();
        for (int i = 0; i < count; i++)
        {
            details.Add(new OrderDetailResponse
            {
                Price = 50.0 + i * 10,
                Quantity = i + 1
            });
        }
        return details;
    }

    public static PaymentCreate CreatePaymentRequest()
    {
        return new PaymentCreate
        {
            Method = "CreditCard",
            Model = new PaymentCardModel
            {
                Holder = "John Doe",
                CardNumber = "4111111111111111",
                MonthExpire = 12,
                YearExpire = 2026,
                Cvv2 = 123
            }
        };
    }

    public static BankPaymentResult CreateBankPaymentResult()
    {
        return new BankPaymentResult
        {
            PdfBytes = new byte[] { 1, 2, 3 },
            FileName = "payment_receipt.pdf"
        };
    }

    public static OrderResponse CreateSingleOrderResponse()
    {
        return new OrderResponse
        {
            Id = Guid.NewGuid()
        };
    }

    public static List<OrderResponse> CreateOrderResponseList(int count)
    {
        var orders = new List<OrderResponse>();
        for (int i = 0; i < count; i++)
        {
            orders.Add(new OrderResponse
            {
                Id = Guid.NewGuid()
            });
        }
        return orders;
    }
    
}