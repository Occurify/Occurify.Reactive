# Occurify.Reactive

Reactive Extensions for Occurify: Enabling seamless scheduling of instant and period-based timelines.

For details on the Occurify ecosystem including examples for this library, please check the following documentation: [Occurify main README](https://github.com/Occurify/Occurify).

## Installation

Occurify.Reactive is distributed as a [NuGet package](https://www.nuget.org/packages/Occurify.Reactive), you can install it from the official NuGet Gallery. Please use the following command to install it using the NuGet Package Manager Console window.
```
PM> Install-Package Occurify.Reactive
```

## Usage

Rather than working with concrete instants and periods in time, Occurify allows for conceptual representation of time using intstant and period timelines.

For example, rather than listing all workdays of a year to work with, you can define the concept of "all workdays", apply transformations or filters, and extract the relevant periods as needed.

The following example demonstrates how to define a period timeline, `workingHours` that includes all periods between **8 AM and 6 PM**. By subtracting weekends, we obtain a new period timeline, `workingTime`, that represents all workdays within that range:
```cs
IPeriodTimeline workingHours = TimeZonePeriods.Between(startHour: 8, endHour: 18);
IPeriodTimeline weekends = TimeZonePeriods.Days(DayOfWeek.Saturday, DayOfWeek.Sunday);
IPeriodTimeline workingTime = workingHours - weekends;
```
Now, `workingTime` represents all workdays from **8 AM and 6 PM** and can be used to schedule:

```cs
workingTime.ToBooleanObservable(scheduler)
    .Subscribe(wt =>
    {
        if (wt)
        {
            Console.WriteLine("Time to start working!");
            return;
        }

        Console.WriteLine("Have a nice evening!");
    });
```

## License

Copyright © 2025 Jasper Lammers. Occurify.Reactive is licensed under The MIT License (MIT).