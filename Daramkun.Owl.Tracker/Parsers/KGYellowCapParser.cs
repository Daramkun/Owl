using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Daramkun.Owl.Tracker.Parsers
{
	public class KGYellowCapParser : IParser
	{
		public string CorpName { get { return "KG옐로우캡"; } }
		public string URL { get { return "https://www.kgyellowcap.co.kr/iframe-delivery.html?delivery={0}"; } }
		public bool IsEuckr { get { return false; } }
		public bool IsWebBrowserMode { get { return false; } }

		private EndPointState GetState ( string str )
		{
			if ( str.IndexOf ( "물품을 접수" ) >= 0 ) return EndPointState.Picking;
			else if ( str.IndexOf ( "물품이 출발" ) >= 0 ) return EndPointState.GetOnToTrunk;
			else if ( str.IndexOf ( "물품이 도착" ) >= 0 ) return EndPointState.GetOffToTrunk;
			else if ( str.IndexOf ( "배송준비중" ) >= 0 ) return EndPointState.DeliveryStart;
			else if ( str.IndexOf ( "물품을 받으셨습니다" ) >= 0 ) return EndPointState.DeliveryComplete;
			else return EndPointState.Unknown;
		}

		public IEnumerable<EndPoint> Parse ( StreamReader stream, TrackingItem item )
		{
			string str = stream.ReadToEnd ().Replace ( "\t", "" ).Replace ( "\n", "" ).Replace ( "\r", "" ).Replace ( "  ", "" );
			string record = "<td>(([0-9 ]|:|\\-)+)</td>" + "<td class=\"l\">(([a-zA-Z0-9가-힣. ]|\\(|\\)|\\-)+)</td>" +
				"<td>([a-zA-Z0-9가-힣 ]+)</td>" + "<td>([0-9]+)</td>";
			Regex regex = new Regex ( record );
			foreach ( Match v in regex.Matches ( str ) )
			{
				EndPoint ep = new EndPoint ();
				ep.ArrivalTime = DateTime.Parse ( v.Groups [ 1 ].Value );
				ep.Terminal = v.Groups [ 5 ].Value;
				ep.OriginalState = v.Groups [ 3 ].Value;
				ep.State = GetState ( v.Groups [ 3 ].Value );
				yield return ep;
			}
		}
	}
}
