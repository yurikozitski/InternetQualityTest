using InternetQualityTest;
using System.Globalization;

#pragma warning disable S2486 // Generic exceptions should not be ignored
#pragma warning disable S108 // Nested blocks of code should not be left empty

string greeting = "==================== Welcome To Internet Quality Test ====================";

while (true)
{
    Console.Clear();


    Console.WriteLine(greeting);
    Console.WriteLine("Please enter duration of the test you want to perform in minutes:");

    int duration = 0;
    bool isTimeValid = false;

    while (!isTimeValid)
    {
        var input = Console.ReadLine();

        if (int.TryParse(input, out duration))
        {
            if (duration > 60 || duration < 1)
            {
                Console.WriteLine("Duration must be between 1 and 60 minutes, reenter the value");
                duration = 0;
            }
            else
            {
                isTimeValid = true;
            }
        }
        else
        {
            Console.WriteLine("Please, enter a valid number of minutes from 1 to 60");
        }
    }

    var test = new QualityTest();

    Console.Clear();
    Console.WriteLine(greeting);
    Console.WriteLine("Your test is being prepared...");

    await test.ServerWarmUp();

    Console.Clear(); 
    Console.WriteLine(greeting);
    Console.WriteLine("Running the test...");
    Console.WriteLine($"Total duration: {duration}m");
    Console.WriteLine("Time left: ");
    Console.WriteLine("Total: ");
    Console.WriteLine("Success: ");
    Console.WriteLine("Max timeout: ");
    Console.WriteLine("Total timeout: ");

    await test.StartTest(TimeSpan.FromMinutes(duration));

    double result = default;
    double percentageResult = default;

    try
    {
        result = Math.Round((double)test.PingsSentSuccessfully / (double)test.PingsSentTotal, 4, MidpointRounding.AwayFromZero);
        percentageResult = Math.Round(result * 100d, 2, MidpointRounding.AwayFromZero);
    }
    catch { }

    Console.Clear();
    Console.WriteLine(greeting);
    Console.WriteLine($"Total duration: {duration}m");
    Console.WriteLine($"Total pings sent:{test.PingsSentTotal}");
    Console.WriteLine($"Successfull pings:{test.PingsSentSuccessfully}");
    Console.WriteLine($"Maximum timeout:{test.MaxConnectionTimeOut.TotalSeconds.ToString("F1", CultureInfo.InvariantCulture)}s");
    Console.WriteLine($"Total timeout:{test.MaxConnectionTimeOut.TotalSeconds.ToString("F1", CultureInfo.InvariantCulture)}s");
    Console.WriteLine($"Your test is done, result: {percentageResult.ToString("F2", CultureInfo.InvariantCulture)}% of pings received");
    Console.WriteLine();
    Console.WriteLine("Do you want to perform one more test? (Y/N)");

    var key = Console.ReadKey();

    if (key.Key != ConsoleKey.Y)
    {
        Environment.Exit(0);       
    }
}


