namespace Order_Management.Order
{
    using Order_Management.Dish;
    struct Order
    {
        private bool _status;

        public int amount;
        public int OrderId { get; init; }
        public int Amount
        {
            get => amount;
            set
            {
                if (value < 0)
                    throw new Exception("Amount cannot be negative");
            }
        }

        private Dish _orderedDish;

        public bool Status { get => _status; init => _status = value; }

        public Order(int orderId, int amountToSet, Dish ordered)
        {
            amount = amountToSet;
            _status = false;
            _orderedDish = ordered;
            OrderId = orderId;
        }
    }
}
