namespace BusTicket.Domain.Enums;

/// <summary>
/// Payment method used for transaction
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Cash payment at counter
    /// </summary>
    Cash = 0,

    /// <summary>
    /// Credit/Debit card payment
    /// </summary>
    Card = 1,

    /// <summary>
    /// Bank transfer
    /// </summary>
    BankTransfer = 2,

    /// <summary>
    /// bKash mobile banking (Bangladesh)
    /// </summary>
    bKash = 3,

    /// <summary>
    /// Nagad mobile banking (Bangladesh)
    /// </summary>
    Nagad = 4,

    /// <summary>
    /// Rocket mobile banking (Bangladesh)
    /// </summary>
    Rocket = 5,

    /// <summary>
    /// Other digital wallets
    /// </summary>
    MobileWallet = 6
}
