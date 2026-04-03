// @@@SNIPSTART money-transfer-project-template-dotnet-start-workflow
// This file is designated to run the workflow
using Temporalio.MoneyTransferProject.MoneyTransferWorker;
using Temporalio.Client;
using Temporalio.Common.EnvConfig;

// Create the Temporal Client that connects to the Temporal Service.
// By default, it will connect to one running locally, on the standard
// port, and use the default Namespace. You can override this by setting
// the TEMPORAL_PROFILE environment variable to the name of a specific
// profile that you've set up using the Temporal CLI.
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


// Define payment details
var details = new PaymentDetails(
    SourceAccount: "85-150",
    TargetAccount: "43-812",
    Amount: 400,
    ReferenceId: "12345"
);

Console.WriteLine($"Starting transfer from account {details.SourceAccount} to account {details.TargetAccount} for ${details.Amount}");

var workflowId = $"pay-invoice-{Guid.NewGuid()}";

try
{
    // Start the workflow
    var handle = await client.StartWorkflowAsync(
        (MoneyTransferWorkflow wf) => wf.RunAsync(details),
        new(id: workflowId, taskQueue: "MONEY_TRANSFER_TASK_QUEUE"));

    Console.WriteLine($"Started Workflow {workflowId}");

    // Await the result of the workflow
    var result = await handle.GetResultAsync();
    Console.WriteLine($"Workflow result: {result}");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Workflow execution failed: {ex.Message}");
}
// @@@SNIPEND
