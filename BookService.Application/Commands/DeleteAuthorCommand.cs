using BookService.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookService.Application.Commands;

public record DeleteAuthorCommand(int id):IRequest<bool>;

public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand, bool>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly ILogger<DeleteAuthorCommandHandler> _logger;
    
    public DeleteAuthorCommandHandler(IAuthorRepository authorRepository, ILogger<DeleteAuthorCommandHandler> logger)
    {
        _authorRepository = authorRepository;
        _logger = logger;
    }
    
    public async Task<bool> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started handler to delete author with id {AuthorId}", request.id);
        
        _logger.LogInformation("Retrieve author from database");
        var author = await _authorRepository.GetByIdAsync(request.id);

        if (author == null)
        {
            _logger.LogError("There is no author with id {id} in database", request.id);
            return false;
        }
        
        _logger.LogInformation("Delete data in database");
        await _authorRepository.DeleteAsync(author);
        return true;
    }
}