namespace AiiaDemoSignalR.Model
{
    public class User
    {
        public string ConnectionId { get; set; }

        public string Name { get; set; }

        public string ApproverName { get; set; }

        public UserType Type { get; set; }
    }
}
