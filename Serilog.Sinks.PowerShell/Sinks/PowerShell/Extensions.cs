using System.Management.Automation;

namespace Serilog.Sinks.PowerShell;

/// <summary>
/// Extension methods for the <see cref="Serilog.Sinks.PowerShell" /> library.
/// </summary>
internal static class Extensions {
  /// <summary>
  /// Gets the <see cref="ErrorCategory" /> from a <see langword="string" /> that represents an <see cref="ErrorCategory" /> value.
  /// </summary>
  /// <param name="name">A <see langword="string" /> that represents an <see cref="ErrorCategory" /> value.</param>
  /// <returns>An <see cref="ErrorCategory" /> parsed from <paramref name="name" /> or if non-found returns <see cref="ErrorCategory.NotSpecified" />.</returns>
  internal static ErrorCategory GetErrorCategory(this string name) {
    if (Enum.TryParse(typeof(ErrorCategory), name, out object? result)) {
      return (ErrorCategory)result;
    }

    return ErrorCategory.NotSpecified;
  }
}
