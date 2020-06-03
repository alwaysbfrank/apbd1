using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OracleClient;
using System.Data.SqlClient;
using APBD1.Models;

namespace APBD1.DAL
{
    public class OracleStudentsService : IStudentsService
    {
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
            return result;
        }

        public Student GetStudent(string index)
        {
            var query = $"SELECT * FROM Student WHERE IndexNumber = @IndexNumber";
            List<OracleParameter> parameters = new List<OracleParameter>()
            {
                new OracleParameter("@IndexNumber", index)
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

            const string query = "INSERT INTO Student(IndexNumber, FirstName, LastName, IdEnrollment) VALUES (@IndexNumber, @FirstName, @LastName, @IdEnrollment)";
            var parameters = new List<OracleParameter>()
            {
                new OracleParameter("@IndexNumber", student.Index),
                new OracleParameter("@FirstNAme", student.FirstName),
                new OracleParameter("@LastName", student.LastName),
                new OracleParameter("@IdEnrollment", student.IdEnrollment)
            };

            var inserted = 1 == ExecuteQuery(query, parameters, command => command.ExecuteNonQuery());
            if (inserted)
            {
                return student;
            }
            
            throw new Exception("Database insert was unsuccessful");
        }
        
        private static OracleDataReader ExecuteReadQuery(string query)
        {
            return ExecuteReadQuery(query, new List<OracleParameter>());
        }

        private static OracleDataReader ExecuteReadQuery(string query, ICollection<OracleParameter> parameters)
        {
            return ExecuteQuery(query, parameters, command => command.ExecuteReader());
        }
        
        private static T ExecuteQuery<T>(string query, ICollection<OracleParameter> parameters, Func<OracleCommand, T> executionFunction)
        {
            using var connection = CreateConnection();
            using var odbcCommand = new OracleCommand(query, connection);
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
        
        private static OracleConnection CreateConnection()
        {
            const string connectionString =
                @"Data Source=(DESCRIPTION=(ADDRESS= (PROTOCOL=tcp)(HOST=db-oracle.pjwstk.edu.pl)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=baza)));User Id=s16289;Password=oracle12;Integrated Security=no;";

            return new OracleConnection(connectionString);
        }
    }
}