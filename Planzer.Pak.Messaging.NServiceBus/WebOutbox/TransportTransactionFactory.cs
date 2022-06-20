using NServiceBus.Transport;
using System.Data.Common;

namespace Planzer.Pak.Messaging.NServiceBus.WebOutbox;

public static class TransportTransactionFactory
{
    public static TransportTransaction CreateFromDbTransaction(DbTransaction transaction)
    {
        if (transaction.Connection == null)
        {
            throw new ArgumentException($"Transaction has no connection associated, transaction is no longer valid");
        }

        var transportTransaction = new TransportTransaction();
        transportTransaction.Set("System.Data.SqlClient.SqlConnection", transaction.Connection);
        transportTransaction.Set("System.Data.SqlClient.SqlTransaction", transaction);
        return transportTransaction;
    }
}
