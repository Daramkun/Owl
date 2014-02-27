using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Daramkun.Owl.Tracker.Parsers
{
	public class LogenTaekbaeParser : IParser
	{
		public string CorpName { get { return "로젠택배"; } }
		public string URL { get { return "http://www.ilogen.com/iLOGEN.Web.New/TRACE/TraceView.aspx?gubun=slipno&slipno={0}"; } }
		public bool IsEuckr { get { return true; } }
		public bool IsWebBrowserMode { get { return false; } }

		private EndPointState GetState ( string str )
		{
			if ( str.IndexOf ( "에서 도착하였습니다" ) >= 0 ) return EndPointState.GetOffToTrunk;
			else if ( str.IndexOf ( "로 출발하였습니다" ) >= 0 ) return EndPointState.GetOnToTrunk;
			else if ( str.IndexOf ( "도착했습니다" ) >= 0 ) return EndPointState.GetOffToTrunk;
			else if ( str.IndexOf ( "배달 준비 중입니다" ) >= 0 ) return EndPointState.DeliveryStart;
			else if ( str.IndexOf ( "배달 완료하였습니다" ) >= 0 ) return EndPointState.DeliveryComplete;
			else return EndPointState.Unknown;
		}

		public IEnumerable<EndPoint> Parse ( StreamReader stream, TrackingItem item )
		{
			string str = stream.ReadToEnd ().Replace ( "\t", "" ).Replace ( "\n", "" ).Replace ( "\r", "" ).Replace ( "  ", "" );
			string record = "<td width=\"100\">([가-힣]+)</td><td align=\"left\">(([a-zA-Z0-9가-힣. ]|\\(|\\)|\\-)+)</td>";
			Regex regex = new Regex ( record );
			foreach ( Match v in regex.Matches ( str ) )
			{
				EndPoint ep = new EndPoint ();
				ep.ArrivalTime = DateTime.MinValue;
				ep.Terminal = v.Groups [ 1 ].Value;
				ep.OriginalState = v.Groups [ 2 ].Value;
				ep.State = GetState ( v.Groups [ 2 ].Value );
				yield return ep;
			}
		}
	}
}
