using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APBD1.Dtos;
using APBD1.Dtos.Ef;
using APBD1.EfModels;
using Microsoft.EntityFrameworkCore;

namespace APBD1.EfServices
{
    public class StudentsService
    {
        private readonly masterContext _db;

        public StudentsService(masterContext db)
        {
            _db = db;
        }

        public List<Students> GetAllStudents()
        {
            return _db.Students.ToList();
        }

        public bool UpdateStudent(string indexNumber, StudentRequest updateRequest)
        {
            var updated = new Students
            {
                IndexNumber = indexNumber,
                BirthDate = updateRequest.BirthDate,
                FirstName = updateRequest.FirstName,
                LastName = updateRequest.LastName
            };
            _db.Students.Attach(updated);

            if (updateRequest.BirthDate != null)
            {
                _db.Entry(updated).Property("BirthDate").IsModified = true;
            }

            if (updateRequest.FirstName != null)
            {
                _db.Entry(updated).Property("FirstName").IsModified = true;
            }

            if (updateRequest.LastName != null)
            {
                _db.Entry(updated).Property("LastName").IsModified = true;
            }

            return _db.SaveChanges() == 1;
        }

        public bool RemoveStudent(string indexNumber)
        {
            _db.Students.Remove(new Students {IndexNumber = indexNumber});

            return _db.SaveChanges() == 1;
        }

        public bool AddStudent(StudentRequest newStudent)
        {
            _db.Add(newStudent);
            return _db.SaveChanges() == 1;
        }

        public bool EnrollStudent(EnrollRequest enrollRequest)
        {
            var study = _db.Studies.Include(study => study.Enrollments).FirstOrDefault(study => study.Name.Equals(enrollRequest.Studies));

            if (study == null)
            {
                return false;
            }

            var enrollment = study.Enrollments.Count > 0
                ? study.Enrollments.First(enrollment => enrollment.Semester == 1)
                : CreateNewEnrollment(study);

            _db.Students.Add(new Students
            {
                FirstName = enrollRequest.FirstName,
                LastName = enrollRequest.LastName,
                BirthDate = enrollRequest.BirthDate,
                IdEnrollment = enrollment.IdEnrollment
            });

            return _db.SaveChanges() > 0;
        }

        private static Enrollments CreateNewEnrollment(Studies study)
        {
            var enrollment = new Enrollments
            {
                IdStudy = study.IdStudy,
                Semester = 1,
                StartDate = DateTime.Now
            };
            study.Enrollments.Add(enrollment);
            return enrollment;
        }
    }
}
