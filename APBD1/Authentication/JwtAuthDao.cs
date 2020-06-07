using System;
using APBD1.DAL;
using APBD1.Utils;
using SQLDatabase.Net.SQLDatabaseClient;

namespace APBD1.Authentication
{
    public class JwtAuthDao
    {
        private const string FIND_PASSWORD_QUERY = @"
SELECT * FROM Auth WHERE IndexNumber = @index
";

        private const string SET_PASSWORD_QUERY = @"
INSERT INTO Auth(Password, Salt, IndexNumber) VALUES (@newPassword, @newSalt, @index)
";
        
        private const string SET_REFRESH_TOKEN_QUERY = @"
UPDATE Auth SET RefreshToken = (@newToken) WHERE IndexNumber = @index
";

        private const string SET_ROLES_QUERY = @"
UPDATE Auth SET Roles = (@newRoles) WHERE IndexNumber = @index
";
        
        private readonly InMemoryDbAccessor _db;

        public JwtAuthDao(InMemoryDbAccessor db)
        {
            _db = db;
            Init();
        }

        private void Init()
        {
            SetPassword("admin", "s0001");
            SetRoles("admin", "s0001");
        }

        public User Find(string index)
        {
            
            var dataReader = _db.ExecuteReadQuery(FIND_PASSWORD_QUERY, new[]{new SqlDatabaseParameter("@index", index)});
            var user = Read(dataReader);
            return user;
        }

        public void SetPassword(string password, string index)
        {
            "Setting new password".Log();
            var hasher = new PasswordHasher(password);
            var parameters = new SqlDatabaseParameter[]
            {
                new SqlDatabaseParameter("@index", index),
                new SqlDatabaseParameter("@newPassword", hasher.GetHashed()),
                new SqlDatabaseParameter("@newSalt", hasher.Salt), 
            };

            var inserted = _db.ExecuteQuery(SET_PASSWORD_QUERY, parameters, com => com.ExecuteNonQuery());

            if (inserted < 1)
            {
                "No rows inserted".Log();
            }
            else
            {
                "New password set".Log();
            }
        }

        public void SetRoles(string roles, string index)
        {
            "Setting new roles".Log();
            var parameters = new SqlDatabaseParameter[]
            {
                new SqlDatabaseParameter("@index", index),
                new SqlDatabaseParameter("@newRoles", roles), 
            };
            
            var inserted = _db.ExecuteQuery(SET_ROLES_QUERY, parameters, com => com.ExecuteNonQuery());

            if (inserted < 1)
            {
                "No rows inserted".Log();
            }
            else
            {
                "New roles set".Log();
            }
        }
        
        public void SetRefreshToken(string token, string index)
        {
            "Setting new token".Log();
            var parameters = new SqlDatabaseParameter[]
            {
                new SqlDatabaseParameter("@index", index),
                new SqlDatabaseParameter("@newToken", token), 
            };

            var inserted = _db.ExecuteQuery(SET_REFRESH_TOKEN_QUERY, parameters, com => com.ExecuteNonQuery());

            if (inserted < 1)
            {
                "No rows inserted".Log();
            }
            else
            {
                "New token set".Log();
            }
        }

        private static User Read(SqlDatabaseDataReader reader)
        {
            if (reader.Read())
            {
                "User found".Log();
                return new User
                {
                    IndexNumber = reader["IndexNumber"]?.ToString(),
                    Password = reader["Password"]?.ToString(),
                    RefreshToken = reader["RefreshToken"]?.ToString(),
                    Roles = reader["Roles"]?.ToString(),
                    Salt = reader["Salt"]?.ToString()
                };
            }
            
            "No user found".Log();
            return null;
        }
    }
}