using MediatR;
using Microsoft.Extensions.Logging;
using PurchaseService.Infrastructure;
using Shared.Enum;
using Shared.Exception;
using PurchaseStatus = PurchaseService.Domain.Enum.PurchaseStatus;

namespace PurchaseService.Application;

public record SuccessfullyPurchaseBookCommand(string purchaseId, string SessionUrl) : IRequest;

public class SuccessfullyPurchaseBookCommandHandler : IRequestHandler<SuccessfullyPurchaseBookCommand>
{
    private readonly ILogger<SuccessfullyPurchaseBookCommandHandler> _logger;
    private readonly IPurchaseRepository  _purchaseRepository;

    public SuccessfullyPurchaseBookCommandHandler(ILogger<SuccessfullyPurchaseBookCommandHandler> logger,
        IPurchaseRepository purchaseRepository)
    {
        _logger = logger;
        _purchaseRepository = purchaseRepository;
    }
    
    public async Task Handle(SuccessfullyPurchaseBookCommand request, CancellationToken cancellationToken)
    {
        int.TryParse(request.purchaseId, out var purchaseId);
        if (purchaseId <= 0)
        {
            throw new InvalidDataException("The request purchase ID is invalid");
        }
        var foundPurchase = await _purchaseRepository.GetPurchaseBookAsync(purchaseId);
        if (foundPurchase == null)
        {
            _logger.LogInformation($"Could not find purchase with id: {purchaseId}");
            throw new NotFoundDataException("Not found purchase id");
        }

        if (foundPurchase.Status != PurchaseStatus.Pending)
        {
            _logger.LogInformation("Cannot update purchase {purchaseId}", purchaseId);
            throw new InvalidDataException("Cannot update purchase status");
        }
        
        _logger.LogInformation("Start update domain and update to database");
        foundPurchase.PurchaseSuccessfully();
        await _purchaseRepository.UpdatePurchaseBookAsync(foundPurchase);
    }
}