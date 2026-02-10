import { describe, it, expect } from "vitest";
import { Dooble } from "../app/scripts/Dooble";

describe("Dooble", () => {
  describe("constructor", () => {
    it("should create a dooble with the given name", () => {
      const dooble = new Dooble("TestDooble");
      expect(dooble.name).toBe("TestDooble");
    });

    it("should default to type 'dooble'", () => {
      const dooble = new Dooble("TestDooble");
      expect(dooble.type).toBe("dooble");
    });

    it("should allow creating a blooble type", () => {
      const blooble = new Dooble("TestBlooble", "blooble");
      expect(blooble.type).toBe("blooble");
    });

    it("should initialize with default stats", () => {
      const dooble = new Dooble("TestDooble");
      expect(dooble.stats.age).toBe(0);
      expect(dooble.stats.hunger).toBe(0);
    });
  });

  describe("feed", () => {
    it("should reduce hunger by the given amount", () => {
      const dooble = new Dooble("TestDooble");
      dooble.stats.hunger = 50;
      dooble.feed(20);
      expect(dooble.stats.hunger).toBe(30);
    });

    it("should not reduce hunger below 0", () => {
      const dooble = new Dooble("TestDooble");
      dooble.stats.hunger = 10;
      dooble.feed(30);
      expect(dooble.stats.hunger).toBe(0);
    });

    it("should handle feeding a full dooble", () => {
      const dooble = new Dooble("TestDooble");
      dooble.stats.hunger = 0;
      dooble.feed(10);
      expect(dooble.stats.hunger).toBe(0);
    });
  });
});
