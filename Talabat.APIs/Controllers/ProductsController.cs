using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.Product_Specifications;

namespace Talabat.APIs.Controllers
{
  
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;

        ///private readonly IGenericRepository<Product> _productRepository;
        ///private readonly IGenericRepository<ProductBrand> _brandsRepo;
        ///private readonly IGenericRepository<ProductCategory> _categoriesRepo;
        private readonly IMapper _mapper;

        public ProductsController(

            IProductService productService,
            ///IGenericRepository<Product> productRepository, 
            ///IGenericRepository<ProductBrand> brandsRepo,
            ///IGenericRepository<ProductCategory> categoriesRepo,
            IMapper mapper)
        {
            _productService = productService;
            ///_productRepository = productRepository;
            ///_brandsRepo = brandsRepo;
            ///_categoriesRepo = categoriesRepo;
            _mapper = mapper;
        }

        [Cached(600)] // Action Filter
        [HttpGet] // GET : /api/Products => Routing Filter
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetAllProductAsync([FromQuery] ProductSpecParams productSpec)
        {
            var products = await _productService.GetProductsAsync(productSpec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            var count = await _productService.GetCountAsync(productSpec);

            return Ok(new Pagination<ProductToReturnDto>(productSpec.PageIndex, productSpec.PageSize, count, data));
        }

        // /api/Products/10
        [Cached(600)]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductToReturnDto), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if(product == null)
                return NotFound(new ApiResponse(404)); // 404

            return Ok(_mapper.Map<Product, ProductToReturnDto>(product)); // 200
        }

        [Cached(600)]
        [HttpGet("brands")] // GET : /api/Products/brands
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _productService.GetBrandsAsync();

            return Ok(brands);
        }

        [HttpGet("categories")] // GET : /api/Products/categories
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()
        {
            var categories = await _productService.GetCategoriesAsync();

            return Ok(categories);
        }
    }
}
