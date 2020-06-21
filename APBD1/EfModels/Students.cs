using System;
using System.Collections.Generic;

namespace APBD1.EfModels
{
    public partial class Students
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IndexNumber { get; set; }
        public int? IdEnrollment { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Password { get; set; }

        public virtual Enrollments IdEnrollmentNavigation { get; set; }
        public virtual Auth Auth { get; set; }
    }
}
