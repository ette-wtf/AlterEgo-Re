using System;
using System.Collections.Generic;

namespace UnityEngine.Analytics
{
	public static class AnalyticsEvent
	{
		private static readonly string k_SdkVersion = "1.1.1";

		private static Action<IDictionary<string, object>> s_StandardEventCallback = delegate
		{
		};

		private static readonly Dictionary<string, object> m_EventData = new Dictionary<string, object>();

		private static bool _debugMode = false;

		private static Dictionary<string, string> enumRenameTable = new Dictionary<string, string>
		{
			{ "RewardedAd", "rewarded_ad" },
			{ "TimedReward", "timed_reward" },
			{ "SocialReward", "social_reward" },
			{ "MainMenu", "main_menu" },
			{ "IAPPromo", "iap_promo" },
			{ "CrossPromo", "cross_promo" },
			{ "FeaturePromo", "feature_promo" },
			{ "TextOnly", "text_only" }
		};

		public static string sdkVersion
		{
			get
			{
				return k_SdkVersion;
			}
		}

		public static bool debugMode
		{
			get
			{
				return _debugMode;
			}
			set
			{
				_debugMode = value;
			}
		}

		public static void Register(Action<IDictionary<string, object>> action)
		{
			s_StandardEventCallback = (Action<IDictionary<string, object>>)Delegate.Combine(s_StandardEventCallback, action);
		}

		public static void Unregister(Action<IDictionary<string, object>> action)
		{
			s_StandardEventCallback = (Action<IDictionary<string, object>>)Delegate.Remove(s_StandardEventCallback, action);
		}

		private static void OnValidationFailed(string message)
		{
			throw new ArgumentException(message);
		}

		private static void AddCustomEventData(IDictionary<string, object> eventData)
		{
			if (eventData == null)
			{
				return;
			}
			foreach (KeyValuePair<string, object> eventDatum in eventData)
			{
				if (!m_EventData.ContainsKey(eventDatum.Key))
				{
					m_EventData.Add(eventDatum.Key, eventDatum.Value);
				}
			}
		}

		public static AnalyticsResult Custom(string eventName, IDictionary<string, object> eventData = null)
		{
			AnalyticsResult analyticsResult = AnalyticsResult.UnsupportedPlatform;
			string text = string.Empty;
			if (string.IsNullOrEmpty(eventName))
			{
				OnValidationFailed("Custom event name cannot be set to null or an empty string.");
			}
			if (eventData == null)
			{
				analyticsResult = Analytics.CustomEvent(eventName);
			}
			else
			{
				s_StandardEventCallback(eventData);
				analyticsResult = Analytics.CustomEvent(eventName, eventData);
			}
			if (debugMode)
			{
				if (eventData == null)
				{
					text += "\n  Event Data (null)";
				}
				else
				{
					text += string.Format("\n  Event Data ({0} params):", eventData.Count);
					foreach (KeyValuePair<string, object> eventDatum in eventData)
					{
						text += string.Format("\n    Key: '{0}';  Value: '{1}'", eventDatum.Key, eventDatum.Value);
					}
				}
			}
			switch (analyticsResult)
			{
			case AnalyticsResult.Ok:
				if (debugMode)
				{
					Debug.LogFormat("Successfully sent '{0}' event (Result: '{1}').{2}", eventName, analyticsResult, text);
				}
				break;
			case AnalyticsResult.TooManyItems:
			case AnalyticsResult.InvalidData:
				Debug.LogErrorFormat("Failed to send '{0}' event (Result: '{1}').{2}", eventName, analyticsResult, text);
				break;
			default:
				Debug.LogWarningFormat("Unable to send '{0}' event (Result: '{1}').{2}", eventName, analyticsResult, text);
				break;
			}
			return analyticsResult;
		}

		public static AnalyticsResult AchievementStep(int stepIndex, string achievementId, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("step_index", stepIndex);
			if (string.IsNullOrEmpty(achievementId))
			{
				throw new ArgumentException(achievementId);
			}
			m_EventData.Add("achievement_id", achievementId);
			AddCustomEventData(eventData);
			return Custom("achievement_step", m_EventData);
		}

		public static AnalyticsResult AchievementUnlocked(string achievementId, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(achievementId))
			{
				throw new ArgumentException(achievementId);
			}
			m_EventData.Add("achievement_id", achievementId);
			AddCustomEventData(eventData);
			return Custom("achievement_unlocked", m_EventData);
		}

		public static AnalyticsResult AdComplete(bool rewarded, AdvertisingNetwork network, string placementId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("rewarded", rewarded);
			m_EventData.Add("network", RenameEnum(network.ToString()));
			if (!string.IsNullOrEmpty(placementId))
			{
				m_EventData.Add("placement_id", placementId);
			}
			AddCustomEventData(eventData);
			return Custom("ad_complete", m_EventData);
		}

		public static AnalyticsResult AdComplete(bool rewarded, string network = null, string placementId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("rewarded", rewarded);
			if (!string.IsNullOrEmpty(network))
			{
				m_EventData.Add("network", network);
			}
			if (!string.IsNullOrEmpty(placementId))
			{
				m_EventData.Add("placement_id", placementId);
			}
			AddCustomEventData(eventData);
			return Custom("ad_complete", m_EventData);
		}

		public static AnalyticsResult AdOffer(bool rewarded, AdvertisingNetwork network, string placementId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("rewarded", rewarded);
			m_EventData.Add("network", RenameEnum(network.ToString()));
			if (!string.IsNullOrEmpty(placementId))
			{
				m_EventData.Add("placement_id", placementId);
			}
			AddCustomEventData(eventData);
			return Custom("ad_offer", m_EventData);
		}

		public static AnalyticsResult AdOffer(bool rewarded, string network = null, string placementId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("rewarded", rewarded);
			if (!string.IsNullOrEmpty(network))
			{
				m_EventData.Add("network", network);
			}
			if (!string.IsNullOrEmpty(placementId))
			{
				m_EventData.Add("placement_id", placementId);
			}
			AddCustomEventData(eventData);
			return Custom("ad_offer", m_EventData);
		}

		public static AnalyticsResult AdSkip(bool rewarded, AdvertisingNetwork network, string placementId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("rewarded", rewarded);
			m_EventData.Add("network", RenameEnum(network.ToString()));
			if (!string.IsNullOrEmpty(placementId))
			{
				m_EventData.Add("placement_id", placementId);
			}
			AddCustomEventData(eventData);
			return Custom("ad_skip", m_EventData);
		}

		public static AnalyticsResult AdSkip(bool rewarded, string network = null, string placementId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("rewarded", rewarded);
			if (!string.IsNullOrEmpty(network))
			{
				m_EventData.Add("network", network);
			}
			if (!string.IsNullOrEmpty(placementId))
			{
				m_EventData.Add("placement_id", placementId);
			}
			AddCustomEventData(eventData);
			return Custom("ad_skip", m_EventData);
		}

		public static AnalyticsResult AdStart(bool rewarded, AdvertisingNetwork network, string placementId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("rewarded", rewarded);
			m_EventData.Add("network", RenameEnum(network.ToString()));
			if (!string.IsNullOrEmpty(placementId))
			{
				m_EventData.Add("placement_id", placementId);
			}
			AddCustomEventData(eventData);
			return Custom("ad_start", m_EventData);
		}

		public static AnalyticsResult AdStart(bool rewarded, string network = null, string placementId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("rewarded", rewarded);
			if (!string.IsNullOrEmpty(network))
			{
				m_EventData.Add("network", network);
			}
			if (!string.IsNullOrEmpty(placementId))
			{
				m_EventData.Add("placement_id", placementId);
			}
			AddCustomEventData(eventData);
			return Custom("ad_start", m_EventData);
		}

		public static AnalyticsResult ChatMessageSent(IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			AddCustomEventData(eventData);
			return Custom("chat_message_sent", m_EventData);
		}

		public static AnalyticsResult CustomEvent(IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			AddCustomEventData(eventData);
			return Custom(string.Empty, m_EventData);
		}

		public static AnalyticsResult CutsceneSkip(string name, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException(name);
			}
			m_EventData.Add("scene_name", name);
			AddCustomEventData(eventData);
			return Custom("cutscene_skip", m_EventData);
		}

		public static AnalyticsResult CutsceneStart(string name, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException(name);
			}
			m_EventData.Add("scene_name", name);
			AddCustomEventData(eventData);
			return Custom("cutscene_start", m_EventData);
		}

		public static AnalyticsResult FirstInteraction(string actionId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (!string.IsNullOrEmpty(actionId))
			{
				m_EventData.Add("action_id", actionId);
			}
			AddCustomEventData(eventData);
			return Custom("first_interaction", m_EventData);
		}

		public static AnalyticsResult GameOver(int index, string name = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("level_index", index);
			if (!string.IsNullOrEmpty(name))
			{
				m_EventData.Add("level_name", name);
			}
			AddCustomEventData(eventData);
			return Custom("game_over", m_EventData);
		}

		public static AnalyticsResult GameOver(string name = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (!string.IsNullOrEmpty(name))
			{
				m_EventData.Add("level_name", name);
			}
			AddCustomEventData(eventData);
			return Custom("game_over", m_EventData);
		}

		public static AnalyticsResult GameStart(IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			AddCustomEventData(eventData);
			return Custom("game_start", m_EventData);
		}

		public static AnalyticsResult IAPTransaction(string transactionContext, float price, string itemId, string itemType = null, string level = null, string transactionId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(transactionContext))
			{
				throw new ArgumentException(transactionContext);
			}
			m_EventData.Add("transaction_context", transactionContext);
			m_EventData.Add("price", price);
			if (string.IsNullOrEmpty(itemId))
			{
				throw new ArgumentException(itemId);
			}
			m_EventData.Add("item_id", itemId);
			if (!string.IsNullOrEmpty(itemType))
			{
				m_EventData.Add("item_type", itemType);
			}
			if (!string.IsNullOrEmpty(level))
			{
				m_EventData.Add("level", level);
			}
			if (!string.IsNullOrEmpty(transactionId))
			{
				m_EventData.Add("transaction_id", transactionId);
			}
			AddCustomEventData(eventData);
			return Custom("iap_transaction", m_EventData);
		}

		public static AnalyticsResult ItemAcquired(AcquisitionType currencyType, string transactionContext, float amount, string itemId, float balance, string itemType = null, string level = null, string transactionId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("currency_type", RenameEnum(currencyType.ToString()));
			if (string.IsNullOrEmpty(transactionContext))
			{
				throw new ArgumentException(transactionContext);
			}
			m_EventData.Add("transaction_context", transactionContext);
			m_EventData.Add("amount", amount);
			if (string.IsNullOrEmpty(itemId))
			{
				throw new ArgumentException(itemId);
			}
			m_EventData.Add("item_id", itemId);
			m_EventData.Add("balance", balance);
			if (!string.IsNullOrEmpty(itemType))
			{
				m_EventData.Add("item_type", itemType);
			}
			if (!string.IsNullOrEmpty(level))
			{
				m_EventData.Add("level", level);
			}
			if (!string.IsNullOrEmpty(transactionId))
			{
				m_EventData.Add("transaction_id", transactionId);
			}
			AddCustomEventData(eventData);
			return Custom("item_acquired", m_EventData);
		}

		public static AnalyticsResult ItemAcquired(AcquisitionType currencyType, string transactionContext, float amount, string itemId, string itemType = null, string level = null, string transactionId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("currency_type", RenameEnum(currencyType.ToString()));
			if (string.IsNullOrEmpty(transactionContext))
			{
				throw new ArgumentException(transactionContext);
			}
			m_EventData.Add("transaction_context", transactionContext);
			m_EventData.Add("amount", amount);
			if (string.IsNullOrEmpty(itemId))
			{
				throw new ArgumentException(itemId);
			}
			m_EventData.Add("item_id", itemId);
			if (!string.IsNullOrEmpty(itemType))
			{
				m_EventData.Add("item_type", itemType);
			}
			if (!string.IsNullOrEmpty(level))
			{
				m_EventData.Add("level", level);
			}
			if (!string.IsNullOrEmpty(transactionId))
			{
				m_EventData.Add("transaction_id", transactionId);
			}
			AddCustomEventData(eventData);
			return Custom("item_acquired", m_EventData);
		}

		public static AnalyticsResult ItemSpent(AcquisitionType currencyType, string transactionContext, float amount, string itemId, float balance, string itemType = null, string level = null, string transactionId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("currency_type", RenameEnum(currencyType.ToString()));
			if (string.IsNullOrEmpty(transactionContext))
			{
				throw new ArgumentException(transactionContext);
			}
			m_EventData.Add("transaction_context", transactionContext);
			m_EventData.Add("amount", amount);
			if (string.IsNullOrEmpty(itemId))
			{
				throw new ArgumentException(itemId);
			}
			m_EventData.Add("item_id", itemId);
			m_EventData.Add("balance", balance);
			if (!string.IsNullOrEmpty(itemType))
			{
				m_EventData.Add("item_type", itemType);
			}
			if (!string.IsNullOrEmpty(level))
			{
				m_EventData.Add("level", level);
			}
			if (!string.IsNullOrEmpty(transactionId))
			{
				m_EventData.Add("transaction_id", transactionId);
			}
			AddCustomEventData(eventData);
			return Custom("item_spent", m_EventData);
		}

		public static AnalyticsResult ItemSpent(AcquisitionType currencyType, string transactionContext, float amount, string itemId, string itemType = null, string level = null, string transactionId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("currency_type", RenameEnum(currencyType.ToString()));
			if (string.IsNullOrEmpty(transactionContext))
			{
				throw new ArgumentException(transactionContext);
			}
			m_EventData.Add("transaction_context", transactionContext);
			m_EventData.Add("amount", amount);
			if (string.IsNullOrEmpty(itemId))
			{
				throw new ArgumentException(itemId);
			}
			m_EventData.Add("item_id", itemId);
			if (!string.IsNullOrEmpty(itemType))
			{
				m_EventData.Add("item_type", itemType);
			}
			if (!string.IsNullOrEmpty(level))
			{
				m_EventData.Add("level", level);
			}
			if (!string.IsNullOrEmpty(transactionId))
			{
				m_EventData.Add("transaction_id", transactionId);
			}
			AddCustomEventData(eventData);
			return Custom("item_spent", m_EventData);
		}

		public static AnalyticsResult LevelComplete(string name, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException(name);
			}
			m_EventData.Add("level_name", name);
			AddCustomEventData(eventData);
			return Custom("level_complete", m_EventData);
		}

		public static AnalyticsResult LevelComplete(int index, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("level_index", index);
			AddCustomEventData(eventData);
			return Custom("level_complete", m_EventData);
		}

		public static AnalyticsResult LevelComplete(string name, int index, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException(name);
			}
			m_EventData.Add("level_name", name);
			m_EventData.Add("level_index", index);
			AddCustomEventData(eventData);
			return Custom("level_complete", m_EventData);
		}

		public static AnalyticsResult LevelFail(string name, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException(name);
			}
			m_EventData.Add("level_name", name);
			AddCustomEventData(eventData);
			return Custom("level_fail", m_EventData);
		}

		public static AnalyticsResult LevelFail(int index, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("level_index", index);
			AddCustomEventData(eventData);
			return Custom("level_fail", m_EventData);
		}

		public static AnalyticsResult LevelFail(string name, int index, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException(name);
			}
			m_EventData.Add("level_name", name);
			m_EventData.Add("level_index", index);
			AddCustomEventData(eventData);
			return Custom("level_fail", m_EventData);
		}

		public static AnalyticsResult LevelQuit(string name, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException(name);
			}
			m_EventData.Add("level_name", name);
			AddCustomEventData(eventData);
			return Custom("level_quit", m_EventData);
		}

		public static AnalyticsResult LevelQuit(int index, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("level_index", index);
			AddCustomEventData(eventData);
			return Custom("level_quit", m_EventData);
		}

		public static AnalyticsResult LevelQuit(string name, int index, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException(name);
			}
			m_EventData.Add("level_name", name);
			m_EventData.Add("level_index", index);
			AddCustomEventData(eventData);
			return Custom("level_quit", m_EventData);
		}

		public static AnalyticsResult LevelSkip(string name, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException(name);
			}
			m_EventData.Add("level_name", name);
			AddCustomEventData(eventData);
			return Custom("level_skip", m_EventData);
		}

		public static AnalyticsResult LevelSkip(int index, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("level_index", index);
			AddCustomEventData(eventData);
			return Custom("level_skip", m_EventData);
		}

		public static AnalyticsResult LevelSkip(string name, int index, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException(name);
			}
			m_EventData.Add("level_name", name);
			m_EventData.Add("level_index", index);
			AddCustomEventData(eventData);
			return Custom("level_skip", m_EventData);
		}

		public static AnalyticsResult LevelStart(string name, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException(name);
			}
			m_EventData.Add("level_name", name);
			AddCustomEventData(eventData);
			return Custom("level_start", m_EventData);
		}

		public static AnalyticsResult LevelStart(int index, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("level_index", index);
			AddCustomEventData(eventData);
			return Custom("level_start", m_EventData);
		}

		public static AnalyticsResult LevelStart(string name, int index, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException(name);
			}
			m_EventData.Add("level_name", name);
			m_EventData.Add("level_index", index);
			AddCustomEventData(eventData);
			return Custom("level_start", m_EventData);
		}

		public static AnalyticsResult LevelUp(string name, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException(name);
			}
			m_EventData.Add("new_level_name", name);
			AddCustomEventData(eventData);
			return Custom("level_up", m_EventData);
		}

		public static AnalyticsResult LevelUp(int index, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("new_level_index", index);
			AddCustomEventData(eventData);
			return Custom("level_up", m_EventData);
		}

		public static AnalyticsResult LevelUp(string name, int index, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException(name);
			}
			m_EventData.Add("new_level_name", name);
			m_EventData.Add("new_level_index", index);
			AddCustomEventData(eventData);
			return Custom("level_up", m_EventData);
		}

		public static AnalyticsResult PostAdAction(bool rewarded, AdvertisingNetwork network, string placementId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("rewarded", rewarded);
			m_EventData.Add("network", RenameEnum(network.ToString()));
			if (!string.IsNullOrEmpty(placementId))
			{
				m_EventData.Add("placement_id", placementId);
			}
			AddCustomEventData(eventData);
			return Custom("post_ad_action", m_EventData);
		}

		public static AnalyticsResult PostAdAction(bool rewarded, string network = null, string placementId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("rewarded", rewarded);
			if (!string.IsNullOrEmpty(network))
			{
				m_EventData.Add("network", network);
			}
			if (!string.IsNullOrEmpty(placementId))
			{
				m_EventData.Add("placement_id", placementId);
			}
			AddCustomEventData(eventData);
			return Custom("post_ad_action", m_EventData);
		}

		public static AnalyticsResult PushNotificationClick(string message_id, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(message_id))
			{
				throw new ArgumentException(message_id);
			}
			m_EventData.Add("message_id", message_id);
			AddCustomEventData(eventData);
			return Custom("push_notification_click", m_EventData);
		}

		public static AnalyticsResult PushNotificationEnable(IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			AddCustomEventData(eventData);
			return Custom("push_notification_enable", m_EventData);
		}

		public static AnalyticsResult ScreenVisit(ScreenName screenName, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("screen_name", RenameEnum(screenName.ToString()));
			AddCustomEventData(eventData);
			return Custom("screen_visit", m_EventData);
		}

		public static AnalyticsResult ScreenVisit(string screenName, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(screenName))
			{
				throw new ArgumentException(screenName);
			}
			m_EventData.Add("screen_name", screenName);
			AddCustomEventData(eventData);
			return Custom("screen_visit", m_EventData);
		}

		public static AnalyticsResult SocialShare(ShareType shareType, SocialNetwork socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("share_type", RenameEnum(shareType.ToString()));
			m_EventData.Add("social_network", RenameEnum(socialNetwork.ToString()));
			if (!string.IsNullOrEmpty(senderId))
			{
				m_EventData.Add("sender_id", senderId);
			}
			if (!string.IsNullOrEmpty(recipientId))
			{
				m_EventData.Add("recipient_id", recipientId);
			}
			AddCustomEventData(eventData);
			return Custom("social_share", m_EventData);
		}

		public static AnalyticsResult SocialShare(ShareType shareType, string socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("share_type", RenameEnum(shareType.ToString()));
			if (string.IsNullOrEmpty(socialNetwork))
			{
				throw new ArgumentException(socialNetwork);
			}
			m_EventData.Add("social_network", socialNetwork);
			if (!string.IsNullOrEmpty(senderId))
			{
				m_EventData.Add("sender_id", senderId);
			}
			if (!string.IsNullOrEmpty(recipientId))
			{
				m_EventData.Add("recipient_id", recipientId);
			}
			AddCustomEventData(eventData);
			return Custom("social_share", m_EventData);
		}

		public static AnalyticsResult SocialShare(string shareType, SocialNetwork socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(shareType))
			{
				throw new ArgumentException(shareType);
			}
			m_EventData.Add("share_type", shareType);
			m_EventData.Add("social_network", RenameEnum(socialNetwork.ToString()));
			if (!string.IsNullOrEmpty(senderId))
			{
				m_EventData.Add("sender_id", senderId);
			}
			if (!string.IsNullOrEmpty(recipientId))
			{
				m_EventData.Add("recipient_id", recipientId);
			}
			AddCustomEventData(eventData);
			return Custom("social_share", m_EventData);
		}

		public static AnalyticsResult SocialShare(string shareType, string socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(shareType))
			{
				throw new ArgumentException(shareType);
			}
			m_EventData.Add("share_type", shareType);
			if (string.IsNullOrEmpty(socialNetwork))
			{
				throw new ArgumentException(socialNetwork);
			}
			m_EventData.Add("social_network", socialNetwork);
			if (!string.IsNullOrEmpty(senderId))
			{
				m_EventData.Add("sender_id", senderId);
			}
			if (!string.IsNullOrEmpty(recipientId))
			{
				m_EventData.Add("recipient_id", recipientId);
			}
			AddCustomEventData(eventData);
			return Custom("social_share", m_EventData);
		}

		public static AnalyticsResult SocialShareAccept(ShareType shareType, SocialNetwork socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("share_type", RenameEnum(shareType.ToString()));
			m_EventData.Add("social_network", RenameEnum(socialNetwork.ToString()));
			if (!string.IsNullOrEmpty(senderId))
			{
				m_EventData.Add("sender_id", senderId);
			}
			if (!string.IsNullOrEmpty(recipientId))
			{
				m_EventData.Add("recipient_id", recipientId);
			}
			AddCustomEventData(eventData);
			return Custom("social_share_accept", m_EventData);
		}

		public static AnalyticsResult SocialShareAccept(ShareType shareType, string socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("share_type", RenameEnum(shareType.ToString()));
			if (string.IsNullOrEmpty(socialNetwork))
			{
				throw new ArgumentException(socialNetwork);
			}
			m_EventData.Add("social_network", socialNetwork);
			if (!string.IsNullOrEmpty(senderId))
			{
				m_EventData.Add("sender_id", senderId);
			}
			if (!string.IsNullOrEmpty(recipientId))
			{
				m_EventData.Add("recipient_id", recipientId);
			}
			AddCustomEventData(eventData);
			return Custom("social_share_accept", m_EventData);
		}

		public static AnalyticsResult SocialShareAccept(string shareType, SocialNetwork socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(shareType))
			{
				throw new ArgumentException(shareType);
			}
			m_EventData.Add("share_type", shareType);
			m_EventData.Add("social_network", RenameEnum(socialNetwork.ToString()));
			if (!string.IsNullOrEmpty(senderId))
			{
				m_EventData.Add("sender_id", senderId);
			}
			if (!string.IsNullOrEmpty(recipientId))
			{
				m_EventData.Add("recipient_id", recipientId);
			}
			AddCustomEventData(eventData);
			return Custom("social_share_accept", m_EventData);
		}

		public static AnalyticsResult SocialShareAccept(string shareType, string socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(shareType))
			{
				throw new ArgumentException(shareType);
			}
			m_EventData.Add("share_type", shareType);
			if (string.IsNullOrEmpty(socialNetwork))
			{
				throw new ArgumentException(socialNetwork);
			}
			m_EventData.Add("social_network", socialNetwork);
			if (!string.IsNullOrEmpty(senderId))
			{
				m_EventData.Add("sender_id", senderId);
			}
			if (!string.IsNullOrEmpty(recipientId))
			{
				m_EventData.Add("recipient_id", recipientId);
			}
			AddCustomEventData(eventData);
			return Custom("social_share_accept", m_EventData);
		}

		public static AnalyticsResult StoreItemClick(StoreType storeType, string itemId, string itemName = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("type", RenameEnum(storeType.ToString()));
			if (string.IsNullOrEmpty(itemId))
			{
				throw new ArgumentException(itemId);
			}
			m_EventData.Add("item_id", itemId);
			if (!string.IsNullOrEmpty(itemName))
			{
				m_EventData.Add("item_name", itemName);
			}
			AddCustomEventData(eventData);
			return Custom("store_item_click", m_EventData);
		}

		public static AnalyticsResult StoreOpened(StoreType storeType, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("type", RenameEnum(storeType.ToString()));
			AddCustomEventData(eventData);
			return Custom("store_opened", m_EventData);
		}

		public static AnalyticsResult TutorialComplete(string tutorialId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (!string.IsNullOrEmpty(tutorialId))
			{
				m_EventData.Add("tutorial_id", tutorialId);
			}
			AddCustomEventData(eventData);
			return Custom("tutorial_complete", m_EventData);
		}

		public static AnalyticsResult TutorialSkip(string tutorialId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (!string.IsNullOrEmpty(tutorialId))
			{
				m_EventData.Add("tutorial_id", tutorialId);
			}
			AddCustomEventData(eventData);
			return Custom("tutorial_skip", m_EventData);
		}

		public static AnalyticsResult TutorialStart(string tutorialId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (!string.IsNullOrEmpty(tutorialId))
			{
				m_EventData.Add("tutorial_id", tutorialId);
			}
			AddCustomEventData(eventData);
			return Custom("tutorial_start", m_EventData);
		}

		public static AnalyticsResult TutorialStep(int stepIndex, string tutorialId = null, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("step_index", stepIndex);
			if (!string.IsNullOrEmpty(tutorialId))
			{
				m_EventData.Add("tutorial_id", tutorialId);
			}
			AddCustomEventData(eventData);
			return Custom("tutorial_step", m_EventData);
		}

		public static AnalyticsResult UserSignup(AuthorizationNetwork authorizationNetwork, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			m_EventData.Add("authorization_network", RenameEnum(authorizationNetwork.ToString()));
			AddCustomEventData(eventData);
			return Custom("user_signup", m_EventData);
		}

		public static AnalyticsResult UserSignup(string authorizationNetwork, IDictionary<string, object> eventData = null)
		{
			m_EventData.Clear();
			if (string.IsNullOrEmpty(authorizationNetwork))
			{
				throw new ArgumentException(authorizationNetwork);
			}
			m_EventData.Add("authorization_network", authorizationNetwork);
			AddCustomEventData(eventData);
			return Custom("user_signup", m_EventData);
		}

		private static string RenameEnum(string enumName)
		{
			return (!enumRenameTable.ContainsKey(enumName)) ? enumName.ToLower() : enumRenameTable[enumName];
		}
	}
}
