using System.Text.Json.Nodes;
using DiscogsProxy.Workers;

namespace DiscogsProxy.DTO;

public class Pagination(JsonNode? input)
{
    public int Page { get; set; } = Convert.ToInt16(input.GetPropertyValue<int>("page"));

    public int Pages { get; set; } = Convert.ToInt16(input.GetPropertyValue<int>("pages"));

    public int Per_Page { get; set; } = Convert.ToInt16(input.GetPropertyValue<int>("per_page"));

    public int Items { get; set; } = Convert.ToInt16(input.GetPropertyValue<int>("items"));
}