using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mongo.MessageBus;
using Mongo.Services.OrderAPI.Data;
using Mongo.Services.OrderAPI.Model.Dto;
using Mongo.Services.OrderAPI.Models;
using Mongo.Services.OrderAPI.Models.Dto;
using Mongo.Services.OrderAPI.Services.Iservice;
using Mongo.Services.OrderAPI.Utils;
using Mongo.Services.ShoppingCartAPI.Models.Dto;
using PayStack.Net;
using System.Linq;

namespace Mongo.Services.OrderAPI.Controllers
{
    [ApiController]
    [Route("/api/order/[action]")]
    public class OrderAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private IMapper _mapper;
        private IProductService _productService;
        private ResponseDto _response;
        private IPayStackApi _payStackApi;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _config;

        public OrderAPIController(AppDbContext db, IMapper mapper, IProductService productService, IPayStackApi payStackApi, IMessageBus messageBus, IConfiguration config)
        {
            _db = db;
            _mapper = mapper;
            _productService = productService;
            _response = new ResponseDto();
            _payStackApi = payStackApi;
            _messageBus = messageBus;
            _config = config;
        }

        [Authorize]
        [HttpGet]
        public async Task<ResponseDto> Get(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeaderDto> objList;
                if (User.IsInRole(SD.RoleAdmin))
                {
                    objList = _mapper.Map<IEnumerable<OrderHeaderDto>>(_db.OrderHeaders.Include(x => x.OrdertDetails).OrderByDescending(u => u.OrderHeaderId).ToList());
                }else
                {
                    objList = _mapper.Map<IEnumerable<OrderHeaderDto>>(_db.OrderHeaders.Include(u => u.OrdertDetails))
                        .Where(u => u.UserId == userId).OrderByDescending(u => u.OrderHeaderId).ToList();
                }
                _response.Result = objList;
            }catch  (Exception ex) {
                _response.message = ex.Message;
                _response.IsSuccessful = false;
            }
            return _response;
        }

            [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public async Task<ResponseDto> Get(int id) {
            try
            {
                OrderHeader orderHeader = await _db.OrderHeaders.Include(u => u.OrdertDetails).FirstAsync(u => u.OrderHeaderId == id);
                _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
            }catch (Exception ex)
            {
                _response.IsSuccessful = false;
                _response.message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cart)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cart.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrdertDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cart.CartDetails);

                OrderHeader orderCreated =  _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                _response.Result = orderHeaderDto;


            }catch (Exception ex)
            {
                _response.message = ex.Message;
                _response.IsSuccessful = false;
            }

            return _response;

        }

        [HttpGet]
        public async Task<ResponseDto> GetOrders()
        {
            try
            {
                IEnumerable<OrderHeader> orderHeader = _db.OrderHeaders.ToList();
                _response.Result = _mapper.Map<IEnumerable<OrderHeader>>(orderHeader);
            }catch(Exception ex)
            {
                _response.message = ex.Message;
                _response.IsSuccessful = false;
            }

            return _response;
        }

        //[Authorize]
        [HttpPost]
        public async Task<ResponseDto> PayWithPayStack([FromBody] PayStackDto payStack)
        {
            try
            {
                int amount = 0;
                foreach (var item in payStack.OrderHeader.OrdertDetails) amount += (int) item.Price * 100;
                TransactionInitializeResponse response = _payStackApi.Transactions.Initialize(new TransactionInitializeRequest()
                {
                    Email = payStack.Email,
                    AmountInKobo = amount,
                    CallbackUrl = payStack.CancelUrl,

                });
                if (response.Data != null)
                {
                    payStack.Reference = response.Data.Reference;
                    payStack.Authorization = response.Data.AuthorizationUrl;
                    _response.Result = payStack;
                }
                else throw new Exception("Transaction failed");
            }
            catch(Exception ex)
            {
                _response.message = ex.Message; 
                _response.IsSuccessful = false;
            }

            return _response;
        }

        //[Authorize]
        [HttpPost]
        public async Task<ResponseDto> VerifyPaymant([FromBody] PayStackDto payStack)
        {
            
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == payStack.OrderHeader.OrderHeaderId);
                TransactionVerifyResponse verify = _payStackApi.Transactions.Verify(payStack.Reference);
                if (verify.Status && verify.Data.Status == "success")
                {
                    orderHeader.Status = SD.Status_Approved;
                    orderHeader.PaymentIntentId = verify.Data.Reference;
                    orderHeader.StripeSessionId = verify.Data.Reference;    
                    _db.SaveChanges();

                    RewardsDto reward = new RewardsDto()
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        RewardActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        UserId = orderHeader.UserId
                    };

                    string topicQueueName = _config.GetValue<string>("TopicsQueueName:OrderCreated");
                    await  _messageBus.PubishMessge(reward, topicQueueName);
                    _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }
                else
                {
                    _response.Result = new
                    {
                        Error = "Failed Transaction",
                        Reference = payStack.Reference
                    };
                }


            }
            catch (Exception ex)
            {
                _response.IsSuccessful=false;
                _response.message = ex.Message;
            }
            return _response;
        }

    }
}
