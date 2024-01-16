
namespace API.Helpers.Utilities
{
    public static class DatetimeConvert
    {
        public static DateTime GetDateConvert(this DateTime date)
        {
            return new DateTime(date.Year,date.Month,date.Day,0,0,0);
        }
    }
}