using BorrowService.Infrastructure.IRepository;

namespace BorrowService.Presentation;

public interface IOverdueCheckService
{
    Task CheckOverdue();
}

public class OverdueCheckService(ILogger<OverdueCheckHostService> logger, IBorrowHistoryRepository borrowRepo) : IOverdueCheckService
{
    public async Task CheckOverdue()
    {
        logger.LogInformation("Background service check new overdue borrow history");
        
    }
}