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
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        UserDbContext _context;
        RabbitMQHandler mQHandler;

        public UserController(UserDbContext context)
        {
            _context = context;
            //mQHandler = new RabbitMQHandler("user");
        }

        [HttpGet("{id:length(24)}", Name = "GetUser")]
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

        //TODO: Validate FromBody with logic classes too
        //TODO: Make the SendMessage asynchronuous
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            try
            {
                User newUser = _context.Users.Find(user.Id);
                newUser.CreatedAt = DateTime.Now;
                mQHandler.SendMessage(newUser);
                return Created($"Users/{newUser.Id}", newUser);
            }
            catch (Exception)
            {
                return StatusCode(500, user);
            }
        }

        // TO DO
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] User user)
        {

            try
            {
                User newUser = _context.Users.Find(user.Id);
                newUser.UpdatedAt = DateTime.Now;
                // Update post here

                mQHandler.SendMessage(newUser);

                // 
                return Created($"Users/{newUser.Id}", newUser);
            }
            catch (Exception)
            {
                return StatusCode(500, user);
            }
        }


        [HttpDelete("{id:length(24)}", Name = "DeleteUser")]
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
