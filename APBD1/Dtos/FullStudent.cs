using System;
using APBD1.Models;

namespace APBD1.Dtos
{
    public class FullStudent
    {
        public Student Student { get; }
        public Enrollment Enrollment { get; }
        public Studies Studies { get; }

        public FullStudent(Student student, Enrollment enrollment, Studies studies)
        {
            Enrollment = enrollment;
            Studies = studies;
            Student = student;
        }
    }
}