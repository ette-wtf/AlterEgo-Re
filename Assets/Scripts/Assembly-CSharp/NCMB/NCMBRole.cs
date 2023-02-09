using System;
using System.Text.RegularExpressions;
using NCMB.Internal;

namespace NCMB
{
	[NCMBClassName("role")]
	public class NCMBRole : NCMBObject
	{
		private static readonly Regex namePattern = new Regex("^[0-9a-zA-Z_\\- ]+$");

		public string Name
		{
			get
			{
				return (string)this["roleName"];
			}
			set
			{
				this["roleName"] = value;
			}
		}

		public NCMBRelation<NCMBUser> Users
		{
			get
			{
				return GetRelation<NCMBUser>("belongUser");
			}
		}

		public NCMBRelation<NCMBRole> Roles
		{
			get
			{
				return GetRelation<NCMBRole>("belongRole");
			}
		}

		internal NCMBRole()
		{
		}

		public NCMBRole(string roleName)
		{
			Name = roleName;
		}

		public NCMBRole(string roleName, NCMBACL acl)
		{
			Name = roleName;
			base.ACL = acl;
		}

		public static NCMBQuery<NCMBRole> GetQuery()
		{
			return NCMBQuery<NCMBRole>.GetQuery("role");
		}

		internal override void _onSettingValue(string key, object value)
		{
			base._onSettingValue(key, value);
			if ("roleName".Equals(key))
			{
				if (base.ObjectId != null)
				{
					throw new NCMBException(new ArgumentException("A role's name can only be set before it has been saved."));
				}
				if (!(value is string))
				{
					throw new NCMBException(new ArgumentException("A role's name must be a String."));
				}
				if (!namePattern.IsMatch((string)value))
				{
					throw new NCMBException(new ArgumentException("A role's name can only contain alphanumeric characters, _, -, and spaces."));
				}
			}
			if ("belongUser".Equals(key))
			{
				throw new NCMBException("belongUser key is already exist. Use this.Users to set it");
			}
			if ("belongRole".Equals(key))
			{
				throw new NCMBException("belongRole key is already exist. Use this.Roles to set it");
			}
		}

		internal override void _beforeSave()
		{
			if (base.ObjectId == null && Name == null)
			{
				throw new NCMBException(new ArgumentException("New roles must specify a name."));
			}
		}

		internal override string _getBaseUrl()
		{
			return NCMBSettings.DomainURL + "/" + NCMBSettings.APIVersion + "/roles";
		}
	}
}
