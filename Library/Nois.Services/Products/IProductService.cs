using System;
using System.Collections.Generic;
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
        IPagedList<Product> GetProductListPaging(string textSearch, int? categoryID, DateTime? dateFrom, DateTime? dateTo, int pageIndex, int pageSize, string sortField, bool orderDescending);
        List<Product> GetProductByStore(int? storeID, int pageIndex, int pageSize, string textSearch, string sortField, bool orderDescending);
        List<Product> GetProductByCategory(int? categoryID, int pageIndex, int pageSize, string textSearch, string sortField, bool orderDescending);
    }
}
