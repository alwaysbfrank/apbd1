using System;
using System.Collections.Generic;
using APBD1.DAL;
using APBD1.Dtos;
using APBD1.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace APBD1.Controllers
{
    [ApiController]
    [Route("api/enrollments/")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly EnrollmentsService _service;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly FullStudentService _fullStudent;

        public EnrollmentsController(EnrollmentsService service, FullStudentService fullStudentService)
        {
            _service = service;
            _mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap<Enrollment, EnrollResponse>());
            _fullStudent = fullStudentService;
        }

        [HttpPost]
        public IActionResult Enroll(EnrollRequest request)
        {
            if (!_service.DoStudyExist(request.Studies))
            {
                return BadRequest();
            }

            var result = _service.Enroll(request);

            var response = _mapperConfiguration.CreateMapper().Map<EnrollResponse>(result);
            return Created("api/enrollments/" + response.IdEnrollment, response);
        }

        [HttpGet]
        [Route("test/")]
        public List<FullStudent> GetAllStudents()
        {
            return _fullStudent.FindAll();
        }
        
        [HttpGet]
        public List<Enrollment> GetAll()
        {
            return _service.FindAll();
        }
    }
}