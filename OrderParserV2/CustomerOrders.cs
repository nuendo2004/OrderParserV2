using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderParserV2
{
    public class CustomerOrders
    {
        List<Order> _orders;
        public CustomerOrders() 
        {
            _orders = new List<Order>();
        }

        public void Parse(string filePath)
        {
            StreamReader sr;
            sr = new StreamReader($"{filePath}");

            string? currentLine = sr.ReadLine();
            int lineNumber = 1;
            Order currentOrder = new Order();
            _orders.Add(currentOrder);
            while (currentLine != null)
            {
                Console.WriteLine($"reading line {lineNumber}...");

                currentOrder.Parse(currentLine, lineNumber);
                lineNumber++;
                currentLine = sr.ReadLine();
                if (currentLine != null && currentLine.Substring(0, 3) == "100")
                {
                    currentOrder = new Order();
                    _orders.Add(currentOrder);
                }
            }
            sr.Close();
        }

        public List<Order> GetValidOrder()
        {
            return _orders.FindAll(order => order.IsValid);
        }
        public List<Order> GetInValidOrder()
        {
            return _orders.FindAll(order => !order.IsValid);
        }
    }
}
