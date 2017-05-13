using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;

namespace mPower.Framework.Environment
{
    /// <summary>
    /// Generate ID as MongoDB ObjectId
    /// </summary>
    public class MongoObjectIdGenerator : IIdGenerator
    {
        /// <summary>
        /// Returns newly generated ID
        /// </summary>
        public String Generate()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}
