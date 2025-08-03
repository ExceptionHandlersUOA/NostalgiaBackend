using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace HoverthArchiver;

public class HtmlParser
{
    private IBrowsingContext context = null;
    private IHtmlParser parser = null;

    private static readonly string[] _imageExtensions =
        [".gif", ".jpg", ".jpeg", ".png", ".bmp", ".tif", ".tiff", ".webp", ".svg"];

    private static readonly string[] _videoExtensions = [".mp4", ".mkv", ".mov", ".avi"];

    private static readonly string[] _documentExtensions =
        [".pdf", ".doc", ".docx", ".odt", ".xlsx", ".xls", ".ppt", ".pptx", ".ods", ".odp"];

    public HtmlParser()
    {
        //Use the default configuration for AngleSharp
        var config = Configuration.Default;

        //Create a new context for evaluating webpages with the given config
        context = BrowsingContext.New(config);
        parser = context.GetService<IHtmlParser>();
    }

    /*
     *
     */
    public string FlattenText(string html)
    {
        var document = parser.ParseDocument(html);
        return document.DocumentElement.TextContent;
    }

    private List<string> GetElementsByAttribute(IHtmlDocument document, string elementName, string attributeName)
    {
        List<string> sourceUrls = [];
        var elements = document.QuerySelectorAll(elementName);
        foreach (var e in elements)
        {
            try
            {
                var srcUrl = e.Attributes.GetNamedItem(attributeName)?.Value;
                srcUrl = srcUrl?.Split("?").First();
                sourceUrls.Add(srcUrl);
            }
            catch (NullReferenceException exception)
            {
                Console.WriteLine(exception);
            }
        }

        return sourceUrls;
    }

    private List<string> ProcessHyperlinks(List<string> links, string[] acceptableExtensions)
    {
        List<string> sourceUrls = [];
        foreach (var link in links)
        {
            var extension = link.Split('?').First().Split('/').Last().Split('.').Last();
            if (acceptableExtensions.Contains(extension))
            {
                sourceUrls.Add(link);
            }
        }

        return sourceUrls;
    }

    private List<string> GetElementHyperlinks(IHtmlDocument document, string element)
    {
        return GetElementsByAttribute(document, element, "href");
    }

    private List<string> GetElementSources(IHtmlDocument document, string element)
    {
        return GetElementsByAttribute(document, element, "src");
    }

    public List<string> GetVideos(string html)
    {
        List<string> videos = [];
        var document = parser.ParseDocument(html);
        videos.Concat(GetElementSources(document, "video"));
        var videoLinks = GetElementHyperlinks(document, "a");
        videos.Concat(ProcessHyperlinks(videoLinks, _videoExtensions));

        return videos;
    }

    public List<string> GetImages(string html)
    {
        List<string> images = [];
        var document = parser.ParseDocument(html);
        images.Concat(GetElementSources(document, "images"));
        var imageLinks = GetElementHyperlinks(document, "a");
        images.Concat(ProcessHyperlinks(imageLinks, _imageExtensions));

        return images;
    }

    public List<string> GetDocuments(string html)
    {
        List<string> documents = [];
        var document = parser.ParseDocument(html);
        var documentLinks = GetElementHyperlinks(document, "a");
        documents.Concat(ProcessHyperlinks(documentLinks, _documentExtensions));

        return documents;
    }
}