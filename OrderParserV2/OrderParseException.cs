using System;
namespace OrderParser.Src
{
	public class OrderParseException : Exception
	{
		public bool IsSuccess;
		public int LineNumber;
		public OrderParseException(string message, int lineNumber) : base(message)
        {
            LineNumber = lineNumber;
        }
    }
}

