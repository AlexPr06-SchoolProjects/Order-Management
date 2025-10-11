namespace App.Order
{
    using App.Dish;

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
}
