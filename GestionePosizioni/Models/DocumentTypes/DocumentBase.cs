using System;
using System.Collections.Generic;
using Models.Companies;

namespace Models.DocumentTypes
{
    public abstract class DocumentBase
    {
        protected DocumentBase()
        {
            ProductDetails = new List<ProductDetails>();
            Provider = new Customer();
            Customer = new Customer();
            Transporter = new Transporter(); 
            DocumentDate = DateTime.Today.Date;
            Notes = "Le coordinate bancarie verranno indicate sulla fattura.";
        }

        public string Id { get; set; }

        public int ProgressiveNumber
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    var strings = Id.Split('/');
                    if (strings.Length > 1 && !string.IsNullOrWhiteSpace(strings[1]))
                        return int.Parse(strings[1]);
                    return 0;
                }
                return 0;
            }
        }

        public Customer Customer { get; set; }
        public Customer Provider { get; set; }
        public Transporter Transporter { get; set; }
        public DateTime? DocumentDate { get; set; }

        public string DocumentDateString
        {
            get
            {
                return DocumentDate.HasValue ? DocumentDate.Value.ToShortDateString() : string.Empty;
            }
        }

        public DateTime? ShippingDate { get; set; }

        public string ShippingDateString 
        { 
            get
            {
                return ShippingDate.HasValue ? ShippingDate.Value.ToShortDateString() : string.Empty;
            } 
        }

        public DateTime? DeliveryDate { get; set; }

        public string DeliveryDateString
        {
            get
            {
                return DeliveryDate.HasValue ? DeliveryDate.Value.ToShortDateString() : string.Empty;
            }
        }

        public string TruckLicensePlate { get; set; }
        public decimal? Rental { get; set; }
        public string DeliveryEx { get; set; }
        public string TermsOfPayment { get; set; }
        public decimal? InvoiceDiscount { get; set; }
        public decimal? CustomerCommission { get; set; }
        public decimal? ProviderCommission { get; set; }
        public string Notes { get; set; }
        public string Lot { get; set; }
        public string OrderCode { get; set; }
        public List<ProductDetails> ProductDetails { get; set; }
    }
}