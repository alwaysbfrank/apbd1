using System;
using System.Collections.Generic;
using System.Text;

namespace APBD1.Dtos.Ef
{
    public class StudentRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
