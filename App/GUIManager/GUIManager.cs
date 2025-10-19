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


        public async Task DisplayOrdersTable(
     Dictionary<int, OrderTask> OrderTasks,
     Func<OrderTask, Task<string>> sendOrderCallback
 )
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title("[yellow bold]🧾 Current Orders[/]")
                .Caption("[dim]Manage your orders easily[/]");

            table.AddColumn("[u]Order ID[/]");
            table.AddColumn("[u]Dish[/]");
            table.AddColumn("[u]Amount[/]");
            table.AddColumn("[u]Status[/]");

            // Initial population of the table
            foreach (var (_, orderTask) in OrderTasks)
            {
                var order = orderTask.Order;
                table.AddRow(
                    order.OrderId.ToString(),
                    $"[blue]{order.OrderedDish}[/]",
                    order.Amount.ToString(),
                    "[yellow]Processing...[/]"
                );
            }

            await AnsiConsole.Live(table)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .StartAsync(async ctx =>
                {
                    foreach (var (id, orderTask) in OrderTasks)
                    {
                        await sendOrderCallback(orderTask);
                        orderTask.Order.Status = true;

                        // Повністю оновлюємо таблицю
                        var updatedTable = new Table()
                            .Border(TableBorder.Rounded)
                            .Title("[yellow bold]🧾 Current Orders[/]")
                            .Caption("[dim]Manage your orders easily[/]");
                        updatedTable.AddColumn("[u]Order ID[/]");
                        updatedTable.AddColumn("[u]Dish[/]");
                        updatedTable.AddColumn("[u]Amount[/]");
                        updatedTable.AddColumn("[u]Status[/]");

                        foreach (var (_, task) in OrderTasks)
                        {
                            var order = task.Order;
                            updatedTable.AddRow(
                                order.OrderId.ToString(),
                                $"[blue]{order.OrderedDish}[/]",
                                order.Amount.ToString(),
                                order.Status ? "[green]Completed[/]" : "[yellow]Processing...[/]"
                            );
                        }

                        ctx.UpdateTarget(updatedTable);
                        await Task.Delay(500); 
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

        public bool askUser(string question)
        {
            var confirmation = AnsiConsole.Prompt(
                new TextPrompt<bool>(question)
                .AddChoice(true)
                .AddChoice(false)
                .DefaultValue(true)
                .WithConverter(choice => choice ? "y" : "n"));

            return confirmation;
        }
    }
}
