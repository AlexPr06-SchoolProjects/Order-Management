namespace App.OrderManagement
{
    using App.Dish;
    using App.Menu;
    using App.Order;

    class OrderManager
    {
        private List<Order> _orderPool = new List<Order>();
        private int _poolLength { get; set; }
        private Menu? _menu;

        public OrderManager(Menu referenceMenu)
        {
            _menu = referenceMenu;
        }

        void increasePoolLength()
        {
            _poolLength++;
        }

        public void CreateOrder(int amount, Dish dish)
        {
            if (_menu == null)
                throw new Exception("Menu is not set");
            if (!_menu.ContainsDish(dish))
                throw new Exception("Dish is not in the menu");

            increasePoolLength();
            Order newOrder = new Order(_poolLength, amount, dish);
            _orderPool.Add(newOrder);
        }

        public void DisplayOrders()
        {
            foreach (var order in _orderPool)
            {
                Console.WriteLine($"Order ID: {order.OrderId}  -  Amount: {order.Amount}  -  Status: {(order.Status ? "Completed" : "Pending")}");
            }
        }
    }
}
