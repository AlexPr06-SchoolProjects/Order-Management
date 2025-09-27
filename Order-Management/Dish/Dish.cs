namespace Order_Management.Dish
{
    class Dish
    {
        public string Name { get; init; }
        public double Price { get; init; }
        public Categories Category { get; init; }

        public static int LastId { get; private set; } = 0;
        public int DishId { get; private set; }
        public Dish(string name, double price, Categories category)
        {
            Name = name;
            Price = price;
            Category = category;
            LastId++;
            DishId = LastId;
        }
    }
}
