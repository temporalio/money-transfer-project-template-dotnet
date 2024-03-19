namespace MoneyTransferProject;

public record PaymentDetails(
    string SourceAccount,
    string TargetAccount,
    int Amount,
    string ReferenceId);
