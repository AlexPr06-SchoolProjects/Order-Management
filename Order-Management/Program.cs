// See https://aka.ms/new-console-template for more information
struct Order
{
    private static int orderId = 0;
    private static int amount = 0;
    private bool status;

    public static int OrderId { get { return Order.orderId; } }
    public static int Amount {
        get => amount;
        set 
        {
            if (value < 0)
                throw new Exception("Amount cannot be negative");
        } 
    }

    public bool Status { get => status; init => status = value; }

    public Order(int amount)
    {
        Amount += amount;
        status = false;
    }

    static Order()
    {
        Order.orderId++;
    }
}

class OrderManager
{

}
