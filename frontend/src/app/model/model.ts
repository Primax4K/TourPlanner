export class Tour {
  constructor(
    public id: number,
    public name: string,
    public from_long: number,
    public from_lat: number,
    public to_long: number,
    public to_lat: number,
    public routeInfo:RouteData|null=null,
    public tourLogs:TourLog[]=[]
  ) {}
}
export interface RouteData {
  distance: number;
  duration: number;
  coordinates: [number, number][];
}

export class Login{
  constructor(
    public username:string,
    public hashedPassword:string
  ){}
}
export class TourLog{
  constructor(
    public id:number,
    public name:string  
  ){}
}