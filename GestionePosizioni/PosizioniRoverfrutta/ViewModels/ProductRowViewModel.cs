using Models;
using PosizioniRoverfrutta.ViewModels.BaseClasses;

namespace PosizioniRoverfrutta.ViewModels
{
    public class ProductRowViewModel : EditableModelBase<ProductRowViewModel>
    {
        public ProductRowViewModel() : this(new ProductDetails())
        {
        }

        public ProductRowViewModel(ProductDetails productDetails)
        {
            ProductDetails = productDetails;
        }

        public int ProductId 
        {
            get { return ProductDetails.ProductId; }
            set { ProductDetails.ProductId = value; } 
        }

        public string Description 
        {
            get { return ProductDetails.Description; }
            set { ProductDetails.Description = value; } 
        }

        public int Pallets
        {
            get { return ProductDetails.Pallets; }
            set { ProductDetails.Pallets = value; }
        }

        public int Packages 
        { 
            get { return ProductDetails.Packages; }
            set { ProductDetails.Packages = value; }
        }

        public decimal GrossWeight
        {
            get { return ProductDetails.GrossWeight; }
            set { ProductDetails.GrossWeight = value; }
        }

        public decimal Price
        {
            get { return ProductDetails.Price; }
            set { ProductDetails.Price = value; }
        }

        public string Currency
        {
            get { return ProductDetails.Currency; }
            set { ProductDetails.Currency = value; }
        }

        public decimal PriceParameter
        {
            get { return ProductDetails.PriceParameter; }
            set { ProductDetails.PriceParameter = value; }
        }

        public ProductDetails ProductDetails { get; set; }
    }
}