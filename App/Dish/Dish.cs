namespace App.Dish
{
    using App.FoodCatagory;
    class Dish
    {
        public string Name { get; init; }
        public double Price { get; init; }
        public FoodCatagory Category { get; init; }

        public static int LastId { get; private set; } = 0;
        public int DishId { get; private set; }
        public Dish(string name, double price, FoodCatagory category)
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
