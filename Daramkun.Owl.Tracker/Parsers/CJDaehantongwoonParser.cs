using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Daramkun.Owl.Tracker.Parsers
{
	public class CJDaehantongwoonParser : IParser
	{
		public string CorpName { get { return "CJ대한통운"; } }
		public string URL { get { return "https://www.doortodoor.co.kr/parcel/doortodoor.do?fsp_action=PARC_ACT_002&fsp_cmd=retrieveInvNoACT&invc_no={0}"; } }
		public bool IsEuckr { get { return false; } }
		public bool IsWebBrowserMode { get { return false; } }

		private EndPointState GetState ( string str )
		{
			switch ( str )
			{
				case "상품인수": return EndPointState.Picking;
				case "상품이동중": return EndPointState.GetOnToTrunk;
				case "배달지도착": return EndPointState.GetOffToTrunk;
				case "배달출발": return EndPointState.DeliveryStart;
				case "배달완료": return EndPointState.DeliveryComplete;
				default: return EndPointState.Unknown;
			}
		}

		public IEnumerable<EndPoint> Parse ( StreamReader stream, TrackingItem item )
		{
			string str = stream.ReadToEnd ().Replace ( "\t", "" ).Replace ( "\n", "" ).Replace ( "\r", "" ).Replace ( "    ", "" );
			string record = "<td class=\"([a-zA-Z0-9_ ]*)\">((([a-zA-Z0-9_ 가-힣\\*,\\.:\\-]|\\(|\\)|[<br />])+)|(<a href=\"javascript:checkDetail\\('[A-Z0-9]+'\\);\" title=\"영업소 정보 팝업\">[a-zA-Z0-9_ 가-힣\\*,\\.:-]+</a>))</td>";
			Regex regex = new Regex ( string.Format ( "<tr>({0})+</tr>", record ) );
			Regex regex2 = new Regex ( record );
			bool skip = false;
			foreach ( Match v in regex.Matches ( str ) )
			{
				if ( skip == false ) { skip = true; continue; }
				EndPoint ep = new EndPoint ();
				int index = 0;
				foreach ( Match vv in regex2.Matches ( v.Value ) )
				{
					switch ( index++ )
					{
						case 0: ep.State = GetState ( vv.Groups [ 2 ].Value ); ep.OriginalState = vv.Groups [ 2 ].Value; break;
						case 1: ep.ArrivalTime = DateTime.Parse ( vv.Groups [ 2 ].Value ); break;
						case 3:
							string t = vv.Groups [ 2 ].Value;
							ep.Terminal = t.Substring ( t.IndexOf ( '>' ) + 1, t.IndexOf ( '<', t.IndexOf ( '>' ) ) - t.IndexOf ( '>' ) - 1 );
							break;
					}
				}
				yield return ep;
			}
		}
	}
}
