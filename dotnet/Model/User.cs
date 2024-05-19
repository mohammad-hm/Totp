namespace TOTPExample.Model
{
    public class User
    {
        public string UserName { get; set; } = string.Empty;
        public string TOTPSecret { get; set; } = string.Empty;
    }

}
