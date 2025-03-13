﻿using Microsoft.Reactive.Testing;
using Occurify.Extensions;
using Occurify.Reactive.Extensions;

namespace Occurify.Reactive.Tests;

[TestClass]
public class PeriodTimelineToObservableTests
{
    [TestMethod]
    public void ToObservable_SinglePeriod()
    {
        const int timeGap1 = 42;
        const int timeGap2 = 1337;

        var now = DateTime.UtcNow;
        var scheduler = new TestScheduler();
        var results = new List<PeriodTimelineSample>();

        var start = now + TimeSpan.FromTicks(timeGap1);
        var end = now + TimeSpan.FromTicks(timeGap1 + timeGap2);
        var periodTimeline = start.To(end).AsPeriodTimeline();
            
        var observable = periodTimeline.ToPeriodObservable(now, scheduler);

        observable.Subscribe(results.Add);
            
        // The observable should have provided a sample with the gap before the period.
        CollectionAssert.AreEqual(new[] { new PeriodTimelineSample(now, null, Period.Create(null, start)) }, results);

        // First set the current time. Note that we do this after creating the observable, as Observable.Generate also uses the scheduler for the first iteration, and this triggers that setup.
        scheduler.AdvanceTo(now.Ticks);
            
        scheduler.AdvanceBy(timeGap1 - 1);
        CollectionAssert.AreEqual(new[] { new PeriodTimelineSample(now, null, Period.Create(null, start)) }, results);

        scheduler.AdvanceBy(1);
        CollectionAssert.AreEqual(new[] { 
            new PeriodTimelineSample(now, null, Period.Create(null, start)),
            new PeriodTimelineSample(start, start.To(end), null)
        }, results);

        scheduler.AdvanceBy(timeGap2 - 1);
        CollectionAssert.AreEqual(new[] {
            new PeriodTimelineSample(now, null, Period.Create(null, start)),
            new PeriodTimelineSample(start, start.To(end), null)
        }, results);

        scheduler.AdvanceBy(1);
        CollectionAssert.AreEqual(new[] {
            new PeriodTimelineSample(now, null, Period.Create(null, start)),
            new PeriodTimelineSample(start, start.To(end), null),
            new PeriodTimelineSample(end, null, Period.Create(end, null))
        }, results);
    }

    [TestMethod]
    public void ToObservable_ConsecutivePeriods()
    {
        const int timeGap1 = 42;
        const int timeGap2 = 1337;

        var now = DateTime.UtcNow;
        var scheduler = new TestScheduler();
        var results = new List<PeriodTimelineSample>();

        var time1 = now + TimeSpan.FromTicks(timeGap1);
        var time2 = now + TimeSpan.FromTicks(timeGap1 + timeGap2);
        var periodTimeline = PeriodTimeline.FromInstantsAsConsecutive(time1, time2);
            
        var observable = periodTimeline.ToPeriodObservable(now, scheduler);

        observable.Subscribe(results.Add);

        // The observable should have provided a sample with the gap before the period.
        CollectionAssert.AreEqual(new[] { new PeriodTimelineSample(now, Period.Create(null, time1), null) }, results);

        // First set the current time. Note that we do this after creating the observable, as Observable.Generate also uses the scheduler for the first iteration, and this triggers that setup.
        scheduler.AdvanceTo(now.Ticks);

        scheduler.AdvanceBy(timeGap1 - 1);
        CollectionAssert.AreEqual(new[] { new PeriodTimelineSample(now, Period.Create(null, time1), null) }, results);

        scheduler.AdvanceBy(1);
        CollectionAssert.AreEqual(new[] {
            new PeriodTimelineSample(now, Period.Create(null, time1), null),
            new PeriodTimelineSample(time1, Period.Create(time1, time2), null)
        }, results);

        scheduler.AdvanceBy(timeGap2 - 1);
        CollectionAssert.AreEqual(new[] {
            new PeriodTimelineSample(now, Period.Create(null, time1), null),
            new PeriodTimelineSample(time1, Period.Create(time1, time2), null)
        }, results);

        scheduler.AdvanceBy(1);
        CollectionAssert.AreEqual(new[] {
            new PeriodTimelineSample(now, Period.Create(null, time1), null),
            new PeriodTimelineSample(time1, Period.Create(time1, time2), null),
            new PeriodTimelineSample(time2, Period.Create(time2, null), null)
        }, results);
    }
}