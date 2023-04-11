﻿using DataAccess.Datas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HiController : ControllerBase
    {
        private readonly TmpDataContext _dataContext;

        public HiController(TmpDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public IActionResult Hi()
        {
            _dataContext.Profile.Add(new()
            {
                Name = "Anirut",
                Description = " test data"
            } );
            _dataContext.SaveChanges();
            return Ok();
        }

        [HttpGet("all")]
        public IActionResult Get()
        {
            var a =_dataContext.Profile.ToList();
            return Ok(a);
        }
    }
}