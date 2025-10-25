namespace BusTicket.Domain.Enums;

/// <summary>
/// Payment gateway provider
/// </summary>
public enum PaymentGateway
{
    /// <summary>
    /// Manual/Cash payment (no gateway)
    /// </summary>
    Manual = 0,

    /// <summary>
    /// SSLCommerz payment gateway (Bangladesh)
    /// </summary>
    SSLCommerz = 1,

    /// <summary>
    /// Stripe payment gateway (International)
    /// </summary>
    Stripe = 2,

    /// <summary>
    /// bKash payment gateway
    /// </summary>
    bKash = 3,

    /// <summary>
    /// Nagad payment gateway
    /// </summary>
    Nagad = 4,

    /// <summary>
    /// Rocket payment gateway
    /// </summary>
    Rocket = 5,

    /// <summary>
    /// Mock gateway for testing
    /// </summary>
    Mock = 99
}
