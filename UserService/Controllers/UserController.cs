using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Database;
using UserService.MessageQueue;
using UserService.Models;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly RabbitMQHandler _mqHandler;

        public UserController(UserDbContext context, RabbitMQHandler mQHandler)
        {
            _context = context;
            _mqHandler = mQHandler;
        }

        [HttpGet("{id:length(24)}", Name = "GetUser"), Authorize]
        public ActionResult<User> Get(string id)
        {
            try
            {
                User u = _context.Users.Find(id);
                return Ok(u);
            }
            catch (Exception)
            {
                return BadRequest(id);
            }
        }

        [HttpPost]
        public ActionResult Create([FromBody] User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            try
            {
                User newUser = _context.Users.Find(user.Id);
                _mqHandler.SendMessage(newUser);
                return Created($"Users/{newUser.Id}", newUser);
            }
            catch (Exception)
            {
                return StatusCode(500, user);
            }
        }

        [HttpPut, Authorize]
        public ActionResult Update([FromBody] User user)
        {

            try
            {
                User newUser = _context.Users.Find(user.Id);
                newUser.UpdatedAt = DateTime.Now;
                // Update post here

                _mqHandler.SendMessage(newUser);

                // 
                return Created($"Users/{newUser.Id}", newUser);
            }
            catch (Exception)
            {
                return StatusCode(500, user);
            }
        }


        [HttpDelete("{id:length(24)}", Name = "DeleteUser"), Authorize]
        public ActionResult<User> Delete(string id)
        {
            try
            {
                User u = _context.Users.Find(id);
                u.DeletedAt = DateTime.Now;
                return Ok(u);
            }
            catch (Exception)
            {
                return BadRequest(id);
            }
        }
    }
}
