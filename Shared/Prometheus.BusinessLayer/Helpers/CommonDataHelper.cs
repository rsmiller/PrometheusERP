﻿using Prometheus.Database;

namespace Prometheus.BusinessLayer.Helpers
{
    public class CommonDataHelper<T> where T : BaseDatabaseModel
    {
        public static T FillCommonFields(T model, int calling_user_id)
        {
            model.created_by = calling_user_id;
            model.updated_by = calling_user_id;

            return FillCommonFields(model);
        }

        public static T FillCommonFields(T model)
        {
            var now = DateTime.UtcNow;

            model.created_on = now;
            model.updated_on = now;
            model.created_on = now;
            model.updated_on = now;
            model.created_on_string = now.ToString("u");
            model.created_on_timezone = GetTimezoneAsString(now);
            model.updated_on_string = now.ToString("u");
            model.updated_on_timezone = GetTimezoneAsString(now);

            return model;
        }


        public static DateTime EnsureUtc(DateTime the_date)
        {
            if (the_date.Kind != DateTimeKind.Utc)
                return DateTime.SpecifyKind(the_date, DateTimeKind.Utc);
            else
                return the_date;
        }
        public static string GetTimezoneAsString(DateTime the_date)
        {
            var offset = TimeZoneInfo.Local.GetUtcOffset(the_date);
            string formattedOffset = offset.ToString(@"hh\:mm");

            if (offset < TimeSpan.Zero)
                formattedOffset = "-" + formattedOffset;
            else
                formattedOffset = "+" + formattedOffset;

            return formattedOffset;
        }
    }
}
