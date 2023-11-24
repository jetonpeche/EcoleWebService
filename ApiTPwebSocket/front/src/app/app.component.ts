import { Component, OnInit, AfterViewInit, OnDestroy, inject, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { PixelService } from 'src/services/PixelService.service';

type Pixel =
{
  Id: string,
  PosX: number,
  PosY: number,
  Couleur: string
}

type PixelReponse<T> =
{
  Reponse: T
}

type MessageReponse = 
{
  listeMessage: string[]
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, AfterViewInit, OnDestroy
{
  @ViewChild("canvas") canvas: ElementRef;

  pixelServ = inject(PixelService);

  taillePixel = 10;
  listeMessage: string[] = [];

  private listePixel: Pixel[] = [];
  private contextCanvas: CanvasRenderingContext2D;

  async ngOnInit() 
  {

    this.InitDonnees();
  }

  ngAfterViewInit(): void 
  {
    this.contextCanvas = (this.canvas.nativeElement as HTMLCanvasElement).getContext("2d");
  }

  ngOnDestroy(): void
  {
    this.pixelServ.Fermer();  

    this.pixelServ.hubConnexion.off("ReponseAjouterPixel");
    this.pixelServ.hubConnexion.off("ReponseAjouterMessage");
    this.pixelServ.hubConnexion.off("ReponseListerPixel");
    this.pixelServ.hubConnexion.off("ReponseListerMessage");
  }

  AjouterPixel(_event: MouseEvent): void
  {
    console.log(_event);

    const CANVAS = (this.canvas.nativeElement as HTMLCanvasElement).getBoundingClientRect();

    const POS_X = Math.floor((_event.clientX - CANVAS.left) / this.taillePixel) * this.taillePixel;
    const POS_Y = Math.floor((_event.clientY - CANVAS.top) / this.taillePixel) * this.taillePixel;

    const INDEX = this.listePixel.findIndex(x => x.PosX == POS_X && x.PosY == POS_Y);

    if(INDEX != -1)
    {      
      this.listePixel.splice(INDEX, 1);
      this.contextCanvas.clearRect(POS_X, POS_Y, this.taillePixel, this.taillePixel);

      return;
    }

    const DATA: Pixel = 
    {
      Id: `${_event.clientX}${_event.clientY}`,
      PosX: POS_X,
      PosY: POS_Y,
      Couleur: "red"
    };

    this.pixelServ.Demander("AjouterPixel", DATA);
  }

  AjouterMessage(_msg: string): void
  {
    if(!_msg)
      return;

    this.pixelServ.Demander("AjouterMessage", _msg);
  }

  private InitDonnees(): void
  {
    setTimeout(() => 
    {
      if(this.pixelServ.hubConnexion.state == "Connected")
      { 
        this.ReponseAjouterPixel();
        this.ReponseListerPixel();
        this.ReponseListerMessage();
        this.ReponseAjouterMessage();

        this.pixelServ.Demander("ListerPixel");
        this.pixelServ.Demander("ListerMessage");
      }
      else
        this.InitDonnees();
        
    }, 1000);
  }

  private AjouterPixelCanvas(_pixel: Pixel): void
  {
    this.contextCanvas.fillStyle = _pixel.Couleur;
    this.contextCanvas.fillRect(_pixel.PosX, _pixel.PosY, this.taillePixel, this.taillePixel);

    this.listePixel.push(_pixel);
  }

  private ReponseAjouterPixel(): void
  {
    this.pixelServ.hubConnexion.on("ReponseAjouterPixel", (retour: string) =>
    {
      const DATA: PixelReponse<Pixel> = JSON.parse(retour);

      this.AjouterPixelCanvas(DATA.Reponse);
    });   
  }

  private ReponseListerPixel(): void
  {
    this.pixelServ.hubConnexion.on("ReponseListerPixel", (retour: string) =>
    {
      const DATA: PixelReponse<Pixel[]> = JSON.parse(retour);
      
      if(!DATA.Reponse)
        return;

      for (const element of DATA.Reponse)
        this.AjouterPixelCanvas(element);
    });
  }

  private ReponseListerMessage(): void
  {
    this.pixelServ.hubConnexion.on("ReponseListerMessage", (retour: string) =>
    {
      const DATA: PixelReponse<string[]> = JSON.parse(retour);

      this.listeMessage = DATA.Reponse;
      
      for (const element of this.listePixel)
        this.AjouterPixelCanvas(element);
    });
  }

  private ReponseAjouterMessage(): void
  {
    this.pixelServ.hubConnexion.on("ReponseAjouterMessage", (retour: string) =>
    {
      const DATA: PixelReponse<string> = JSON.parse(retour);
      
      this.listeMessage.push(DATA.Reponse);
    });
  }
}
