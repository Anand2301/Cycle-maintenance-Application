namespace Cycle_App.Features.Customers.CreateCustomer;

public class CreateCustomerRequest
{
    public string Name { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Address { get; set; }
}
