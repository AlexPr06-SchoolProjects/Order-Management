using Newtonsoft.Json.Linq;
using Spectre.Console;

namespace GUIManager
{
    class GUIManager
    {
        public GUIManager() { }

        public void DisplayOrderInTable(string jsonOrder)
        {
            try
            {
                var objOrder = JObject.Parse(jsonOrder);

                int orderId = (int)objOrder["OrderId"]!;
                int amount = (int)objOrder["Amount"]!;
                string dishName = (string)objOrder["OrderedDish"]!["Name"]!;
                double price = (double)objOrder["OrderedDish"]!["Price"]!;

                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .Title("[yellow bold]🧾 Order Details[/]")
                    .Caption("[dim]Chef's special is being prepared! 🍳[/]");

                table.AddColumn("[u]Order ID[/]");
                table.AddColumn("[u]Dish[/]");
                table.AddColumn("[u]Amount[/]");
                table.AddColumn("[u]Price[/]");
                table.AddColumn("[u]Total Price[/]");

                table.AddRow(
                    orderId.ToString(),
                    $"[blue]{dishName}[/]",
                    amount.ToString(),
                    $"${price:F2}",
                    $"{Math.Round(price * amount, 2):F2}"
                );

                AnsiConsole.Write(table);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error parsing order JSON: {ex.Message}");
            }
        }
    }
}
