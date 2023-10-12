namespace Weather.Web.Administration
{
    public class AspNetRoles
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? Name { get; set; }

        public string? NormalizedName { get; set; }

        public string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }
}
