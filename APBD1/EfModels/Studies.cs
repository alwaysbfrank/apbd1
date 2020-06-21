using System;
using System.Collections.Generic;

namespace APBD1.EfModels
{
    public partial class Studies
    {
        public Studies()
        {
            Enrollments = new HashSet<Enrollments>();
        }

        public int IdStudy { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Enrollments> Enrollments { get; set; }
    }
}
