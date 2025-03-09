using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs
{
    public class GameHub : Hub
    {
        private static int jugadoresListos = 0; // cuenta el nuemro de jugadores listos
        private const int MAXIMO_JUGADORS = 2; // constante con el numero maximo de jugadores
        public async Task SendAcierto(int acierto)
        {
            await Clients.Others.SendAsync("ReceiveAcierto", acierto);
        }
        /// <summary>
        /// maneja la entrada de jugadores
        /// </summary>
        /// <returns>numero de jugadores listos si son mayores al limite</returns>
        public async Task SendEntarLobby()
        {
            jugadoresListos++;
            if (jugadoresListos == MAXIMO_JUGADORS)
            {
                await Clients.All.SendAsync("ReceiveIniciarJuego");
            }
            else { await Clients.All.SendAsync("ReceiveLobby", jugadoresListos); }
        }

        /// <summary>
        /// maneja la salida de jugadores
        /// </summary>
        /// <returns>numero de jugadores listos si son mayores al limite</returns>
        public async Task SalirLobby()
        {
            jugadoresListos--;
            if (jugadoresListos == MAXIMO_JUGADORS)
            {
                await Clients.All.SendAsync("ReceiveIniciarJuego");
            }
            else { await Clients.All.SendAsync("ReceiveLobby", jugadoresListos); }
        }
    }
}
