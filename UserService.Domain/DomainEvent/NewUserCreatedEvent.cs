using MediatR;

namespace Domain.DomainEvent;

public record NewUserCreatedEvent(int userId, string email, DateTime dateCreated, string address)
    : INotification;  