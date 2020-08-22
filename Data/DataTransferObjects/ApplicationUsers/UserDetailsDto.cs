namespace MessengerAPI.Data.DataTransferObjects.ApplicationUsers
{
    public class UserDetailsDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public int ActiveChats { get; set; }
        public int MessagesSent { get; set; }
    }
}
