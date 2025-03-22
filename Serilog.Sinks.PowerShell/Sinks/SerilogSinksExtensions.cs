using System.Management.Automation;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.PowerShell;

// Ignore Spelling: Serilog;

namespace Serilog.Sinks;

/// <summary>
/// Extension methods for the <see cref="LoggerSinkConfiguration" /> class.
/// </summary>
public static class SerilogSinksExtensions {
  /// <summary>
  /// Creates a PowerShell <see cref="Cmdlet" /> sink for <see cref="Serilog" />. Includes an optional <paramref name="formatProvider" /> parameter.
  /// </summary>
  /// <param name="loggerConfiguration">The <see cref="Serilog" /> <see cref="LoggerSinkConfiguration" /> class used to configure the sinks.</param>
  /// <param name="cmdlet">The PowerShell <see cref="Cmdlet" /> or <see cref="PSCmdlet" /> to use to print information while logging.</param>
  /// <param name="formatProvider">An optional <see cref="IFormatProvider" /> for logging via a specific format.</param>
  /// <param name="restrictedToMinimum">An optional <see cref="LogEventLevel" /> enum to restrict the minimum logging level of this type.</param>
  /// <param name="loggingLevelSwitch">An optional <see cref="LoggingLevelSwitch" /> to update the current logging level.</param>
  /// <returns>The resulting <see cref="Serilog" /> <see cref="LoggerConfiguration" /> from creating the sink.</returns>
  public static LoggerConfiguration PowerShell(this LoggerSinkConfiguration loggerConfiguration, Cmdlet cmdlet, IFormatProvider? formatProvider, LogEventLevel restrictedToMinimum = LogEventLevel.Verbose, LoggingLevelSwitch? loggingLevelSwitch = null) {
    return loggerConfiguration.Sink(new PowerShellSink(cmdlet, formatProvider, restrictedToMinimum, loggingLevelSwitch), restrictedToMinimum, loggingLevelSwitch);
  }

  /// <summary>
  /// Creates a PowerShell <see cref="Cmdlet" /> sink for <see cref="Serilog" />. Includes an optional <paramref name="formatProvider" /> parameter.
  /// </summary>
  /// <param name="loggerConfiguration">The <see cref="Serilog" /> <see cref="LoggerSinkConfiguration" /> class used to configure the sinks.</param>
  /// <param name="cmdlet">The PowerShell <see cref="Cmdlet" /> or <see cref="PSCmdlet" /> to use to print information while logging.</param>
  /// <param name="restrictedToMinimum">An optional <see cref="LogEventLevel" /> enum to restrict the minimum logging level of this type.</param>
  /// <param name="loggingLevelSwitch">An optional <see cref="LoggingLevelSwitch" /> to update the current logging level.</param>
  /// <returns>The resulting <see cref="Serilog" /> <see cref="LoggerConfiguration" /> from creating the sink.</returns>
  public static LoggerConfiguration PowerShell(this LoggerSinkConfiguration loggerConfiguration, Cmdlet cmdlet, LogEventLevel restrictedToMinimum = LogEventLevel.Verbose, LoggingLevelSwitch? loggingLevelSwitch = null) {
    return loggerConfiguration.Sink(new PowerShellSink(cmdlet, null, restrictedToMinimum, loggingLevelSwitch), restrictedToMinimum, loggingLevelSwitch);
  }
}
