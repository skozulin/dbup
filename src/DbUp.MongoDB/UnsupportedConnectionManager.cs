using System;
using System.Collections.Generic;
using System.Data;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;

namespace DbUp.MongoDB
{
    class UnsupportedConnectionManager: IConnectionManager
    {
        public IDisposable OperationStarting(IUpgradeLog upgradeLog, List<SqlScript> executedScripts)
        {
            return new DoNothingDisposible();
        }

        public TransactionMode TransactionMode { get; set; }
        public bool IsScriptOutputLogged { get; set; }
        public IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            throw new NotImplementedException();
        }

        public bool TryConnect(IUpgradeLog upgradeLog, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public void ExecuteCommandsWithManagedConnection(Action<Func<IDbCommand>> action)
        {
            throw new NotImplementedException();
        }

        public T ExecuteCommandsWithManagedConnection<T>(Func<Func<IDbCommand>, T> actionWithResult)
        {
            throw new NotImplementedException();
        }

        class DoNothingDisposible : IDisposable
        {
            public void Dispose()
            {

            }
        }
    }
}
