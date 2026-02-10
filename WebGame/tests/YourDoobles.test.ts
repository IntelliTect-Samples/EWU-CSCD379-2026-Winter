import { describe, it, expect, beforeEach } from "vitest";
import { Dooble } from "../app/scripts/Dooble";
import { YourDoobles } from "../app/scripts/YourDoobles";

describe("YourDoobles", () => {
  let yourDoobles: YourDoobles;

  beforeEach(() => {
    yourDoobles = new YourDoobles();
  });

  describe("initial state", () => {
    it("should start with no doobles", () => {
      expect(yourDoobles.getDoobleCount()).toBe(0);
      expect(yourDoobles.getDoobles()).toEqual([]);
    });
  });

  describe("addDooble", () => {
    it("should add an existing dooble to the collection", () => {
      const dooble = new Dooble("ExistingDooble");
      yourDoobles.addDooble(dooble);

      expect(yourDoobles.getDoobleCount()).toBe(1);
      expect(yourDoobles.getDoobles()).toContain(dooble);
    });

    it("should add multiple doobles", () => {
      const dooble1 = new Dooble("Dooble1");
      const dooble2 = new Dooble("Dooble2");

      yourDoobles.addDooble(dooble1);
      yourDoobles.addDooble(dooble2);

      expect(yourDoobles.getDoobleCount()).toBe(2);
    });
  });

  describe("createDooble", () => {
    it("should create and add a new dooble with the given name", () => {
      const dooble = yourDoobles.createDooble("NewDooble");

      expect(dooble.name).toBe("NewDooble");
      expect(yourDoobles.getDoobleCount()).toBe(1);
      expect(yourDoobles.getDoobles()).toContain(dooble);
    });

    it("should return the created dooble", () => {
      const dooble = yourDoobles.createDooble("TestDooble");

      expect(dooble).toBeInstanceOf(Dooble);
      expect(dooble.name).toBe("TestDooble");
    });
  });

  describe("getDoobles", () => {
    it("should return all doobles in the collection", () => {
      yourDoobles.createDooble("Dooble1");
      yourDoobles.createDooble("Dooble2");
      yourDoobles.createDooble("Dooble3");

      const doobles = yourDoobles.getDoobles();

      expect(doobles).toHaveLength(3);
      expect(doobles.map((d) => d.name)).toEqual([
        "Dooble1",
        "Dooble2",
        "Dooble3",
      ]);
    });
  });

  describe("getDoobleCount", () => {
    it("should return the correct count after adding doobles", () => {
      expect(yourDoobles.getDoobleCount()).toBe(0);

      yourDoobles.createDooble("Dooble1");
      expect(yourDoobles.getDoobleCount()).toBe(1);

      yourDoobles.createDooble("Dooble2");
      expect(yourDoobles.getDoobleCount()).toBe(2);
    });
  });
});
