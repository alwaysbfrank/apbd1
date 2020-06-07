using System;
using System.Collections.Generic;
using SQLDatabase.Net.SQLDatabaseClient;

namespace APBD1.DAL
{
    public class InMemoryDbAccessor
    {
        private const string STUDIES_DDL = @"
CREATE TABLE IF NOT EXISTS Studies (
IdStudy INTEGER PRIMARY KEY AUTOINCREMENT,
Name TEXT NOT NULL
)";

        private const string ENROLLMENT_DDL = @"
CREATE TABLE IF NOT EXISTS Enrollments (
IdEnrollment INTEGER PRIMARY KEY AUTOINCREMENT,
Semester INTEGER,
IdStudy INTEGER,
StartDate TEXT,
FOREIGN KEY (IdStudy) REFERENCES Studies (IdStudy)
)";

        private const string STUDENTS_DDL = @"
CREATE TABLE IF NOT EXISTS Students (
FirstName Text,
LastName Text,
IndexNumber TEXT PRIMARY KEY NOT NULL,
IdEnrollment Integer,
BirthDate TEXT,
Password TEXT,
FOREIGN KEY (IdEnrollment) REFERENCES Enrollment (IdEnrollment)
)";

        private const string JWT_AUTH_DDL = @"
CREATE TABLE IF NOT EXISTS Auth (
IndexNumber TEXT PRIMARY KEY NOT NULL,
Password TEXT,
Salt Text,
Roles Text,
RefreshToken Text,
FOREIGN KEY (IndexNumber) REFERENCES Students (IndexNumber)
)
";

        private const string INSERT_IT_STUDY = @"
INSERT INTO Studies(Name, IdStudy) VALUES ('IT', 1)
";

        private const string INSERT_FIRST_ENROLLMENT = @"
INSERT INTO Enrollments(IdEnrollment, Semester, IdStudy, StartDate) 
VALUES (1, 1, 1, '01.01.2020')
";
        
        private const string INSERT_ADMIN_STUDENT = @"
INSERT INTO STUDENTS (FirstName, LastName, IndexNumber, IdEnrollment, BirthDate, Password)
VALUES ('Admin', 'Admin', 's0001', 1, '01.01.1980', 'admin')
";

        private readonly Guid guid;
        private readonly string _connectionString;

        public InMemoryDbAccessor()
        {
            guid = Guid.NewGuid();
            Console.WriteLine("Creating database with guid: " + guid);
            _connectionString = "SchemaName=sdbn;datasource=file://C:\\APBD\\db"+guid+".db;";
            ExecuteNonQuery(STUDIES_DDL);
            ExecuteNonQuery(ENROLLMENT_DDL);
            ExecuteNonQuery(STUDENTS_DDL);
            ExecuteNonQuery(INSERT_IT_STUDY);
            ExecuteNonQuery(INSERT_FIRST_ENROLLMENT);
            ExecuteNonQuery(INSERT_ADMIN_STUDENT);
            ExecuteNonQuery(JWT_AUTH_DDL);
        }

        private void ExecuteNonQuery(string query)
        {
            ExecuteQuery(query, null, con => con.ExecuteNonQuery());
        }
        
        public SqlDatabaseDataReader ExecuteReadQuery(string query)
        {
            return ExecuteReadQuery(query, new List<SqlDatabaseParameter>());
        }

        public SqlDatabaseDataReader ExecuteReadQuery(string query, ICollection<SqlDatabaseParameter> parameters)
        {
            return ExecuteQuery(query, parameters, command => command.ExecuteReader());
        }
        
        public T ExecuteQuery<T>(string query, ICollection<SqlDatabaseParameter> parameters, Func<SqlDatabaseCommand, T> executionFunction)
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

        public SqlDatabaseConnection CreateConnection()
        {
            return new SqlDatabaseConnection(_connectionString);
        }

        public T Transaction<T>(Func<SqlDatabaseCommand, T> calls)
        {
            using (var connection = CreateConnection())
            using (var command = new SqlDatabaseCommand())
            {
                command.Connection = connection;
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    var result = calls.Invoke(command);
                    transaction.Commit();
                    return result;
                }
                catch (SqlDatabaseException ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}