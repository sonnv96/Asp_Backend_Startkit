using Nois.Framework.Caching;
using Nois.Framework.Data;
using Nois.Framework.Histories;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApi.Services
{
    [TestFixture]
    public class ChuteServiceTest
    {
        private IHistoryService _historyService;
        private ICacheManager _cacheManager;
        /// <summary>
        /// 
        /// </summary>
        [SetUp]
        public new void SetUp()
        {
            #region Test Data
            
            #endregion

            

            _cacheManager = new NullCacheManager();
            _historyService = MockRepository.GenerateMock<IHistoryService>();
        }

        [Test]
        public void Ensure_get_all()
        {
            
        }
    }
}
