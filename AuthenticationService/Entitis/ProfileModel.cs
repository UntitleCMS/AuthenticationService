namespace AuthenticationService.Entitis
{
    public class ProfileModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
