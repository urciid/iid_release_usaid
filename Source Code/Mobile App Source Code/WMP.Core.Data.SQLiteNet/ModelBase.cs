using SQLite.Net.Attributes;
using System;
using Newtonsoft.Json.Serialization;

namespace WMP.Core.Data.SQLiteNet
{
    public interface IModel
    {
        int Id { get; set; }

        DateTime Created { get; set; }

        DateTime Modified { get; set; }

		bool ModifiedLocally { get; set;}
    }
}