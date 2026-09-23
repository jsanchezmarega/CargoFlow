namespace CargoFlow.Domain;

public class InvalidShipmentStateException : Exception
{
    public InvalidShipmentStateException(string message) : base(message)
    {
    }
}
