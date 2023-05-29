using Ecommerce.Dtos;
using Ecommerce.Enums;
using Ecommerce.Filters;
using Ecommerce.Interfaces;
using Ecommerce.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderService _orderService;
    private readonly IValidator<OrderDto> _orderValidator;

    public OrdersController(IRepositoryFactory repositoryFactory, IOrderService orderService,
        IValidator<OrderDto> orderValidator)
    {
        _orderRepository = repositoryFactory.CreateOrderRepository();
        _orderService = orderService;
        _orderValidator = orderValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll([FromQuery] OrderStatus? status = null)
    {
        var userId = HttpContext.GetUserIdFromClaim();
        if (userId is null)
        {
            return Unauthorized();
        }

        return Ok(await _orderRepository.GetUserOrders(userId, status));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> Get(string id)
    {
        var userId = HttpContext.GetUserIdFromClaim();
        if (userId is null)
        {
            return Unauthorized();
        }

        var order = await _orderRepository.GetById(id);
        if (order is null)
        {
            return NotFound();
        }

        if (order.UserId != userId)
        {
            return Forbid();
        }

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Add(OrderDto order)
    {
        var validationResult = await _orderValidator.ValidateAsync(order);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var userId = HttpContext.GetUserIdFromClaim();
        if (userId is null)
        {
            return Unauthorized();
        }

        var updatedOrder = _orderService.CompleteOrderDefinition(order, userId, true);
        var inserted = await _orderRepository.Add(updatedOrder);
        return CreatedAtAction(nameof(Get), new {inserted.Id}, inserted);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OrderDto>> Update(string id, OrderDto order)
    {
        var validationResult = await _orderValidator.ValidateAsync(order);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        if (id != order.Id)
        {
            return BadRequest();
        }

        var userId = HttpContext.GetUserIdFromClaim();
        if (userId is null)
        {
            return Unauthorized();
        }

        if (await _orderRepository.Exists(id))
        {
            var updatedOrder = _orderService.CompleteOrderDefinition(order, userId, false);
            var updated = await _orderRepository.Update(id, updatedOrder);
            return Ok(updated);
        }

        var updatedInsertOrder = _orderService.CompleteOrderDefinition(order, userId, true);
        var inserted = await _orderRepository.Add(updatedInsertOrder);
        return CreatedAtAction(nameof(Get), new {inserted.Id}, inserted);
    }

    [HttpPatch("{id}/status")]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    public async Task<ActionResult> UpdateStatus(string id, OrderStatusDto orderStatus)
    {
        if (!await _orderRepository.Exists(id))
        {
            return NotFound();
        }

        await _orderRepository.UpdateStatus(id, orderStatus);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    public async Task<ActionResult> Delete(string id)
    {
        await _orderRepository.Delete(id);
        return NoContent();
    }
}