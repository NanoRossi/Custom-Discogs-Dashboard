using System.Text.Json.Nodes;

namespace DiscogsProxy.DTO;

public class Pagination
{
    public Pagination(JsonNode? input)
    {
        this.Page = Convert.ToInt16(input.GetProperty<int>("page"));
        this.Pages = Convert.ToInt16(input.GetProperty<int>("pages"));
        this.Per_Page = Convert.ToInt16(input.GetProperty<int>("per_page"));
        this.Items = Convert.ToInt16(input.GetProperty<int>("items"));
    }

    public int Page { get; set; }

    public int Pages { get; set; }

    public int Per_Page { get; set; }

    public int Items { get; set; }
}