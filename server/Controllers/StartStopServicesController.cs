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
        public IActionResult ControlService([FromQuery] string action, [FromQuery] string serviceName)
        {
            if (string.IsNullOrWhiteSpace(action) || string.IsNullOrWhiteSpace(serviceName))
            {
                return BadRequest("Action and service name must be provided.");
            }

            string? command = action.ToLower() switch
            {
                "start" => $"sudo systemctl start {serviceName}",
                "stop" => $"sudo systemctl stop {serviceName}",
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
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process? process = Process.Start(psi))
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