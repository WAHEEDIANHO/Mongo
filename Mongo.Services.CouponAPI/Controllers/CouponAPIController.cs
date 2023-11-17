using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mongo.Services.CouponAPI.Data;
using Mongo.Services.CouponAPI.Model;
using Mongo.Services.CouponAPI.Model.Dto;

namespace Mongo.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _responseDto;
        private IMapper _mapper;

        public  CouponAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _responseDto = new ResponseDto();
            _mapper = mapper;
        }   

        [HttpGet]
        public ResponseDto Get() 
        {
            try 
            {
                IEnumerable<Coupon> objList = _db.Coupons.ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<CouponDto>>(objList);
             }
            catch (Exception ex) {
                _responseDto.IsSuccessful = false;
                _responseDto.message = ex.Message;
                    }
            return _responseDto;
        }

        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public ResponseDto Get(int id) 
        {
            try
            {
                Coupon coupon = _db.Coupons.First(cou => cou.CouponId == id);
                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
            }catch (Exception ex) {
                _responseDto.IsSuccessful = false;
                _responseDto.message = ex.Message;
            }
            return _responseDto;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public  ResponseDto Get(string code)
        {
            try
            {
                Coupon obj = _db.Coupons.First(u => u.CouponCode.ToLower() == code.ToLower());
                _responseDto.Result = _mapper.Map<CouponDto>(obj);
            }catch(Exception ex) 
            {
                _responseDto.IsSuccessful = false;
                _responseDto.message = ex.Message;
            }

            return _responseDto;

        }

        [HttpPost]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon obj = _mapper.Map<Coupon>(couponDto);
                _db.Coupons.Add(obj);
                _db.SaveChanges();
                _responseDto.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccessful = false;
                _responseDto.message = ex.Message;
            }

            return _responseDto;

        }

        [HttpPut]
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon obj = _mapper.Map<Coupon>(couponDto);
                _db.Coupons.Update(obj);
                _db.SaveChanges();
                _responseDto.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
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
                Coupon obj = _db.Coupons.First(u => u.CouponId == id);
                _db.Coupons.Remove(obj);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccessful = false;
                _responseDto.message = ex.Message;
            }

            return _responseDto;

        }
    }
}
