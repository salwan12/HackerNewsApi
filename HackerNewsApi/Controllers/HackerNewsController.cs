using HackerNews.Application.Contracts.Models;
using HackerNews.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HackerNewsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HackerNewsController : ControllerBase
    {
        private readonly IHackerNewsService hackerNewsService;

        public HackerNewsController(IHackerNewsService hackerNewsService)
        {
            this.hackerNewsService = hackerNewsService;
        }

        /// <summary>
        /// This Get request is made to retrive the latest stories from the Hacker News URL
        /// </summary>
        /// <returns>Newest Story list consist of title and url</returns>
        [HttpGet("newest")]
        public async Task<IActionResult> GetNewestStories()
        {
            try
            {
                List<HackerNewsModel> stories = await hackerNewsService.GetNewestStoriesAsync();

                if (stories == null || stories.Count == 0)
                {
                    return NotFound("No newest stories found.");
                }

                return Ok(stories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}