namespace APBD1.Authentication
{
    public class User
    {
        public string IndexNumber { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Roles { get; set; }
        public string RefreshToken { get; set; }
    }
}