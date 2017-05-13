using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using MongoDB.Bson.Serialization.Attributes;
using Paralect.Domain;

namespace mPower.TempDocuments.Server.Documents
{
    public class Pair
    {
        public Pair(string k, string v)
        {
            this.k = k;
            this.v = v;
        }
        public string k { get; set; }
        public string v { get; set; }
    }

    public class CommandLogDocument
    {
        public static CommandLogDocument Create(ICommand command, NameValueCollection headers, NameValueCollection parameters, string url)
        {
            return new CommandLogDocument()
            {
                UserId = command.Metadata.UserId,
                Id = command.Metadata.CommandId,
                Command = command,
                CommandName = command.Metadata.TypeName,
                RequestHeaders = MapNameValueCollection(headers),
                RequestParams = MapNameValueCollection(parameters),
                RequestUrl = url
            };
            
        }

        private static List<Pair> MapNameValueCollection(NameValueCollection col)
        {
            if(col == null)
                return new List<Pair>();
            return col.AllKeys.Select(item => new Pair(item, col[item])).ToList();
        }

        [BsonId]
        public string Id { get; set; }

        public string UserId { get; set; }

        public string CommandName { get; set; }

        public List<Pair> RequestHeaders { get; set; }

        public List<Pair> RequestParams { get; set; }

        public string RequestUrl { get; set; }

        public ICommand Command { get; set; }
    }
}
