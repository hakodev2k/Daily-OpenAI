using System;
using System.Collections.Generic;

// Reference only. Copy this method implementation into starter/Program.cs
// after your own attempt; do not compile this file with the starter.
public static class InspectionSchedule
{
    public static IReadOnlyList<DateOnly> Build(DateOnly firstDue, int count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        var result = new List<DateOnly>(count);
        var firstMonth = new DateOnly(firstDue.Year, firstDue.Month, 1);
        for (var offset = 0; offset < count; offset++)
        {
            var month = firstMonth.AddMonths(offset);
            var day = Math.Min(firstDue.Day, DateTime.DaysInMonth(month.Year, month.Month));
            result.Add(new DateOnly(month.Year, month.Month, day));
        }
        return result;
    }
}
