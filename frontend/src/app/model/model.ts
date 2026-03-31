export class Tour {
  constructor(
    public id: number,
    public name: string,
    public from_long: number,
    public from_lat: number,
    public to_long: number,
    public to_lat: number
  ) {}
}