using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Daramkun.Owl.Tracker
{
	public class TrackingItem : INotifyPropertyChanged
	{
		[XmlIgnore]
		string itemName, parserName, itemNumber;
		[XmlIgnore]
		DateTime addDate;
		[XmlIgnore]
		int lastTrackedCount;

		public string ItemName { get { return itemName; } set { itemName = value; PC ( "ItemName" ); } }
		public string ParserName { get { return parserName; } set { parserName = value; PC ( "ParserName" ); } }
		public string ItemNumber { get { return itemNumber; } set { itemNumber = value; PC ( "ItemNumber" ); } }
		public DateTime AddDate { get { return addDate; } set { addDate = value; PC ( "AddDate" ); } }
		public int LastTrackedCount { get { return lastTrackedCount; } set { lastTrackedCount = value; PC ( "LastTrackedCount" ); } }

		private void PC ( string property )
		{
			if ( PropertyChanged != null ) PropertyChanged ( this, null );
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
