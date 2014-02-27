using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daramkun.Owl.Tracker.Parsers
{
	public class EpostParser : IParser
	{
		public string CorpName { get { return "우체국택배"; } }
		public string URL { get { return "http://service.epost.go.kr/trace.RetrieveRegiPrclDeliv.postal?sid1={0}"; } }
		public bool IsEuckr { get { return false; } }
		public bool IsWebBrowserMode { get { return false; } }

		private EndPointState GetState ( string str )
		{
			switch ( str )
			{
				case "접수": return EndPointState.Picking;
				case "발송": return EndPointState.GetOnToTrunk;
				case "도착": return EndPointState.GetOffToTrunk;
				case "배달준비": return EndPointState.DeliveryStart;
				case "배달완료": return EndPointState.DeliveryComplete;
				default: return EndPointState.Unknown;
			}
		}

		public IEnumerable<EndPoint> Parse ( StreamReader stream, TrackingItem item )
		{
			string str = GetRealPage ( item ).Result.ReadToEnd ().Replace ( "\t", "" ).Replace ( "\n", "" ).Replace ( "\r", "" ).Replace ( "  ", "" );
			string record = "<td>([0-9.]+)</td>" + "<td>([0-9:]+)</td>" +
				"<td><a href=\"#\" onclick=\"return goPostDetail\\(([0-9]+), '([가-힣]+)', event\\)\" onkeypress=\"return goPostDetail\\(([0-9]+), '([가-힣]+)', event\\)\">([a-zA-Z가-힣0-9 ]+)</a></td>" +
				"<td>([a-zA-Z가-힣0-9 ]+)</td>";
			Regex regex = new Regex ( record );
			foreach ( Match v in regex.Matches ( str ) )
			{
				EndPoint ep = new EndPoint ();
				ep.ArrivalTime = DateTime.Parse ( string.Format ( "{0} {1}", v.Groups [ 1 ].Value, v.Groups [ 2 ].Value ) );
				ep.Terminal = v.Groups [ 7 ].Value;
				ep.OriginalState = v.Groups [ 4 ].Value;
				ep.State = GetState ( v.Groups [ 4 ].Value );
				yield return ep;
			}
		}

		private async Task<StreamReader> GetRealPage ( TrackingItem item )
		{
			HttpWebRequest req = HttpWebRequest.CreateHttp ( "http://trace.epost.go.kr/xtts/servlet/kpl.tts.common.svl.SttSVL" );
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			Stream stream = await req.GetRequestStreamAsync ();
			byte [] data = Encoding.UTF8.GetBytes ( string.Format ( "target_command=kpl.tts.tt.epost.cmd.RetrieveOrderConvEpostPoCMD&sid1={0}&JspURI=/xtts/tt/epost/trace/Trace_list.jsp",
				item.ItemNumber ) );
			stream.Write ( data, 0, data.Length );
			stream.Dispose ();
			req.Headers [ HttpRequestHeader.ContentEncoding ] = "utf-8";
			WebResponse response = await req.GetResponseAsync ();
			stream = response.GetResponseStream ();
			return new StreamReader ( stream );
		}
	}
}
