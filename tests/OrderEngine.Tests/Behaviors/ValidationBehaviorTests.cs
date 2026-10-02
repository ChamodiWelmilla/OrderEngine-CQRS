namespace OrderEngine.Tests.Behaviors;

using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OrderEngine.Application.Common.Behaviors;
using OrderEngine.Application.Dtos;
using OrderEngine.Application.Orders.Commands;
using Xunit;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        var validator = new CreateOrderCommandValidatorTestsMock();
        var behavior = new ValidationBehavior<CreateOrderCommand, OrderDto>(new[] { validator });
        var command = new CreateOrderCommand("", new List<CreateOrderItemRequest>());

        RequestHandlerDelegate<OrderDto> next = _ => Task.FromResult(new OrderDto(
            Guid.NewGuid(), "ORD-1", "", 0, Domain.Enums.OrderStatus.Pending, DateTime.UtcNow, new List<OrderItemDto>()
        ));

        await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(command, next, CancellationToken.None));
    }

    private class CreateOrderCommandValidatorTestsMock : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidatorTestsMock()
        {
            RuleFor(x => x.CustomerEmail).NotEmpty();
        }
    }
}
