﻿using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.ServerAggregate;

public class Server : Entity, IAggregateRoot
{
    private readonly List<Channel> _channels;
    private readonly List<Member> _members;
    private readonly List<Moderator> _moderators;
    private readonly List<Category> _categories;

    public Server(string name, string shortDescription, string longDescription, string creatorEmail, string? thumbnail = "")
    {
        Name = name;
        ShortDescription = shortDescription;
        LongDescription = longDescription;
        CreatorEmail = creatorEmail;
        Thumbnail = thumbnail;
        _channels = new List<Channel>();
        _members = new List<Member>();
        _moderators = new List<Moderator>();
        _categories = new List<Category>();
    }

    public string Name { get; private set; }
    public string ShortDescription { get; private set; }
    public string LongDescription { get; private set; }
    public string CreatorEmail { get; private set; }
    public string? Thumbnail { get; private set; }
    public IReadOnlyCollection<Channel> Channels => _channels;
    public IReadOnlyCollection<Member> Members => _members;
    public IReadOnlyCollection<Moderator> Moderators => _moderators;
    public IReadOnlyCollection<Category> Categories => _categories;

    public Channel AddChannel(Guid id, string name, bool isPrivate, Guid createdById, DateTime dateCreated, Guid? lastModifiedById = null, DateTime? dateLastModified = null)
    {
        Channel? existingChannel = _channels.FirstOrDefault(x => x.Id == id);
        if (existingChannel is not null)
            return existingChannel;

        Channel newChannel = new(name, isPrivate);
        newChannel.SetId(id);
        newChannel.SetDateCreated(dateCreated);
        newChannel.SetCreatedById(createdById);

        if (lastModifiedById.HasValue)
            newChannel.SetLastModifiedById(lastModifiedById.Value);

        if (dateLastModified.HasValue)
            newChannel.SetDateLastModified(dateLastModified.Value);

        _channels.Add(newChannel);

        return newChannel;
    }

    public Channel? UpdateChannel(Guid channelId, string name, DateTime dateLastModified, Guid lastModifiedId)
    {
        Channel? channel = _channels.FirstOrDefault(x => x.Id == channelId);
        if (channel is null)
            return null;

        channel.SetName(name);
        channel.SetDateLastModified(dateLastModified);
        channel.SetLastModifiedById(lastModifiedId);

        return channel;
    }

    public void RemoveChannel(Guid channelId)
    {
        Channel? channel = _channels.FirstOrDefault(x => x.Id == channelId);
        if (channel is null)
            return;

        _channels.Remove(channel);
    }

    public void AddMember(Guid userId, string username, DateTime dateJoined) => _members.Add(new(userId, username, dateJoined));

    public void RemoveMember(Guid userId)
    {
        Member? member = _members.FirstOrDefault(x => x.UserId == userId);
        if (member is null)
            return;

        _members.Remove(member);
    }

    public void ChangeUsername(Guid userId, string newUsername)
    {
        Member? member = _members.FirstOrDefault(x => x.UserId == userId);
        if (member is null)
            return;

        _members.Remove(member);
        _members.Add(new(userId, newUsername, member.DateJoined));
    }

    public void AddModerator(Guid userId, DateTime dateStarted) => _moderators.Add(new Moderator(userId, dateStarted));

    public void RemoveModerator(Guid userId)
    {
        Moderator? mod = _moderators.FirstOrDefault(x => x.UserId == userId);
        if (mod is null)
            return;

        _moderators.Remove(mod);
    }

    public void AddChannelMember(Guid channelId, Guid userId)
    {
        Channel? channel = _channels.FirstOrDefault(x => x.Id == channelId);
        if (channel is null)
            return;

        channel.AddMember(userId);
    }

    public void RemoveChannelMember(Guid channelId, Guid userId)
    {
        Channel? channel = _channels.FirstOrDefault(x => x.Id == channelId);
        if (channel is null)
            return;

        channel.RemoveMember(userId);
    }

    public void AddCategory(Category category)
    {
        Category? existing = _categories.FirstOrDefault(x => x == category);
        if (existing is not null)
            return;

        _categories.Add(category);
    }

    public void RemoveCategory(Category category)
    {
        Category? existing = _categories.FirstOrDefault(x => x == category);
        if (existing is null)
            return;

        _categories.Remove(existing);
    }
}
