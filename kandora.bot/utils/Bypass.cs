using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace kandora.bot.utils
{
    class Bypass
    {
        public static bool isSuperUser(string userId)
        {
            return userId == ConfigurationManager.AppSettings.Get("SuperUserId");
        }
        public static bool isKandora(string userId)
        {
            return userId == ConfigurationManager.AppSettings.Get("BotId");
        }
        
    }
}
