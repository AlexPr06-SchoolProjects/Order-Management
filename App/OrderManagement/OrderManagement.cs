namespace App.OrderManagement
{
    using App.Dish;
    using App.Menu;
    using App.Order;
    using Spectre.Console;

    class OrderManager
    {
        public Dictionary<int, Order> OrderPool { get; set; }
        private int PoolLength { get; set; }
        private readonly Menu? _menu;

        public OrderManager(Menu referenceMenu)
        {
            _menu = referenceMenu;
            OrderPool = new Dictionary<int, Order>();
        }

        private void IncreasePoolLength()
        {
            PoolLength++;
        }

        public Order CreateOrder(int amount, Dish dish)
        {
            if (_menu == null)
                throw new Exception("Menu is not set");
            if (!_menu.ContainsDish(dish))
                throw new Exception("Dish is not in the menu");

            IncreasePoolLength();
            Order newOrder = new Order(PoolLength, amount, dish);
            return newOrder;
        }

        public void FullfillAsSuccessfullOrder(int orderIndex)
        {
            if (orderIndex < 0 || orderIndex >= OrderPool.Count)
                return;
            OrderPool[orderIndex].Status = true;
        }

        public void PushOrderToOrdersPool(Order orderToPush) {
            IncreasePoolLength();
            OrderPool.Add(PoolLength, orderToPush);
        }

        public void RemoveOrderFromOrdersPool(int orderIndex) {
            IncreasePoolLength();
            OrderPool.Remove(orderIndex);
        }
    }
}
