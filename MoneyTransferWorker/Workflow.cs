using Temporalio.Workflows;
using MoneyTransferShared;
using Temporalio.Common; 

namespace MoneyTransferProject
{
    [Workflow]
    public interface IMoneyTransferWorkflow
    {
        [WorkflowRun]
        Task<string> RunAsync(PaymentDetails details);
    }
        [Workflow]
        public class MoneyTransferWorkflow : IMoneyTransferWorkflow
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
                MaximumAttempts = 5,
                NonRetryableErrorTypes = new[] { "InvalidAccountError", "InsufficientFundsError" }
            };

            string withdrawResult;
            try
            {
                withdrawResult = await Workflow.ExecuteActivityAsync<string>(
                    "Withdraw", // registered activity name
                    new object[] { details },
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
                depositResult = await Workflow.ExecuteActivityAsync<string>(
                    "Deposit",
                    new object[] { details },
                    new ActivityOptions { ScheduleToCloseTimeout = TimeSpan.FromMinutes(5), RetryPolicy = retryPolicy }
                );
            }
            catch (ApplicationException ex)
            {
                // Attempt to refund the withdrawal if the deposit fails
                string refundResult = await Workflow.ExecuteActivityAsync<string>(
                    "Refund",
                    new object[] { details },
                    new ActivityOptions { ScheduleToCloseTimeout = TimeSpan.FromMinutes(5), RetryPolicy = retryPolicy }
                );
                // Rethrow the original deposit failure exception
                throw new ApplicationException($"Deposit failed, refund attempted with result: {refundResult}", ex);
            }
            // If everything succeeds, success message
            return $"Transaction completed successfully. Withdrawal result: {withdrawResult}, Deposit result: {depositResult}";
            

        }
    }
}
