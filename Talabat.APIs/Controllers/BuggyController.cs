using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly StoreContext _context;

        public BuggyController(StoreContext context)
        {
            _context = context;
        }


        [HttpGet("notfound")] // GET: /api/buggy/notfound
        public ActionResult GetNotFoundRequest()
        {
            var product = _context.Products.Find(100);

            if(product is null)
                return NotFound(new ApiResponse(404));

            return Ok(product);
        }

        [HttpGet("servererror")] // GET  : /api/buggy/servererror
        public ActionResult GetServerErrorRequest()
        {
            var product = _context.Products.Find(100);
            var productToReturn = product.ToString();  // will Throw Exception [NullReferenceException]

            return Ok(productToReturn);

        }

        [HttpGet("badrequest")] // GET : /api/buggy/badrequest
        public ActionResult DetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("badrequest/{id}")] // GET : /api/buggy/badrequest/five
        public ActionResult GetbadRequest(int id) // Validation Error
        {
            return Ok();
        }

        [HttpGet("unauthorized")] // GET : /api/buggy/unauthorized
        public ActionResult GetUnauthorizedError()
        {
            return Unauthorized(new ApiResponse(401));
        }


    }
}
