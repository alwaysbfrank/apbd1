using System.Collections.Generic;
using APBD1.Models;

namespace APBD1.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
        public Student GetStudent(int id);
        public Student AddStudent(Student student);
    }
}