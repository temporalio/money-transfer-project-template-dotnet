using Temporalio.Client;
using MoneyTransferProject;

class Program
{
    static async Task Main(string[] args)
    {
        await RunWorkflowAsync();
    }

    private static async Task RunWorkflowAsync()
    {
        try
        {
            // Connect to the Temporal server
            var client = await TemporalClient.ConnectAsync(new("localhost:7233"));
            
            // Define payment details
            var details = new PaymentDetails(
                "85-150", // SourceAccount
                "43-812", // TargetAccount
                400,      // Amount
                "12345"   // ReferenceID
            );

            Console.WriteLine($"Starting transfer from account {details.SourceAccount} to account {details.TargetAccount} for ${details.Amount}");

            var workflowId = $"pay-invoice-{Guid.NewGuid()}";

            // Start the workflow
            var handle = await client.StartWorkflowAsync(
                (MoneyTransferWorkflow wf) => wf.RunAsync(details),
                new WorkflowOptions(id: workflowId, taskQueue: "MONEY_TRANSFER_TASK_QUEUE")
            );

            Console.WriteLine($"Started Workflow {workflowId}");

            // Await the result of the workflow
            var result = await handle.GetResultAsync<string>();
            Console.WriteLine($"Workflow result: {result}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex}");
            Environment.ExitCode = 1;
        }
    }
}
