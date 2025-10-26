using Restaurant.Dish;
using Restaurant.FoodCategory;
using Restaurant.Menu;
using Restaurant.Order;
using Restaurant.OrderManagement;
using MyGUI = Restaurant.GUI.GUIManager;

using RabbitMQRpcPublisher = Publisher.Publisher;
using RabbitMQConfiguration = RabbitMQConfig.RabbitMQConfig;

using Spectre.Console;


// Set console to UTF8
Console.OutputEncoding = System.Text.Encoding.UTF8;

RabbitMQConfiguration rabbitMQConfig = new RabbitMQConfiguration();

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
    new Dish("Pizza Margherita", 12.99, FoodCategory.MainCourse),
    new Dish("Caesar Salad", 9.99, FoodCategory.Appetizer),
    new Dish("Grilled Salmon", 17.49, FoodCategory.MainCourse),
    new Dish("Cheesecake", 6.99, FoodCategory.Dessert),
    new Dish("Espresso", 2.50, FoodCategory.Beverage),
    new Dish("Bruschetta", 7.20, FoodCategory.Appetizer),
    new Dish("Tiramisu", 8.40, FoodCategory.Dessert),
    new Dish("Cappuccino", 3.10, FoodCategory.Beverage),

    new Dish("Garlic Bread", 5.50, FoodCategory.Appetizer),
    new Dish("Stuffed Mushrooms", 8.20, FoodCategory.Appetizer),
    new Dish("Greek Salad", 10.00, FoodCategory.Appetizer),

    new Dish("Spaghetti Carbonara", 14.30, FoodCategory.MainCourse),
    new Dish("Beef Steak", 21.99, FoodCategory.MainCourse),
    new Dish("Chicken Alfredo", 16.50, FoodCategory.MainCourse),
    new Dish("Vegetable Stir Fry", 13.20, FoodCategory.MainCourse),

    new Dish("Panna Cotta", 7.50, FoodCategory.Dessert),
    new Dish("Chocolate Lava Cake", 8.90, FoodCategory.Dessert),
    new Dish("Fruit Tart", 6.70, FoodCategory.Dessert),

    new Dish("Latte", 3.50, FoodCategory.Beverage),
    new Dish("Green Tea", 2.80, FoodCategory.Beverage),
    new Dish("Fresh Orange Juice", 4.20, FoodCategory.Beverage),
    new Dish("Mineral Water", 1.50, FoodCategory.Beverage),
};


MyGUI gUIManager = new MyGUI();
Menu menu = new Menu();
menu.Init(myDishes);

OrderManager orderManager = new OrderManager(menu);
MenuManager menuManager = new MenuManager(menu);
menuManager.SepearateOntoCategories();
List<object> ChefAnswers = new List<object>();



async Task<string> SendOrderToChef(OrderTask order)
{
    Order gottenOrder = order.Order;
    OrderMessage message = new OrderMessage(gottenOrder.OrderId, gottenOrder.OrderedDish, gottenOrder.amount);

    try
    {
        string response = await publisher.CallRpcAsync(message, TimeSpan.FromSeconds(10));
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

bool ifUserWantsToExit = gUIManager.askUser("Would you like to order something?");

if (!ifUserWantsToExit)
    weAreWorking = false;
while (weAreWorking)
{
    var ordering = true;
    List<Order> orders = new List<Order>();
    List<string> chefAnswers = new List<string>();
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

    // Fullfil chefAnswers list with chef's replies from server
    await gUIManager.DisplayOrdersTable(orderTasks, SendOrderToChef, chefAnswers);

    Console.WriteLine();
    AnsiConsole.MarkupLine("[green]✅ All orders completed![/]");

    gUIManager.DisplayBill(chefAnswers);

    bool ifUserWantsToOrderSthElse = gUIManager.askUser("Would you like to orde something?");
    if (!ifUserWantsToOrderSthElse)
        weAreWorking = false;
}


Console.WriteLine("Thank you for using our application! Goodbye! 👋");
return;
