using System.Threading.Tasks;
using System.Transactions;

namespace WesternStatesWater.WaDE.Accessors
{
    internal static class AccessorUtilities
    {
        public static async Task<T> BlockTaskInTransaction<T>(this Task<T> task)
        {
            return Transaction.Current == null ? await task : Task.Run(() => task).Result;
        }

        public static async Task BlockTaskInTransaction(this Task task)
        {
            if (Transaction.Current == null)
            {
                await task;
            }
            else
            {
                Task.Run(() => task).Wait();
            }
        }

        public static async ValueTask<T> BlockTaskInTransaction<T>(this ValueTask<T> task)
        {
            return await BlockTaskInTransaction(task.AsTask());
        }
    }
}
