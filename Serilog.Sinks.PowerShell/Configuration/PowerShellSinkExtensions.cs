using System.Management.Automation;
using Serilog.Sinks.PowerShell;

namespace Serilog.Configuration;
public static class PowerShellSinkExtensions {
  public static LoggerConfiguration PowerShell(this LoggerSinkConfiguration loggerConfiguration, Cmdlet cmdlet, IFormatProvider? formatProvider = null) {
    return loggerConfiguration.Sink(new PowerShellSink(cmdlet, formatProvider));
  }
}
