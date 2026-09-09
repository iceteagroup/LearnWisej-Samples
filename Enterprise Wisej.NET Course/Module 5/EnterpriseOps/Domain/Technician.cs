using System;
using System.Collections.Generic;

namespace EnterpriseOps.Domain
{
    /// <summary>A field technician a work order can be assigned to, with the certifications they hold.</summary>
    public sealed class Technician
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public HashSet<string> Certifications { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public bool Holds(string certification) =>
            string.IsNullOrEmpty(certification) || Certifications.Contains(certification);
    }
}
