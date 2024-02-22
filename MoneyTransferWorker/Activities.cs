using Temporalio.Activities;
using MoneyTransferShared;

namespace MoneyTransferActivity
{
    public interface IBankingActivities
    {
        Task<string> WithdrawAsync(PaymentDetails details);
        Task<string> DepositAsync(PaymentDetails details);
        Task<string> RefundAsync(PaymentDetails details);
    }
    public class BankingActivities : IBankingActivities
    {
        [Activity]
        public async Task<string> WithdrawAsync(PaymentDetails details)
        {
            var bankService = new BankingService("bank1.example.com");
            Console.WriteLine($"Withdrawing ${details.Amount} from account {details.SourceAccount}.");
            try
            {
                return await bankService.Withdraw(details.SourceAccount, details.Amount, details.ReferenceID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Withdrawal failed", ex);
            }
        }

        [Activity]
        public async Task<string> DepositAsync(PaymentDetails details)
        {
            var bankService = new BankingService("bank2.example.com");
            Console.WriteLine($"Depositing ${details.Amount} into account {details.TargetAccount}.");
            try
            {
                return await bankService.Deposit(details.TargetAccount, details.Amount, details.ReferenceID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Deposit failed", ex);
            }
        }

        [Activity]
        public async Task<string> RefundAsync(PaymentDetails details)
        {
            var bankService = new BankingService("bank1.example.com");
            Console.WriteLine($"Refunding ${details.Amount} to account {details.SourceAccount}.");
            try
            {
                return await bankService.Refund(details.SourceAccount, details.Amount, details.ReferenceID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Refund failed", ex);
            }
        }
    }
}
