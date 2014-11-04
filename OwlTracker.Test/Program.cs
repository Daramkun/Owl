using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Daramkun.Owl.Tracker;
using Daramkun.Owl.Tracker.Parsers;

namespace OwlTracker.Test
{
	class Program
	{
		static async void Test ()
		{
			Console.WriteLine ( "==== CJ대한통운 ====" );
			foreach ( EndPoint ep in await Tracker.Tracking ( new TrackingItem () { ItemNumber = "684948034991", ParserName = "CJ대한통운" } ) )
				Console.WriteLine ( "{0} {1}({2}) {3}", ep.Terminal, ep.State, ep.OriginalState, ep.ArrivalTime );

			/* This test has not running with my Delivery number */
			Console.WriteLine ( "==== 현대택배 ====" );
			foreach ( EndPoint ep in await Tracker.Tracking ( new TrackingItem () { ItemNumber = "302202197054", ParserName = "현대택배" } ) )
				Console.WriteLine ( "{0} {1}({2}) {3}", ep.Terminal, ep.State, ep.OriginalState, ep.ArrivalTime );

			Console.WriteLine ( "==== 동부익스프레스 ====" );
			foreach ( EndPoint ep in await Tracker.Tracking ( new TrackingItem () { ItemNumber = "303543025634", ParserName = "동부익스프레스" } ) )
				Console.WriteLine ( "{0} {1}({2}) {3}", ep.Terminal, ep.State, ep.OriginalState, ep.ArrivalTime );

			Console.WriteLine ( "==== 우체국택배 ====" );
			foreach ( EndPoint ep in await Tracker.Tracking ( new TrackingItem () { ItemNumber = "7304702003900", ParserName = "우체국택배" } ) )
				Console.WriteLine ( "{0} {1}({2}) {3}", ep.Terminal, ep.State, ep.OriginalState, ep.ArrivalTime );

			Console.WriteLine ( "==== 로젠택배 ====" );
			foreach ( EndPoint ep in await Tracker.Tracking ( new TrackingItem () { ItemNumber = "97543184432", ParserName = "로젠택배" } ) )
				Console.WriteLine ( "{0} {1}({2}) {3}", ep.Terminal, ep.State, ep.OriginalState, ep.ArrivalTime );

			/* This test has not running with my Delivery number */
			Console.WriteLine ( "==== KG옐로우캡 ====" );
			foreach ( EndPoint ep in await Tracker.Tracking ( new TrackingItem () { ItemNumber = "53450235090", ParserName = "KG옐로우캡" } ) )
				Console.WriteLine ( "{0} {1}({2}) {3}", ep.Terminal, ep.State, ep.OriginalState, ep.ArrivalTime );

			Console.Write ( "Press any key to continue..." );
		}

		static void Main ( string [] args )
		{
			Tracker.AddTrackingParser ( new CJDaehantongwoonParser () );
			Tracker.AddTrackingParser ( new HyundaeTaekbaeParser () );
			Tracker.AddTrackingParser ( new DongbuExpressParser () );
			Tracker.AddTrackingParser ( new EpostParser () );
			Tracker.AddTrackingParser ( new LogenTaekbaeParser () );
			Tracker.AddTrackingParser ( new KGYellowCapParser () );
			Test ();
			while ( Console.ReadKey ( true ).Key == ConsoleKey.Zoom ) ;
		}
	}
}
