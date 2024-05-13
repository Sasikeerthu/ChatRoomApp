using System.ComponentModel.DataAnnotations;

namespace ChatRoomApp.Models
{
    public class Registration_Credentials
    {
        public string name { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;

        [Compare("password", ErrorMessage = "Password and Confirm Password do not match.")]
        public string Confirm_password { get; set; } = string.Empty;
    }
}
