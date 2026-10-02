namespace OrderEngine.Application.Dtos;

using OrderEngine.Domain.Enums;

public record CreateOrderItemRequest(string ProductName, int Quantity, decimal UnitPrice);
public record CreateOrderRequest(string CustomerEmail, List<CreateOrderItemRequest> Items);
public record UpdateOrderStatusRequest(OrderStatus Status);

public record OrderItemDto(Guid Id, string ProductName, int Quantity, decimal UnitPrice);
public record OrderDto(
    Guid Id,
    string OrderNumber,
    string CustomerEmail,
    decimal TotalAmount,
    OrderStatus Status,
    DateTime CreatedAt,
    List<OrderItemDto> Items
);
