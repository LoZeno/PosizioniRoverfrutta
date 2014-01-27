using System;
using System.Collections.Generic;

namespace Models
{
    public class SaleConfirmation
    {
        public SaleConfirmation()
        {
            Products = new List<ProductSold>();
        }

        public int Id { get; set; }
        public Customer Customer { get; set; }
        public Provider Provider { get; set; }
        public Transporter Transporter { get; set; }
        public DateTime? ShippingDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string TruckLicensePlate { get; set; }
        public decimal Rental { get; set; }
        public string DeliveryEx { get; set; }
        public string TermsOfPayment { get; set; }
        public decimal InvoiceDiscount { get; set; }
        public decimal CustomerCommission { get; set; }
        public decimal ProviderCommission { get; set; }
        public List<ProductSold> Products { get; set; } 

    }
}
