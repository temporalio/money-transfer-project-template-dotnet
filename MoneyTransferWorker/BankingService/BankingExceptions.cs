namespace Temporalio.MoneyTransferProject.Worker.BankingServiceExceptions;

public class InvalidAccountError : Exception
{
    public InvalidAccountError(string message) : base(message) { }
}

public class InsufficientFundsError : Exception
{
    public InsufficientFundsError(string message) : base(message) { }
}