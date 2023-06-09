

using OrderParserV2;
using static OrderParserV2.Order;

List<Order> valid = new List<Order>();
List<Order> inValid = new List<Order>();

while (true)
{
    Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
    Console.WriteLine("Enter a file path to continue:");
    Console.WriteLine("ls: print orders, reset: rest data, exist: close app");

    string filePath = Console.ReadLine() ?? "";

    if (filePath.Trim() == "exit")
        Environment.Exit(0);
    if (filePath.Trim() == "reset")
        continue;
    if (filePath.Trim() == "ls")
    {
        Console.WriteLine("Print orders..");
        printOrder(valid);
        continue;
    }
    if (!File.Exists(filePath) || filePath.Split(".").Last() != "txt")
    {
        Console.WriteLine("Invalid file path or command:");
        break;
    }

    CustomerOrders customerOrders = new CustomerOrders();

    try
    {
        customerOrders.Parse(filePath);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        continue;
    }
    valid = customerOrders.GetValidOrder();
    inValid = customerOrders.GetInValidOrder();

    Console.WriteLine($"Order parse finished, {valid.Count} Succeed, {inValid.Count} Failed");
    if (inValid.Count > 0) 
    {
        Console.WriteLine("_______________________________________________Errors____________________________________________");
        foreach (var order in inValid)
        {
            Console.WriteLine($"Order {order.Id}: {order.ErrorMessage}");
        }
    }

}

void printOrder(List<Order> orders)
{
    if (orders.Count == 0)
        Console.WriteLine("No order found");
    foreach(Order order in orders)
    {
        Console.WriteLine($"* Order: {order.Id}   Total Item: {order.header.TotalItem}    Total Cost: {order.header.TotalCost}");
        Console.WriteLine($"  Paid: {order.header.Paid}     Shipped: {order.header.Shipped}     Completed: {order.header.Complete}");
        Console.WriteLine($"  Customer: {order.header.CustomerName}");
        Console.WriteLine($"  Phone: {order.header.CustomerPhone}     Email: {order.header.CustomerEmail}");
        Console.WriteLine($"  Address: {order.address.AddressLine1} {order.address.AddressLine2 ?? ""} {order.address.City}, {order.address.State}, {order.address.Zip}");
        Console.WriteLine(" ");
        foreach (OrderDetail item in order.OrderDetails)
        {
            Console.WriteLine($"    {item.LineNumber}   {item.Description}              Unit price: $ {item.CostEach}   Qt: {item.CostEach}  Total: ${item.CostTotal}");
        }
        Console.WriteLine("________________________________________________________________________________________________");
        Console.WriteLine(" ");
    }
}

