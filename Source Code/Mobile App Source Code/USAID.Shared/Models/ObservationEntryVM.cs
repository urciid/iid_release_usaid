using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SQLite.Net.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
	public class ObservationEntryVM
	{
		public ObservationEntry ObservationEntry { get; set;}
		public Period Period { get; set; }
		public bool Enabled { get; set;}
		//public ObservableCollection<ObservationChange> Changes { get; set; }
		//public ObservableCollection<ObservationAttachment> Attachments { get; set; }
		//public ObservableCollection<ObservationComment> Comments { get; set; }

	}
}
