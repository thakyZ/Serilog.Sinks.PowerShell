using System.Management.Automation;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.PowerShell;
internal class PowerShellSink : ILogEventSink {
  private readonly IFormatProvider? _formatProvider;
  private readonly Cmdlet _cmdlet;

  /// <summary>
  /// Creates a new instance of <see cref="PowerShellSink"/>
  /// </summary>
  /// <param name="formatProvider">The <see cref="IFormatProvider"/> used when rendering the message</param>
  public PowerShellSink(Cmdlet cmdlet, IFormatProvider? formatProvider) {
    _cmdlet = cmdlet;
    _formatProvider = formatProvider;
  }

  /// <summary>
  /// Emits the provided log event from a sink 
  /// </summary>
  /// <param name="logEvent">The event being logged</param>
  public void Emit(LogEvent logEvent) {
    var message = logEvent.RenderMessage(_formatProvider);
    Console.WriteLine(DateTimeOffset.Now.ToString() + " "  + message);
    switch (logEvent.Level) {
      case LogEventLevel.Verbose:
        _cmdlet.WriteVerbose(message);
        break;
      case LogEventLevel.Debug:
        _cmdlet.WriteDebug(message);
        break;
      case LogEventLevel.Information:
        _cmdlet.WriteInformation(message, []);
        break;
      case LogEventLevel.Warning:
        _cmdlet.WriteWarning(message);
        break;
      case LogEventLevel.Error when logEvent.Exception is not null && logEvent.Properties.ContainsKey("errorId") && logEvent.Properties.ContainsKey("errorCategory") && logEvent.Properties.ContainsKey("target"):
        _cmdlet.WriteError(new ErrorRecord(new Exception(message, logEvent.Exception), logEvent.Properties["errorId"].ToString(), logEvent.Properties["errorCategory"].ToString().GetErrorCategory(), logEvent.Properties["target"].ToString()));
        break;
      case LogEventLevel.Error when logEvent.Exception is not null:
        _cmdlet.WriteError(new ErrorRecord(new Exception(message, logEvent.Exception), "", ErrorCategory.NotSpecified, ""));
        break;
      case LogEventLevel.Error when logEvent.Exception is null && logEvent.Properties.ContainsKey("errorId") && logEvent.Properties.ContainsKey("errorCategory") && logEvent.Properties.ContainsKey("target"):
        _cmdlet.WriteError(new ErrorRecord(new Exception(message), logEvent.Properties["errorId"].ToString(), logEvent.Properties["errorCategory"].ToString().GetErrorCategory(), logEvent.Properties["target"].ToString()));
        break;
      case LogEventLevel.Error when logEvent.Exception is null:
        _cmdlet.WriteError(new ErrorRecord(new Exception(message), "", ErrorCategory.NotSpecified, ""));
        break;
      case LogEventLevel.Fatal when logEvent.Exception is not null && logEvent.Properties.ContainsKey("errorId") && logEvent.Properties.ContainsKey("errorCategory") && logEvent.Properties.ContainsKey("target"):
        _cmdlet.ThrowTerminatingError(new ErrorRecord(new Exception(message, logEvent.Exception), logEvent.Properties["errorId"].ToString(), logEvent.Properties["errorCategory"].ToString().GetErrorCategory(), logEvent.Properties["target"].ToString()));
        break;
      case LogEventLevel.Fatal when logEvent.Exception is not null:
        _cmdlet.ThrowTerminatingError(new ErrorRecord(new Exception(message, logEvent.Exception), "", ErrorCategory.NotSpecified, ""));
        break;
      case LogEventLevel.Fatal when logEvent.Exception is null && logEvent.Properties.ContainsKey("errorId") && logEvent.Properties.ContainsKey("errorCategory") && logEvent.Properties.ContainsKey("target"):
        _cmdlet.ThrowTerminatingError(new ErrorRecord(new Exception(message), logEvent.Properties["errorId"].ToString(), logEvent.Properties["errorCategory"].ToString().GetErrorCategory(), logEvent.Properties["target"].ToString()));
        break;
      case LogEventLevel.Fatal when logEvent.Exception is null:
        _cmdlet.ThrowTerminatingError(new ErrorRecord(new Exception(message), "", ErrorCategory.NotSpecified, ""));
        break;
      default:
        // throw new ArgumentException($"Got unknown log event level \"{logEvent.Level}\".", nameof(logEvent.Level));
        break;
    }
  }
}
