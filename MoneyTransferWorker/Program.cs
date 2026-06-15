// @@@SNIPSTART money-transfer-project-template-dotnet-worker
// This file is designated to run the worker
using Temporalio.Client;
using Temporalio.Common.EnvConfig;
using Temporalio.Worker;
using Temporalio.MoneyTransferProject.MoneyTransferWorker;

// Connect to Temporal Cloud by loading the "cloud-setup" profile from the shared
// Temporal client config (temporal.toml), which supplies the Cloud address,
// namespace, TLS settings, and API key.
var client = await TemporalClient.ConnectAsync(
    ClientEnvConfig.LoadClientConnectOptions(new ClientEnvConfig.ProfileLoadOptions
    {
        Profile = "cloud-setup",
    }));

// Cancellation token to shutdown worker on ctrl+c
using var tokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    tokenSource.Cancel();
    eventArgs.Cancel = true;
};

// Create an instance of the activities since we have instance activities.
// If we had all static activities, we could just reference those directly.
var activities = new BankingActivities();

// Create a worker with the activity and workflow registered
using var worker = new TemporalWorker(
    client, // client
    new TemporalWorkerOptions(taskQueue: "MONEY_TRANSFER_TASK_QUEUE")
        .AddAllActivities(activities) // Register activities
        .AddWorkflow<MoneyTransferWorkflow>() // Register workflow
);

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
// @@@SNIPEND