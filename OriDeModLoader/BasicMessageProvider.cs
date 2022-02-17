using System.Collections.Generic;
using System.Diagnostics;

namespace BaseModLib
{
    public class BasicMessageProvider : MessageProvider
    {
        public BasicMessageProvider()
        {
            this.messages = new MessageDescriptor[1];
        }

        public BasicMessageProvider(string message)
        {
            this.messages = new MessageDescriptor[1];
            this.messages[0] = new MessageDescriptor(message);
        }

        [DebuggerHidden]
        public override IEnumerable<MessageDescriptor> GetMessages()
        {
            return this.messages;
        }

        public void SetMessage(string message)
        {
            this.messages[0] = new MessageDescriptor(message);
        }

        public MessageDescriptor[] messages;
    }
}
