using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CustomActionFilter.Context;
using CustomActionFilter.Model;
using CustomActionFilter.ActionFilter;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using CustomAuthorization.ActionFilter;

namespace CustomActionFilter.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    //[CustomActionFilter]
    //[Authorize(Policy = "CustomRole")]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public static string Role { get; set; }
        public IConfiguration Configuration { get; }

        public StudentsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;

            if (!_context.Students.Any())
            {
                _context.Students.Add(new Student
                {
                    Id = 1,
                    RollNo = "C1901",
                    FullName = "test",
                    FatherName = "testfather",
                    MotherName = "testmother",
                    BloodGroup = "A+",
                    Role = "Admin"
                });
                _context.SaveChanges();
            }

            //var student = _context.Students.FindAsync();
            Role = "Admin";

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult RequestToken([FromBody] TokenRequest request)
        {
            if ((request.Username == "jon" || request.Username == "jan") && request.Password == "jon")
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim("CompletedBasicTraining", ""),
                    new Claim(CustomClaimType.Role, request.Username,ClaimValueTypes.String)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "yourdomain.com",
                    audience: "yourdomain.com",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }

            return BadRequest("Could not verify username and password");
        }




        public static string GetRole ()
        {
            return Role;

        }

        //  [HttpGet]
        public void UnAuth ()
        {
            //logic 
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

        // GET: api/Students/5

        [Authorize(Policy = "TrainedStaffOnly")]
        [HttpGet("{id}")]
        //[Authorize(Policy = "CustomRole")]
        //[Authorize(Roles = "Administrator")]
        // [CustomActionFilter]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {

            var userName = User.Identity.Name;
            var student = await _context.Students.FindAsync(id);

           

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Students/AddStudent

        [HttpPost]
        [Route("AddStudent")]
        //[Authorize(Policy = "CustomRole")]
       // [Authorize(Roles = "Administrator")]
       [CustomActionFilter]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.Id }, student);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Student>> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return student;
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }


    public class TokenRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
