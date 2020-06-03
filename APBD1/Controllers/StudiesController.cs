using APBD1.DAL;
using APBD1.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace APBD1.Controllers
{
    [ApiController]
    [Route("api/studies/")]
    public class StudiesController : ControllerBase
    {
        private readonly StudiesService _studies;

        public StudiesController(StudiesService studies)
        {
            _studies = studies;
        }
        
        
        [HttpPost]
        public IActionResult Create(CreateStudiesRequest request)
        {
            var result = _studies.Create(request.Name);

            if (result == null)
            {
                return BadRequest();
            }

            return Created("no-uri", result);
        }
    }
}