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
    public description:string=""
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
    public rating:number
  ){}
}