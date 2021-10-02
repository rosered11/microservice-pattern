using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Polly;
using Polly.Bulkhead;

namespace bulkhead_pattern
{
    class Program
    {
        const int TaskAmount = 10;
        static string endpoint = "https://jsonplaceholder.typicode.com/posts";
        static HttpClient httpClient = new HttpClient();
        static AsyncBulkheadPolicy<HttpResponseMessage> bulkheadIsolatePolicy;
        const int MaxParallel = 2;
        const int MaxQueueAction = 5;
        static void Main(string[] args)
        {
            Setup();
            List<Task> taskList = new List<Task>();
            for(int i = 0; i < TaskAmount; i++)
            {
                taskList.Add(Task.Run(() => Fetch(i)));
            }

            Task.WaitAll(taskList.ToArray());
        }

        static void Setup()
        {
            bulkheadIsolatePolicy = Policy.BulkheadAsync<HttpResponseMessage>(MaxParallel, MaxQueueAction, OnBulkHeadRejectAsync);
        }

        static async Task Fetch(int id)
        {
            try
            {
                LogBulkHeadInfo();
                var response = await bulkheadIsolatePolicy.ExecuteAsync(() => {
                    return httpClient.GetAsync($"{endpoint}/{id}");
                });
                if(response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Status: {response.StatusCode}");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Status: {response.StatusCode}");
                    Console.ResetColor();
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Exception: {ex.Message}");
                Console.ResetColor();
            }
        }

        static Task OnBulkHeadRejectAsync(Context context)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Polly bulk reject execute");
            Console.ResetColor();
            return Task.CompletedTask;
        }

        static void LogBulkHeadInfo()
        {
            Console.WriteLine($"BulkHeadAvalibleCount: {bulkheadIsolatePolicy.BulkheadAvailableCount}");

            Console.WriteLine($"QueueAvalibleCount: {bulkheadIsolatePolicy.QueueAvailableCount}");
        }
    }
}
