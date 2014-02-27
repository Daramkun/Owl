using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Daramkun.Owl.Tracker
{
	public static class Tracker
	{
		static Dictionary<string, IParser> parsers = new Dictionary<string, IParser> ();

		public static Encoding EucKrEncoding { get; set; }

		static Tracker () { EucKrEncoding = Encoding.GetEncoding ( "euc-kr" ); }

		public static void AddTrackingParser ( IParser parser )
		{
			parsers.Add ( parser.CorpName, parser );
		}

		public static async Task<IEnumerable<EndPoint>> Tracking ( TrackingItem trackingItem )
		{
			IParser parser = parsers [ trackingItem.ParserName ];
			if ( parser.IsWebBrowserMode ) return null;
			HttpWebRequest req = HttpWebRequest.CreateHttp ( string.Format ( parser.URL, trackingItem.ItemNumber ) );
			req.Headers [ HttpRequestHeader.ContentEncoding ] = "utf-8";
			WebResponse response = await req.GetResponseAsync ();
			Stream stream = response.GetResponseStream ();
			return parsers [ trackingItem.ParserName ].Parse ( new StreamReader ( stream, parser.IsEuckr ? EucKrEncoding : Encoding.UTF8 ), trackingItem );
		}
	}
}
