using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using APBD1.Dtos;
using Microsoft.VisualBasic;

namespace APBD1.DAL
{
    public class FullStudentService
    {
        private readonly IStudentsService _students;
        private readonly StudiesService _studies;
        private readonly EnrollmentsService _enrollments;

        public FullStudentService(IStudentsService students, StudiesService studies, EnrollmentsService enrollments)
        {
            _students = students;
            _studies = studies;
            _enrollments = enrollments;
        }

        public List<FullStudent> FindAll()
        {
            var students = _students.GetStudents();

            return students.Select(student =>
            {
                var enrollment = _enrollments.FindById(student.IdEnrollment);
                var study = _studies.GetById(enrollment.IdStudy);
                return new FullStudent(student, enrollment, study);
            }).ToList();
        }
    }
}