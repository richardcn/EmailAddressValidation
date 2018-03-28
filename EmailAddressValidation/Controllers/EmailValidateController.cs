using DnsClient;
using EmailAddressValidation.DataConnections;
using EmailValidation;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace EmailAddressValidation.Controllers
{
    [Produces("application/json")]
    [Route("api/EmailValidate")]
    public class EmailValidateController : Controller
    {
        RedisSharedConnection _redis;

        public EmailValidateController(RedisSharedConnection redis)
        {
            _redis = redis;
        }

        // GET {email}
        [HttpGet("{email}")]
        async public Task<IActionResult> Validate(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return NotFound();
            }

            //// check the format of email address            
            if (!EmailValidator.Validate(email))
            {
                return Ok(new
                {
                    email = email,
                    valid = false,
                    reason = "Email address has invalid format"
                });
            }

            // check email server
            // get the domain name
            var domainName = email.Substring(email.LastIndexOf('@') + 1);

            var isDomainNameValid = await IsDomainNameValid(domainName);

            if (!isDomainNameValid)
            {
                return Ok(new
                {
                    email = email,
                    valid = false,
                    reason = "Domain name is invalid"
                });
            }

            return Ok(new
            {
                email = email,
                valid = true,
                reason = "Email address is valid"
            });
        }

        async Task<bool> IsDomainNameValid(string domainName)
        {
            // check redis cache            
            var db = _redis.Connection.GetDatabase();
            int i = (int)(await db.StringGetAsync(domainName));

            if (i > 0)
                return true;

            // check dns records
            var client = new LookupClient();
            client.UseCache = true;

            var recs = client.Query(domainName, QueryType.MX).Answers.MxRecords();

            if (recs.Count() > 0)
            {
                await db.StringSetAsync(domainName, 1);
                return true;
            }

            return false;
        }
    }
}