import AppHero from "../src/components/app/AppHero";
import AppList from "../src/components/app/AppList";
import { useState, useRef } from "react";
import initialBanner from "../public/images/placeholder/initial-banner.jpg";
import Cookies from "js-cookie";

const appName = process.env.NEXT_PUBLIC_APP_NAME;

export async function getStaticProps() {
  return { props: { title: `Home | ${appName}` } };
}

export default function HomePage() {
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
