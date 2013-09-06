using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Data;

namespace Sitecore.T4Class.Generate.Module.BusinessLayer
{
  public class ConfigManager
  {

    public static T4ClassConfig GetT4ClassConfig()
    {
      return new T4ClassConfig(ConfigManager.GetContentDB.GetItem(T4ClassConsts.Configurations.ScT4ClassConfig));
    }

    private Database _db;

		public static Database GetContentDB
		{
			get
			{
				return Context.ContentDatabase ?? Factory.GetDatabase("master");
			}
		}
  }
}