using System.Linq;
using System.Threading.Tasks;
using Nois.Core.Domain;
using Nois.Framework.Services;

namespace Nois.Services.Categories
{
    /// <summary>
    /// Category service interface
    /// </summary>
    public interface ICategoryService : IBaseService<Category>
    {
        IPagedList<Category> GetCategoryListPaging(int pageIndex, int pageSize, string textSearch, string sortField, bool orderDescending);
    }
}
