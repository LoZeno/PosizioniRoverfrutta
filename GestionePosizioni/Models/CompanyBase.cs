﻿namespace Models
{
    public abstract class CompanyBase
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateOrProvince { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string VatCode { get; set; }
    }
}