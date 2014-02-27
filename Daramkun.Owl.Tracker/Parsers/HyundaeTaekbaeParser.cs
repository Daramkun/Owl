using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Daramkun.Owl.Tracker.Parsers
{
	public class HyundaeTaekbaeParser : IParser
	{
		public string CorpName { get { return "현대택배"; } }
		public string URL { get { return "http://www.hlc.co.kr/personalService/tracking/06/tracking_goods_result.jsp?InvNo={0}"; } }
		public bool IsEuckr { get { return true; } }
		public bool IsWebBrowserMode { get { return false; } }

		private EndPointState GetState ( string str )
		{
			if ( str.IndexOf ( "예약을 하셨습니다" ) >= 0 ) return EndPointState.Storaged;
			else if ( str.IndexOf ( "물품을 보내셨습니다" ) >= 0 ) return EndPointState.Picking;
			else if ( str.IndexOf ( "에서 도착하였습니다" ) >= 0 ) return EndPointState.GetOffToTrunk;
			else if ( str.IndexOf ( "배달 준비중" ) >= 0 ) return EndPointState.DeliveryStart;
			else if ( str.IndexOf ( "배달 완료하였습니다" ) >= 0 ) return EndPointState.DeliveryComplete;
			else if ( str.IndexOf ( "물품을 받으셨습니다" ) >= 0 ) return EndPointState.DeliveryComplete;
			else return EndPointState.Unknown;
		}

		public IEnumerable<EndPoint> Parse ( StreamReader stream, TrackingItem item )
		{
			string str = stream.ReadToEnd ().Replace ( "\t", "" ).Replace ( "\n", "" ).Replace ( "\r", "" ).Replace ( "  ", "" );
			string record = "<td class=\"left\">([0-9.]+)</td>" + "<td>(([0-9]|:|\\-)+)</td>" +
				"<td>((<a href=\"#\" onClick=\"JavaScript:(([a-zA-Z0-9가-힣,' ]|\\(|\\)|\\-)+)\">([a-zA-Z0-9가-힣 ]+)</a>)|(고객))</td>" +
				"<td class=\"left_a\"><p>(([a-zA-Z가-힣0-9. ,@#!+?]|\\-|\\(\\))+)</p></td>";
			Regex regex = new Regex ( record );
			foreach ( Match v in regex.Matches ( str ) )
			{
				EndPoint ep = new EndPoint ();
				ep.ArrivalTime = DateTime.Parse (
					v.Groups [ 2 ].Value != "--:--" ?
						string.Format ( "{0} {1}", v.Groups [ 1 ].Value, v.Groups [ 2 ].Value ) :
						v.Groups [ 1 ].Value
				);
				ep.Terminal = ( v.Groups [ 4 ].Value == "고객" ) ? "고객" : v.Groups [ 8 ].Value;
				ep.OriginalState = v.Groups [ 10 ].Value;
				ep.State = GetState ( v.Groups [ 10 ].Value );
				yield return ep;
			}
		}
	}
}
