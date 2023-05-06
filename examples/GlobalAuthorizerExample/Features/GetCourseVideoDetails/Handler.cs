using Mediator;

namespace GlobalAuthorizerExample.Features.GetCourseVideoDetails
{
    public partial class GetCourseVideoDetails
    {
        public class Handler : IRequestHandler<Request, Response>
        {
            public ValueTask<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var random = new Random(request.VideoId);

                return ValueTask.FromResult(new Response
                {
                    VideoId = request.VideoId,
                    Title = $"Video {request.VideoId}",
                    Duration = random.Next(10, 60*60*10),
                    Thumbnail = $"https://localhost:3000/cdn/video-thumbnails/{request.VideoId}"
                });
            }
        }
    }
}
