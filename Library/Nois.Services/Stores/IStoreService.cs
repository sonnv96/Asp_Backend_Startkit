using System.Linq;
using System.Threading.Tasks;
using Nois.Core.Domain;
using Nois.Framework.Services;

namespace Nois.Services.Stores
{
    /// <summary>
    /// Store service interface
    /// </summary>
    public interface IStoreService : IBaseService<Store>
    {
        IPagedList<Store> GetProductListPaging(int pageIndex, int pageSize, string textSearch, string sortField, bool orderDescending);
    }
}
