namespace WebApi.Shared.Utils;

public record AgeInfo(int Years, int Months, string Display);

public static class AgeCalculator
{
    public static AgeInfo Calculate(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var years = today.Year - birthDate.Year;
        var months = today.Month - birthDate.Month;

        if (today.Day < birthDate.Day)
            months--;

        if (months < 0)
        {
            years--;
            months += 12;
        }

        var display = Format(years, months);

        return new AgeInfo(years, months, display);
    }

    private static string Format(int years, int months)
    {
        var yearText = years == 1 ? "1 año" : $"{years} años";
        var monthText = months == 1 ? "1 mes" : $"{months} meses";

        if (years <= 0)
            return monthText;

        if (months <= 0)
            return yearText;

        return $"{yearText} y {monthText}";
    }
}
