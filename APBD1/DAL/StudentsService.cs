using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using APBD1.Models;
using APBD1.Utils;
using SQLDatabase.Net.SQLDatabaseClient;

namespace APBD1.DAL
{
    public class StudentsService : IStudentsService
    {
        private readonly InMemoryDbAccessor _db;

        public StudentsService(InMemoryDbAccessor dbAccessor)
        {
            _db = dbAccessor;
        }
        
        public ICollection<Student> GetStudents()
        {
            const string query = "SELECT * FROM Students";
            var reader = _db.ExecuteReadQuery(query);
            var result = new List<Student>();
            while (reader.Read())
            {
                result.Add(ReadFrom(reader));
            }

            return result;
        }

        private static Student ReadFrom(IDataRecord reader)
        {
            var result = new Student();
            result.Index = reader["IndexNumber"].ToString();
            result.FirstName = reader["FirstName"].ToString();
            result.LastName = reader["LastName"].ToString();
            result.IdEnrollment = reader.GetInt32(reader.GetOrdinal("IdEnrollment"));
            result.BirthDate = reader["BirthDate"].ToString().AsDateTime();
            return result;
        }

        public Student GetStudent(string index)
        {
            var query = $"SELECT * FROM Students WHERE IndexNumber = @IndexNumber";
            List<SqlDatabaseParameter> parameters = new List<SqlDatabaseParameter>()
            {
                new SqlDatabaseParameter("@IndexNumber", index)
            };

            var reader = _db.ExecuteReadQuery(query, parameters);
            if (reader.Read())
            {
                return ReadFrom(reader);
            }
            
            throw new Exception($"No student with index {index} found");
        }

        public Student AddStudent(Student student)
        {
            return AddStudent(student, null);
        }
        
        public Student AddStudent(Student student, SqlDatabaseCommand command)
        {
            var currentCount = GetStudents().Count;
            student.Index = $"s{currentCount + 1}";

            const string query = "INSERT INTO Students(IndexNumber, FirstName, LastName, IdEnrollment, BirthDate) VALUES (@IndexNumber, @FirstName, @LastName, @IdEnrollment, @BirthDate)";
            var parameters = new List<SqlDatabaseParameter>()
            {
                new SqlDatabaseParameter("@IndexNumber", student.Index),
                new SqlDatabaseParameter("@FirstName", student.FirstName),
                new SqlDatabaseParameter("@LastName", student.LastName),
                new SqlDatabaseParameter("@IdEnrollment", student.IdEnrollment),
                new SqlDatabaseParameter("@BirthDate", student.BirthDate.AsString())
            };

            bool inserted;

            if (command == null)
            {
                inserted = 1 == _db.ExecuteQuery(query, parameters, command => command.ExecuteNonQuery());
            }
            else
            {
                command.CommandText = query;
                command.Parameters.Clear();
                parameters.ForEach(parameter => command.Parameters.Add(parameter));
                inserted = 1 == command.ExecuteNonQuery();
            }
            
            if (inserted)
            {
                return student;
            }
            
            throw new Exception("Database insert was unsuccessful");
        }
        
    }
}