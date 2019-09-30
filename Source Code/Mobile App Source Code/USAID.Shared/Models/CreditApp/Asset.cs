using System;
namespace USAID.Models
{
    public class Asset
    {
        public int Quantity { get; set; }

        public string Description { get; set; }

        public double TotalAmount { get; set; }

        public string SerialNumber { get; set; }

        public string Model { get; set; }

        public string Manufacturer { get; set; }

        public int IsUsed { get; set; }
    }
}

