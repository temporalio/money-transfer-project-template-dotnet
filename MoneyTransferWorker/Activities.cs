// @@@SNIPSTART money-transfer-project-template-dotnet-withdraw-activity
namespace Temporalio.MoneyTransferProject.Activities;
using Temporalio.Activities;
using Temporalio.MoneyTransferProject.Worker.BankingService;
using Temporalio.MoneyTransferProject.Shared;

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
            throw new ApplicationException("Withdrawal failed", ex);
        }
    }
// @@@SNIPEND

// @@@SNIPSTART money-transfer-project-template-dotnet-deposit-activity
    [Activity]
    public static async Task<string> DepositAsync(PaymentDetails details)
    {
        var bankService = new BankingService("bank2.example.com");
        Console.WriteLine($"Depositing ${details.Amount} into account {details.TargetAccount}.");
        try
        {
            return await bankService.DepositAsync(details.TargetAccount, details.Amount, details.ReferenceId);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Deposit failed", ex);
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
            throw new ApplicationException("Refund failed", ex);
        }
    }
}
// @@@SNIPEND