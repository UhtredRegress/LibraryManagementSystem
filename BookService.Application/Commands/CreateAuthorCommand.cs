using BookService.Application.DTO;
using BookService.Domain.Model;
using BookService.Infrastructure;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookService.Application.Commands;

public record CreateAuthorCommand(string Name) : IRequest<Result<AuthorResponseDTO>>;

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, Result<AuthorResponseDTO>>
{
    private readonly IAuthorRepository _repository;
    private readonly ILogger<CreateAuthorCommandHandler> _logger;

    public CreateAuthorCommandHandler(IAuthorRepository repository, ILogger<CreateAuthorCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<Result<AuthorResponseDTO>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handler Create New Author started to process request");
        
        _logger.LogInformation("Started checking whether {AuthorName} is existed in database", request.Name);
        var foundAuthor = await _repository.FindAsync(a => a.Name == request.Name);

        if (foundAuthor != null)
        {
            _logger.LogInformation("The {AuthorName} is already existed in database return failed to user", request.Name);
            return Result.Fail(new Error("This author name is already existed in database"));
        }
        
        _logger.LogInformation("Creating a new Author {AuthorName}", request.Name);
        var newAuthor = new Author(request.Name);
        
        _logger.LogInformation("Persistence this data into database");
        await _repository.CreateAsync(newAuthor);
        
        _logger.LogInformation("Successfully created new Author {AuthorName}", request.Name);

        var resultDto = new AuthorResponseDTO();
        resultDto.Name = newAuthor.Name;
        
        return Result.Ok(resultDto);
    }
}