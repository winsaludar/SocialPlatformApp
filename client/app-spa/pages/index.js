import Login from "../components/authentication/Login";

export default function Home() {
  const appName = "App Name";

  return (
    <>
      <Login title={`Login | ${appName}`} />

      {/* TODO: SHOW INNER PAGES WHEN LOGGED-IN */}
    </>
  );
}
