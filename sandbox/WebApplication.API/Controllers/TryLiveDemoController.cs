using AutoFilterer.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApplication.API.Contexts;
using WebApplication.API.Models.LiveDemo;
using AutoFilterer.Extensions;

namespace WebApplication.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("try-live-demo")]
    public class TryLiveDemoController : ControllerBase
    {
        protected readonly LiveDemoDbContext liveDemoDbContext;

        public TryLiveDemoController(LiveDemoDbContext liveDemoDbContext)
        {
            this.liveDemoDbContext = liveDemoDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            liveDemoDbContext.Visits.Add(
                new Visit(
                    GetRemoteIPAddress().ToString(),
                     Request.Headers["Referer"].ToString(),
                     Request.Headers["Accept-Language"].ToString(),
                     Request.Headers["User-Agent"].ToString()
                     )
                );

            await liveDemoDbContext.SaveChangesAsync();

            return Redirect("/swagger/index.html#/Books/get_api_Books");
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetListAsync([FromQuery] PaginationFilterBase filter)
        {
            var list = await liveDemoDbContext.Visits.ApplyFilter(filter).ToListAsync();

            return Ok(list);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCountAsync()
        {
            var count = await liveDemoDbContext.Visits.CountAsync();

            return Ok(count);
        }

        [HttpGet("clear")]
        public async Task<IActionResult> Clear()
        {
            liveDemoDbContext.Visits.RemoveRange(liveDemoDbContext.Visits);
            await liveDemoDbContext.SaveChangesAsync();

            return Ok();
        }


        private IPAddress GetRemoteIPAddress(bool allowForwarded = true)
        {
            if (allowForwarded)
            {
                string header = (HttpContext.Request.Headers["CF-Connecting-IP"].FirstOrDefault() ?? HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault());
                if (IPAddress.TryParse(header, out IPAddress ip))
                {
                    return ip;
                }
            }
            return HttpContext.Connection.RemoteIpAddress;
        }
    }
}
