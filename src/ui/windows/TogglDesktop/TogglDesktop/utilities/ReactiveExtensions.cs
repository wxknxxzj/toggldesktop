using System;
using System.Reactive.Subjects;

namespace TogglDesktop
{
    public static class ReactiveExtensions
    {
        public static BehaviorSubject<T> ToBehaviorSubject<T>(this IObservable<T> observable)
        {
            var subject = new BehaviorSubject<T>(default);
            observable.Subscribe(next => subject.OnNext(next));
            return subject;
        }
    }
}
