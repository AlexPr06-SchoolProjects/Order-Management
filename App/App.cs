using App.Dish;
using App.Order;
using App.Menu;
using App.OrderManagement;
using App.FoodCatagory;
using MyGUI = App.GUI.GUIManager;
using System.Diagnostics;
using Spectre.Console;
using Order_Management.RabbitMQConfig;
using RabbitMQRpcPublisher = Publisher.Publisher;
using System.Linq;

// Set console to UTF8
Console.OutputEncoding = System.Text.Encoding.UTF8;



/* --------------------------------------------     Creating multiple consoles      ------------------------------------------- */
Console.WriteLine(". Запускаю вторую...");
Console.WriteLine(Directory.GetCurrentDirectory());

var process = new Process();

// Configure the process start info
var processStartInfo = new ProcessStartInfo
{
    FileName = "cmd.exe", // запускаем командную строку
    Arguments = "/k dotnet run --project \"..\\..\\..\\..\\Consumer\\Consumer.csproj\"",
    UseShellExecute = true,   // нужно, чтобы открылось новое окно
    CreateNoWindow = false    // false — чтобы окно было видно
};


// Execute process
Process.Start(processStartInfo);


Console.WriteLine("Вторая консоль запущена!");

/* ----------------------------------------------------------------------------------------------------------------------------- */


/* --------------------------------------------     Setting up RabbitMQ      ------------------------------------------- */
RabbitMQConfig rabbitMQConfig = new RabbitMQConfig();

await using RabbitMQRpcPublisher publisher = new RabbitMQRpcPublisher(rabbitMQConfig);

// Step 1: Connect to RabbitMQ
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("🧑‍🍳 The waiter grabs the tray and tries to connect to RabbitMQ...");
Console.ResetColor();

bool isConnected = await publisher.ConnectAsync();
if (!isConnected)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ The waiter couldn’t reach RabbitMQ! No orders can be delivered 😢");
    Console.ResetColor();
    return;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("✅ The waiter is connected! Orders are ready to fly to the chef! 🍽️");
Console.ResetColor();

// Step 2: Create exchange
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("🔄 The waiter is setting up the exchange table for orders...");
Console.ResetColor();

bool isExchangeCreated = await publisher.CreateExchangeAsync();
if (!isExchangeCreated)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ Failed to set up the exchange! The orders are confused 😅");
    Console.ResetColor();
    return;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("✅ Exchange table ready! The waiter knows where to put the orders! 🍲");
Console.ResetColor();

// Step 3: Create queue
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("📋 The waiter is preparing the order queue...");
Console.ResetColor();

bool isQueueCreated = await publisher.CreateQueueAsync();
if (!isQueueCreated)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ Queue creation failed! Orders are floating in the air 🫣");
    Console.ResetColor();
    return;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("✅ Order queue is ready! All orders have a place to wait patiently 😎");
Console.ResetColor();

// Step 4: Bind queue to exchange
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("🔗 The waiter is connecting the queue to the exchange...");
Console.ResetColor();

bool isBound = await publisher.BindRabbitMQQueueAsync();
if (!isBound)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ Failed to bind the queue! Orders are lost between tables 😵");
    Console.ResetColor();
    return;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("✅ Queue successfully bound! The waiter can now deliver orders straight to the chef! 🍛");
Console.ResetColor();


/* ----------------------------------------------------------------------------------------------------------------------------- */



await publisher.InitializeRpcAsync();


List<Dish> myDishes = new List<Dish>()
{
    new Dish("Pizza Margherita", 12.99, FoodCatagory.MainCourse),
    new Dish("Caesar Salad", 9.99, FoodCatagory.Appetizer),
    new Dish("Grilled Salmon", 17.49, FoodCatagory.MainCourse),
    new Dish("Cheesecake", 6.99, FoodCatagory.Dessert),
    new Dish("Espresso", 2.50, FoodCatagory.Beverage),
    new Dish("Bruschetta", 7.20, FoodCatagory.Appetizer),
    new Dish("Tiramisu", 8.40, FoodCatagory.Dessert),
    new Dish("Cappuccino", 3.10, FoodCatagory.Beverage),

    new Dish("Garlic Bread", 5.50, FoodCatagory.Appetizer),
    new Dish("Stuffed Mushrooms", 8.20, FoodCatagory.Appetizer),
    new Dish("Greek Salad", 10.00, FoodCatagory.Appetizer),

    new Dish("Spaghetti Carbonara", 14.30, FoodCatagory.MainCourse),
    new Dish("Beef Steak", 21.99, FoodCatagory.MainCourse),
    new Dish("Chicken Alfredo", 16.50, FoodCatagory.MainCourse),
    new Dish("Vegetable Stir Fry", 13.20, FoodCatagory.MainCourse),

    new Dish("Panna Cotta", 7.50, FoodCatagory.Dessert),
    new Dish("Chocolate Lava Cake", 8.90, FoodCatagory.Dessert),
    new Dish("Fruit Tart", 6.70, FoodCatagory.Dessert),

    new Dish("Latte", 3.50, FoodCatagory.Beverage),
    new Dish("Green Tea", 2.80, FoodCatagory.Beverage),
    new Dish("Fresh Orange Juice", 4.20, FoodCatagory.Beverage),
    new Dish("Mineral Water", 1.50, FoodCatagory.Beverage),
};


MyGUI gUIManager = new MyGUI();
Menu menu = new Menu();
menu.Init(myDishes);

OrderManager orderManager = new OrderManager(menu);
MenuManager menuManager = new MenuManager(menu);
menuManager.SepearateOntoCategories();




async Task<string> SendOrderToChef(OrderTask order)
{
    Order gottenOrder = order.Order;
    OrderMessage message = new OrderMessage(gottenOrder.OrderId, gottenOrder.OrderedDish, gottenOrder.amount);

    try
    {
        string response = await publisher.CallRpcAsync(message, TimeSpan.FromSeconds(10));
        //Console.WriteLine($"[.] Got response for Order {order.Order.OrderId}: {response}");

        order.CompletionSignal.TrySetResult(true);
        return response;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[!] Error for Order {order.Order.OrderId}: {ex.Message}");
        order.CompletionSignal.TrySetException(ex);
        return $"Error: {ex.Message}";
    }

}






bool weAreWorking = true;

bool ifUserWantsToExit = gUIManager.askUser("Would you like to orde something?");

if (!ifUserWantsToExit)
    weAreWorking = false;
while (weAreWorking)
{
    var ordering = true;
    List<Order> orders = new List<Order>();
    while (ordering)
    {

        // Ask for the user's category
        var userCategory = gUIManager.getUserCategoryChoice();

        // Define appropriate category table
        var choosedCategoryOfDishes = menuManager.DividedOntoCategories[userCategory];

        // Display appropriate table
        gUIManager.DisplayMenuTable(userCategory, choosedCategoryOfDishes);

        // Get from user dish index
        int userDishIndex = gUIManager.getUserDishIndex(choosedCategoryOfDishes.Keys.ToList());

        // Get from user amount of dishes he'd like
        var amountOfDishes = gUIManager.getUserAmountOfDishes();

        // Create order
        Order createdOrder = orderManager.CreateOrder(amountOfDishes, choosedCategoryOfDishes[userDishIndex]);
        orders.Add(createdOrder);

        bool continueOrdering = gUIManager.askUser("Would you like to order something else?");
        if (!continueOrdering)
            ordering = false;
    }
    Dictionary<int, OrderTask> orderTasks = new Dictionary<int, OrderTask> { };

    // Push orders to order pool
    foreach (var order in orders)
    {
        orderManager.PushOrderToOrdersPool(order);
        orderTasks.Add(order.OrderId, new OrderTask(order));
    }

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Your orders have been added to the orderTable and send to the Chef. Please, wait...");
    Console.ResetColor();

    // Display orders' table


    await gUIManager.DisplayOrdersTable(orderTasks, SendOrderToChef);

    Console.WriteLine();
    AnsiConsole.MarkupLine("[green]✅ All orders completed![/]");

    bool ifUserWantsToOrderSthElse = gUIManager.askUser("Would you like to orde something?");
    if (!ifUserWantsToOrderSthElse)
        weAreWorking = false;
}



Console.WriteLine("Thank you for using our application! Goodbye! 👋");
return;
/// ------------------------------- EXPEREMENTING WITH SPECTRE CONSOLE ------------------------------- ///



/// ------------------------------- EXPEREMENTING WITH SPECTRE CONSOLE ------------------------------- ///


//string message = "Order Created: OrderID 12345";

//string response = "";
//try
//{
//    response = await publisher.CallRpcAsync(message, TimeSpan.FromSeconds(10));
//}
//catch { Console.WriteLine("Run out!"); }
//finally
//{
//    Console.WriteLine("RPC call completed.");
//}
//Console.WriteLine($" [.] Got '{response}'");
