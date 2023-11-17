using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mongo.Services.ProductAPI.Data;
using Mongo.Services.ProductAPI.Model;
using Mongo.Services.ProductAPI.Model.Dto;

namespace Mongo.Services.ProductAPI.Controllers
{
    [ApiController]
    [Route("api/products")]
   // [Authorize]
    public class ProductAPIController : ControllerBase
    {
        private readonly ILogger<ProductAPIController> _logger;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private readonly ResponseDto _responseDto;
        public ProductAPIController(ILogger<ProductAPIController> logger, IMapper mapper, AppDbContext db)
        {
            _logger = logger;
            _mapper = mapper;
            _db = db;
            _responseDto = new ResponseDto();
        }

        [HttpGet]
        public ResponseDto GetProduct()
        {
            try
            {
                IEnumerable<Product> products = _db.Products.ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<ProductDto>>(products);

            }catch (Exception ex) { 
                _logger.LogInformation("Error retrieving data");
                _responseDto.IsSuccessful = false;
                _responseDto.message = ex.Message;
            }
            return _responseDto;   
        }

        [HttpGet]
        [Route("{id}")]
        public ResponseDto GetProductById(int id)
        {
            try
            {
                Product product = _db.Products.FirstOrDefault(u => u.ProductId == id);
                if (product == null)
                {
                    _responseDto.IsSuccessful = false;
                    _responseDto.message = "product not find";
                }
                _responseDto.Result = _mapper.Map<ProductDto>(product);    

            }catch (Exception ex)
            {
                _logger.LogInformation("Error ftecing Post");
                _responseDto.IsSuccessful = false;
                _responseDto.message = ex.Message;
            }
            return _responseDto; 
        }

        [HttpPost]
        public ResponseDto PostProduct([FromForm] ProductDto product)
        {
            try
            {
                Product prod = _mapper.Map<Product>(product);
                _db.Products.Add(prod);
                _db.SaveChanges();

                if(product.Image != null)
                {
                    var filename = prod.ProductId + Path.GetExtension(product.Image.FileName);
                    var filePath = @"wwwroot\ProductImages\" + filename;
                    var fileDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(fileDirectory, FileMode.Create))
                    {
                        product.Image.CopyTo(fileStream);
                    };
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    prod.ImageUrl = baseUrl + "/ProductImages/" + filename;
                    _db.Products.Update(prod);
                    _db.SaveChanges();
                }
                _responseDto.Result = _mapper.Map<ProductDto>(prod);

            }catch (Exception ex) {
                _logger.LogInformation("Error creating product");
                _responseDto.message = "Error creating product";
                _responseDto.IsSuccessful = false;
            }

            return _responseDto;
        }
        [Authorize(Roles = "ADMIN")]
        [HttpPut]
        public ResponseDto Put([FromBody] ProductDto couponDto)
        {
            try
            {
                Product obj = _mapper.Map<Product>(couponDto);
                _db.Products.Update(obj);
                _db.SaveChanges();
                _responseDto.Result = _mapper.Map<ProductDto>(obj);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _responseDto.IsSuccessful = false;
                _responseDto.message = ex.Message;
            }

            return _responseDto;

        }

        [HttpDelete]
        public ResponseDto Delete(int id)
        {
            try
            {
                Product obj = _db.Products.First(u => u.ProductId == id);
                if (!string.IsNullOrEmpty(obj.ImageUrl))
                {
                    var filename = Path.GetFileName(obj.ImageUrl);
                    var filePath = @"wwwroot\ProductImages\" + filename;
                    var fileDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    FileInfo fileInfo = new FileInfo(fileDirectory);
                    if(fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }
                }
                _db.Products.Remove(obj);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _responseDto.IsSuccessful = false;
                _responseDto.message = ex.Message;
            }

            return _responseDto;

        }
    }
}
