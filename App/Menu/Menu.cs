namespace App.Menu
{
    using App.Dish;

    class Menu
    {
        public Dictionary<int, Dish> Dishes { get; set; }

        public Menu()
        {
            Dishes = new Dictionary<int, Dish>();
        }

        public int getLastKey()
        {
            return Dishes.Any() ? Dishes.Keys.Max() : 0;
        }

        public void Init(List<Dish> dishesToBeInitialized)
        {
            int lastKey = getLastKey();
            foreach (var dish in dishesToBeInitialized)
            {
                lastKey++;
                Dishes.Add(lastKey, dish);
            }
        }

        public void AddDish(Dish dish)
        {
            int lastKey = getLastKey();
            Dishes.Add(lastKey++, new Dish(dish.Name, dish.Price, dish.Category));
        }

        public void RemoveDish(int key)
        {
            Dishes.Remove(key);
        }

        public bool ContainsDish(Dish dish)
        {
            return Dishes.ContainsValue(dish);
        }
    }

}
