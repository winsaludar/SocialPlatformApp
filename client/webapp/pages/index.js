import Login from "../components/authentication/Login";
import AppList from "../components/app/AppList";

const appName = process.env.NEXT_PUBLIC_APP_NAME;
const isLoggedIn = true; // TODO: USE REAL LOGIN SESSION

export default function Home() {
  return (
    <>
      {isLoggedIn ? (
        <AppList title={`Home | ${appName}`} />
      ) : (
        <Login title={`Login | ${appName}`} />
      )}
    </>
  );
}
