using TestRail.Types;

namespace TestRail.TestLogger
{
    public class CaseResult
    {
        public ulong? CaseId { get; set; }
        public string Title { get; set; }
        public ResultStatus Status { get; set; }
        public string Comment { get; set; }
    }
}