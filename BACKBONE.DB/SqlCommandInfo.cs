using System.Data;

namespace BACKBONE.DB
{
    public class SqlCommandInfo
    {
        public string? Sql { get; set; }
        public object? Parameters { get; set; }
        public CommandType CommandType { get; set; } = CommandType.Text;
    }
}
