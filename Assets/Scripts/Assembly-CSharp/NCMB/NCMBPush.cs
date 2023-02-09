using System;
using NCMB.Internal;
using UnityEngine;

namespace NCMB
{
	[NCMBClassName("push")]
	public class NCMBPush : NCMBObject
	{
		private static AndroidJavaClass m_AJClass;

		public string Message
		{
			get
			{
				return (string)this["message"];
			}
			set
			{
				this["message"] = value;
			}
		}

		public object SearchCondition
		{
			get
			{
				return this["searchCondition"];
			}
			set
			{
				this["searchCondition"] = value;
			}
		}

		public DateTime DeliveryTime
		{
			get
			{
				return TimeZoneInfo.ConvertTimeFromUtc((DateTime)this["deliveryTime"], TimeZoneInfo.Local);
			}
			set
			{
				this["deliveryTime"] = DateTime.Parse(TimeZoneInfo.ConvertTimeToUtc(value).ToString());
			}
		}

		public bool ImmediateDeliveryFlag
		{
			get
			{
				return (bool)this["immediateDeliveryFlag"];
			}
			set
			{
				this["immediateDeliveryFlag"] = value;
			}
		}

		public string Title
		{
			get
			{
				return (string)this["title"];
			}
			set
			{
				this["title"] = value;
			}
		}

		public bool PushToIOS
		{
			get
			{
				bool result = false;
				if (ContainsKey("target"))
				{
					string[] array = (string[])this["target"];
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] == "ios")
						{
							result = true;
						}
					}
				}
				return result;
			}
			set
			{
				bool pushToAndroid = PushToAndroid;
				if (value && !pushToAndroid)
				{
					this["target"] = new string[1] { "ios" };
				}
				else if (!value && pushToAndroid)
				{
					this["target"] = new string[1] { "android" };
				}
				else if (ContainsKey("target"))
				{
					Remove("target");
				}
			}
		}

		public bool PushToAndroid
		{
			get
			{
				bool result = false;
				if (ContainsKey("target"))
				{
					string[] array = (string[])this["target"];
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] == "android")
						{
							result = true;
						}
					}
				}
				return result;
			}
			set
			{
				bool pushToIOS = PushToIOS;
				if (value && !pushToIOS)
				{
					this["target"] = new string[1] { "android" };
				}
				else if (!value && pushToIOS)
				{
					this["target"] = new string[1] { "ios" };
				}
				else if (ContainsKey("target"))
				{
					Remove("target");
				}
			}
		}

		public int? Badge
		{
			get
			{
				return (int)this["badgeSetting"];
			}
			set
			{
				this["badgeSetting"] = value;
			}
		}

		public bool BadgeIncrementFlag
		{
			get
			{
				return (bool)this["badgeIncrementFlag"];
			}
			set
			{
				this["badgeIncrementFlag"] = value;
			}
		}

		public string RichUrl
		{
			get
			{
				return (string)this["richUrl"];
			}
			set
			{
				this["richUrl"] = value;
			}
		}

		public bool Dialog
		{
			get
			{
				return (bool)this["dialog"];
			}
			set
			{
				this["dialog"] = value;
			}
		}

		public bool ContentAvailable
		{
			get
			{
				return (bool)this["contentAvailable"];
			}
			set
			{
				this["contentAvailable"] = value;
			}
		}

		public string Category
		{
			get
			{
				return (string)this["category"];
			}
			set
			{
				this["category"] = value;
			}
		}

		public DateTime? DeliveryExpirationDate
		{
			get
			{
				return DeliveryExpirationDate = (DateTime)this["deliveryExpirationDate"];
			}
			set
			{
				this["deliveryExpirationDate"] = value;
			}
		}

		public string DeliveryExpirationTime
		{
			get
			{
				return (string)this["deliveryExpirationTime"];
			}
			set
			{
				this["deliveryExpirationTime"] = value;
			}
		}

		static NCMBPush()
		{
			m_AJClass = new AndroidJavaClass("com.nifcloud.mbaas.ncmbfcmplugin.FCMInit");
		}

		public static void Register()
		{
			m_AJClass.CallStatic("Init");
		}

		public static void RegisterWithLocation()
		{
			m_AJClass.CallStatic("Init");
		}

		public void SendPush()
		{
			SendPush(null);
		}

		public void SendPush(NCMBCallback callback)
		{
			if (ContainsKey("deliveryExpirationDate") && ContainsKey("deliveryExpirationTime"))
			{
				throw new ArgumentException("DeliveryExpirationDate and DeliveryExpirationTime can not be set at the same time.Please set only one.");
			}
			if (ContainsKey("deliveryTime") && ContainsKey("immediateDeliveryFlag") && ImmediateDeliveryFlag)
			{
				throw new ArgumentException("deliveryTime and immediateDeliveryFlag can not be set at the same time.Please set only one.");
			}
			if (!ContainsKey("deliveryTime"))
			{
				ImmediateDeliveryFlag = true;
			}
			base.SaveAsync(callback);
		}

		public static NCMBQuery<NCMBPush> GetQuery()
		{
			return NCMBQuery<NCMBPush>.GetQuery("push");
		}

		internal override string _getBaseUrl()
		{
			return NCMBSettings.DomainURL + "/" + NCMBSettings.APIVersion + "/push";
		}
	}
}
