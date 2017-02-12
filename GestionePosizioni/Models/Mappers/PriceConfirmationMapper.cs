using System;
using Models.DocumentTypes;

namespace Models.Mappers
{
    public static class PriceConfirmationMapper
    {
        public static PriceConfirmation MapToPriceConfirmation(this LoadingDocument loadingDocument)
        {
            return new PriceConfirmation
            {
                Id = "PriceConfirmations/" + loadingDocument.ProgressiveNumber,
                Customer = loadingDocument.Customer.MapToNewCompany(),
                Provider = loadingDocument.Provider.MapToNewCompany(),
                Transporter = loadingDocument.Transporter.MapToNewCompany(),
                DocumentDate = DateTime.Today,
                ProductDetails = loadingDocument.ProductDetails,
                ShippingDate = loadingDocument.ShippingDate,
                DeliveryDate = loadingDocument.DeliveryDate,
                TruckLicensePlate = loadingDocument.TruckLicensePlate,
                Rental = loadingDocument.Rental,
                DeliveryEx = loadingDocument.DeliveryEx,
                TermsOfPayment = loadingDocument.TermsOfPayment,
                InvoiceDiscount = loadingDocument.InvoiceDiscount,
                CustomerCommission = loadingDocument.CustomerCommission,
                ProviderCommission = loadingDocument.ProviderCommission,
                Notes = loadingDocument.Notes,
                Lot = loadingDocument.Lot,
                OrderCode = loadingDocument.OrderCode,
                TransportDocument = loadingDocument.TransportDocument
            };
        }
    }
}
