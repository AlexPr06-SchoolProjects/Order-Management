namespace Restaurant.Menu
{
    using Restaurant.Dish;
    using Restaurant.FoodCategory;

    public class MenuManager
    {
        private Menu _menu;
        public Dictionary<FoodCategory, Dictionary<int, Dish>> DividedOntoCategories { get; set; }
        public MenuManager(Menu menu)
        {
            _menu = menu;
            DividedOntoCategories = new Dictionary<FoodCategory, Dictionary<int, Dish>>();
            Init();
        }
        private void Init()
        {
            foreach (FoodCategory category in Enum.GetValues(typeof(FoodCategory)))
            {
                DividedOntoCategories.Add(category, new Dictionary<int, Dish>());
            }
        }
        public void SepearateOntoCategories()
        {
            foreach (var (index, dish) in _menu.Dishes)
            {
                DividedOntoCategories[dish.Category].Add(index, dish);
            }
        }
    }
}
