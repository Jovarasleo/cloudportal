using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
namespace server.Controllers

{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("AllowLocalhost")]
    public class StartStopServicesController : ControllerBase
    {
        private readonly ILogger<StartStopServicesController> _logger;

        public StartStopServicesController(ILogger<StartStopServicesController> logger)
        {
            _logger = logger;
        }

        [HttpPost("control")]
        public IActionResult ControlService([FromQuery] string action, [FromQuery] string serviceName, [FromQuery] string sudoPassword)
        {
            if (string.IsNullOrWhiteSpace(action) || string.IsNullOrWhiteSpace(serviceName) || string.IsNullOrWhiteSpace(sudoPassword))
            {
                return BadRequest("Action, service name, and sudo password must be provided.");
            }

            string command = action.ToLower() switch
            {
                "start" => $"service {serviceName} start",
                "stop" => $"service {serviceName} stop",
                _ => null
            };

            if (command == null)
            {
                return BadRequest("Invalid action. Only 'start' and 'stop' are supported.");
            }

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"echo {sudoPassword} | sudo -S {command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    if (process == null)
                    {
                        return StatusCode(500, "Failed to start the process.");
                    }

                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    if (process.ExitCode != 0)
                    {
                        return StatusCode(500, $"Error: {error}");
                    }

                    return Ok(output);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
