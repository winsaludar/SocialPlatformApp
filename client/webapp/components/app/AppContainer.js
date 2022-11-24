import AppHero from "./AppHero";
import AppList from "./AppList";
import { useState, useRef } from "react";
import initialBanner from "../../public/images/placeholder/initial-banner.jpg";

export default function AppContainer() {
  const [heroData, setHeroData] = useState({
    title: "Some really interesting text here",
    description:
      "Donec eleifend erat lacus, ac porttitor purus ultrices at. Proin sagittis interdum ex in sollicitudin. Integer et nibh fringilla, vehicula velit quis, blandit lorem. Morbi id tortor ante. Quisque inest egestas, posuere magna feugiat, iaculis purus.",
    backgroundImage: initialBanner.src,
  });
  const myHeaderRef = useRef(null);

  function handleItemClick(title, description, backgroundImage) {
    setHeroData({ title, description, backgroundImage });
    myHeaderRef.current.scrollIntoView({ behavior: "smooth", block: "center" });
  }

  return (
    <>
      <header ref={myHeaderRef}>
        <AppHero data={heroData} />
      </header>

      <section>
        <AppList onItemClick={handleItemClick} />
      </section>
    </>
  );
}
