using HackerNews.Application.Contracts.Models;
using HackerNews.Application.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNews.Application.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly string hackerNewsApiUrl;

        public HackerNewsService(string hackerNewsApiUrl)
        {
            this.hackerNewsApiUrl = hackerNewsApiUrl;
        }

        public async Task<List<HackerNewsModel>> GetNewestStoriesAsync()
        {
            try
            {
                string newestStoriesUrl = $"{hackerNewsApiUrl}newstories.json";

                using (HttpClient client = new HttpClient())
                {
                    // Fetch the list of newest story IDs
                    int[]? newestStoryIds = await client.GetFromJsonAsync<int[]>(newestStoriesUrl);

                    if (newestStoryIds == null || newestStoryIds.Length == 0)
                    {
                        return new List<HackerNewsModel>();
                    }

                    // Display information for the first few stories (adjust the count as needed)
                    int storiesToDisplay = Math.Min(200, newestStoryIds.Length);

                    // Use MemoryCache to store fetched stories
                    MemoryCache cache = new MemoryCache(new MemoryCacheOptions());

                    List<Task<HackerNewsModel>> fetchTasks = new List<Task<HackerNewsModel>>();

                    for (int i = 0; i < storiesToDisplay; i++)
                    {
                        int storyId = newestStoryIds[i];
                        string storyUrl = $"{hackerNewsApiUrl}item/{storyId}.json";

                        // Check if the story is in the cache
                        if (cache.TryGetValue(storyId, out HackerNewsModel? cachedStory))
                        {
                            fetchTasks.Add(Task.FromResult(cachedStory));
                        }
                        else
                        {
                            // Create a task for fetching individual story details
                            Task<HackerNewsModel> fetchTask = FetchStoryAsync(client, storyUrl, cache, storyId);
                            fetchTasks.Add(fetchTask);
                        }
                    }

                    // Wait for all tasks to complete
                    HackerNewsModel[] stories = await Task.WhenAll(fetchTasks);

                    return stories.ToList();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (e.g., log, rethrow, return default value)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new List<HackerNewsModel>();
            }
        }

        public async Task<HackerNewsModel> FetchStoryAsync(HttpClient client, string storyUrl, MemoryCache cache, int storyId)
        {
            try
            {
                // Fetch individual story details
                HackerNewsModel story = await client.GetFromJsonAsync<HackerNewsModel>(storyUrl) ?? new HackerNewsModel(); // Ensure non-null result

                // Cache the fetched story
                cache.Set(storyId, story, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Set cache expiration time
                });

                return story;
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (e.g., log, rethrow, return default value)
                Console.WriteLine($"An error occurred while fetching a story: {ex.Message}");
                return new HackerNewsModel();
            }
        }
    }
}