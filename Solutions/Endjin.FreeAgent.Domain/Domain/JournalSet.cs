// <copyright file="JournalSet.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/*
{ 
  "journal_set":
  {
    "url":"https://api.freeagent.com/v2/journal_sets/1",
    "dated_on":"2011-07-28",
    "description":"An example journal set",
    "tag":"MYAPPTAG",
    "journal_entries":[
      { 
        "url":"https://api.freeagent.com/v2/journal_entries/1",
        "category":"https://api.freeagent.com/v2/categories/001",
        "description":"A Sales Correction",
        "debit_value":"-123.45"
      },
      {
        "url":"https://api.freeagent.com/v2/journal_entries/2",
        "category":"https://api.freeagent.com/v2/categories/901",
        "user":"https://api.freeagent.com/v2/users/1",
        "description":"Director's Capital Introduced",
        "debit_value":"123.45"
      }
    ]
  }
}
*/

public record JournalSet
{

    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Url { get; init; }

    [JsonPropertyName("dated_on")]
    public required string DatedOn { get; init; }

    [JsonPropertyName("description")]
    public required string Description { get; init; }

    [JsonPropertyName("tag")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Tag { get; init; }

    [JsonPropertyName("journal_entries")]
    public ImmutableList<JournalEntry> JournalEntries { get; init; } = [];
}