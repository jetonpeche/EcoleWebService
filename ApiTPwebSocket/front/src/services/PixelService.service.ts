import * as signalR from '@microsoft/signalr';

export class PixelService
{
    hubConnexion: signalR.HubConnection;

    constructor() 
    { 
        this.InitHubConnexion();
    }

    OuvrirConnexion(): void
    {
        if(this.hubConnexion.state == "Connecting" || this.hubConnexion.state == "Connected")
            return;

        // lancer la connexion
        this.hubConnexion.start().then(() => console.log("ok"));
    }

    Fermer(): void
    {
        this.hubConnexion.stop();
    }

    async Demander(_routeWebSocket: string, _info: any = null)
    {
        if(_info == null || _info == undefined)
            await this.hubConnexion.invoke(_routeWebSocket);
        else
            await this.hubConnexion.invoke(_routeWebSocket, _info);
    }

    private InitHubConnexion(): void
    {
        this.hubConnexion = new signalR.HubConnectionBuilder()

        // toastr fait reference au program.cs sur app.maphub
        .withUrl("http://localhost:5197/pixel", 
        {
            skipNegotiation: true,
            transport: signalR.HttpTransportType.WebSockets
        })
        .build();

        this.OuvrirConnexion();
    }
}