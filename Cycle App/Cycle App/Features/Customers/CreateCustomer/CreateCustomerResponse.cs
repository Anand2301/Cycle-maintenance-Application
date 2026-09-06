namespace CycleMaintenance.Features.Customers.CreateCustomer;

public class CreateCustomerResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; }
}