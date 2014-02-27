using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Daramkun.Owl.Tracker.Parsers
{
	public class DongbuExpressParser : IParser
	{
		public string CorpName { get { return "동부익스프레스"; } }
		public string URL { get { return "http://www.dongbups.com/newHtml/delivery/dvsearch_View.jsp?item_no={0}"; } }
		public bool IsEuckr { get { return true; } }
		public bool IsWebBrowserMode { get { return false; } }

		private EndPointState GetState ( string str )
		{
			switch ( str )
			{
				case "집하": return EndPointState.Picking;
				case "간선출고": return EndPointState.GetOnToTrunk;
				case "배달지도착": return EndPointState.GetOffToTrunk;
				case "배송출발": return EndPointState.DeliveryStart;
				case "배송완료": return EndPointState.DeliveryComplete;
				default: return EndPointState.Unknown;
			}
		}

		public IEnumerable<EndPoint> Parse ( StreamReader stream, TrackingItem item )
		{
			string str = stream.ReadToEnd ().Replace ( "\t", "" ).Replace ( "\n", "" ).Replace ( "\r", "" ).Replace ( "  ", "" );
			string record = "<td>([0-9.]+)</td>" + "<td>([0-9:]+)</td>" +
				"<td>(([a-zA-Z가-힣0-9. ,]|-|/)+)</td>" +
				"<td class=\"lst\">([a-zA-Z가-힣0-9. ,@#!+?]+)</td>";
			Regex regex = new Regex ( record );
			foreach ( Match v in regex.Matches ( str ) )
			{
				if ( v.Groups [ 5 ].Value == "운송장출력" ) continue;
				EndPoint ep = new EndPoint ();
				ep.ArrivalTime = DateTime.Parse ( string.Format ( "{0} {1}", v.Groups [ 1 ].Value, v.Groups [ 2 ].Value ) );
				ep.Terminal = v.Groups [ 3 ].Value.Split ( '/' ) [ 0 ];
				ep.OriginalState = v.Groups [ 5 ].Value;
				ep.State = GetState ( v.Groups [ 5 ].Value );
				yield return ep;
			}
		}
	}
}
