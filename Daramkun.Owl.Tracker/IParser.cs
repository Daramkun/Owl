using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Owl.Tracker
{
	public interface IParser
	{
		string CorpName { get; }
		string URL { get; }
		bool IsEuckr { get; }
		bool IsWebBrowserMode { get; }

		IEnumerable<EndPoint> Parse ( StreamReader stream, TrackingItem item );
	}
}
