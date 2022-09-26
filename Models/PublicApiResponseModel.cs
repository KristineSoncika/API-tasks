using System.Collections.Generic;

namespace API_01.Models;

public class PublicApiResponseModel
{
    public int Count { get; set; }
    public List<Entry> Entries { get; set; }
}

public class Entry
{
    public string Api { get; set; }
    public string Description { get; set; }
    public string Auth { get; set; }
    public bool Https { get; set; }
    public string Cors { get; set; }
    public string Link { get; set; }
    public string Category { get; set; }
}