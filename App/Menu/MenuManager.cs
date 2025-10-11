namespace App.Menu
{
    using App.Dish;
    using App.FoodCatagory;
    using Spectre.Console;

    class MenuManager
    {
        private Menu _menu;
        public Dictionary<FoodCatagory, Dictionary<int, Dish>> DividedOntoCategories { get; set; }
        public MenuManager(Menu menu)
        {
            _menu = menu;
            DividedOntoCategories = new Dictionary<FoodCatagory, Dictionary<int, Dish>>();
            Init();
        }
        private void Init()
        {
            foreach (FoodCatagory category in Enum.GetValues(typeof(FoodCatagory)))
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
