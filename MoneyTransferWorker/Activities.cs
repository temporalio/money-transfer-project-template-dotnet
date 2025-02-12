// @@@SNIPSTART money-transfer-project-template-dotnet-withdraw-activity
namespace Temporalio.MoneyTransferProject.MoneyTransferWorker;
using Temporalio.Activities;
using Temporalio.Exceptions;

public class BankingActivities
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
        Console.WriteLine($"Depositing ${details.Amount} into account {details.TargetAccount}.");

        // Uncomment below and comment out the try-catch block below to simulate unknown failure
        /*
        return await bankService.DepositThatFailsAsync(details.TargetAccount, details.Amount, details.ReferenceId);
        */

        try
        {
            return await bankService.DepositAsync(details.TargetAccount, details.Amount, details.ReferenceId);
        }
        catch (Exception ex)
        {
            throw new ApplicationFailureException("Deposit failed", ex);
        }
    }
// @@@SNIPEND

// @@@SNIPSTART money-transfer-project-template-dotnet-refund-activity
    [Activity]
    public static async Task<string> RefundAsync(PaymentDetails details)
    {
        var bankService = new BankingService("bank1.example.com");
        Console.WriteLine($"Refunding ${details.Amount} to account {details.SourceAccount}.");
        try
        {
            return await bankService.RefundAsync(details.SourceAccount, details.Amount, details.ReferenceId);
        }
        catch (Exception ex)
        {
            throw new ApplicationFailureException("Refund failed", ex);
        }
    }
}
// @@@SNIPEND