using System.Linq;
using System.Threading.Tasks;
using Nois.Core.Domain;
using Nois.Framework.Services;

namespace Nois.Services.Products
{
    /// <summary>
    /// Product service interface
    /// </summary>
    public interface IProductService : IBaseService<Product>
    {
        //Product GetProductByName(string name);
        IPagedList<Product> GetProductListPaging(int pageIndex, int pageSize, string textSearch, string sortField, bool orderDescending);
    }
}
