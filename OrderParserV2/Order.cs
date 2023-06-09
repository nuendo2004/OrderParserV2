using OrderParser.Src;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace OrderParserV2
{
    public class Order
    {
        public int Id;
        public List<OrderDetail> OrderDetails = new List<OrderDetail>();
        public string ErrorMessage = "";
        public bool IsValid;
        public Header header = new Header();
        public Address address = new Address();
        public List<OrderDetail> detail = new List<OrderDetail>();
        public class Header
        {
            public int OrderNumber = 0;
            public int TotalItem;
            public decimal TotalCost;
            public DateTime OrderDate = DateTime.MinValue;
            public string CustomerName = "";
            public string CustomerPhone = "";
            public string CustomerEmail = "";
            public bool Paid;
            public bool Shipped;
            public bool Complete;
            public bool IsHeaderValid;
            public bool Parse(string line)
            {
                bool order = int.TryParse(line.Substring(3, 10).TrimStart().TrimEnd(), out OrderNumber);
                bool itemNumber = int.TryParse(line.Substring(13, 5).TrimStart().TrimEnd(), out TotalItem);
                bool cost = decimal.TryParse(line.Substring(18, 10).TrimStart().TrimEnd(), out TotalCost);
                bool date = DateTime.TryParse(line.Substring(28, 19).TrimStart().TrimEnd(), out OrderDate);
                CustomerName = line.Substring(47, 50).TrimStart().TrimEnd();
                CustomerPhone = line.Substring(97, 30).TrimStart().TrimEnd();
                CustomerEmail = line.Substring(127, 50).TrimStart().TrimEnd();
                bool paid = ParseBool(line.Substring(177, 1), ref Paid);
                bool shipped = ParseBool(line.Substring(178, 1), ref Shipped);
                bool complete = ParseBool(line.Substring(179, 1), ref Complete);
                IsHeaderValid = order && itemNumber && cost && date && paid && shipped && complete && CustomerName.Length > 0 && CustomerPhone.Length > 10 && CustomerEmail.Contains("@");
                return IsHeaderValid;
            }
        }

        public class Address
        {
            public string AddressLine1 = "";
            public string AddressLine2 = "";
            public string City = "";
            public string State = "";
            public string Zip = "";
            public bool IsAddressValid;
            public bool Parse(string line)
            {
                AddressLine1 = line.Substring(3, 50).TrimStart().TrimEnd();
                AddressLine2 = line.Substring(53, 50).TrimStart().TrimEnd();
                City = line.Substring(103, 50).TrimStart().TrimEnd();
                State = line.Substring(153, 2).TrimStart().TrimEnd();
                Zip = line.Substring(155).TrimStart().TrimEnd();
                IsAddressValid =  AddressLine1.Length > 1 && City.Length > 1 && State.Length == 2 && Zip.Length > 0;
                return IsAddressValid;
            }
        }
        public class OrderDetail
        {
            public int LineNumber;
            public decimal Quantity;
            public decimal CostEach;
            public decimal CostTotal;
            public string Description = "";
            public bool IsDetailValid;
            public bool Parse(string line)
            {
                bool lineNumber = int.TryParse(line.Substring(3, 2).TrimStart().TrimEnd(), out LineNumber);
                bool quantity = decimal.TryParse(line.Substring(5, 5), out Quantity);
                bool costEach = decimal.TryParse(line.Substring(10, 10), out CostEach);
                bool costTotal = decimal.TryParse(line.Substring(20, 10), out CostTotal);
                Description = line.Substring(30).TrimStart().TrimEnd();
                IsDetailValid = lineNumber && quantity && costEach && costTotal && CostTotal == CostEach * Quantity && Description.Length > 0;  
                return IsDetailValid;
            }
        }

        private static bool ParseBool(string substring, ref bool state)
        {
            int boolean;
            bool success = int.TryParse(substring, out boolean);
            state = boolean == 1;
            return success;
        }

        private bool IsValidOrder()
        {
            return header.IsHeaderValid && address.IsAddressValid && detail.Count > 0 && detail.FindAll(detail => detail.IsDetailValid == false).Count == 0;
        }

        public void Parse(string line, int lineNumber)
        {
            try
            {
                switch (line.Substring(0, 3))
                {
                    case "100":
                        if (!header.Parse(line))
                        {
                            ErrorMessage = $"Header parse error at line {lineNumber}";
                        }
                        Id = header.OrderNumber;
                        break;
                    case "200":
                        if (!address.Parse(line))
                        {
                            ErrorMessage = $"Address parse error at line {lineNumber}";
                        }
                        break;
                    case "300":
                        OrderDetail item = new OrderDetail();
                        if (!item.Parse(line))
                        {
                            ErrorMessage = $"Order detail parse error at line {lineNumber}";
                        }
                        detail.Add(item);
                        if (IsValidOrder())
                            IsValid = true;
                        else IsValid = false;
                        break;
                    default:
                        throw new OrderParseException("Invalid line number", lineNumber);
                }
            }
            catch (OrderParseException e)
            {
                IsValid = false;
                ErrorMessage = $"{e.Message}, loacation: line {e.LineNumber}";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "DWDW");
            }
        }
    }
}
