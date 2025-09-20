// <copyright file="Timeslip.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

[DebuggerDisplay("Comment = {" + nameof(Comment) + "} {" + nameof(Hours) + "} hours")]
public record Timeslip
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("user")]
    public Uri? User { get; init; }

    [JsonPropertyName("project")]
    public Uri? Project { get; init; }

    [JsonIgnore]
    public Project? ProjectEntry { get; init; }

    [JsonPropertyName("task")]
    public Uri? Task { get; init; }

    [JsonIgnore]
    public TaskItem? TaskEntry { get; init; }

    [JsonPropertyName("dated_on")]
    public DateTimeOffset? DatedOn { get; init; }

    [JsonPropertyName("hours")]
    public decimal? Hours { get; init; }

    [JsonPropertyName("comment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Comment { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    [JsonIgnore]
    public User? UserEntry { get; init; }
}