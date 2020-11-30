﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using TogglDesktop.Resources;

namespace TogglDesktop.ViewModels
{
    public class TimelineBlockViewModel : ReactiveObject
    {
        [Reactive]
        public double HorizontalOffset { get; set; }
        [Reactive]
        public double Height { get; set; }
        [Reactive]
        public double VerticalOffset { get; set; }

        public double Bottom => VerticalOffset + Height;
    }

    public class GapTimeEntryBlock : TimelineBlockViewModel
    {
        public ReactiveCommand<Unit, string> CreateTimeEntryFromBlock { get; }

        public GapTimeEntryBlock(Func<double, double, string> addNewTimeEntry)
        {
            CreateTimeEntryFromBlock = ReactiveCommand.Create(() =>
            {
                var id = addNewTimeEntry(VerticalOffset, Height);
                Toggl.Edit(id, false, Toggl.Description);
                return id;
            });
        }
    }

    public class TimeEntryBlock : TimelineBlockViewModel
    {
        [Reactive]
        public string Color { get; set; }
        [Reactive]
        public bool ShowDescription { get; set; }
        [Reactive]
        public string Description { get; set; }
        [Reactive]
        public string ProjectName { get; set; }
        [Reactive]
        public string ClientName { get; set; }
        public string TaskName { get; set; }
        [Reactive]
        public bool HasTag { get; set; }
        [Reactive]
        public bool IsBillable { get; set; }
        public string Duration { [ObservableAsProperty]get; }
        public string StartEndCaption { [ObservableAsProperty]get; }
        public ReactiveCommand<Unit, Unit> OpenEditView { get; }
        public ReactiveCommand<Unit, bool> ContinueEntry { get; }
        public ReactiveCommand<Unit, Unit> CreateFromEnd { get; }
        public ReactiveCommand<Unit, Unit> StartFromEnd { get; }
        public ReactiveCommand<Unit, bool> Delete { get; }
        public string TimeEntryId { get; }

        [Reactive]
        public bool IsEditViewOpened { get; set; }

        public bool IsResizable { [ObservableAsProperty] get; }

        public BehaviorSubject<(DateTime Started, DateTime Ended)> StartEnd { get; }

        [Reactive]
        public bool IsDragged { get; set; }

        public DateTime DateCreated { get; }

        private readonly double _hourHeight;

        public TimeEntryBlock(string timeEntryId, int hourHeight, DateTime date)
        {
            _hourHeight = hourHeight;
            DateCreated = date;
            TimeEntryId = timeEntryId;
            OpenEditView = ReactiveCommand.Create(() => Toggl.Edit(TimeEntryId, false, Toggl.Description));
            ContinueEntry = ReactiveCommand.Create(() => Toggl.Continue(TimeEntryId));
            CreateFromEnd = ReactiveCommand.Create(() => TimelineUtils.CreateAndEditTimeEntry(StartEnd.Value.Ended, StartEnd.Value.Ended.AddHours(1)));
            StartFromEnd = ReactiveCommand.Create(() => TimelineUtils.CreateAndEditRunningTimeEntryFrom(StartEnd.Value.Ended));
            Delete = ReactiveCommand.Create(() => Toggl.DeleteTimeEntry(TimeEntryId));
            StartEnd = this.WhenAnyValue(x => x.VerticalOffset, x => x.Height, (offset, height) =>
                (Started: TimelineUtils.ConvertOffsetToDateTime(offset, date, _hourHeight), Ended: TimelineUtils.ConvertOffsetToDateTime(offset + height, date, _hourHeight)))
                .ToBehaviorSubject();
            StartEnd.Select(tuple => $"{tuple.Started:HH:mm tt} - {tuple.Ended:HH:mm tt}")
                .ToPropertyEx(this, x => x.StartEndCaption);
            StartEnd.Select(tuple =>
                {
                    var duration = tuple.Ended.Subtract(tuple.Started);
                    return duration.Hours + " h " + duration.Minutes + " min";
                })
                .ToPropertyEx(this, x => x.Duration);
            this.WhenAnyValue(x => x.Height)
                .Select(h => h >= TimelineConstants.MinResizableTimeEntryBlockHeight)
                .Where(_ => !IsDragged)
                .ToPropertyEx(this, x => x.IsResizable);
        }

        public void ChangeStartTime()
        {
            Toggl.SetTimeEntryStartTimeStamp(TimeEntryId, Toggl.UnixFromDateTime(StartEnd.Value.Started));
        }

        public void ChangeEndTime()
        {
            Toggl.SetTimeEntryEndTimeStamp(TimeEntryId, Toggl.UnixFromDateTime(StartEnd.Value.Ended));
        }

        public void ChangeStartEndTime()
        {
            ChangeStartTime();
            ChangeEndTime();
        }
    }
}
