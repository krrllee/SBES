using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public enum AuditEventTypes
	{
		UserForgiven = 0,
		UserBanned = 1
	}

	internal class AuditEvents
	{
		private static ResourceManager resourceManager = null;
		private static object resourceLock = new object();

		private static ResourceManager ResourceMgr
		{
			get
			{
				lock (resourceLock)
				{
					if (resourceManager == null)
					{
						resourceManager = new ResourceManager
							(typeof(AuditEventFile).ToString(),
							Assembly.GetExecutingAssembly());
					}
					return resourceManager;
				}
			}
		}


		public static string UserForgiven
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.UserForgiven.ToString());

			}
		}
		public static string UserBanned
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.UserBanned.ToString());

			}
		}

	}
}
