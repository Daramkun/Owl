using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Daramkun.Owl.Tracker
{
	public enum EndPointState
	{
		/// <summary>
		/// 알 수 없음
		/// </summary>
		Unknown,
		/// <summary>
		/// 집하 처리
		/// </summary>
		Picking,
		/// <summary>
		/// 입고 처리
		/// </summary>
		Storaged,
		/// <summary>
		/// 간선 상차
		/// </summary>
		GetOnToTrunk,
		/// <summary>
		/// 간선 하차
		/// </summary>
		GetOffToTrunk,
		/// <summary>
		/// 배송 출발
		/// </summary>
		DeliveryStart,
		/// <summary>
		/// 배달 완료
		/// </summary>
		DeliveryComplete,
	}

	public class EndPoint
	{
		public string Terminal { get; set; }
		public EndPointState State { get; set; }
		public string OriginalState { get; set; }
		public DateTime ArrivalTime { get; set; }
	}
}
