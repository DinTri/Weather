namespace Weather.CustomExceptions
{
    public class WeatherException : Exception
    {
        public int StatusCode { get; }

        public WeatherException(string? message) : base(message)
        {
        }
        public WeatherException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public WeatherException(string message, int statusCode, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}