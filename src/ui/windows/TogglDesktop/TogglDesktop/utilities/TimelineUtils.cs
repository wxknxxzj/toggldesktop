﻿using System;

namespace TogglDesktop
{
    public static class TimelineUtils
    {
        public static ulong ConvertOffsetToUnixTime(double height, DateTime date, double hourHeight)
        {
            var dateTime = ConvertOffsetToDateTime(height, date, hourHeight);
            var unixTime = Toggl.UnixFromDateTime(dateTime);
            return unixTime >= 0 ? (ulong)unixTime : 0;
        }

        public static DateTime ConvertOffsetToDateTime(double height, DateTime date, double hourHeight)
        {
            var hours = 1.0 * height / hourHeight;
            var dateTime = date.AddHours(hours);
            return dateTime;
        }

        public static void CreateAndEditRunningTimeEntryFrom(DateTime started)
        {
            var teId = Toggl.Start("", "", 0, 0, "", "");
            Toggl.SetTimeEntryStartTimeStamp(teId, Toggl.UnixFromDateTime(started));
            Toggl.Edit(teId, true, Toggl.Description);
        }

        public static void CreateAndEditTimeEntry(DateTime started, DateTime ended)
        {
            var teId = Toggl.CreateEmptyTimeEntry((ulong)Toggl.UnixFromDateTime(started), (ulong)Toggl.UnixFromDateTime(ended));
            Toggl.Edit(teId, false, Toggl.Description);
        }

        public static DateTime StartTime(this Toggl.TogglTimeEntryView te) => Toggl.DateTimeFromUnix(te.Started);

        public static DateTime EndTime(this Toggl.TogglTimeEntryView te) => Toggl.DateTimeFromUnix(te.Ended);

        public static DateTime StartTime(this Toggl.TimelineChunkView chunk) => Toggl.DateTimeFromUnix(chunk.Started);

        public static DateTime EndTime(this Toggl.TimelineChunkView chunk) => Toggl.DateTimeFromUnix(chunk.Ended);
    }
}
