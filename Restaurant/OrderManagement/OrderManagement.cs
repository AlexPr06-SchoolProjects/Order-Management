namespace Restaurant.OrderManagement
{
    using Restaurant.Dish;
    using Restaurant.Menu;
    using Restaurant.Order;

    public class OrderManager
    {
        public Dictionary<int, Order> OrderPool { get; set; }
        private int PoolLength { get; set; }
        private readonly Menu? _menu;

        public OrderManager(Menu referenceMenu)
        {
            _menu = referenceMenu;
            OrderPool = new Dictionary<int, Order>();
        }

        private void IncreasePoolLength() => PoolLength++;

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

        public void PushOrderToOrdersPool(Order orderToPush)
        {
            int index = orderToPush.OrderId;
            if (!OrderPool.ContainsKey(index))
                OrderPool.Add(index, orderToPush);
        }

        public void RemoveOrderFromOrdersPool(int orderIndex)
        {
            OrderPool.Remove(orderIndex);
        }
    }
}
