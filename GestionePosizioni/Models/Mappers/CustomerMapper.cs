using Models.Companies;

namespace Models.Mappers
{
    public static class CustomerMapper
    {
        public static Customer MapToNewCompany(this Customer company)
        {
            return new Customer
            {
                Address = company.Address,
                City = company.City,
                CompanyName = company.CompanyName,
                Country = company.Country,
                DoNotApplyVat = company.DoNotApplyVat,
                EmailAddress = company.EmailAddress,
                Id = company.Id,
                PostCode = company.PostCode,
                StateOrProvince = company.StateOrProvince,
                VatCode = company.VatCode
            };
        }
    }
}