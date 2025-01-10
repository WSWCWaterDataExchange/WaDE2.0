using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Tests
{
    public class DbTestBase
    {
        protected TransactionScope TransactionScope { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            ResetGlobalIndexes();
            TransactionScope =
                new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        }


        [TestCleanup]
        public void TestCleanup()
        {
            TransactionScope?.Dispose();
        }

        private void ResetGlobalIndexes()
        {
            StateBuilder._globalIndex = 0;
        }
    }
}