import Login from "../components/authentication/Login";

const appName = process.env.NEXT_PUBLIC_APP_NAME;

export default function Home() {
  return (
    <>
      <Login title={`Login | ${appName}`} />

      {/* TODO: SHOW INNER PAGES WHEN LOGGED-IN */}
    </>
  );
}
