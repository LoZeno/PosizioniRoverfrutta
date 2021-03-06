﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Models.DocumentTypes;
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
            get => ProductDetails.ProductId;
            set
            {
                ProductDetails.ProductId = value;
                OnPropertyChanged();
            } 
        }

        public string Description 
        {
            get => ProductDetails.Description;
            set
            {
                ProductDetails.Description = value;
                OnPropertyChanged();
            } 
        }

        public decimal Pallets
        {
            get => ProductDetails.Pallets;
            set
            {
                ProductDetails.Pallets = value;
                OnPropertyChanged();
            }
        }

        public string PalletType
        {
            get => ProductDetails.PalletType;
            set
            {
                ProductDetails.PalletType = value;
                OnPropertyChanged();
            }
        }

        public int Packages 
        { 
            get => ProductDetails.Packages;
            set
            {
                ProductDetails.Packages = value;
                OnPropertyChanged();
            }
        }

        public decimal GrossWeight
        {
            get => ProductDetails.GrossWeight;
            set
            {
                ProductDetails.GrossWeight = value;
                OnPropertyChanged();
            }
        }

        public decimal NetWeight
        {
            get => ProductDetails.NetWeight;
            set
            {
                ProductDetails.NetWeight = value;
                OnPropertyChanged();
            }
        }

        public decimal Price
        {
            get => ProductDetails.Price;
            set
            {
                ProductDetails.Price = value;
                OnPropertyChanged();
                OnPropertyChanged("TotalPrice");
            }
        }

        public string Currency
        {
            get => ProductDetails.Currency;
            set
            {
                ProductDetails.Currency = value;
                OnPropertyChanged();
            }
        }

        public decimal PriceParameter
        {
            get => ProductDetails.PriceParameter;
            set
            {
                ProductDetails.PriceParameter = value;
                OnPropertyChanged();
                OnPropertyChanged("TotalPrice");
            }
        }

        public decimal TotalPrice => Math.Round(Price*PriceParameter, 2);

        public ProductDetails ProductDetails { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}