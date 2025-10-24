namespace Restaurant.Order
{
    using Restaurant.Dish;

    class Order
    {
        public bool Status { get; set; }

        public int amount;
        public int OrderId { get; init; }
        public Dish OrderedDish { get; set; }
        public int Amount
        {
            get => amount;
            set
            {
                if (value < 0)
                    throw new Exception("Amount cannot be negative");
            }
        }

        public Order(int orderId, int amountToSet, Dish ordered)
        {
            amount = amountToSet;
            Status = false;
            OrderedDish = ordered;
            OrderId = orderId;
        }
    }

    class OrderTask
    {
        public Order Order { get; set; }
        public TaskCompletionSource<bool> CompletionSignal { get; set; }

        public OrderTask(Order order)
        {
            Order = order;
            CompletionSignal = new TaskCompletionSource<bool>();
        }
    }

     class OrderMessage
    {
        public int OrderId { get; set; }
        public Dish OrderedDish { get; set; }
        public int Amount { get; set; }

        public OrderMessage(int orderId, Dish orderedDish, int amount)
        {
            OrderId = orderId;
            OrderedDish = orderedDish;
            Amount = amount;
        }
    }
}
