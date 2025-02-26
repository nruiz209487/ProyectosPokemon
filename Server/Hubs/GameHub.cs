using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs
{
    public class GameHub : Hub
    {
        public async Task SendAcierto(int acierto)
        {
            await Clients.Others.SendAsync("ReceiveAcierto", acierto);
        }
    }
}
