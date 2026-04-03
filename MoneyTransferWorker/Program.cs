// @@@SNIPSTART money-transfer-project-template-dotnet-worker
// This file is designated to run the Worker
using Temporalio.Client;
using Temporalio.Common.EnvConfig;
using Temporalio.Worker;
using Temporalio.MoneyTransferProject.MoneyTransferWorker;

// Create the Temporal Client that the Worker will use to communicate
// with the Temporal Service. By default, it connects to a service
// running locally, on the standard port, using the default Namespace.
// You can override this by setting the TEMPORAL_PROFILE environment
// variable to the name of a specific profile that you've set up with
// the Temporal CLI.
var profile = Environment.GetEnvironmentVariable("TEMPORAL_PROFILE");

TemporalClientConnectOptions connectOptions;
if (profile is not null)
{
     connectOptions = ClientEnvConfig.LoadClientConnectOptions(
        new ClientEnvConfig.ProfileLoadOptions { Profile = profile });
}
else
{
     connectOptions = new("localhost:7233") { Namespace = "default" };
}

var client = await TemporalClient.ConnectAsync(connectOptions);


// Cancellation token to shut down Worker upon pressing ctrl+c
using var tokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    tokenSource.Cancel();
    eventArgs.Cancel = true;
};

// Create an instance of the activities since we have instance activities.
// If we had all static activities, we could just reference those directly.
var activities = new BankingActivities();

// Create a Worker with the Activity and Workflow registered
using var worker = new TemporalWorker(
    client, // client
    new TemporalWorkerOptions(taskQueue: "MONEY_TRANSFER_TASK_QUEUE")
        .AddAllActivities(activities) // Register Activities
        .AddWorkflow<MoneyTransferWorkflow>() // Register Workflow
);

// Run the Worker until it's cancelled
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
