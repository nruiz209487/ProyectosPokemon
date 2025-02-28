using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs
{
    public class GameHub : Hub
    {
        private static int jugadoresListos = 0;
        private static int totalJugadoresRequeridos = 2;
        public async Task SendAcierto(int acierto)
        {
            await Clients.Others.SendAsync("ReceiveAcierto", acierto);
        }


        public async Task SendEntarLobby()
        {
            jugadoresListos++;
            if (jugadoresListos == totalJugadoresRequeridos)
            {
                await Clients.All.SendAsync("ReceiveIniciarJuego");
                jugadoresListos = 0;

            }
   
        }
    }
}
