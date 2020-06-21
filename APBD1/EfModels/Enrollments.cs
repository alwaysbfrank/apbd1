using System;
using System.Collections.Generic;

namespace APBD1.EfModels
{
    public partial class Enrollments
    {
        public Enrollments()
        {
            Students = new HashSet<Students>();
        }

        public int IdEnrollment { get; set; }
        public int? Semester { get; set; }
        public int? IdStudy { get; set; }
        public DateTime? StartDate { get; set; }

        public virtual Studies IdStudyNavigation { get; set; }
        public virtual ICollection<Students> Students { get; set; }
    }
}
