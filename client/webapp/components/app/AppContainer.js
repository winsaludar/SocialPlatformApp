import AppHero from "./AppHero";
import AppList from "./AppList";

export default function AppContainer() {
  return (
    <>
      <header>
        <AppHero />
      </header>

      <section>
        <AppList />
      </section>
    </>
  );
}
