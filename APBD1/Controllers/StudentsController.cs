using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APBD1.DAL;
using APBD1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APBD1.Controllers
{
    
    [ApiController]
    [Route("api/students/")]
    public class StudentsController : ControllerBase
    {

        private readonly IDbService students;

        public StudentsController(IDbService dbService)
        {
            students = dbService;
        }
        
        [HttpGet("{id}")]
        public Student GetStudent(int id)
        {
            return students.GetStudent(id);
        }
        
        [HttpGet]
        public IEnumerable<Student> GetStudents()
        {
            return students.GetStudents();
        }

        [HttpPost]
        public IActionResult CreateStudent(Student request)
        {
            return Ok(students.AddStudent(request));
        }
    }
}