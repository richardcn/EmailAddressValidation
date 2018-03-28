using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace EmailAddressValidation.DataConnections
{
    public class RedisSharedConnection
    {
        public ConnectionMultiplexer Connection { get; set; }

        IConfiguration _iconfiguration;
        public RedisSharedConnection(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            Connection = ConnectionMultiplexer.Connect(_iconfiguration.GetSection("Redis").GetSection("ConnectionString").Value);
        }        
    }
}
