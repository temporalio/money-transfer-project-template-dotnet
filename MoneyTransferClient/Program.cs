// @@@SNIPSTART money-transfer-project-template-dotnet-start-workflow
// This file is designated to run the workflow
using Temporalio.MoneyTransferProject.MoneyTransferWorker;
using Temporalio.Client;

// Use a helper method to create a Temporal Client configured to use a 
// specific endpoint address, Namespace, and authentication options for 
// the Temporal Service based on the presence of some environment variables
var client = await TemporalClientHelper.CreateClientAsync();

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
