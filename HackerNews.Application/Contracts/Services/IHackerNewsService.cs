using HackerNews.Application.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.Application.Contracts.Services
{
    public interface IHackerNewsService
    {
        Task<List<HackerNewsModel>> GetNewestStoriesAsync();
    }
}