using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
	public class Observation : IModel
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public DateTime Created { get; set; }
		public DateTime Modified { get; set; }
		public bool ModifiedLocally { get; set; }

		public string Title { get; set; }
		//public int ObservationId { get; set; }
		public int Observation_id { get; set;}
		public int Indicator_id { get; set; }
		public int Site_id { get; set;}
		public DateTime? Begin_Date { get; set;}
		public DateTime? End_Date { get; set;}


		//[OneToOne(CascadeOperations = CascadeOperation.All)]
		//public Period SelectedPeriod { get; set; }

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public ObservableCollection<ObservationChange> Changes { get; set; }

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public ObservableCollection<ObservationComment> Comments { get; set; }

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public ObservableCollection<ObservationAttachment> Attachments { get; set; }

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public ObservableCollection<ObservationEntry> ObservationEntries { get; set; }

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public ObservableCollection<Period> ObservationPeriods { get; set; }


	}
}
