using System;
using System.Collections.Generic;
using System.Data;
using APBD1.Dtos;
using APBD1.Models;
using APBD1.Utils;
using SQLDatabase.Net.SQLDatabaseClient;

namespace APBD1.DAL
{
    
    public class EnrollmentsService
    {
        private const string CountStudiesQuery = "SELECT Count(*) FROM Studies WHERE name = @name";
        private const string FindStudiesQuery = "SELECT IdStudy FROM Studies WHERE name = @name";
        private const string FindEnrollmentQuery = "SELECT * FROM Enrollments WHERE IdEnrollment = @idEnrollment";
        private const string FindAllQuery = "SELECT * FROM Enrollments";

        private const string FindFirstSemesterEnrollmentQuery =
            "SELECT * FROM Enrollments WHERE Semester = 1 AND IdStudy = @idStudies";

        private const string InsertSemesterEnrollmentQuery =
            "INSERT INTO Enrollments (Semester, StartDate, IdStudy) VALUES (@semester, @startDate, @idStudies)";
        
        private readonly InMemoryDbAccessor _db;
        private readonly IStudentsService _students;

        public EnrollmentsService(InMemoryDbAccessor db, IStudentsService students)
        {
            _db = db;
            _students = students;
        }

        public Enrollment Enroll(EnrollRequest request)
        {
            return _db.Transaction(command =>
            {
                var sqlDatabaseParameter = new SqlDatabaseParameter("@name", request.Studies);
                command.CommandText = FindStudiesQuery;
                command.Parameters.Clear();
                command.Parameters.Add(sqlDatabaseParameter);
                var reader = command.ExecuteReader();
                reader.Read();
                var idStudies = (int) reader["IdStudy"];
                EnsureFirstSemesterEnrollment(command, idStudies);
            
                var enrollment = FindFirstSemesterEnrollment(command, idStudies);
            var student = new Student
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Index = request.IndexNumber,
                BirthDate = request.BirthDate,
                IdEnrollment = enrollment.IdEnrollment
            };
            _students.AddStudent(student);
            return enrollment;
        });
        }

        public bool DoStudyExist(string studyName)
        {
            var sqlDatabaseParameter = new SqlDatabaseParameter("@name", studyName);

            var queryResult = _db.ExecuteQuery(CountStudiesQuery, new List<SqlDatabaseParameter> {sqlDatabaseParameter},
                command => command.ExecuteScalar());

            return queryResult != null && int.Parse(queryResult.ToString()) > 0;
        }

        private static void EnsureFirstSemesterEnrollment(SqlDatabaseCommand command, int studyId)
        {
            var enrollment = FindFirstSemesterEnrollment(command, studyId);

            if (enrollment != null)
            {
                Console.WriteLine("Enrollment already exists");
                return;
            }
            
            Console.WriteLine("No enrollment existing, creating one");
            CreateFirstSemesterEnrollment(command, studyId);
        }

        private static Enrollment FindFirstSemesterEnrollment(SqlDatabaseCommand command, int studyId)
        {
            command.CommandText = FindFirstSemesterEnrollmentQuery;
            command.Parameters.Clear();
            command.Parameters.Add("@idStudies", studyId);

            return Read(command.ExecuteReader());
        }

        private static void CreateFirstSemesterEnrollment(SqlDatabaseCommand command, int studyId)
        {
            command.CommandText = InsertSemesterEnrollmentQuery;
            command.Parameters.Clear();
            command.Parameters.Add("@idStudies", studyId);
            command.Parameters.Add("@semester", 1);
            command.Parameters.Add("@startDate", DateTime.Now.AsString());
            var insert = command.ExecuteNonQuery();
            Console.WriteLine("Enrollment created, inserted " + insert +" rows");
        }

        private static Enrollment Read(SqlDatabaseDataReader reader)
        {
            if (!reader.Read())
            {
                Console.WriteLine("No enrollment found");
                return null;
            }
            
            var result = new Enrollment();
            result.IdEnrollment = reader.GetInt32(reader.GetOrdinal("IdEnrollment"));
            result.Semester = reader.GetInt32(reader.GetOrdinal("Semester"));
            //result.IdStudy = reader.GetInt32(reader.GetOrdinal("IdStudy"));
            var idStudy = reader["IdStudy"];
            var idStudyString = idStudy.ToString();
            result.IdStudy = int.Parse(idStudyString);
            result.StartDate = reader["StartDate"].ToString().AsDateTime();
            return result;
        }

        public Enrollment FindById(int enrollmentId)
        {
            var parameters = new[] {new SqlDatabaseParameter("@idEnrollment", enrollmentId)};

            var reader = _db.ExecuteReadQuery(FindEnrollmentQuery, parameters);

            return Read(reader);
        }

        public List<Enrollment> FindAll()
        {
            var reader = _db.ExecuteReadQuery(FindAllQuery, null);

            var result = new List<Enrollment>();
            var current = Read(reader);
            while (current != null)
            {
                result.Add(current);
                current = Read(reader);
            }

            return result;
        }
    }
}