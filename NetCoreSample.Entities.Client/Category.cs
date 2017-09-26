namespace NetCoreSample.Entities.Client
{
    using System;
    using System.Collections.Generic;
	using TrackableEntities.Client;

    public partial class Category : EntityBase
    {
        public Category()
        {
            Products = new ChangeTrackingCollection<Product>();
        }

		public int CategoryId
		{ 
			get { return _CategoryId; }
			set
			{
				if (Equals(value, _CategoryId)) return;
				_CategoryId = value;
				NotifyPropertyChanged();
			}
		}
		private int _CategoryId;

		public string CategoryName
		{ 
			get { return _CategoryName; }
			set
			{
				if (Equals(value, _CategoryName)) return;
				_CategoryName = value;
				NotifyPropertyChanged();
			}
		}
		private string _CategoryName;

		public ChangeTrackingCollection<Product> Products
		{
			get { return _Products; }
			set
			{
				if (Equals(value, _Products)) return;
				_Products = value;
				NotifyPropertyChanged();
			}
		}
		private ChangeTrackingCollection<Product> _Products;
    }
}
