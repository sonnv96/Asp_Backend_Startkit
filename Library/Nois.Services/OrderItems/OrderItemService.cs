using System.Linq;
using System.Threading.Tasks;
using Nois.Framework.Services;
using Nois.Framework.Data;
using Nois.Framework.Caching;
using Nois.Core.Domain;
using Nois.Helper;

namespace Nois.Services.OrderItems
{
    /// <summary>
    /// OrderItem service
    /// </summary>
    public class OrderItemService : BaseService<OrderItem>, IOrderItemService
    {
        private readonly IRepository<OrderItem> _tRepository;
        private readonly ICacheManager _cacheManager;
        public OrderItemService(IRepository<OrderItem> tRepository,
            ICacheManager cacheManager) : base(tRepository, cacheManager)
        {
            this._tRepository = tRepository;
            this._cacheManager = cacheManager;
        }

        protected override string PatternKey
        {
            get
            {
                return "Nois.OrderItem.";
            }
        }

        public IPagedList<OrderItem> GetOrderItemListPaging(string textSearch, int pageIndex, int pageSize, string sortField, bool orderDescending)
        {

            var query = _tRepository.Table;

            if (!string.IsNullOrEmpty(textSearch))
                query = query.Where(x => x.Name.Contains(textSearch));

            query = query.OrderByDynamic(sortField, "Name", orderDescending);
            return new PagedList<OrderItem>(query, pageIndex, pageSize);
            //throw new System.NotImplementedException();
        }
    }
}
