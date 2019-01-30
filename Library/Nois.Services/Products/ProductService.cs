using System.Linq;
using System.Threading.Tasks;
using Nois.Framework.Services;
using Nois.Framework.Data;
using Nois.Framework.Caching;
using Nois.Core.Domain;
using Nois.Helper;
using System.Collections.Generic;
using System;

namespace Nois.Services.Products
{
    /// <summary>
    /// Product service
    /// </summary>
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IRepository<Product> _tRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly ICacheManager _cacheManager;
        public ProductService(IRepository<Product> tRepository,
            IRepository<Category> categoryRepository,
            ICacheManager cacheManager) : base(tRepository, cacheManager)
        {
            this._tRepository = tRepository;
            this._categoryRepository = categoryRepository;
            this._cacheManager = cacheManager;
        }

        protected override string PatternKey
        {
            get
            {
                return "Nois.Product.";
            }
        }

        public List<Product> GetProductByCategory(int? categoryID, int pageIndex, int pageSize, string textSearch, string sortField, bool orderDescending)
        {
            var query = _tRepository.Table;

            if (categoryID.HasValue)
                query = query.Where(x => x.CategoryId == categoryID);
            if (!string.IsNullOrEmpty(textSearch))
                query = query.Where(x => x.Name.Contains(textSearch));
            query = query.OrderByDynamic(sortField, "Name", orderDescending);
            return new PagedList<Product>(query, pageIndex, pageSize);
        }

        public List<Product> GetProductByStore(int? storeId = null, int pageIndex = 0, int pageSize = int.MaxValue, string textSearch = null, 
            string sortField = null, bool orderDescending = false)
        {
            var query = _tRepository.Table;

            if (storeId.HasValue)
                query = query.Where(x => x.StoreId == storeId);
            if (!string.IsNullOrEmpty(textSearch))
                query = query.Where(x => x.Name.Contains(textSearch));
            query = query.OrderByDynamic(sortField, "Name", orderDescending);
            return new PagedList<Product>(query, pageIndex, pageSize);
        }

        //public Product GetProductByName(string name)
        //{
        //    throw new System.NotImplementedException();
        //}

        public IPagedList<Product> GetProductListPaging(string textSearch = null, 
            int? categoryId = null, DateTime? dateFrom = null, DateTime? dateTo = null,
            int pageIndex = 0, int pageSize = int.MaxValue, 
            string sortField = null, bool orderDescending = false)
        {
            var query = _tRepository.Table;
            var queryz = _categoryRepository.Table;

            if(dateFrom.HasValue)
                query = query.Where(x => x.DatePurchase >= dateFrom);

            if(dateTo.HasValue)
                query = query.Where(x => x.DatePurchase <= dateTo);
           
            if (categoryId.HasValue)
                query = query.Where(x => x.CategoryId == categoryId.Value);

            if (!string.IsNullOrEmpty(textSearch))
                query = query.Where(x => x.Name.Contains(textSearch));

            query = query.OrderByDynamic(sortField, "Name", orderDescending);
            return new PagedList<Product>(query, pageIndex, pageSize);
            //throw new System.NotImplementedException();
        }
    }
}
