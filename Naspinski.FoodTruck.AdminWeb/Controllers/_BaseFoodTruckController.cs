using Naspinski.FoodTruck.Data;
using Naspinski.FoodTruck.Data.Models.System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using static Naspinski.FoodTruck.Data.Constants;
using Query = Naspinski.FoodTruck.Data.Access.Queries;

namespace Naspinski.FoodTruck.AdminWeb.Controllers
{
    public class _BaseFoodTruckController : Controller
    {
        protected readonly FoodTruckContext _context;
        protected string _user;
        protected AdminAppSettings _settings;

        public _BaseFoodTruckController(FoodTruckContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _user = User?.Identity?.Name ?? string.Empty;

            var settings = new Query.Settings.Get(_context, new List<string> {
                SettingName.Title,
                SettingName.IsOrderingOn,
                SettingName.ContactEmail,
                SettingName.TwilioSid,
                SettingName.TwilioAuthToken,
                SettingName.TwilioPhoneNumber,
                SettingName.GoogleMapsDeveloperApiKey,
                SettingName.TimeZoneOffsetFromUtcInHours,
                SettingName.AutoNotificationDelayInMinutes
            })
            .ExecuteAndReturnResults();

            ViewData["Title"] = settings.FirstOrDefault(x => x.Name == SettingName.Title)?.Value 
                ?? "Food Truck";
            ViewData["IsOrderingOn"] = settings.FirstOrDefault(x => x.Name == SettingName.IsOrderingOn)?.Value 
                ?? "False";

            _settings = new AdminAppSettings(settings);

            base.OnActionExecuting(context);
        }
    }

    public class AdminAppSettings
    {
        public string Title;
        public string ContactEmail;
        public string TwilioSid;
        public string TwilioAuthToken;
        public string TwilioPhoneNumber;
        public string GoogleMapsDeveloperApiKey;
        public int TimeZoneOffsetFromUtcInHours;
        public int AutoNotificationDelayInMinutes;
        public bool IsTwilioValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(TwilioSid)
                    && !string.IsNullOrWhiteSpace(TwilioAuthToken)
                    && !string.IsNullOrWhiteSpace(TwilioPhoneNumber);
            }
        }

        public AdminAppSettings(IEnumerable<Setting> settings)
        {
            Title = settings.FirstOrDefault(x => x.Name == SettingName.Title.ToString())?.Value
                ?? "Food Truck";
            GoogleMapsDeveloperApiKey = settings.FirstOrDefault(x => x.Name == SettingName.GoogleMapsDeveloperApiKey.ToString())?.Value
                ?? "Food Truck";
            ContactEmail = settings.FirstOrDefault(x => x.Name == SettingName.ContactEmail.ToString())?.Value
                ?? string.Empty;
            TwilioSid = settings.FirstOrDefault(x => x.Name == SettingName.TwilioSid.ToString())?.Value
                ?? string.Empty;
            TwilioAuthToken = settings.FirstOrDefault(x => x.Name == SettingName.TwilioAuthToken.ToString())?.Value
                ?? string.Empty;
            TwilioPhoneNumber = settings.FirstOrDefault(x => x.Name == SettingName.TwilioPhoneNumber.ToString())?.Value
                ?? string.Empty;

            int.TryParse(settings.FirstOrDefault(x => x.Name == SettingName.TimeZoneOffsetFromUtcInHours.ToString())?.Value ?? "0",
                out TimeZoneOffsetFromUtcInHours);

            int.TryParse(settings.FirstOrDefault(x => x.Name == SettingName.AutoNotificationDelayInMinutes.ToString())?.Value ?? "0",
                out AutoNotificationDelayInMinutes);
        }
    }
}
