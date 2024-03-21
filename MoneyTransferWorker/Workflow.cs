// @@@SNIPSTART money-transfer-project-template-dotnet-workflow
namespace Temporalio.MoneyTransferProject.Workflow;
using Temporalio.Workflows;
using Temporalio.Common;
using Temporalio.MoneyTransferProject.Worker.BankingServiceExceptions;
using Temporalio.MoneyTransferProject.Shared;
using Temporalio.MoneyTransferProject.Activities;

[Workflow]
public class MoneyTransferWorkflow
{
    [WorkflowRun]
    public async Task<string> RunAsync(PaymentDetails details)
    {
        // Retry policy
        var retryPolicy = new RetryPolicy
        {
            InitialInterval = TimeSpan.FromSeconds(1),
            MaximumInterval = TimeSpan.FromSeconds(100),
            BackoffCoefficient = 2,
            MaximumAttempts = 500,
            NonRetryableErrorTypes = new[] { "InvalidAccountError", "InsufficientFundsError" }
        };

        string withdrawResult;
        try
        {
            withdrawResult = await Workflow.ExecuteActivityAsync(
                () => BankingActivities.WithdrawAsync(details),
                new ActivityOptions { ScheduleToCloseTimeout = TimeSpan.FromMinutes(5), RetryPolicy = retryPolicy }
            );
        }
        catch (ApplicationException ex) when (ex.InnerException is InsufficientFundsError)
        {
            throw new ApplicationException("Withdrawal failed due to insufficient funds.", ex);
        }

        string depositResult;
        try
        {
            depositResult = await Workflow.ExecuteActivityAsync(
                () => BankingActivities.DepositAsync(details),
                new ActivityOptions { ScheduleToCloseTimeout = TimeSpan.FromMinutes(5), RetryPolicy = retryPolicy }
            );
        }
        catch (ApplicationException depositEx)
        {
            try
            {
                // Attempt to refund the withdrawal if the deposit fails
                string refundResult = await Workflow.ExecuteActivityAsync(
                    () => BankingActivities.RefundAsync(details),
                    new ActivityOptions { ScheduleToCloseTimeout = TimeSpan.FromMinutes(5), RetryPolicy = retryPolicy }
                );
                // If refund is successful, but deposit failed
                throw new ApplicationException($"Failed to deposit money into account {details.TargetAccount}. Money returned to {details.SourceAccount}. Cause: {depositEx.Message}", depositEx);
            }
            catch (ApplicationException refundEx)
            {
                // If both deposit and refund fail
                throw new ApplicationException($"Failed to deposit money into account {details.TargetAccount}. Money could not be returned to {details.SourceAccount}. Cause: {refundEx.Message}", refundEx);
            }
        }
        // If everything succeeds, return transfer complete
        return $"Transfer complete (transaction IDs: {withdrawResult}, {depositResult})";
    }
}
// @@@SNIPEND