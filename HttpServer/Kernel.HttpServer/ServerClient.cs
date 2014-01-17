using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Kernel.HttpServer
{
    internal class ServerClient
    {
        private readonly Server _server;
        private readonly Socket _socket;
        private readonly NetworkStream _networkStream;
        private readonly StreamReader _streamReader;

        public ServerClient(Server server, Socket socket)
        {
            _server = server;
            _socket = socket;

            // Set up streams
            _networkStream = new NetworkStream(socket, true);
            _streamReader = new StreamReader(_networkStream);
        }

        public async void Do()
        {
            // We are executed on a separate thread from listener, but will release this back to the threadpool as often as we can.
            for (; ; )
            {
                var line = await _streamReader.ReadLineAsync();
                if (!_socket.Connected || line == null)
                    return;

                // You probably want to throttle incoming data (if someone dumped a large file with no GET you would be reading forever)

                // Look for GET-portion of header (first line) that contains the filename being requested
                if (line.ToUpperInvariant().StartsWith("GET "))
                {
                    // We got a request: GET /file HTTP/1.1
                    var file = line.Split(' ')[1].TrimStart('/');
                    // Default document is index.html
                    if (string.IsNullOrWhiteSpace(file))
                        file = "index.html";
                    // Send header+file
                    await SendFile(file);
                    return;
                }
            }
        }

        private async Task SendFile(string file)
        {
            // Get info and assemble header
            byte[] data;
            string responseCode = "";
            string contentType = "";
            try
            {
                if (File.Exists(file))
                {
                    // Read file
                    data = File.ReadAllBytes(file);
                    contentType = GetContentType(Path.GetExtension(file).TrimStart(".".ToCharArray()));
                    responseCode = "200 OK";
                }
                else
                {
                    data = System.Text.Encoding.ASCII.GetBytes("<html><body><h1>404 File Not Found</h1></body></html>");
                    contentType = GetContentType("html");
                    responseCode = "404 Not found";
                }
            }
            catch (Exception exception)
            {
                // In case of error dump exception to client.
                data =
                    System.Text.Encoding.ASCII.GetBytes("<html><body><h1>500 Internal server error</h1><pre>" +
                                                        exception.ToString() + "</pre></body></html>");
                responseCode = "500 Internal server error";
            }

            await SendResponse(responseCode, contentType, data);
        }

        public async Task SendResponse(string responseCode, string contentType, byte[] data)
        {

            string header = string.Format("HTTP/1.1 {0}\r\n"
                                              + "Server: {1}\r\n"
                                              + "Content-Length: {2}\r\n"
                                              + "Content-Type: {3}\r\n"
                                              + "Keep-Alive: Close\r\n"
                                              + "\r\n",
                                              responseCode, _server.ServerName, data.Length, contentType);
            // Send header & data
            var headerBytes = System.Text.Encoding.ASCII.GetBytes(header);
            await _networkStream.WriteAsync(headerBytes, 0, headerBytes.Length);
            await _networkStream.WriteAsync(data, 0, data.Length);
            await _networkStream.FlushAsync();
            // Close connection (we don't support keep-alive)
            _networkStream.Dispose();
            _streamReader.Dispose();
        }

        /// <summary>
        /// Get mime type from a file extension
        /// </summary>
        /// <param name="extension">File extension without starting dot (html, not .html)</param>
        /// <returns>Mime type or default mime type "application/octet-stream" if not found.</returns>
        private string GetContentType(string extension)
        {
            // We are accessing the registry with data received from third party, so we need to have a strict security test. We only allow letters and numbers.
            if (Regex.IsMatch(extension, "^[a-z0-9]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled))
                return (Registry.GetValue(@"HKEY_CLASSES_ROOT\." + extension, "Content Type", null) as string) ?? "application/octet-stream";
            return "application/octet-stream";
        }
    }
}
