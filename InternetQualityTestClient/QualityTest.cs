using System.Diagnostics;
using System.Text.Json;

#pragma warning disable S2486 // Generic exceptions should not be ignored
#pragma warning disable S108 // Nested blocks of code should not be left empty
namespace InternetQualityTest
{
    public class QualityTest
    {
        private readonly AppSettings settings;

        public int PingsSentTotal { get; private set; } = 0;

        public int PingsSentSuccessfully { get; private set; } = 0;

        public TimeSpan TotalConnectionTimeOut { get; private set; } = new TimeSpan(0);

        public TimeSpan MaxConnectionTimeOut { get; private set; } = new TimeSpan(0);

        public QualityTest()
        {
            this.settings = GetAppSettings();            
        }

        public async Task StartTest(TimeSpan duration)
        {
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMilliseconds(this.settings.TimeOut);

            Stopwatch totalWatch = new Stopwatch();
            Stopwatch currentWatch = new Stopwatch();
            totalWatch.Start();            
            currentWatch.Start();

            int timeLetfCursorTop = 3;
            int totalCursorTop = 4;
            int successCursorTop = 5;
            int maxTimeoutCursorTop = 6;
            int totalTimeoutCursorTop = 7;

            TimeSpan previousRequestTime = new TimeSpan(0);

            while (totalWatch.Elapsed < duration)
            {
                if (!currentWatch.IsRunning)
                {
                    currentWatch.Restart();
                }
                
                var response = default(HttpResponseMessage)!;

                try 
                { 
                    response = await httpClient.GetAsync(this.settings.RequestUrl); 
                } 
                catch { }

                this.PingsSentTotal++;

                if (response != null && response.IsSuccessStatusCode)
                {
                    this.PingsSentSuccessfully++;
                    currentWatch.Reset();
                    previousRequestTime = new TimeSpan(0);
                }
                else
                {
                    if (currentWatch.Elapsed > this.MaxConnectionTimeOut)
                    {
                        this.MaxConnectionTimeOut = currentWatch.Elapsed;
                    }
                    
                    this.TotalConnectionTimeOut += currentWatch.Elapsed - previousRequestTime;
                    previousRequestTime = currentWatch.Elapsed;
                }

                TimeSpan remainingTime = duration - totalWatch.Elapsed;
                string formattedTime = $"{(int)remainingTime.TotalMinutes}.{remainingTime.Seconds}";
                
                Console.SetCursorPosition(10, timeLetfCursorTop);
                Console.Write(formattedTime.PadRight(10));
                Console.SetCursorPosition(6, totalCursorTop);
                Console.Write(this.PingsSentTotal.ToString().PadRight(10));
                Console.SetCursorPosition(8, successCursorTop);
                Console.Write(this.PingsSentSuccessfully.ToString().PadRight(10));
                Console.SetCursorPosition(12, maxTimeoutCursorTop);
                Console.Write($"{this.MaxConnectionTimeOut.ToString("s\\.f")}s".PadRight(10));
                Console.SetCursorPosition(14, totalTimeoutCursorTop);
                Console.Write($"{this.TotalConnectionTimeOut.ToString("s\\.f")}s".PadRight(10));

                await Task.Delay(this.settings.Interval);                
            }

            totalWatch.Stop();
            currentWatch.Stop();
        }

        private static AppSettings GetAppSettings()
        {
            var appSettings = JsonSerializer.Deserialize<AppSettings>(
                File.ReadAllText("appsettings.json"),
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                });

            return appSettings!;
        }
    }
}
