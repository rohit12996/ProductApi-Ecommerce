using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversion;
using ProductApi.Application.Interface;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProduct()
        {
            //get all the product from repo
            var products = await productInterface.GetAllAsync();
            if(!products.Any())
            {
                return NotFound("No Product detected in the db");
            }


            //convert data from entity to dto
            var (data ,list) = ProductConversion.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No Product Found");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id) 
        {
            //get all the product from repo
            var product = await productInterface.GetByIdAsync(id);
            if (product == null)
                return NotFound("product requested not found");

            //convert data from entity to dto
            var (_product, data) = ProductConversion.FromEntity(product, null!);
            return _product is not null ? Ok(_product) : NotFound("Product not found");

        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            //check modal state is all data annotation are passed
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            //convert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var  response = await productInterface.CreateAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);

        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            //check modal state is all data annotation are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.UpdateAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);

        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            //check modal state is all data annotation are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.DeleteAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }
    }
}
