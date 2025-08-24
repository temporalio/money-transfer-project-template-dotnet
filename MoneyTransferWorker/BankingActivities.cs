// @@@SNIPSTART money-transfer-project-template-dotnet-withdraw-activity
namespace Temporalio.MoneyTransferProject.MoneyTransferWorker;

using Temporalio.Activities;
using Temporalio.Exceptions;
using Temporalio.MoneyTransferProject.BankingService.Exceptions;

public static class BankingActivities
{
    [Activity]
    public static async Task<string> WithdrawAsync(PaymentDetails details)
    {
        var bankService = new BankingService("bank1.example.com");
        Console.WriteLine($"Withdrawing ${details.Amount} from account {details.SourceAccount}.");
        try
        {
            return await bankService.WithdrawAsync(details.SourceAccount, details.Amount, details.ReferenceId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new ApplicationFailureException("Withdrawal failed", ex);
        }
    }
// @@@SNIPEND

// @@@SNIPSTART money-transfer-project-template-dotnet-deposit-activity
    [Activity]
    public static async Task<string> DepositAsync(PaymentDetails details)
    {
        var bankService = new BankingService("bank2.example.com");
        Console.WriteLine($"Depositing ${details.Amount} into account {details.TargetAccount}.\n    ");
        try
        {
            // Simulate error for testing purposes
            if (details.TargetAccount == "simulateInvalidAccount")
            {
                throw new InvalidAccountException("Invalid target account");
            }
            return await bankService.DepositAsync(details.TargetAccount, details.Amount, details.ReferenceId);
        }
        catch (InvalidAccountException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Deposit failed: " + ex.Message);
            throw new ApplicationFailureException("Deposit failed", ex);
        }
    }
// @@@SNIPEND

// @@@SNIPSTART money-transfer-project-template-dotnet-refund-activity
    [Activity]
    public static async Task<string> RefundAsync(PaymentDetails details)
    {
        var bankService = new BankingService("bank1.example.com");
        Console.WriteLine($"Refunding ${details.Amount} to account {details.SourceAccount}.\n    ");
        try
        {
            return await bankService.RefundAsync(details.SourceAccount, details.Amount, details.ReferenceId);
        }
        catch (InvalidAccountException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Refund failed: " + ex.Message);
            throw new ApplicationFailureException("Refund failed", ex);
        }
    }
}
// @@@SNIPEND
