using Crayon.Repository.ApiClients;
using MediatR;

namespace Crayon.Services.Services.Events;

public record CompletedOrderEvent(int AccountId, CcpOrder Order): INotification;