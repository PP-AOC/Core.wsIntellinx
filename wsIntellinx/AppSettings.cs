using AOC.Data.AppSettings.Entities;
using Core.Nuget.Security.BaseClasses;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wsIntellinx
{
    /// <summary>
    /// Class representing appsettings.json configuration file.
    /// </summary>
    public class AppSettings : AppSettingsBase
    {
        // Keys for when needing to use IConfigure and not AppSettings.

        /// <summary>Key to use with IConfigure for connection to AppSettings.Database.AppSettingsConn</summary>
        public static readonly string AppSettingsDbConnectionKey = "AppSettings:ConnectionStrings:AppSettingsConn";

        /// <summary>
        /// Database settings
        /// </summary>
        public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();

        /// <summary>
        /// Allowed origins for CORS policies
        /// </summary>
        public string[] AllowedOrigins { get; set; }

        /// <summary>
        /// This method will contain mapping for all properties unique to this service.
        /// It will then call the base method.
        /// </summary>
        /// <param name="ownerSettings"></param>
        /// <returns></returns>
        protected override int MapSettings(IEnumerable<WebServiceSettings> ownerSettings)
        {
            int updatedSettings = 0;
            int updatedBaseSettings = base.MapSettings(ownerSettings);
            var countOfOwnerSettings = ownerSettings.Count();
            foreach (var setting in ownerSettings)
            {
                switch (setting.AppKey)
                {
                    case "AllowedOrigins":
                        var allowedOrigins = setting.KeyValue.Split(",");
                        AllowedOrigins = allowedOrigins;
                        updatedSettings++;
                        break;
                    default:
                        // if there is a setting in the DB that is not present here we need to log the discrepency.                        
                        Log.Logger.Error("Setting:{0} was not set by extended class.", new object[] { setting.AppKey });
                        break;
                }
            }
            var totalUpdatedSettings = updatedBaseSettings + updatedSettings;
            var extraSettings = countOfOwnerSettings - totalUpdatedSettings;
            Log.Logger.Error("Settings successfully reset from DB={0}. Extra setting(s) found in DB={1}. ", new object[] { totalUpdatedSettings, extraSettings });
            return extraSettings;
        }
    }

    /// <summary>
    /// Class for Database configuration settings
    /// </summary>
    public class ConnectionStrings : ConnectionStringsBase
    {
        // <summary>
        // CourtNetAuditConn string to Courtnetaudit DB
        // </summary>
        //public string CourtNetAuditConn { get; set; } = string.Empty;
    }
}
