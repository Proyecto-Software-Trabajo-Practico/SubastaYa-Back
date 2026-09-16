using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Application.Hubs
{
    /*
     * Hub de SignalR para comunicación bidireccional en tiempo real.
     * Permite a los clientes de la sala en vivo unirse o abandonar el grupo 
     * específico de una subasta para recibir ofertas en vivo sin saturar la red.
     */
    public class SubastaHub : Hub
    {
        public async Task UnirseASubasta(string subastaId)
        {
            // Se utiliza spinal-case (subasta-{id}) para coincidir exactamente con el canal de difusión del Handler
            await Groups.AddToGroupAsync(Context.ConnectionId, $"subasta-{subastaId}");
        }

        public async Task SalirDeSubasta(string subastaId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"subasta-{subastaId}");
        }
    }
}
