using System.Collections.Concurrent;
using System.Diagnostics;

namespace Pasteimg.Server.Transformers
{
    public interface IBackgroundTransformer : IHostedService
    {
        void EnqueueRequest(BackgroundTransformationRequest request);
    }

    public class BackgroundTransformer : BackgroundService, IBackgroundTransformer
    {
        private ConcurrentQueue<BackgroundTransformationRequest> requests;

        public BackgroundTransformer()
        {
            requests = new ConcurrentQueue<BackgroundTransformationRequest>();
        }

        public void EnqueueRequest(BackgroundTransformationRequest request)
        {
            requests.Enqueue(request);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                while (!requests.IsEmpty)
                {
                    if (requests.TryDequeue(out BackgroundTransformationRequest request))
                    {
                        request.Transform();
                        await Task.Delay(10);
                    }
                }
                await Task.Delay(100);
            }
            await Task.CompletedTask;
        }
    }
}