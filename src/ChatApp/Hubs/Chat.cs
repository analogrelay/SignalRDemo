// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatSample.Hubs
{
    [Authorize]
    public class Chat : Hub
    {
        public Chat()
        {
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.InvokeAsync("UserJoined", new { name = Context.User.Identity.Name, connectionId = Context.ConnectionId });
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Clients.All.InvokeAsync("UserLeft", new { name = Context.User.Identity.Name, connectionId = Context.ConnectionId });
            await base.OnConnectedAsync();
        }

        public async Task Send(string message)
        {
            if (message.StartsWith("/msg"))
            {
                // Hacky fast parsing ;)
                var splat = message.Split(' ', 3);
                var to = splat[1];
                message = splat[2];
                await Clients.User(to).InvokeAsync("Send", Context.User.Identity.Name, $"[direct]: {message}");
                await Clients.Client(Context.ConnectionId).InvokeAsync("Send", Context.User.Identity.Name, $"[direct]: {message}");
            }
            else
            {
                await Clients.All.InvokeAsync("Send", Context.User.Identity.Name, message);
            }
        }
    }
}
