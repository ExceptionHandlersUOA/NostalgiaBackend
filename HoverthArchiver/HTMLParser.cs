using System.Runtime.CompilerServices;
using AngleSharp;
using AngleSharp.Html.Parser;

namespace HoverthArchiver;

public class HtmlParser
{
    private static IBrowsingContext context = null;
    private static IHtmlParser parser = null;
    
    public HtmlParser()
    {
        //Use the default configuration for AngleSharp
        var config = Configuration.Default;

        //Create a new context for evaluating webpages with the given config
        context = BrowsingContext.New(config);
        parser = context.GetService<IHtmlParser>();
    }
    
    public string FlattenText(string html)
    {
        // Console.WriteLine("HTML:\n" + html);
        //Create a parser to specify the document to load (here from our fixed string)
        var document = parser.ParseDocument(html);
        //Do something with document like the following
        // Console.WriteLine("Flattened Text: \n" + document.DocumentElement.TextContent);
        return document.DocumentElement.TextContent;
    }
    public List<string> GetVideos(string html)
    {
        List<string> videos = new List<string>();
        var document = parser.ParseDocument(html);
        Console.WriteLine(html);
        var vids = document.QuerySelectorAll("video");
        foreach(var vid in vids)
        {
            Console.WriteLine(vid.OuterHtml);
            var vid_url = vid.Attributes.GetNamedItem("src").Value;
            Console.WriteLine(vid_url);
            videos.Add(vid_url);
        }
        return videos;
    }
    public List<string> GetImages(string html)
    {
        List<string> images = new List<string>();
        var document = parser.ParseDocument(html);
        Console.WriteLine(html);
        var imgs = document.QuerySelectorAll("img");
        foreach(var img in imgs)
        {
            Console.WriteLine(img.OuterHtml);
            var img_url = img.Attributes.GetNamedItem("src").Value;
            Console.WriteLine(img_url);
            images.Add(img_url);
        }
        return images;
    }
}