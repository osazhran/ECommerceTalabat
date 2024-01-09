using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;

namespace Talabat.APIs.Controllers
{
    
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(
            IBasketRepository basketRepository,
            IMapper mapper
            )
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        [HttpGet] // GET : /api/basket?id=
        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);

            if (basket is null)
                return new CustomerBasket(id);

            return Ok(basket);
        }

        [HttpPost] // POST : /api/basket
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdateBasket(CustomerBasketDto customerBasket)
        {
            var mappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(customerBasket);

            var basket = await _basketRepository.UpdateBasketAsync(mappedBasket);

            if (basket is null)
                return BadRequest(new ApiResponse(400));

            return Ok(basket);
        }

        [HttpDelete] // DELETE : /api/basket?id=
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            var response = await _basketRepository.DeleteBasketAsync(id);

            return Ok(response);
        }

    }
}
