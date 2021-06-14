namespace Wiz.Chapter4.Domain.Events
{
    public struct EmailChangedEvent
    {
        public EmailChangedEvent(int userId, string newEmail)
        {
            UserId = userId;
            NewEmail = newEmail;
        }

        public int UserId { get; }

        public string NewEmail { get; }
    }
}