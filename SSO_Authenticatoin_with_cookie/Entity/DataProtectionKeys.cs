namespace SSO_Authenticatoin_with_cookie.Entity
{
    public class DataProtectionKeys
    {
        public int Id {  get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } 
    }
}
