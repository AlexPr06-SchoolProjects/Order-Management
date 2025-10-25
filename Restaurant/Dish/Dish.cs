namespace Restaurant.Dish
{
    using Restaurant.FoodCategory;
    public class Dish
    {
        public string Name { get; init; }
        public double Price { get; init; }
        public FoodCategory Category { get; init; }

        public static int LastId { get; private set; } = 0;
        public int DishId { get; private set; }
        public Dish(string name, double price, FoodCategory category)
        {
            Name = name;
            Price = price;
            Category = category;
            LastId++;
            DishId = LastId;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
