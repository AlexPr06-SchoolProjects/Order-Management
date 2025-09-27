// See https://aka.ms/new-console-template for more information

using Order_Management.Dish;
using Order_Management.Menu;
using Order_Management.Order;
using Order_Management.OrderManagement;


List<Dish> myDishes = new List<Dish>()
{
    new Dish("Bruschetta", 6.50, Categories.Appetizer),
    new Dish("Caesar Salad", 7.00, Categories.Appetizer),
    new Dish("Margherita Pizza", 12.00, Categories.MainCourse),
    new Dish("Spaghetti Carbonara", 14.00, Categories.MainCourse),
    new Dish("Tiramisu", 8.00, Categories.Dessert),
    new Dish("Panna Cotta", 7.50, Categories.Dessert),
    new Dish("Coca-Cola", 3.00, Categories.Beverage),
    new Dish("Espresso", 2.50, Categories.Beverage)
};


Menu menu = new Menu();
menu.Init(myDishes);
menu.DisplayMenu();

Console.WriteLine($"Current static dishId: {Dish.LastId}");


OrderManager orderManager = new OrderManager(menu);

int index = 1;
Dish dishToOrder = myDishes[index];


orderManager.CreateOrder(2, dishToOrder);
orderManager.CreateOrder(1, myDishes[3]);
orderManager.DisplayOrders();

enum Categories
{
    Appetizer,
    MainCourse,
    Dessert,
    Beverage
}

