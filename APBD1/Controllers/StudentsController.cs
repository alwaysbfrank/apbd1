using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APBD1.Authentication;
using APBD1.DAL;
using APBD1.Dtos;
using APBD1.Models;
using APBD1.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APBD1.Controllers
{
    
    [ApiController]
    [Route("api/students/")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsService _students;
        private readonly AuthenticationService _auth;

        public StudentsController(IStudentsService students, AuthenticationService auth)
        {
            _students = students;
            _auth = auth;
        }

        [HttpGet("{index}")]
        public Student GetStudent(string index)
        {
            return _students.GetStudent(index);
        }
        
        [HttpGet]
        public IEnumerable<Student> GetStudents()
        {
            return _students.GetStudents();
        }

        [HttpPost]
        public IActionResult CreateStudent(Student request)
        {
            return Ok(_students.AddStudent(request));
        }

        [HttpPost]
        [Route("{index}/login")]
        public IActionResult Login(string index, Password password)
        {
            try
            {
                return Ok(_auth.Login(index, password.Pass));
            }
            catch (Exception ex)
            {
                "Exception while logging in".Log();
                ex.Message.Log();
                ex.StackTrace.Log();
                return Unauthorized(); //Forbidden? Bad Request?
            }
        }

        [HttpPost]
        [Route("{index}/login/refresh")]
        public IActionResult RefreshLogin(string index, string refreshToken)
        {
            try
            {
                return Ok(_auth.RefreshToken(index, refreshToken));
            }
            catch (Exception ex)
            {
                "Exception while refreshing login".Log();
                ex.Message.Log();
                ex.StackTrace.Log();
                return Unauthorized(); //Forbidden? Bad Request?
            }
        }
    }
}