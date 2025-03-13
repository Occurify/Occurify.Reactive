﻿using Microsoft.Reactive.Testing;
using Occurify.Reactive.Extensions;

namespace Occurify.Reactive.Tests;

[TestClass]
public class TimelineToObservableTests
{
    [TestMethod]
    public void ToInstantObservable()
    {
        const int timeGap1 = 42;
        const int timeGap2 = 1337;

        var now = DateTime.UtcNow;
        var scheduler = new TestScheduler();
        var results = new List<DateTime>();

        var time1 = now + TimeSpan.FromTicks(timeGap1);
        var time2 = now + TimeSpan.FromTicks(timeGap1 + timeGap2);
        var timeline = Timeline.FromInstants(time1, time2);
            
        var observable = timeline.ToInstantObservable(now, scheduler);
            
        observable.Subscribe(results.Add);

        // First set the current time. Note that we do this after creating the observable, as Observable.Generate also uses the scheduler for the first iteration, and this triggers that setup.
        scheduler.AdvanceTo(now.Ticks);
            
        Assert.IsTrue(!results.Any());

        scheduler.AdvanceBy(timeGap1 - 1);
        Assert.IsTrue(!results.Any());

        scheduler.AdvanceBy(1);
        CollectionAssert.AreEqual(new[] { time1 }, results);

        scheduler.AdvanceBy(timeGap2 - 1);
        CollectionAssert.AreEqual(new[] { time1 }, results);

        scheduler.AdvanceBy(1);
        CollectionAssert.AreEqual(new[] { time1, time2 }, results);
    }
}