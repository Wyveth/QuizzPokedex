using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Text;

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
