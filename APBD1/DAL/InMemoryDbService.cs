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
    public class InMemoryDbService : IDbService
    {
        private const string STUDIES_DDL = @"
CREATE TABLE IF NOT EXISTS Studies (
IdStudy INTEGER PRIMARY KEY NOT NULL,
Name TEXT NOT NULL
)";

        private const string ENROLLMENT_DDL = @"
CREATE TABLE IF NOT EXISTS Enrollment (
IdEnrollment INTEGER PRIMARY KEY NOT NULL,
Semester INTEGER,
IdStudy INTEGER,
StartDate TEXT,
FOREIGN KEY (IdStudy) REFERENCES Studies (IdStudy)
)";

        private const string STUDENTS_DDL = @"
CREATE TABLE IF NOT EXISTS Student (
FirstName Text,
LastName Text,
IndexNumber TEXT PRIMARY KEY NOT NULL,
IdEnrollment Integer,
BirthDate TEXT,
FOREIGN KEY (IdEnrollment) REFERENCES Enrollment (IdEnrollment)
)";

        static InMemoryDbService()
        {
            var emptyList = new List<SqlDatabaseParameter>();
            ExecuteQuery(STUDIES_DDL, emptyList, con => con.ExecuteNonQuery());
            ExecuteQuery(ENROLLMENT_DDL, emptyList, con => con.ExecuteNonQuery());
            ExecuteQuery(STUDENTS_DDL, emptyList, con => con.ExecuteNonQuery());
        }
        
        public ICollection<Student> GetStudents()
        {
            const string query = "SELECT * FROM Student";
            var reader = ExecuteReadQuery(query);
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
            var query = $"SELECT * FROM Student WHERE IndexNumber = @IndexNumber";
            List<SqlDatabaseParameter> parameters = new List<SqlDatabaseParameter>()
            {
                new SqlDatabaseParameter("@IndexNumber", index)
            };

            var reader = ExecuteReadQuery(query, parameters);
            if (reader.Read())
            {
                return ReadFrom(reader);
            }
            
            throw new Exception($"No student with index {index} found");
        }

        public Student AddStudent(Student student)
        {
            var currentCount = GetStudents().Count;
            student.Index = $"s{currentCount + 1}";

            const string query = "INSERT INTO Student(IndexNumber, FirstName, LastName, IdEnrollment, BirthDate) VALUES (@IndexNumber, @FirstName, @LastName, @IdEnrollment, @BirthDate)";
            var parameters = new List<SqlDatabaseParameter>()
            {
                new SqlDatabaseParameter("@IndexNumber", student.Index),
                new SqlDatabaseParameter("@FirstName", student.FirstName),
                new SqlDatabaseParameter("@LastName", student.LastName),
                new SqlDatabaseParameter("@IdEnrollment", student.IdEnrollment),
                new SqlDatabaseParameter("@BirthDate", student.BirthDate.AsString())
            };

            var inserted = 1 == ExecuteQuery(query, parameters, command => command.ExecuteNonQuery());
            if (inserted)
            {
                return student;
            }
            
            throw new Exception("Database insert was unsuccessful");
        }
        
        private static SqlDatabaseDataReader ExecuteReadQuery(string query)
        {
            return ExecuteReadQuery(query, new List<SqlDatabaseParameter>());
        }

        private static SqlDatabaseDataReader ExecuteReadQuery(string query, ICollection<SqlDatabaseParameter> parameters)
        {
            return ExecuteQuery(query, parameters, command => command.ExecuteReader());
        }
        
        private static T ExecuteQuery<T>(string query, ICollection<SqlDatabaseParameter> parameters, Func<SqlDatabaseCommand, T> executionFunction)
        {
            using var connection = CreateConnection();
            using var odbcCommand = new SqlDatabaseCommand(query, connection);
            if (parameters != null && parameters.Count != 0)
            {
                foreach (var parameter in parameters)
                {
                    odbcCommand.Parameters.Add(parameter);
                }
            }
            connection.Open();
            return executionFunction.Invoke(odbcCommand);
        }
        
        private static SqlDatabaseConnection CreateConnection()
        {
            const string connectionString =
                @"SchemaName=sdbn;datasource=file://C:\APBD\db3.db;";

            return new SqlDatabaseConnection(connectionString);
        }
    }
}