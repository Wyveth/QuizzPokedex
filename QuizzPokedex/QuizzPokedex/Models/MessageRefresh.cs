using MvvmCross.Plugin.Messenger;

namespace QuizzPokedex.Models
{
    public class MessageRefresh : MvxMessage
    {
        public MessageRefresh(object sender, bool refresh) : base(sender)
        {
            Refresh = refresh;
        }

        public bool Refresh { get; private set; }
    }
}
