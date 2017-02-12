using System;
using Models.DocumentTypes;

namespace Models.Mappers
{
    public static class LoadingDocumentMapper
    {
        public static LoadingDocument MapToLoadingDocument(this SaleConfirmation saleConfirmation)
        {
            return new LoadingDocument
            {
                Id = "LoadingDocuments/" + saleConfirmation.ProgressiveNumber,
                Customer = saleConfirmation.Customer.MapToNewCompany(),
                Provider = saleConfirmation.Provider.MapToNewCompany(),
                Transporter = saleConfirmation.Transporter.MapToNewCompany(),
                DocumentDate = DateTime.Today,
                ProductDetails = saleConfirmation.ProductDetails,
                ShippingDate = saleConfirmation.ShippingDate,
                DeliveryDate = saleConfirmation.DeliveryDate,
                TruckLicensePlate = saleConfirmation.TruckLicensePlate,
                Rental = saleConfirmation.Rental,
                DeliveryEx = saleConfirmation.DeliveryEx,
                TermsOfPayment = saleConfirmation.TermsOfPayment,
                InvoiceDiscount = saleConfirmation.InvoiceDiscount,
                CustomerCommission = saleConfirmation.CustomerCommission,
                ProviderCommission = saleConfirmation.ProviderCommission,
                Notes = saleConfirmation.Notes,
                Lot = saleConfirmation.Lot,
                OrderCode = saleConfirmation.OrderCode
            };
        }
    }
}
