namespace Order_Management.Menu
{
    using Order_Management.Dish;
    class Menu
    {
        private Dictionary<int, Dish> _dishes;

        public Menu()
        {
            _dishes = new Dictionary<int, Dish>();
        }

        public int getLastKey()
        {
            return _dishes.Any() ? _dishes.Keys.Max() : 0;
        }

        public void Init(List<Dish> dishesToBeInitialized)
        {
            int lastKey = getLastKey();
            foreach (var dish in dishesToBeInitialized)
            {
                lastKey++;
                _dishes.Add(lastKey, dish);
            }
        }

        public void AddDish(Dish dish)
        {
            int lastKey = getLastKey();
            _dishes.Add(lastKey++, new Dish(dish.Name, dish.Price, dish.Category));
        }

        public void RemoveDish(int key)
        {
            _dishes.Remove(key);
        }

        public void DisplayMenu()
        {
            foreach (var pair in _dishes)
            {
                int dishId = pair.Key;
                Dish dish = pair.Value;
                Console.WriteLine($"{dish.Category} {dish.Name}  -  Price: ${dish.Price}");
            }
        }

        public bool ContainsDish(Dish dish)
        {
            return _dishes.ContainsValue(dish);
        }
    }

}
