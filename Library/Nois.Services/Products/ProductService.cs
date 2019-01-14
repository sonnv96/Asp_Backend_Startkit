using System.Linq;
using System.Threading.Tasks;
using Nois.Framework.Services;
using Nois.Framework.Data;
using Nois.Framework.Caching;
using Nois.Core.Domain;
using Nois.Helper;

namespace Nois.Services.Products
{
    /// <summary>
    /// Product service
    /// </summary>
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IRepository<Product> _tRepository;
        private readonly ICacheManager _cacheManager;
        public ProductService(IRepository<Product> tRepository,
            ICacheManager cacheManager) : base(tRepository, cacheManager)
        {
            this._tRepository = tRepository;
            this._cacheManager = cacheManager;
        }

        protected override string PatternKey
        {
            get
            {
                return "Nois.Product.";
            }
        }

        //public Product GetProductByName(string name)
        //{
        //    throw new System.NotImplementedException();
        //}

        public IPagedList<Product> GetProductListPaging(int pageIndex = 0, int pageSize = int.MaxValue, string textSearch = null, 
            string sortField = null, bool orderDescending = false)
        {
            var query = _tRepository.Table;
            if (!string.IsNullOrEmpty(textSearch))
                query = query.Where(x => x.Name.Contains(textSearch));
            query = query.OrderByDynamic(sortField, "Name", orderDescending);
            return new PagedList<Product>(query, pageIndex, pageSize);
            //throw new System.NotImplementedException();
        }
    }
}
