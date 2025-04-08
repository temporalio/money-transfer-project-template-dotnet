namespace Temporalio.MoneyTransferProject.MoneyTransferWorker;

using System;
using System.Threading;
using System.Threading.Tasks;
using Temporalio.Client;
using Temporalio.Worker;

// @@@SNIPSTART money-transfer-project-template-dotnet-worker
// This file is designated to run the worker
public static class Program
{
    public static async Task Main(string[] args)
    {
        // Create a client to connect to localhost on "default" namespace
        var client = await TemporalClient.ConnectAsync(new("localhost:7233"));

        // Cancellation token to shutdown worker on ctrl+c
        using var tokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            tokenSource.Cancel();
            eventArgs.Cancel = true;
        };

        // Register the activities and workflow
        var workerOptions = new TemporalWorkerOptions(taskQueue: "MONEY_TRANSFER_TASK_QUEUE")
            .AddActivity(BankingActivities.WithdrawAsync) // Register static activities
            .AddActivity(BankingActivities.DepositAsync)
            .AddActivity(BankingActivities.RefundAsync)
            .AddWorkflow<MoneyTransferWorkflow>(); // Register workflow

        using var worker = new TemporalWorker(client, workerOptions);

        // Run the worker until it's cancelled
        Console.WriteLine("Running worker...");
        try
        {
            await worker.ExecuteAsync(tokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Worker cancelled");
        }
    }
}
// @@@SNIPEND
