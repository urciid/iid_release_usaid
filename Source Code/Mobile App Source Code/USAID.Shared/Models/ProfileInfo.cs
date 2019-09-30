using System;
using SQLite.Net.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
    public class ProfileInfo : IModel
    {
        [PrimaryKey]
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public string DealerContactName { get; set; }

        public string DealerName { get; set; }

        public string DealerContactEmail { get; set; }

        public string AutoCcEmail { get; set; }

        public string DealerContactPhone { get; set; }
    }
}

