using System;
using System.Collections.Generic;

namespace IntegrationLab.Contracts
{
    /// <summary>
    /// DELIBERATELY BAD EXAMPLE — a domain / persistence-style object that must never cross the wire.
    /// <para>
    /// It exists only for the docs (docs/DtoContract.md) and for the "Leak a domain object" button
    /// in Window1, which assigns an instance to <c>Options.debugDump</c> so the trace can show how
    /// much (and what) reaches the browser: internal remarks, approver names, the whole
    /// <see cref="Customer"/> navigation object with its tax id and credit limit, every order line.
    /// </para>
    /// <para>
    /// Compare with <see cref="GaugeStateDto"/>: five primitives named for what the widget needs.
    /// </para>
    /// </summary>
    public class DomainWorkOrder
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? ClosedUtc { get; set; }
        public string Status { get; set; }
        public string Site { get; set; }
        public string Asset { get; set; }
        public decimal LaborHours { get; set; }
        public decimal PartsCost { get; set; }
        public decimal TotalCost { get; set; }
        public string Notes { get; set; }

        /// <summary>Internal-only text. Serializing this object ships it to the browser.</summary>
        public string InternalRemarks { get; set; }

        public string CreatedBy { get; set; }
        public string ApprovedBy { get; set; }
        public long RowVersion { get; set; }

        /// <summary>Child collection: every line travels with the parent.</summary>
        public List<DomainWorkOrderLine> Lines { get; set; } = new List<DomainWorkOrderLine>();

        /// <summary>
        /// Navigation property. This is the leak the lesson warns about: an entity with a
        /// navigation to a customer record serializes the customer too.
        /// </summary>
        public DomainCustomer Customer { get; set; }

        /// <summary>A realistic-looking instance for the failure-path demo (all values are fictional).</summary>
        public static DomainWorkOrder Sample()
        {
            return new DomainWorkOrder
            {
                Id = 48213,
                Number = "WO-2026-048213",
                CreatedUtc = new DateTime(2026, 9, 8, 6, 42, 0, DateTimeKind.Utc),
                ClosedUtc = null,
                Status = "InProgress",
                Site = "Plant North",
                Asset = "Boiler 3",
                LaborHours = 6.5m,
                PartsCost = 1240.80m,
                TotalCost = 1890.80m,
                Notes = "Burner control loop retuned; watch for threshold trips over 100°F.",
                InternalRemarks = "Customer disputes last invoice; do not escalate before Q4 review.",
                CreatedBy = "j.alvarez",
                ApprovedBy = "m.chen",
                RowVersion = 0x1A2B3C4D5E6F,
                Lines =
                {
                    new DomainWorkOrderLine { LineNo = 1, Sku = "GSK-1140", Description = "Gasket kit, flange 4in",   Quantity = 2, UnitPrice = 184.40m },
                    new DomainWorkOrderLine { LineNo = 2, Sku = "SNS-PT100", Description = "PT100 sensor, 1/2in NPT", Quantity = 1, UnitPrice = 612.00m },
                    new DomainWorkOrderLine { LineNo = 3, Sku = "SVC-CAL",   Description = "Calibration service",    Quantity = 1, UnitPrice = 260.00m },
                },
                Customer = new DomainCustomer
                {
                    Id = 907,
                    Name = "Northfield Energy Cooperative",
                    Email = "ops@northfield-energy.example",
                    Phone = "+1 555 0142 7781",
                    TaxId = "83-4471902",
                    CreditLimit = 250000m,
                    BillingAddress = "14 Turbine Road, Northfield",
                    IsKeyAccount = true,
                    AccountManager = "s.okoro",
                },
            };
        }
    }

    /// <summary>One line of a <see cref="DomainWorkOrder"/> (part of the bad example).</summary>
    public class DomainWorkOrderLine
    {
        public int LineNo { get; set; }
        public string Sku { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    /// <summary>
    /// The navigation target of <see cref="DomainWorkOrder.Customer"/>. Security-sensitive fields
    /// (tax id, credit limit, account manager) leak with the parent when the parent is serialized.
    /// </summary>
    public class DomainCustomer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string TaxId { get; set; }
        public decimal CreditLimit { get; set; }
        public string BillingAddress { get; set; }
        public bool IsKeyAccount { get; set; }
        public string AccountManager { get; set; }
    }
}
