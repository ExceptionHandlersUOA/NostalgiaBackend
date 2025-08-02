using System.Runtime.CompilerServices;
using AngleSharp;
using AngleSharp.Html.Parser;

namespace HoverthArchiver;

public class HtmlParser
{
    private IBrowsingContext context = null;
    private IHtmlParser parser = null;
    
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
    public List<string> GetVideos(string html)
    {
        List<string> videos = new List<string>();
        var document = parser.ParseDocument(html);
        // Console.WriteLine(html);
        var vids = document.QuerySelectorAll("video");
        foreach(var vid in vids)
        {
            try
            {
                var vidUrl = vid.Attributes.GetNamedItem("src")?.Value;
                //Console.WriteLine(vidUrl);
                videos.Add(vidUrl);
            }
            catch (System.NullReferenceException e)
            {
                Console.WriteLine(e);
            }
        }
        return videos;
    }
    public List<string> GetImages(string html)
    {
        List<string> images = new List<string>();
        var document = parser.ParseDocument(html);
        // Console.WriteLine(html);
        var imgs = document.QuerySelectorAll("img");
        foreach(var img in imgs)
        {
            try
            {
                //Console.WriteLine(img.OuterHtml);
                var imgUrl = img.Attributes.GetNamedItem("src")?.Value;
                imgUrl = imgUrl?.Split("?").First();
                //Console.WriteLine(imgUrl);
                images.Add(imgUrl);
            }
            catch (System.NullReferenceException e)
            {
                Console.WriteLine(e);
            }
        }
        return images;
    }
}