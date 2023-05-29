namespace Ecommerce.Enums;

public enum PaymentStatus
{
    Pending,
    Authorized,
    Charged,
    Declined,
    Refunded,
    Cancelled,
    Error
}

public static class PaymentStatusExtensions
{
    public static string GetName(this PaymentStatus paymentStatus) =>
        paymentStatus switch
        {
            PaymentStatus.Pending => "Pending",
            PaymentStatus.Authorized => "Authorized",
            PaymentStatus.Charged => "Charged",
            PaymentStatus.Declined => "Declined",
            PaymentStatus.Refunded => "Refunded",
            PaymentStatus.Cancelled => "Cancelled",
            PaymentStatus.Error => "Error",
            _ => throw new ArgumentOutOfRangeException(nameof(paymentStatus))
        };
}