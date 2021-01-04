using EmailService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FolderService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEmailSender _emailSender;
        private FileSystemWatcher watcher;
        private readonly string directory = @"C:\Users\mrmar\OneDrive\桌面\MyFolder";

        public Worker(ILogger<Worker> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }
        
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            watcher = new FileSystemWatcher();
            watcher.Path = directory;
            watcher.Created += OnChanged;
            return base.StartAsync(cancellationToken);
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            SendEmail(e.FullPath);
        }

        private void SendEmail(string fullPath)
        {
            _logger.LogInformation("New message had sended at: {time}", DateTimeOffset.Now);
            var message = new Message(new string[] { "assistantsender@gmail.com" },"Test subject","Test content", fullPath);
            _emailSender.SendEmail(message);


        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);

                }
                watcher.EnableRaisingEvents = true;// starts listening   
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(3000, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

    }
}
