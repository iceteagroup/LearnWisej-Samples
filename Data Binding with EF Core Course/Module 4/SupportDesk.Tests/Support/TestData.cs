using SupportDesk.Data;

namespace SupportDesk.Tests.Support;

/// <summary>Small helpers so a test can insert a valid ticket graph (Module 2 requires a customer and a category).</summary>
internal static class TestData
{
    public static Customer Customer(string name = "Halden Logistics") => new() { Name = name };

    public static Category Category(string name = "Hardware") => new() { Name = name };

    public static Ticket Ticket(Customer customer, Category category, string number, string title, Agent? agent = null) => new()
    {
        Number = number,
        Title = title,
        Status = "Open",
        Priority = "Normal",
        Customer = customer,
        Category = category,
        Agent = agent
    };
}
