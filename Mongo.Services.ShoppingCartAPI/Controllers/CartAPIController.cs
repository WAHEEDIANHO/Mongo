using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mongo.MessageBus;
using Mongo.Services.CouponAPI.Model.Dto;
using Mongo.Services.ShoppingCartAPI.Data;
using Mongo.Services.ShoppingCartAPI.Model.Dto;
using Mongo.Services.ShoppingCartAPI.Models;
using Mongo.Services.ShoppingCartAPI.Models.Dto;
using Mongo.Services.ShoppingCartAPI.Services.Iservice;

namespace Mongo.Services.ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/cart/[action]")]
    public class CartAPIController : ControllerBase
    {
        private readonly ResponseDto _response;
        private readonly AppDbContext _db;
        private IMapper _mapper;
        private IProductService _produtcService;
        private ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        IConfiguration _configiuration;


        public CartAPIController(AppDbContext db, IMapper mapper, 
            IProductService productService,  
            ICouponService couponService, IMessageBus messageBus,
            IConfiguration configuration) 
        {
            _response = new ResponseDto();
            _db = db;
            _mapper = mapper;
            _produtcService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configiuration = configuration;
        }

        [Authorize]
        [HttpPost("cartUpsert")]
        public async Task<ResponseDto> Upsert([FromBody] CartDto cart) 
        {
           try
            {
                var CardHeaderFromDb = await _db.Set<CartHeader>().AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cart.CartHeader.UserId);
                if (CardHeaderFromDb == null)
                {
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cart.CartHeader);
                    _db.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    CartDetails cartDetails = _mapper.Map<CartDetails>(cart.CartDetails.First());
                    cartDetails.CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(cartDetails);
                    await _db.SaveChangesAsync();
                }else
                {
                    var cartDetailFromDb =  _db.CartDetails.AsNoTracking().FirstOrDefault(u => u.ProductId == cart.CartDetails.First().ProductId && u.CartHeaderId == CardHeaderFromDb.CartHeaderId);
                    if(cartDetailFromDb == null)
                    {
                        CartDetails cartDetails = _mapper.Map<CartDetails>(cart.CartDetails.First());
                        cartDetails.CartHeaderId = CardHeaderFromDb.CartHeaderId; 
                        _db.CartDetails.Add(cartDetails);
                        await _db.SaveChangesAsync();
                    }else
                    {
                        cart.CartDetails.First().Count += cartDetailFromDb.Count;
                        cart.CartDetails.First().CartHeaderId = CardHeaderFromDb.CartHeaderId;
                        cart.CartDetails.First().CartDetailsId = CardHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cart.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }
                _response.Result = cart;

            }catch (Exception ex)
            {
                _response.IsSuccessful = false;
                _response.message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpDelete]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _db.CartDetails.First(u => u.CartDetailsId == cartDetailsId);

                int count = _db.CartDetails.Count(u=> u.CartHeaderId == cartDetails.CartHeaderId);
                _db.CartDetails.Remove(cartDetails);
                if(count == 1)
                {
                    CartHeader CartHeaderToRemove = _db.CartHeaders.First(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    _db.CartHeaders.Remove(CartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                _response.Result = true;

            }catch (Exception ex)
            {
                _response.IsSuccessful = false;
                _response.message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        public async Task<ResponseDto> LoadCart([FromQuery] string userId)
        {

            try
            {
                var userCartHeader = await _db.CartHeaders.FirstAsync(u => u.UserId == userId);
                IEnumerable<CartDetails> userCartDetails = _db.CartDetails.Where(u => u.CartHeaderId == userCartHeader.CartHeaderId).ToList();
                CartHeaderDto cartHeader = _mapper.Map<CartHeaderDto>(userCartHeader);
                IEnumerable<CartDetailsDto> cartDetailsDtos = _mapper.Map<IEnumerable<CartDetailsDto>>(userCartDetails);

                var products = await _produtcService.GetProducts();               

                foreach (var item in userCartDetails)
                {
                    item.product = products.FirstOrDefault(u => u.ProductId == item.ProductId);
                    if (item.product != null) cartHeader.cartTotal += item.Count * item.product.Price;
                    else continue;
                }

                if (!String.IsNullOrEmpty(cartHeader.CouponCode))
                {
                    CouponDto coupon = await _couponService.GetCoupon(cartHeader.CouponCode);
                    if(coupon != null && cartHeader.cartTotal > coupon.MinAmount) {
                        cartHeader.cartTotal -= coupon.DiscountAmount;
                        cartHeader.Discount = coupon.DiscountAmount;
                    }

                }

                CartDto cart = new CartDto
                {
                    CartHeader = cartHeader,
                    CartDetails = cartDetailsDtos
                };
                _response.Result= cart;

            }catch (Exception ex)
            {
                _response.IsSuccessful = false;
                _response.message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.Set<CartHeader>().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.Result = true;
            }catch (Exception ex)
            {
                _response.IsSuccessful = false;
                _response.message = ex.Message.ToString();
            }
            return _response;
        }

        [Authorize]
        [HttpPost]
        public async Task<object> EmailShopingCart([FromBody] CartDto cartDto)
        {
            try
            {
                await _messageBus.PubishMessge(cartDto, _configiuration.GetValue<string>("TopicsQueueName:EmailShoppingCart"));
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccessful = false;
                _response.message = ex.Message.ToString();
            }
            return _response;
        }

        
    }
}
