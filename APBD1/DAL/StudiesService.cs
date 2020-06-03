using System.Collections.Generic;
using APBD1.Models;
using Microsoft.VisualBasic;
using SQLDatabase.Net.SQLDatabaseClient;

namespace APBD1.DAL
{
    public class StudiesService
    {
        private const string FindQuery = "SELECT * FROM Studies WHERE IdStudy = @IdStudies";
        private const string FindByNameQuery = "SELECT * FROM Studies WHERE Name = @Name";
        private const string InsertQuery = "INSERT INTO Studies(Name) VALUES(@Name)";
        
        private readonly InMemoryDbAccessor _db;

        public StudiesService(InMemoryDbAccessor dbAccessor)
        {
            _db = dbAccessor;
        }

        public Studies GetById(int studyId)
        {
            var parameters = new[] {new SqlDatabaseParameter("@IdStudies", studyId)};

            var reader = _db.ExecuteReadQuery(FindQuery, parameters);

            return Read(reader);
        }

        private Studies Read(SqlDatabaseDataReader reader)
        {
            if (!reader.Read())
            {
                return null;
            }
            
            var result = new Studies();
            result.IdStudy = reader.GetInt32(reader.GetOrdinal("IdStudy"));
            result.Name = reader["Name"].ToString();

            return result;
        }

        public Studies Create(string name)
        {
            var parameters = new[] {new SqlDatabaseParameter("@Name", name)};
            var insertedRows = _db.ExecuteQuery(InsertQuery, parameters, command => command.ExecuteNonQuery());

            if (insertedRows < 1)
            {
                return null;
            }
            
            var reader = _db.ExecuteReadQuery(FindByNameQuery, parameters);

            return Read(reader);
        }
    }
}