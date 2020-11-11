using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMedia.Messaging
{
    public class MoveMediaCompletedMessage
    {
        public string OperationId { get; set; }

        public Guid MediaId { get; set; }

        public string OldFolder { get; set; }

        public string NewFolder { get; set; }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }
    }

    public class MoveMediaRequestCompletedMessage
    {
        public string OperationId { get; set; }

        public int ErrorCount { get; set; }

        public int SuccessCount { get; set; }
    }
}
