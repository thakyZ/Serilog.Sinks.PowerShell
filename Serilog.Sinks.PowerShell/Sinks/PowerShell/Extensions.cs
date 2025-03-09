using System.Management.Automation;

namespace Serilog.Sinks.PowerShell;

internal static class Extensions {
  internal static ErrorCategory GetErrorCategory(this string name) {
    if (Enum.TryParse(typeof(ErrorCategory), name, out object? result)) {
      return (ErrorCategory)result;
    }
    return ErrorCategory.NotSpecified;
  }
}
