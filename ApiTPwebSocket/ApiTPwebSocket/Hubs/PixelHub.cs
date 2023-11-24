using ApiTPwebSocket.ModelsBdd;
using ApiTPwebSocket.Services;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace ApiTPwebSocket.Hubs;

public class PixelHub: Hub
{
    JoueurService joueurService = new();
    PixelService pixelService = new();
    MessageService messageService = new();

    public async Task Connexion(string _pseudo)
    {
        if (joueurService.Existe(_pseudo))
        {
            await Clients.Caller.SendAsync("ReponseConnexion", JsonSerializer.Serialize(new { Reponse = false }));
            return;
        }

        joueurService.Ajouter(_pseudo);

        await Clients.Caller.SendAsync("ReponseConnexion", JsonSerializer.Serialize(new { Reponse = true }));
    }

    public async Task ListerPixel()
    {
        await Clients.Caller.SendAsync("ReponseListerPixel", JsonSerializer.Serialize(new { Reponse = pixelService.Lister() }));
    }

    public async Task AjouterPixel(Pixel _pixel)
    {
        pixelService.Ajouter(_pixel);

        await Clients.All.SendAsync("ReponseAjouterPixel", JsonSerializer.Serialize(new { Reponse = _pixel }));
    }

    public async Task ModifierPixel(Pixel _pixel)
    {
        pixelService.Modifier(_pixel);

        await Clients.All.SendAsync("ReponseModifierPixel", JsonSerializer.Serialize(new { Reponse = _pixel }));
    }

    public async Task SupprimerPixel(Pixel _pixel)
    {
        pixelService.Supprimer(_pixel);

        await Clients.Others.SendAsync("ReponseSupprimerPixel", JsonSerializer.Serialize(new { Reponse = _pixel }));
    }

    public async Task ListerMessage()
    {
        await Clients.Caller.SendAsync("ReponseListerMessage", JsonSerializer.Serialize(new { Reponse = messageService.Lister() }));
    }

    public async Task AjouterMessage(string _msg)
    {
        messageService.Ajouter(_msg);

        await Clients.All.SendAsync("ReponseAjouterMessage", JsonSerializer.Serialize(new { Reponse = _msg }));
    }
}
