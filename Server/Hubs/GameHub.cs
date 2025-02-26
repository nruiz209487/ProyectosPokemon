using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs
{
    public class GameHub : Hub
    {
        public async Task SendAcierto(int acierto)
        {
            await Clients.All.SendAsync("ReceiveAcierto", acierto);
        }
    }
}
