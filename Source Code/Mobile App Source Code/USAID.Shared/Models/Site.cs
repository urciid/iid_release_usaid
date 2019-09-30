using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
	public class Site : IModel
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public DateTime Created { get; set; }
		public DateTime Modified { get; set; }
		public bool ModifiedLocally { get; set; }

		public Int32 SiteId { get; set;}

		public string SiteName { get; set; }

		public string Country { get; set; }

		public string FundingType { get; set; }

		public string Partner { get; set; }

		public string Longitude { get; set; }

		public string Lattitude { get; set; }

		public string RuralUrban { get; set; }

		public int? QIIndexScore { get; set; }

		public string AdminDivision1 { get; set; }

		public string AdminDivision2 { get; set; }

		public string AdminDivision3 { get; set; }

		public string asAdminDivision4 { get; set; }

		public string AdminDivisionLabel1 { get; set; }

		public string AdminDivisionLabel2 { get; set; }

		public string AdminDivisionLabel3 { get; set; }

		public string AdminDivisionLabel4 { get; set; }





		public string SiteType { get; set; }

		public string Region { get; set; }

		public string District { get; set;}

		public string SubDistrict { get; set;}

		public string CityTownVillage { get; set;}

		public string Manufacturer { get; set; }

		public string OtherKeyInformation { get; set; }

		public string SiteLine1 { get { return SiteName; }}
		public string SiteLine2 { get { return AdminDivision1 + " " + AdminDivision2 + " " + AdminDivision3; } }
		public string SiteLine3 { get { return District + " " + Country; } }

		//public List<Indicator> Indicators { get; set;}
	}
}

