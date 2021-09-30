using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;

namespace circuit_breaker_pattern
{
    class Program
    {
        static string endpoint = "https://jsonplaceholder.typicode.com/postsa/1";
        static string endpointFail = "https://jsonplaceholder.typicode.com/postsa/1";
        static string endpont2 = "https://jsonplaceholder.typicode.com/posts/1";
        static int allowBeforeBreaking = 2;
        static TimeSpan durationOfBreak = TimeSpan.FromSeconds(5);
        static HttpClient HttpClient = new HttpClient();
        static AsyncCircuitBreakerPolicy<HttpResponseMessage> policy;

        #region Setup Circuit Breaker on Polly
        static void Setup()
        {
            policy = Policy.HandleResult<HttpResponseMessage>(rp => !rp.IsSuccessStatusCode)
                .CircuitBreakerAsync(allowBeforeBreaking, durationOfBreak
                , OnBreak, OnRest, OnHalfOpen);
            
        }

        static void OnHalfOpen()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Half Open");
            Console.ResetColor();

            // Reset Circuit if it don't setting, it will show status HalfOpen
            policy.Reset();
        }

        static void OnRest(Context context)
        {
            Console.WriteLine("Reset");
            endpoint = endpointFail;
        }
        
        static void OnBreak(DelegateResult<HttpResponseMessage> delegateResult, TimeSpan timeSpan, Context context)
        {
            Console.WriteLine($"Break - braker state {policy.CircuitState}");
        }

        static Task<HttpResponseMessage> MakeHttpCall()
        {
            Console.WriteLine($"made http get call {endpoint} - break state{policy.CircuitState}");
            return HttpClient.GetAsync(endpoint);
        }

        #endregion

        static async Task Fetch()
        {
            // Old code
            //var response = await HttpClient.GetAsync(endpoint);

            // New code
            var response = await policy.ExecuteAsync(MakeHttpCall);

            if(response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(json);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"Http status code: {response.StatusCode}");
            }
        }

        static void Main(string[] args)
        {
            Setup();
            for(int i = 0; i < 8; i++)
            {
                try
                {
                    Thread.Sleep(3000);
                    Fetch().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Exception: {ex.Message}");
                    Console.ResetColor();
                    
                    endpoint = endpont2;
                }
            }
        }
    }
}
