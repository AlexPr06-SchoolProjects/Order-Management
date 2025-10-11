using App.Dish;
using App.Order;
using App.Menu;
using App.OrderManagement;
using App.FoodCatagory;
using App.GUIManager;
using Spectre.Console;
using Order_Management.RabbitMQConfig;
using RabbitMQRpcConsumer = Consumer.Consumer;
using RabbitMQRpcPublisher = Publisher.Publisher;
Console.OutputEncoding = System.Text.Encoding.UTF8; 


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


GUIManager gUIManager = new GUIManager();
Menu menu = new Menu();
menu.Init(myDishes);

OrderManager orderManager = new OrderManager(menu);
MenuManager menuManager = new MenuManager(menu);
menuManager.SepearateOntoCategories();

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

// Push order to Pool of orders
orderManager.PushOrderToOrdersPool(createdOrder);

// Display orders' table
await gUIManager.DisplayOrdersTable(orderManager.OrderPool);


/// ------------------------------- EXPEREMENTING WITH SPECTRE CONSOLE ------------------------------- ///



/// ------------------------------- EXPEREMENTING WITH SPECTRE CONSOLE ------------------------------- ///





RabbitMQConfig rabbitMQConfig = new RabbitMQConfig();

await using RabbitMQRpcConsumer consumer = new RabbitMQRpcConsumer(rabbitMQConfig);
bool isConnectedConsumer = await consumer.ConnectAsync();
if (!isConnectedConsumer)
{
    Console.WriteLine("Failed to connect to RabbitMQ.");
    return;
}
bool isQueueCreatedConsumer = await consumer.CreateQueueAsync();
if (!isQueueCreatedConsumer)
{
    Console.WriteLine("Failed to create queue.");
    return;
}
bool isBoundConsumer = await consumer.BindRabbitMQQueueAsync();
if (!isBoundConsumer)
{
    Console.WriteLine("Failed to bind queue to exchange.");
    return;
}
await consumer.StartListeneningAsync();



await using RabbitMQRpcPublisher publisher = new RabbitMQRpcPublisher(rabbitMQConfig);
bool isConnected = await publisher.ConnectAsync();
if (!isConnected)
{
    Console.WriteLine("Failed to connect to RabbitMQ.");
    return;
}

bool isExchangeCreated = await publisher.CreateExchangeAsync();
if (!isExchangeCreated)
{
    Console.WriteLine("Failed to create exchange.");
    return;
}

Console.WriteLine("Exchange created successfully.");

bool isQueueCreated = await publisher.CreateQueueAsync();

if (!isQueueCreated)
{
    Console.WriteLine("Failed to create queue.");
    return;
}
Console.WriteLine("Queue created successfully.");

bool isBound = await publisher.BindRabbitMQQueueAsync();
if (!isBound)
{
    Console.WriteLine("Failed to bind queue to exchange.");
    return;
}

string message = "Order Created: OrderID 12345";

await publisher.InitializeRpcAsync();
string response = "";
try
{
    response = await publisher.CallRpcAsync(message, TimeSpan.FromSeconds(5));
}
catch { Console.WriteLine("Run out!"); }
finally
{
    Console.WriteLine("RPC call completed.");
}
Console.WriteLine($" [.] Got '{response}'");
