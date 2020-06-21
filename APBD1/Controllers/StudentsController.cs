using System;
using System.Collections.Generic;
using System.Text;
using APBD1.Dtos;
using APBD1.Dtos.Ef;
using APBD1.EfModels;
using APBD1.EfServices;
using APBD1.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;

namespace APBD1.Controllers
{
    [ApiController]
    [Route("api/students")]
    class StudentsController : ControllerBase
    {

        private readonly StudentsService _students;

        public StudentsController(StudentsService students)
        {
            _students = students;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var allStudents = _students.GetAllStudents();
            ("returning " + allStudents.Count + " students").Log();
            return Ok(allStudents);
        }

        [HttpPut]
        [Route("/{index}")]
        public IActionResult UpdateStudent(string index, StudentRequest request)
        {
            return Return(_students.UpdateStudent(index, request));
        }

        private IActionResult Return(bool result)
        {
            if (result)
            {
                return NoContent();
            }

            return BadRequest();
        }

        [HttpDelete]
        [Route("/{index}")]
        public IActionResult DeleteStudent(string index)
        {
            return Return(_students.RemoveStudent(index));
        }

        [HttpPost]
        public IActionResult Enroll(EnrollRequest request)
        {
            return Return(_students.EnrollStudent(request));
        }

    }
}
