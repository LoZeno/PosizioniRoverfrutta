using System;
using System.Collections.Generic;

namespace Models
{
    public class SaleConfirmation
    {
        public SaleConfirmation()
        {
            ProductDetails = new List<ProductDetails>();
            Provider = new Customer();
            Customer = new Customer();
            Transporter = new Transporter();
            DocumentDate = DateTime.Today;
        }

        public int Id { get; set; }
        public Customer Customer { get; set; }
        public Customer Provider { get; set; }
        public Transporter Transporter { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime? ShippingDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string TruckLicensePlate { get; set; }
        public decimal Rental { get; set; }
        public string DeliveryEx { get; set; }
        public string TermsOfPayment { get; set; }
        public decimal InvoiceDiscount { get; set; }
        public decimal CustomerCommission { get; set; }
        public decimal ProviderCommission { get; set; }
        [Obsolete]
        public List<ProductSold> Products { get; set; } 

        public List<ProductDetails> ProductDetails { get; set; }

    }
}
