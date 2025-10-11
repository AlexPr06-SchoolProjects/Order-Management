namespace App.GUIManager
{
    using App.Dish;
    using App.Order;
    using App.FoodCatagory;
    using Spectre.Console;

    class GUIManager
    {
        public GUIManager() { }
        public void DisplayMenuTable(FoodCatagory category, Dictionary<int, Dish> dishes)
        {
            var table = new Table()
                    .Border(TableBorder.Rounded)
                    .Title($"[yellow bold]{category} Menu[/]")
                    .Caption("[dim]Enjoy your meal![/]");

            table.AddColumn(new TableColumn("[u]Order #[/]").Centered());
            table.AddColumn(new TableColumn("[u]Dish[/]").LeftAligned());
            table.AddColumn(new TableColumn("[u]Price[/]").LeftAligned());
            foreach (var (index, dish) in dishes)
            {
                table.AddRow(
                    index.ToString(),
                    $"[blue]{dish.Name}[/]",
                    $"[green]${dish.Price}[/]"
                );
            }

            // Table Output
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine(); // Padding 
        }

       
        public async Task DisplayOrdersTable(Dictionary<int, Order> orders)
        {

            var table = new Table()
                       .Border(TableBorder.Rounded)
                       .Title("[yellow bold]🧾 Current Orders[/]")
                       .Caption("[dim]Manage your orders easily[/]");

            table.AddColumn(new TableColumn("[u]Order ID[/]").Centered());
            table.AddColumn(new TableColumn("[u]Dish[/]").Centered());
            table.AddColumn(new TableColumn("[u]Amount[/]").Centered());
            table.AddColumn(new TableColumn("[u]Status[/]").Centered());

            await AnsiConsole.Live(new Table())
                 .AutoClear(false)
                 .Overflow(VerticalOverflow.Ellipsis)
                 .StartAsync(async ctx =>
                 {

                     foreach (var (index, order) in orders) 
                     {
                         table.AddRow(
                            order.OrderId.ToString(),
                            $"[blue]{order.OrderedDish}[/]",
                            order.Amount.ToString(),
                            order.Status ? "[green]Completed[/]" : "[yellow]Processing...[/]"
                         );

                         ctx.UpdateTarget(table);

                         await Task.Delay(500); // Imitation

                         order.Status = true;

                         table.Rows.Update(
                             order.OrderId - 1, 
                             3, 
                             cellData: new Markup("[green]Completed[/]")
                         );

                         ctx.UpdateTarget(table); // Table rerendering 
                     }
                 });
        }
        
        public FoodCatagory getUserCategoryChoice()
        {
            var userCategory = AnsiConsole.Prompt(
                new TextPrompt<FoodCatagory>("Choice category of your food?")
                  .AddChoices(Enum.GetValues<FoodCatagory>())
                  .DefaultValue(FoodCatagory.MainCourse));
            return userCategory;
        }
           
        public int getUserDishIndex(List<int> availableIndexes)
        {
            int choosedIndex = AnsiConsole.Prompt(
                new TextPrompt<int>("Select your dish: ")
                  .AddChoices(availableIndexes)
                  .DefaultValue(availableIndexes[0]));
            return choosedIndex;
        }

        public int getUserAmountOfDishes()
        {
            int userInput = AnsiConsole.Prompt(
                new TextPrompt<int>("How much would you like to order?(input amount)")
                  .Validate((amount) =>
                  {
                      if (amount < 0)
                          return ValidationResult.Error("[red]You should order at least one dish[/]");
                      if (amount > 10)
                          return ValidationResult.Error("[red]Too high[/]");
                      return ValidationResult.Success();
                  })
            );
            return userInput;
        }
    }
}
