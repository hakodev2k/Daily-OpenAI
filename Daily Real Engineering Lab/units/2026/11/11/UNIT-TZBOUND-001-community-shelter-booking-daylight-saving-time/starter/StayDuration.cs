namespace ShelterLab;
public static class StayDuration
{
    public static decimal BillableHours(DateTimeOffset checkIn, DateTimeOffset checkOut)
    {
        if (checkOut < checkIn) throw new ArgumentException("Checkout precedes checkin.");
        return (decimal)(checkOut.DateTime - checkIn.DateTime).TotalHours;
    }
}
