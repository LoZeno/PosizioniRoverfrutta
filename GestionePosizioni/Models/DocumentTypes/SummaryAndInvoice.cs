using System;
using System.Collections.Generic;
using Models.Companies;
using Models.Entities;

namespace Models.DocumentTypes
{
    public class SummaryAndInvoice
    {
        public Customer Customer { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<SummaryRow> SummaryRows { get; set; }
        public decimal CommissionsTotal { get; set; }
    }
}