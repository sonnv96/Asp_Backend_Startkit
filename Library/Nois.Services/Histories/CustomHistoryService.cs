using Nois.Framework.Histories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nois.Framework.Caching;
using Nois.Framework.Data;
using Nois.Services.Data;
using Nois.Framework;

namespace Nois.Services.Histories
{
    public class CustomHistoryService : HistoryService
    {
        private readonly IRepository<History> _historyRepository;
        private readonly IWorkContext _workContext;
        public CustomHistoryService(IDbContext dbContext, IRepository<History> historyRepository, ICacheManager cacheManager, IWorkContext workContext) : base(dbContext, historyRepository)
        {
            _workContext = workContext;
            _historyRepository = historyRepository;
        }

        public override string DefaultActionActor
        {
            get
            {
                return _workContext.CurrentUser == null ? "[Unknow]": _workContext.CurrentUser.Username;
            }
        }
    }
}
