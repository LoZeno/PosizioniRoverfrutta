namespace Models.Companies
{
    public abstract class CompanyBase
    {
        private string _companyName ;
        public string Id { get; set; }
        public string CompanyName 
        {
            get
            {
                return string.IsNullOrWhiteSpace(_companyName) ? string.Empty : _companyName;
            }
            set
            {
                _companyName = value;
            } 
        }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateOrProvince { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string VatCode { get; set; }
        public bool DoNotApplyVat { get; set; }
    }
}