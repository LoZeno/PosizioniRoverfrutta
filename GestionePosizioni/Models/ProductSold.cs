using System.Collections.Generic;
using System.ComponentModel;

namespace Models
{
    public class ProductSold : IEditableObject
    {
        struct ProductData
        {
            internal int productId;
            internal string productDescription;
            internal int pallets;
            internal int packages;
            internal decimal grossWeight;
            internal decimal netWeight;
            internal decimal price;
            internal string currency;
            internal decimal priceParameter;
        }

        private List<ProductSold> parent;
        private ProductData custData;
        private ProductData backupData;
        private bool inTransaction = false;

        public int ProductId
        {
            get { return custData.productId; }
            set { custData.productId = value; }
        }

        public string ProductDescription
        {
            get { return custData.productDescription; }
            set { custData.productDescription = value; }
        }

        public int Pallets
        {
            get { return custData.pallets;}
            set { custData.pallets = value; }
        }

        public int Packages
        {
            get { return custData.packages; }
            set { custData.packages = value; }
        }

        public decimal GrossWeight
        {
            get { return custData.grossWeight; }
            set { custData.grossWeight = value; }
        }

        public decimal NetWeight
        {
            get { return custData.netWeight; }
            set { custData.netWeight = value; }
        }

        public decimal Price
        {
            get { return custData.price; }
            set { custData.price = value; }
        }

        public string Currency
        {
            get { return custData.currency; }
            set { custData.currency = value; }
        }

        public decimal PriceParameter
        {
            get { return custData.priceParameter; }
            set { custData.priceParameter = value; }
        }

        public void BeginEdit()
        {
            if (!inTransaction)
            {
                backupData = custData;
                inTransaction = true;
            }
        }

        public void EndEdit()
        {
            if (inTransaction)
            {
                backupData = new ProductData();
                inTransaction = false;
            }
        }

        public void CancelEdit()
        {
            if (inTransaction)
            {
                custData = backupData;
                inTransaction = false;
            }
        }
    }
}
