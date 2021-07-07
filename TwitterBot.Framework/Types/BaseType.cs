using System;

namespace TwitterBot.Framework.Types
{
    public class BaseType
    {
        public String Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public String CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public String ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}