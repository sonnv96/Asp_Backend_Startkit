using System.Linq;
using System.Threading.Tasks;
using Nois.Framework.Services;
using Nois.Framework.Data;
using Nois.Framework.Caching;
using Nois.Core.Domain;
using Nois.Helper;

namespace Nois.Services.Stores
{
    /// <summary>
    /// Store service
    /// </summary>
    public class StoreService : BaseService<Store>, IStoreService
    {
        private readonly IRepository<Store> _tRepository;
        private readonly ICacheManager _cacheManager;
        public StoreService(IRepository<Store> tRepository,
            ICacheManager cacheManager) : base(tRepository, cacheManager)
        {
            this._tRepository = tRepository;
            this._cacheManager = cacheManager;
        }

        protected override string PatternKey
        {
            get
            {
                return "Nois.Store.";
            }
        }

        public IPagedList<Store> GetProductListPaging(int pageIndex = 0, int pageSize = int.MaxValue, string textSearch = null, string sortField = null, bool orderDescending = false)
        {
            var query = _tRepository.Table;
            if (!string.IsNullOrEmpty(textSearch))
                query = query.Where(x => x.Name.Contains(textSearch));
            query = query.OrderByDynamic(sortField, "Name", orderDescending);
            return new PagedList<Store>(query, pageIndex, pageSize);
        }
    }
}
