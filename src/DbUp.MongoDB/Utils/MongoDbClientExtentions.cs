using System;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Bindings;
using MongoDB.Driver.Core.Operations;

namespace DbUp.MongoDB.Utils
{
    public static class MongoDbClientExtensions
    {
        /// <summary>
        /// Evaluates the specified javascript within a MongoDb database
        /// </summary>
        /// <param name="database">MongoDb Database to execute the javascript</param>
        /// <param name="javascript">Javascript to execute</param>
        /// <returns>A BsonValue result</returns>
        public static BsonValue Eval(this IMongoDatabase database, string javascript)
        {
            var client = database.Client as MongoClient;

            if (client == null)
                throw new ArgumentException("Client is not a MongoClient");

            var function = new BsonJavaScript(javascript);
            var op = new EvalOperation(database.DatabaseNamespace, function, null);

            using (var writeBinding = new WritableServerBinding(client.Cluster))
            {
                return op.Execute(writeBinding, CancellationToken.None);
            }
        }
    }
}
