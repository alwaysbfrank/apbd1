using System.Collections.Generic;
using APBD1.Models;

namespace APBD1.DAL
{
    public interface IStudentsService
    {
        public ICollection<Student> GetStudents();
        public Student GetStudent(string index);
        public Student AddStudent(Student student);
    }
}