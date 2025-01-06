using System.Runtime.CompilerServices;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace WesternStatesWater.WaDE.Accessors.Tests
{
    public class DbTestBase
    {
        protected TransactionScope TransactionScope { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            TransactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadUncommitted,
            }, TransactionScopeAsyncFlowOption.Enabled);
        }


        [TestCleanup]
        public void TestCleanup()
        {
            TransactionScope?.Dispose();
        }
    }
}