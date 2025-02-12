// @@@SNIPSTART money-transfer-project-template-dotnet-workflow
namespace Temporalio.MoneyTransferProject.MoneyTransferWorker;
using Temporalio.MoneyTransferProject.BankingService.Exceptions;
using Temporalio.Workflows;
using Temporalio.Common;
using Temporalio.Exceptions;

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
            MaximumAttempts = 3,
            NonRetryableErrorTypes = new[] { "InvalidAccountException", "InsufficientFundsException" }
        };

        string withdrawResult;
        try
        {
            withdrawResult = await Workflow.ExecuteActivityAsync(
                () => BankingActivities.WithdrawAsync(details),
                new ActivityOptions { StartToCloseTimeout = TimeSpan.FromMinutes(5), RetryPolicy = retryPolicy }
            );
        }
        catch (ApplicationFailureException ex) when (ex.ErrorType == "InsufficientFundsException")
        {
            throw new ApplicationFailureException("Withdrawal failed due to insufficient funds.", ex);
        }

        string depositResult;
        try
        {
            depositResult = await Workflow.ExecuteActivityAsync(
                () => BankingActivities.DepositAsync(details),
                new ActivityOptions { StartToCloseTimeout = TimeSpan.FromMinutes(5), RetryPolicy = retryPolicy }
            );
            // If everything succeeds, return transfer complete
            return $"Transfer complete (transaction IDs: {withdrawResult}, {depositResult})";
        }
        catch (Exception depositEx)
        {
            try
            {
                // if the deposit fails, attempt to refund the withdrawal
                string refundResult = await Workflow.ExecuteActivityAsync(
                    () => BankingActivities.RefundAsync(details),
                    new ActivityOptions { StartToCloseTimeout = TimeSpan.FromMinutes(5), RetryPolicy = retryPolicy }
                );
                // If refund is successful, but deposit failed
                throw new ApplicationFailureException($"Failed to deposit money into account {details.TargetAccount}. Money returned to {details.SourceAccount}.", depositEx);
            }
            catch (Exception refundEx)
            {
                // If both deposit and refund fail
                throw new ApplicationFailureException($"Failed to deposit money into account {details.TargetAccount}. Money could not be returned to {details.SourceAccount}. Cause: {refundEx.Message}", refundEx);
            }
        }
    }
}
// @@@SNIPEND