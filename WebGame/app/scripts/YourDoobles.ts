import { Dooble } from "./Dooble";

export class YourDoobles {
  private doobles: Dooble[] = [];

  addDooble(dooble: Dooble) {
    this.doobles.push(dooble);
  }

  createDooble(name: string): Dooble {
    const newDooble = new Dooble(name);
    this.doobles.push(newDooble);
    return newDooble;
  }

  getDoobles(): Dooble[] {
    return this.doobles;
  }

  getDoobleCount(): number {
    return this.doobles.length;
  }
}
