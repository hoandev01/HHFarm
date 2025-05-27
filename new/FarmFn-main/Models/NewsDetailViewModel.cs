using Farm.Models;

namespace Farm.Models;

public class NewsDetailViewModel
{
    public News News { get; set; }
    public List<News> RelatedNews { get; set; }
}
