using System;
using System.Collections.Generic;
using APBD1.Models;

namespace APBD1.DAL
{
    public class MemoryDbService : IDbService
    {

        private Dictionary<int, Student> students = new Dictionary<int, Student>();
        private Random random = new Random();

        public IEnumerable<Student> GetStudents()
        {
            return students.Values;
        }

        public Student GetStudent(int id)
        {
            return students[id];
        }

        public Student AddStudent(Student student)
        {
            int id;
            do
            {
                id = random.Next();
            } while (students.ContainsKey(id));

            student.Id = id;

            student.Index = $"s{students.Count+1}";
            students.Add(student.Id, student);
            return student;
        }
    }
}