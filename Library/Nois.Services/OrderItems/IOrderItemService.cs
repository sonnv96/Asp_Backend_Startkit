using System.Linq;
using System.Threading.Tasks;
using Nois.Core.Domain;
using Nois.Framework.Services;

namespace Nois.Services.OrderItems
{
    /// <summary>
    /// OrderItem service interface
    /// </summary>
    public interface IOrderItemService : IBaseService<OrderItem>
    {
        IPagedList<OrderItem> GetOrderItemListPaging(string textSearch, int pageIndex, int pageSize, string sortField, bool orderDescending);
    }
}
