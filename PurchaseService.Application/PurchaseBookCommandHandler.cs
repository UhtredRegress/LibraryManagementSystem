using MediatR;
using Microsoft.Extensions.Configuration;
using PurchaseService.Domain.Aggregate;
using PurchaseService.Infrastructure;
using Shared.Enum;
using Stripe;
using Stripe.Checkout;
using PurchaseStatus = PurchaseService.Domain.Enum.PurchaseStatus;

namespace PurchaseService.Application;

public record PurchaseBookCommand(int UserId, string Username, int BookId, int BookType, int Amount) : IRequest<string>;

public class PurchaseBookCommandHandler : IRequestHandler<PurchaseBookCommand, string>
{
    private readonly IGrpcClient _grpcClient;
    private readonly IPurchaseRepository _purchaseRepository;
    

    public PurchaseBookCommandHandler(IGrpcClient grpcClient, IPurchaseRepository purchaseRepository, IConfiguration configuration)
    {
        _grpcClient = grpcClient;
        _purchaseRepository = purchaseRepository;
        StripeConfiguration.ApiKey = configuration["Stripe:ApiKey"];
    }

    public async Task<string> Handle(PurchaseBookCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(typeof(BookType), request.BookType))
        {
            throw new InvalidDataException("Invalid book type");
        }
        var response = await _grpcClient.RetrieveBookPriceCall(request.BookId, request.BookType);

        if (response == null) throw new InvalidDataException("There is no data to sell this book");
        
        var foundPendingPurchase = await _purchaseRepository
            .GetRangePurchaseBooksFilterAsync(pb => pb.UserId == request.UserId && pb.BookId == request.BookId && pb.BookType.Equals(request.BookType) && pb.Status == PurchaseStatus.Pending);
        
        if (foundPendingPurchase.Any())
        {
            return foundPendingPurchase.First().SessionUrl;
        }

        var priceUnit = response.PricePerUnit.Units + response.PricePerUnit.Micros / (decimal)1000000;
        var totalCost = priceUnit * request.Amount;

        var purchase =
            PurchaseBook.CreatePurchase(request.UserId, request.Username, request.BookId, bookType: (BookType)request.BookType,
                amount: request.Amount, title: response.Title, author: response.Author, finalCost: priceUnit);
        
        await _purchaseRepository.AddPurchaseBookAsync(purchase);
        
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            Mode = "payment",
            SuccessUrl = "http://localhost:5000",
            CancelUrl = "http://localhost:5000",
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long) totalCost * 100,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = purchase.Title,
                        },
                    },
                    Quantity = purchase.Amount,
                },
            },
            Metadata = new Dictionary<string, string>()
            {
                {"purchase_id", purchase.Id.ToString()},
            },
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        
        purchase.UpdateSessionUrl(session.Url);
        await _purchaseRepository.UpdatePurchaseBookAsync(purchase);
        return session.Url;

    }
}