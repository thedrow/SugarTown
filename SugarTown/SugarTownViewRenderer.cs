using System.IO;
using Nancy;
using Nancy.Responses;
using Nancy.ViewEngines;

namespace SugarTown
{
    public class SugarTownViewRenderer
    {
        private readonly NancyContext context;
        private readonly IViewResolver ViewResolver;

        private readonly IViewEngine Engine = new Nancy.ViewEngines.Razor.RazorViewEngine();

        public SugarTownViewRenderer(NancyContext context)
        {
            this.context = context;
            //this.ViewResolver = viewResolver;
        }

        public Response this[string name]
        {
            get { return RenderView(name, null, this.context); }
        }

        public Response this[string name, dynamic model]
        {
            get { return RenderView(name, model, this.context); }
        }

        private Response RenderView(string name, dynamic model, NancyContext context)
        {
            var fullName = string.Concat(name, ".cshtml");

            var stream = GetBodyStream(fullName);

            var location = GetViewLocationResult(fullName, stream);

            var cache = new DefaultViewCache();

            //context.Items.Add(CsrfToken.DEFAULT_CSRF_KEY, "DIAGNOSTICSTOKEN");

            var renderContext =
                new DefaultRenderContext(ViewResolver, cache, new ViewLocationContext() { Context = context });

            return Engine.RenderView(location, model, renderContext);
        }

        public Stream GetBodyStream(string name)
        {
            var view = new EmbeddedFileResponse(typeof(SugarTownViewRenderer).Assembly, "SugarTown.Views.Posts", name);

            var stream = new MemoryStream();

            view.Contents.Invoke(stream);
            stream.Position = 0;
            return stream;
        }

        public ViewLocationResult GetViewLocationResult(string name, Stream bodyStream)
        {
            return new ViewLocationResult(
                "SugarTown/Views",
                name,
                "cshtml",
                () => new StreamReader(bodyStream));
        }


    }
}