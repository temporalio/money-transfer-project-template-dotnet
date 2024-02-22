namespace MoneyTransferShared
{
    public record PaymentDetails(
        string SourceAccount,
        string TargetAccount,
        int Amount,
        string ReferenceID);
}
