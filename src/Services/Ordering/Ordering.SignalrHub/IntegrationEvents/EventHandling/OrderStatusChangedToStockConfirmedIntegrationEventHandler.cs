﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Ordering.SignalrHub.IntegrationEvents.Events;
using Serilog.Context;
using System;
using System.Threading.Tasks;
using DotNetCore.CAP;

namespace Ordering.SignalrHub.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToStockConfirmedIntegrationEventHandler :ICapSubscribe
    {
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> _logger;

        public OrderStatusChangedToStockConfirmedIntegrationEventHandler(
            IHubContext<NotificationsHub> hubContext,
            ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> logger)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [CapSubscribe(nameof(OrderStatusChangedToStockConfirmedIntegrationEvent))]
        public async Task Handle(OrderStatusChangedToStockConfirmedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {AppName} - ({@IntegrationEvent})", Program.AppName, @event);

                await _hubContext.Clients
                    .Group(@event.BuyerName)
                    .SendAsync("UpdatedOrderState", new { OrderId = @event.OrderId, Status = @event.OrderStatus });
            }
        }
    }
}
