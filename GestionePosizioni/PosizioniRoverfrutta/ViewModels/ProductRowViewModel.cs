using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Models;
using PosizioniRoverfrutta.Annotations;
using PosizioniRoverfrutta.ViewModels.BaseClasses;

namespace PosizioniRoverfrutta.ViewModels
{
    public class ProductRowViewModel : EditableModelBase<ProductRowViewModel>, INotifyPropertyChanged
    {
        public ProductRowViewModel() : this(new ProductDetails())
        {
            Currency = "Euro";
        }

        public ProductRowViewModel(ProductDetails productDetails)
        {
            ProductDetails = productDetails;
        }

        public int? ProductId 
        {
            get { return ProductDetails.ProductId; }
            set
            {
                ProductDetails.ProductId = value;
                OnPropertyChanged();
            } 
        }

        public string Description 
        {
            get { return ProductDetails.Description; }
            set
            {
                ProductDetails.Description = value;
                OnPropertyChanged();
            } 
        }

        public int Pallets
        {
            get { return ProductDetails.Pallets; }
            set
            {
                ProductDetails.Pallets = value;
                OnPropertyChanged();
            }
        }

        public int Packages 
        { 
            get { return ProductDetails.Packages; }
            set
            {
                ProductDetails.Packages = value;
                OnPropertyChanged();
            }
        }

        public decimal GrossWeight
        {
            get { return ProductDetails.GrossWeight; }
            set
            {
                ProductDetails.GrossWeight = value;
                OnPropertyChanged();
            }
        }

        public decimal NetWeight
        {
            get { return ProductDetails.NetWeight; }
            set
            {
                ProductDetails.NetWeight = value;
                OnPropertyChanged();
            }
        }

        public decimal Price
        {
            get { return ProductDetails.Price; }
            set
            {
                ProductDetails.Price = value;
                OnPropertyChanged();
                OnPropertyChanged("TotalPrice");
            }
        }

        public string Currency
        {
            get { return ProductDetails.Currency; }
            set
            {
                ProductDetails.Currency = value;
                OnPropertyChanged();
            }
        }

        public decimal PriceParameter
        {
            get { return ProductDetails.PriceParameter; }
            set
            {
                ProductDetails.PriceParameter = value;
                OnPropertyChanged();
                OnPropertyChanged("TotalPrice");
            }
        }

        public decimal TotalPrice
        {
            get { return Math.Round(Price*PriceParameter, 2); }
        }

        public ProductDetails ProductDetails { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}