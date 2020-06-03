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
        private readonly IStudentsService students;

        public StudentsController(IStudentsService studentsService)
        {
            students = studentsService;
        }
        
        [HttpGet("{index}")]
        public Student GetStudent(string index)
        {
            return students.GetStudent(index);
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