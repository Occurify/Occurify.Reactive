﻿using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Occurify.Reactive.Extensions;

public static class TimelineExtensions
{
    /// <summary>
    /// Returns a <see cref="IObservable{Unit}"/> that emits a <see cref="Unit"/> every time an instant occurs.
    /// </summary>
    public static IObservable<Unit>
        ToPulseObservable(this ITimeline timeline, IScheduler scheduler) =>
        timeline.ToPulseObservable(DateTime.UtcNow, scheduler);

    /// <summary>
    /// Returns a <see cref="IObservable{Unit}"/> that emits a <see cref="Unit"/> every time an instant occurs using <paramref name="relativeTo"/> as a starting time.
    /// </summary>
    public static IObservable<Unit> ToPulseObservable(this ITimeline timeline, DateTime relativeTo, IScheduler scheduler) => timeline.ToInstantObservable(relativeTo, scheduler).Select(_ => Unit.Default);

    /// <summary>
    /// Returns a <see cref="IObservable{DateTime}"/> that emits an instant as <see cref="DateTime"/> when it occurs.
    /// </summary>
    public static IObservable<DateTime> ToInstantObservable(this ITimeline timeline,
        IScheduler scheduler) => timeline.ToInstantObservable(DateTime.UtcNow, scheduler);

    /// <summary>
    /// Returns a <see cref="IObservable{DateTime}"/> that emits an instant as <see cref="DateTime"/> when it occurs using <paramref name="relativeTo"/> as a starting time.
    /// </summary>
    public static IObservable<DateTime> ToInstantObservable(this ITimeline timeline, DateTime relativeTo, IScheduler scheduler)
    {
        return Observable.Generate(
            timeline.GetNextUtcInstant(relativeTo),
            sample => sample != null,
            sample => timeline.GetNextUtcInstant(sample!.Value),
            sample => sample!.Value,
            sample => sample!.Value, scheduler);
    }

    /// <summary>
    /// Returns a <see cref="IObservable{DateTime}"/> that immediately emits <see cref="DateTime.UtcNow"/> upon subscribing and then emits an instant as <see cref="DateTime"/> when it occurs.
    /// </summary>
    public static IObservable<DateTime> ToInstantObservableIncludingCurrentInstant(this ITimeline timeline, IScheduler scheduler)
    {
        return Observable.Defer(() =>
        {
            var utcNow = DateTime.UtcNow;
            return timeline.ToInstantObservable(utcNow, scheduler).Prepend(utcNow);
        });
    }

    /// <summary>
    /// Returns a <see cref="IObservable{DateTime}"/> that immediately emits <paramref name="relativeTo"/> upon subscribing and then emits an instant as <see cref="DateTime"/> when it occurs using <paramref name="relativeTo"/> as a starting time.
    /// </summary>
    public static IObservable<DateTime> ToInstantObservableIncludingCurrentInstant(this ITimeline timeline, DateTime relativeTo,
        IScheduler scheduler)
    {
        return Observable.Defer(() => timeline.ToInstantObservable(relativeTo, scheduler).Prepend(relativeTo));
    }
}