// <copyright file="TimeslipBuilder.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain.Tests.Builders;

using System;

public class TimeslipBuilder
{
    private Uri? url = new("https://api.freeagent.com/v2/timeslips/1");
    private Uri? user = new("https://api.freeagent.com/v2/users/1");
    private Uri? project = new("https://api.freeagent.com/v2/projects/1");
    private Project? projectEntry;
    private Uri? task = new("https://api.freeagent.com/v2/tasks/1");
    private TaskItem? taskEntry;
    private DateTimeOffset datedOn = new(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);
    private decimal hours = 8m;
    private string? comment = "Working on feature implementation";
    private DateTimeOffset updatedAt = new(2024, 6, 15, 12, 0, 0, TimeSpan.Zero);
    private DateTimeOffset createdAt = new(2024, 6, 15, 11, 0, 0, TimeSpan.Zero);
    private User? userEntry;

    public TimeslipBuilder WithUrl(Uri? url)
    {
        this.url = url;
        return this;
    }

    public TimeslipBuilder WithUser(User user)
    {
        this.user = user.Url;
        this.userEntry = user;
        return this;
    }

    public TimeslipBuilder WithProject(Project project)
    {
        this.project = project.Url;
        this.projectEntry = project;
        return this;
    }

    public TimeslipBuilder WithTask(TaskItem task)
    {
        this.task = task.Url;
        this.taskEntry = task;
        return this;
    }

    public TimeslipBuilder WithHours(decimal hours)
    {
        this.hours = hours;
        return this;
    }

    public TimeslipBuilder WithComment(string? comment)
    {
        this.comment = comment;
        return this;
    }

    public TimeslipBuilder OnDate(DateTimeOffset date)
    {
        this.datedOn = date;
        return this;
    }

    public TimeslipBuilder ForToday()
    {
        this.datedOn = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);
        return this;
    }

    public TimeslipBuilder ForYesterday()
    {
        this.datedOn = new DateTimeOffset(2024, 6, 14, 0, 0, 0, TimeSpan.Zero);
        return this;
    }

    public Timeslip Build() => new()
    {
        Url = url,
        User = user,
        Project = project,
        ProjectEntry = projectEntry,
        Task = task,
        TaskEntry = taskEntry,
        DatedOn = datedOn,
        Hours = hours,
        Comment = comment,
        UpdatedAt = updatedAt,
        CreatedAt = createdAt,
        UserEntry = userEntry
    };

    public static implicit operator Timeslip(TimeslipBuilder builder) => builder.Build();
}
