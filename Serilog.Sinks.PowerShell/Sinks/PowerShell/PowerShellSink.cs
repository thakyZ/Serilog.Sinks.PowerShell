using System.Management.Automation;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.PowerShell;

/// <summary>
/// The <see cref="Serilog" /> <see cref="ILogEventSink" /> class for PowerShell <see cref="Cmdlet" />s.
/// </summary>
internal class PowerShellSink : ILogEventSink {
  /// <summary>
  /// Gets the PowerShell <see cref="Cmdlet" /> used to log to <see cref="Serilog" /> and PowerShell.
  /// </summary>
  private readonly Cmdlet _cmdlet;

  /// <summary>
  /// Gets an optional <see cref="IFormatProvider" /> for rendering messages to the PowerShell <see cref="System.Management.Automation.Host.PSHost" />.
  /// </summary>
  private readonly IFormatProvider? _formatProvider;

  /// <summary>
  /// Gets the restricted <see cref="LogEventLevel" /> from which to only print above.
  /// </summary>
  private readonly LogEventLevel _restrictedToMinimum;

  /// <summary>
  /// Gets an optional instance of a <see cref="LoggingLevelSwitch" />, defaults to <see cref="_restrictedToMinimum" /> if it is null.
  /// </summary>
  private readonly LoggingLevelSwitch? _loggingLevelSwitch;

  /// <summary>
  /// Creates a new instance of <see cref="PowerShellSink"/>
  /// </summary>
  /// <param name="cmdlet">The PowerShell <see cref="Cmdlet" /> used to log to <see cref="Serilog" /> and PowerShell.</param>
  /// <param name="formatProvider">An optional <see cref="IFormatProvider" /> for rendering messages to the PowerShell <see cref="System.Management.Automation.Host.PSHost" />.</param>
  /// <param name="formatProvider">The restricted <see cref="LogEventLevel" /> from which to only print above.</param>
  /// <param name="formatProvider">An optional instance of a <see cref="LoggingLevelSwitch" />, defaults to <see cref="_restrictedToMinimum" /> if it is null.</param>
  public PowerShellSink(Cmdlet cmdlet, IFormatProvider? formatProvider, LogEventLevel restrictedToMinimum, LoggingLevelSwitch? loggingLevelSwitch) {
    _cmdlet = cmdlet;
    _formatProvider = formatProvider;
    _restrictedToMinimum = restrictedToMinimum;
    _loggingLevelSwitch = loggingLevelSwitch;
  }

  /// <summary>
  /// Tests if the provided <see cref="LogEventLevel" /> is a valid level to print.
  /// </summary>
  /// <param name="logEventLevel">The <see cref="LogEventLevel" /> to test.</param>
  /// <returns>
  /// <see langword="true" /> when if <paramref name="logEventLevel" /> is above the threshold provided by
  /// <see cref="_loggingLevelSwitch.MinimumLevel" /> or if null, <see cref="_restrictedToMinimum" />;
  /// otherwise <see langword="false" />.
  /// </returns>
  private bool DetermineValidLogLevel(LogEventLevel logEventLevel) {
    if (this._loggingLevelSwitch is not null) {
      return this._loggingLevelSwitch.MinimumLevel >= logEventLevel;
    }

    return this._restrictedToMinimum >= logEventLevel;
  }

  /// <summary>
  /// Emits the provided log event from a sink
  /// </summary>
  /// <param name="logEvent">The event being logged</param>
  public void Emit(LogEvent logEvent) {
    var message = logEvent.RenderMessage(_formatProvider);
    Console.WriteLine($"{DateTimeOffset.Now.ToString(_formatProvider)} {message}");
    switch (logEvent.Level) {
      case LogEventLevel.Verbose when DetermineValidLogLevel(logEvent.Level):
        this._cmdlet.WriteVerbose(message);
        break;
      case LogEventLevel.Debug when DetermineValidLogLevel(logEvent.Level):
        this._cmdlet.WriteDebug(message);
        break;
      case LogEventLevel.Information when DetermineValidLogLevel(logEvent.Level):
        this._cmdlet.WriteInformation(message, []);
        break;
      case LogEventLevel.Warning when DetermineValidLogLevel(logEvent.Level):
        this._cmdlet.WriteWarning(message);
        break;
      case LogEventLevel.Error when DetermineValidLogLevel(logEvent.Level) && logEvent.Exception is not null && logEvent.Properties.ContainsKey("errorId") && logEvent.Properties.ContainsKey("errorCategory") && logEvent.Properties.ContainsKey("target"):
        this._cmdlet.WriteError(new ErrorRecord(new Exception(message, logEvent.Exception), logEvent.Properties["errorId"].ToString(), logEvent.Properties["errorCategory"].ToString().GetErrorCategory(), logEvent.Properties["target"].ToString()));
        break;
      case LogEventLevel.Error when DetermineValidLogLevel(logEvent.Level) && logEvent.Exception is not null:
        this._cmdlet.WriteError(new ErrorRecord(new Exception(message, logEvent.Exception), "", ErrorCategory.NotSpecified, ""));
        break;
      case LogEventLevel.Error when DetermineValidLogLevel(logEvent.Level) && logEvent.Exception is null && logEvent.Properties.ContainsKey("errorId") && logEvent.Properties.ContainsKey("errorCategory") && logEvent.Properties.ContainsKey("target"):
        this._cmdlet.WriteError(new ErrorRecord(new Exception(message), logEvent.Properties["errorId"].ToString(), logEvent.Properties["errorCategory"].ToString().GetErrorCategory(), logEvent.Properties["target"].ToString()));
        break;
      case LogEventLevel.Error when DetermineValidLogLevel(logEvent.Level) && logEvent.Exception is null:
        this._cmdlet.WriteError(new ErrorRecord(new Exception(message), "", ErrorCategory.NotSpecified, ""));
        break;
      case LogEventLevel.Fatal when DetermineValidLogLevel(logEvent.Level) && logEvent.Exception is not null && logEvent.Properties.ContainsKey("errorId") && logEvent.Properties.ContainsKey("errorCategory") && logEvent.Properties.ContainsKey("target"):
        this._cmdlet.ThrowTerminatingError(new ErrorRecord(new Exception(message, logEvent.Exception), logEvent.Properties["errorId"].ToString(), logEvent.Properties["errorCategory"].ToString().GetErrorCategory(), logEvent.Properties["target"].ToString()));
        break;
      case LogEventLevel.Fatal when DetermineValidLogLevel(logEvent.Level) && logEvent.Exception is not null:
        this._cmdlet.ThrowTerminatingError(new ErrorRecord(new Exception(message, logEvent.Exception), "", ErrorCategory.NotSpecified, ""));
        break;
      case LogEventLevel.Fatal when DetermineValidLogLevel(logEvent.Level) && logEvent.Exception is null && logEvent.Properties.ContainsKey("errorId") && logEvent.Properties.ContainsKey("errorCategory") && logEvent.Properties.ContainsKey("target"):
        this._cmdlet.ThrowTerminatingError(new ErrorRecord(new Exception(message), logEvent.Properties["errorId"].ToString(), logEvent.Properties["errorCategory"].ToString().GetErrorCategory(), logEvent.Properties["target"].ToString()));
        break;
      case LogEventLevel.Fatal when DetermineValidLogLevel(logEvent.Level) && logEvent.Exception is null:
        this._cmdlet.ThrowTerminatingError(new ErrorRecord(new Exception(message), "", ErrorCategory.NotSpecified, ""));
        break;
      default:
        this._cmdlet.WriteDebug($"Got unknown log event level \"{logEvent.Level}\". Or conditions for an existing level are not met.");
        break;
    }
  }
}
