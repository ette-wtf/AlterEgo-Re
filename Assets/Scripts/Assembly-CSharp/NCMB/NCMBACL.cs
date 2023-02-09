using System;
using System.Collections.Generic;

namespace NCMB
{
	public class NCMBACL
	{
		private static NCMBACL defaultACL;

		private Dictionary<string, object> permissionsById;

		private bool shared;

		private static NCMBACL defaultACLWithCurrentUser;

		private static bool defaultACLUsesCurrentUser;

		public bool PublicReadAccess
		{
			get
			{
				return GetReadAccess("*");
			}
			set
			{
				SetReadAccess("*", value);
			}
		}

		public bool PublicWriteAccess
		{
			get
			{
				return GetWriteAccess("*");
			}
			set
			{
				SetWriteAccess("*", value);
			}
		}

		public NCMBACL()
		{
			permissionsById = new Dictionary<string, object>();
		}

		public NCMBACL(string objectId)
			: this()
		{
			if (objectId == null)
			{
				throw new NCMBException(new ArgumentException("objectId may not be null "));
			}
			SetWriteAccess(objectId, true);
			SetReadAccess(objectId, true);
		}

		internal bool _isShared()
		{
			return shared;
		}

		internal void _setShared(bool shared)
		{
			this.shared = shared;
		}

		internal NCMBACL _copy()
		{
			NCMBACL nCMBACL = new NCMBACL();
			try
			{
				nCMBACL.permissionsById = new Dictionary<string, object>(permissionsById);
				return nCMBACL;
			}
			catch (NCMBException error)
			{
				throw new NCMBException(error);
			}
		}

		public void SetReadAccess(string objectId, bool allowed)
		{
			if (objectId == null)
			{
				throw new NCMBException(new ArgumentException("cannot SetReadAccess for null objectId "));
			}
			_setAccess("read", objectId, allowed);
		}

		public void SetWriteAccess(string objectId, bool allowed)
		{
			if (objectId == null)
			{
				throw new NCMBException(new ArgumentException("cannot SetWriteAccess for null objectId "));
			}
			_setAccess("write", objectId, allowed);
		}

		public void SetRoleReadAccess(string roleName, bool allowed)
		{
			SetReadAccess("role:" + roleName, allowed);
		}

		public void SetRoleWriteAccess(string roleName, bool allowed)
		{
			SetWriteAccess("role:" + roleName, allowed);
		}

		public static void SetDefaultACL(NCMBACL acl, bool withAccessForCurrentUser)
		{
			defaultACLWithCurrentUser = null;
			if (acl != null)
			{
				defaultACL = acl._copy();
				defaultACL._setShared(true);
				defaultACLUsesCurrentUser = withAccessForCurrentUser;
			}
			else
			{
				defaultACL = null;
			}
		}

		private void _setAccess(string accessType, string objectId, bool allowed)
		{
			try
			{
				Dictionary<string, object> dictionary = null;
				object value;
				if (permissionsById.TryGetValue(objectId, out value))
				{
					dictionary = (Dictionary<string, object>)value;
				}
				if (dictionary == null)
				{
					if (!allowed)
					{
						return;
					}
					dictionary = new Dictionary<string, object>();
					permissionsById[objectId] = dictionary;
				}
				if (allowed)
				{
					dictionary[accessType] = true;
					return;
				}
				dictionary.Remove(accessType);
				if (dictionary.Count == 0)
				{
					permissionsById.Remove(objectId);
				}
			}
			catch (NCMBException ex)
			{
				throw new NCMBException(new ArgumentException("JSON failure with ACL: " + ex.GetType().ToString()));
			}
		}

		public bool GetReadAccess(string objectId)
		{
			if (objectId == null)
			{
				throw new NCMBException(new ArgumentException("cannot GetReadAccess for null objectId "));
			}
			return _getAccess("read", objectId);
		}

		public bool GetWriteAccess(string objectId)
		{
			if (objectId == null)
			{
				throw new NCMBException(new ArgumentException("cannot GetWriteAccess for null objectId "));
			}
			return _getAccess("write", objectId);
		}

		public bool GetRoleReadAccess(string roleName)
		{
			return GetReadAccess("role:" + roleName);
		}

		public bool GetRoleWriteAccess(string roleName)
		{
			return GetWriteAccess("role:" + roleName);
		}

		internal static NCMBACL _getDefaultACL()
		{
			if (defaultACLUsesCurrentUser && defaultACL != null)
			{
				if (NCMBUser.CurrentUser == null || NCMBUser.CurrentUser.ObjectId == null)
				{
					return defaultACL;
				}
				defaultACLWithCurrentUser = defaultACL._copy();
				defaultACLWithCurrentUser._setShared(true);
				defaultACLWithCurrentUser.SetReadAccess(NCMBUser.CurrentUser.ObjectId, true);
				defaultACLWithCurrentUser.SetWriteAccess(NCMBUser.CurrentUser.ObjectId, true);
				return defaultACLWithCurrentUser;
			}
			return defaultACL;
		}

		private bool _getAccess(string accessType, string objectId)
		{
			try
			{
				Dictionary<string, object> dictionary = null;
				object value;
				if (permissionsById.TryGetValue(objectId, out value))
				{
					dictionary = (Dictionary<string, object>)value;
				}
				if (dictionary == null)
				{
					return false;
				}
				if (!dictionary.TryGetValue(accessType, out value))
				{
					return false;
				}
				return (bool)value;
			}
			catch (NCMBException ex)
			{
				throw new NCMBException(new ArgumentException("JSON failure with ACL: " + ex.GetType().ToString()));
			}
		}

		internal IDictionary<string, object> _toJSONObject()
		{
			return permissionsById;
		}

		internal static NCMBACL _createACLFromJSONObject(Dictionary<string, object> aclValue)
		{
			NCMBACL nCMBACL = new NCMBACL();
			if (aclValue != null)
			{
				foreach (KeyValuePair<string, object> item in aclValue)
				{
					foreach (KeyValuePair<string, object> item2 in (Dictionary<string, object>)item.Value)
					{
						nCMBACL._setAccess(item2.Key, item.Key, true);
					}
				}
				return nCMBACL;
			}
			return nCMBACL;
		}
	}
}
