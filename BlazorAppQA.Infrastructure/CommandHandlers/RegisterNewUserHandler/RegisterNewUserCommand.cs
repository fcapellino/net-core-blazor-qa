namespace BlazorAppQA.Infrastructure.CommandHandlers.RegisterNewUserHandler
{
    public class RegisterNewUserCommand
    {
        public string UserName { get; set; }
        public string Base64AvatarImage { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmedPassword { get; set; }
        public string LinkedinUrl { get; set; }
        public string Biography { get; set; }
    }
}
