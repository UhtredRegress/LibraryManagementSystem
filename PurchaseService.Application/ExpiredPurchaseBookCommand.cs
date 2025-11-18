using MediatR;
using Microsoft.Extensions.Logging;
using PurchaseService.Infrastructure;
using Shared.Exception;

namespace PurchaseService.Application;

public record ExpiredPurchaseBookCommand(string purchaseId, string purchaseUrl):IRequest;

public class ExpiredPurchaseBookCommandHandler : IRequestHandler<ExpiredPurchaseBookCommand>
{
    private readonly ILogger<ExpiredPurchaseBookCommandHandler> _logger;
    private readonly IPurchaseRepository _purchaseRepository;

    public ExpiredPurchaseBookCommandHandler( ILogger<ExpiredPurchaseBookCommandHandler> logger,
        IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
        _logger = logger;
    }
    
    public async Task Handle(ExpiredPurchaseBookCommand request, CancellationToken cancellationToken)
    {
        int.TryParse(request.purchaseId, out int purchaseId);
        if (purchaseId <= 0)
        {
            throw new InvalidDataException("Invalid purchase id");
        }
        
        var foundPurchase = await _purchaseRepository.GetPurchaseBookAsync(purchaseId);

        if (foundPurchase == null)
        {
            throw new NotFoundDataException("Purchase history is not found");
        }

        if (foundPurchase.SessionUrl != request.purchaseUrl)
        {
            throw new InvalidDataException("purchase url and purchase id is not valid");
        }
        
        await _purchaseRepository.DeletePurchaseBookAsync(foundPurchase);
    }
}