using FluentValidation;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.Dtos.Request.Product;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HiveSpace.Application.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    //[Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IValidator<ProductSearchRequestDto> _productSearchValidator;

        public ProductController(
            IProductService productService,
            IValidator<ProductSearchRequestDto> productSearchValidator)
        {
            _productService = productService;
            _productSearchValidator = productSearchValidator;
        }

        [HttpGet("{productId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductDetail(int productId)
        {
            var result = await _productService.GetProductDetailAsync(productId);
            return Ok(result);
        }

        [HttpPost("search")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetProductSearchViewModel([FromBody] ProductSearchRequestDto param)
        {
            var validationResult = _productSearchValidator.Validate(param);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _productService.GetProductSearchViewModelAsync(param);
            return Ok(result);
        }

        [HttpGet("home={isHome}&pageSize={pageSize}&pageIndex={pageIndex}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductHomeViewModel(bool isHome = false, int pageSize = 50, int pageIndex = 0)
        {
            var result = await _productService.GetProductHomeViewModelAsync(pageSize, pageIndex);
            return Ok(result);
        }

        [HttpGet("categoryId={categoryId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var result = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(result);
        }
    }
}
