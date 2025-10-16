using BookService.Application.DTO;
using BookService.Infrastructure;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookService.Application.Commands;

public record UpdateAuthorCommand(int Id, string Name):IRequest<Result<AuthorResponseDTO>>;

public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, Result<AuthorResponseDTO>>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly ILogger<UpdateAuthorCommandHandler> _logger;

    public UpdateAuthorCommandHandler(IAuthorRepository authorRepository, ILogger<UpdateAuthorCommandHandler> logger)
    {
        _authorRepository = authorRepository;
        _logger = logger;
    }
    
    public async Task<Result<AuthorResponseDTO>> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started handler to update author information");
        
        _logger.LogInformation("Retrieve data with id {AuthorId} in database to update author", request.Id);
        var foundAuthor = await _authorRepository.GetByIdAsync(request.Id);
        if (foundAuthor == null)
        {
            _logger.LogError("Author with id {AuthorId} not found returned failed to user", request.Id);
            return Result.Fail(new Error("Author of your request is not found"));
        }

        if (foundAuthor.Name == request.Name)
        {
            _logger.LogError("Name of the request to update author is identical with the old name");
            return Result.Fail(new Error("You request to update name same as the old name"));
        }
        
        _logger.LogInformation("Check whether the request name {AuthorName} is already existed", request.Name);
        var foundAuthorWithName = await _authorRepository.FindAsync(a => a.Name == request.Name);
        if (foundAuthorWithName != null)
        {
            _logger.LogError("The request name is already existed in database");
            return Result.Fail(new Error("Your request update name is already existed in database"));
        }    
        
        _logger.LogInformation("Started to update author information");
        foundAuthor.UpdateName(request.Name);
        
        _logger.LogInformation("Started to persistence data in database");
        await _authorRepository.UpdateAsync(foundAuthor);

        var resultDTO = new AuthorResponseDTO();
        resultDTO.Name =  foundAuthor.Name;
        
        return Result.Ok(resultDTO);
    }
}