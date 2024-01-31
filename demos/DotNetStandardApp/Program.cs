using Saleslogix.SData.Client;
using Saleslogix.SData.Client.Linq;

namespace DotNetStandardApp;

[SDataPath("contacts")]
public class Contact
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Status { get; set; }
}

public class Program
{
    static async Task Main(string[] args)
    {
        var client = new SDataClient("http://localhost/sdata/slx/dynamic/-/")
        {
            UserName = "admin",
            Password = ""
        };

        var contacts = await client.Query<Contact>()
            .Where(c => c.Status == "Active")
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync();

        Console.WriteLine("Active Contacts:");
        foreach (var contact in contacts)
        {
            Console.WriteLine($"{contact.LastName}, {contact.FirstName}");
        }
        Console.WriteLine("Press any key to continue.");
        Console.ReadLine();
    }
}
