export class Tour {
  constructor(
    public id: number,
    public name: string,
    public from_long: number,
    public from_lat: number,
    public to_long: number,
    public to_lat: number,
    public routeInfo:RouteData|null=null,
    public tourLogs:TourLog[]=[],
    public description:string="",
    public transport:TransportType=TransportType.Car
  ) {}
  public getChildFriendliness():number{
    if(this.tourLogs.length===0){
      return 0;
    }
    let score=0;
    this.tourLogs.forEach(tourLog=>{
      score+=(6-tourLog.difficulty)
    })
    return score/this.tourLogs.length;
  }
  public getPopularity():number{
    return this.tourLogs.length;
  }
}


export function createTourDto(tour: Tour) {
  return {
    name: tour.name,
    fromLongitude: tour.from_long,
    fromLatitude: tour.from_lat,
    toLongitude: tour.to_long,
    toLatitude: tour.to_lat,
    description: tour.description,
    transportType: tour.transport
  };
}
export function editTourDto(tour: Tour) {
  return {
    id: tour.id,
    name: tour.name,
    fromLongitude: tour.from_long,
    fromLatitude: tour.from_lat,
    toLongitude: tour.to_long,
    toLatitude: tour.to_lat,
    description: tour.description,
    transportType: tour.transport
  };
}
export function receiveTourDto(tour: any) {
  return new Tour(
    tour.id,
    tour.name,
    tour.fromLongitude,
    tour.fromLatitude,
    tour.toLongitude,
    tour.toLatitude,
    tour.routeInformation,
    [],
    tour.description,
    tour.transportType
  );
}

export enum TransportType{
  Car=0,
  Cycling=0,
  Walking=2
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
    public name:string,
    public timeOfTour:Date,
    public difficulty: number,
    public totalDistanceInM: number,
    public totalTimeInM: number,
    public rating:number,
    public comment:string
  ){}
  public formatedTimeDate():string{
    const date=this.timeOfTour.getDate()+"."+this.timeOfTour.getMonth()+"."+this.timeOfTour.getFullYear()+
    " "+this.timeOfTour.getHours()+":"+this.timeOfTour.getMinutes()+"Uhr";
    return date;
  }
}
