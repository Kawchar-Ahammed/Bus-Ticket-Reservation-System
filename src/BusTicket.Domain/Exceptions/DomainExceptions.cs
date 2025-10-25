namespace BusTicket.Domain.Exceptions;

/// <summary>
/// Base exception for all domain exceptions
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when seat is not available
/// </summary>
public class SeatNotAvailableException : DomainException
{
    public SeatNotAvailableException(string seatNumber)
        : base($"Seat {seatNumber} is not available for booking")
    {
    }
}

/// <summary>
/// Exception thrown when seat is already booked
/// </summary>
public class SeatAlreadyBookedException : DomainException
{
    public SeatAlreadyBookedException(string seatNumber)
        : base($"Seat {seatNumber} is already booked")
    {
    }
}

/// <summary>
/// Exception thrown when trying to perform invalid booking operation
/// </summary>
public class InvalidBookingOperationException : DomainException
{
    public InvalidBookingOperationException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Exception thrown when entity is not found
/// </summary>
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, Guid id)
        : base($"{entityName} with ID {id} was not found")
    {
    }

    public EntityNotFoundException(string entityName, string identifier)
        : base($"{entityName} with identifier '{identifier}' was not found")
    {
    }
}

/// <summary>
/// Exception thrown for business rule violations
/// </summary>
public class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message)
        : base(message)
    {
    }
}
