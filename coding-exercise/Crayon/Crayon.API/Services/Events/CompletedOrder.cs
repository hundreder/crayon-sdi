using Crayon.API.ApiClients;
using MediatR;

namespace Crayon.API.Services.Events;

public record CompletedOrderEvent(int AccountId, CcpOrder Order): INotification;