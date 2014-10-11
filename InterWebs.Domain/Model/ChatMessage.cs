namespace InterWebs.Domain.Model
{
    public class ChatMessage: Entity
    {
        public string ChatName { get; set; }
        public string User { get; set; }
        public string Message { get; set; }
    }
}
