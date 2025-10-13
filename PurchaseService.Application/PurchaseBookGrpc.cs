using Grpc.Core;
using Library.Grpc;
using PurchaseService.Infrastructure;
using Shared.Enum;
using PurchaseStatus = PurchaseService.Domain.Enum.PurchaseStatus;

namespace PurchaseService.Application;

public class PurchaseBookGrpc : PurchaseGrpcAPI.PurchaseGrpcAPIBase
{
    private readonly IPurchaseRepository  _purchaseRepository;

    public PurchaseBookGrpc(IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }
    public override async Task<FindSuccessfullyPurchaseEbookResponse> FindSuccessfullyPurchaseEbook(
        FindSuccessfullyPurchaseEBookRequest request, ServerCallContext context)
    {
        var foundPurchaseEbook = await 
            _purchaseRepository.GetPurchaseBookFilteredAsync(pb => pb.UserId == request.UserId && pb.BookId == request.BookId && pb.BookType == BookType.Ebook);
        if (foundPurchaseEbook == null)
        {
            return new FindSuccessfullyPurchaseEbookResponse() {Result = false, Message = "Not found purchase"};
        }

        if (foundPurchaseEbook.Status != PurchaseStatus.Approved)
        {
            return new FindSuccessfullyPurchaseEbookResponse()  {Result = false, Message = "You haven't made transaction to this book yet"};
        }
        
        return new FindSuccessfullyPurchaseEbookResponse()  {Result = true, Message = "Success"};
    }
}