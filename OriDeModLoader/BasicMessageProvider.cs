using System.Collections.Generic;
using System.Diagnostics;

namespace BaseModLib
{
    public class BasicMessageProvider : MessageProvider
    {
        public BasicMessageProvider()
        {
            messages = new MessageDescriptor[1];
        }

        public BasicMessageProvider(string message)
        {
            messages = new MessageDescriptor[1];
            messages[0] = new MessageDescriptor(message);
        }

        [DebuggerHidden]
        public override IEnumerable<MessageDescriptor> GetMessages()
        {
            return messages;
        }

        public void SetMessage(string message)
        {
            messages[0] = new MessageDescriptor(message);
        }

        public MessageDescriptor[] messages;
    }
}
