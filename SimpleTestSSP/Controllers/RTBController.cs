using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SimpleTestSSP.Controllers
{
    public class RTBController : ApiController
    {
        public async Task<string> Get()
        {
            var winningBid = await RTB.Instance.BroadcastNewAuction(new DAL.Auction());
            
            return winningBid?.Ad;
        }
    }
}
