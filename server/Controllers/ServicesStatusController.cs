using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("AllowLocalhost")]
    public class ServicesStatusController : ControllerBase
    {
        private readonly ILogger<ServicesStatusController> _logger;

        public ServicesStatusController(ILogger<ServicesStatusController> logger)
        {
            _logger = logger;
        }

        [HttpGet("status")]
        public IActionResult GetServiceStatus([FromQuery] string serviceName, [FromQuery] string sudoPassword)
        {
            if (string.IsNullOrWhiteSpace(serviceName) || string.IsNullOrWhiteSpace(sudoPassword))
            {
                return BadRequest("Service name and sudo password must be provided.");
            }

            string command = $"service {serviceName} status";

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
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    var serviceInfo = ParseServiceStatus(output);
                    return Ok(serviceInfo);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private object ParseServiceStatus(string output)
        {
            // Example logic to parse service status. Customize as needed.
            var lines = output.Split('\n');
            var statusInfo = new
            {
                ServiceName = lines[0].Split(new[] { ' ' }, 2)[0],
                Status = lines[2].Trim(),
                Description = lines[3].Trim()
            };

            return statusInfo;
        }
    }
}
