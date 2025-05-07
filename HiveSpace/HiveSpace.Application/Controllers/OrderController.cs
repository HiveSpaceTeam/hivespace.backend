using FluentValidation;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.Dtos.Request.Paging;
using HiveSpace.Application.Models.Dtos.Request.ShoppingCart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HiveSpace.Application.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IValidator<CreateOrderRequestDto> _createOrderValidator;
        private readonly IValidator<PagingRequestDto> _pagingValidator;

        public OrderController(
            IOrderService orderService,
            IValidator<CreateOrderRequestDto> createOrderValidator,
            IValidator<PagingRequestDto> pagingValidator)
        {
            _orderService = orderService;
            _createOrderValidator = createOrderValidator;
            _pagingValidator = pagingValidator;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableContent)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto param)
        {
            var validationResult = _createOrderValidator.Validate(param);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _orderService.CreateOrder(param);
            return Ok(result);
        }

        [HttpPost("paging")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableContent)]
        public async Task<IActionResult> GetPaging([FromBody] PagingRequestDto param)
        {
            var validationResult = _pagingValidator.Validate(param);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _orderService.GetPaging(param);
            return Ok(result);
        }
    }
}
