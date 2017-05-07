using SimpleTestSSP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SimpleTestSSP.Controllers
{
    public class AuctionsController : ApiController
    {
        public IEnumerable<Auction> Get()
        {
            return RTB.Instance.auctions;
        }
    }
}
